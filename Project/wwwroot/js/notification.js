/**
 * Spaceara Real-Time Notification System Client
 * Handles SignalR real-time events, audio chime, toasts, dropdown, and full notification page interactions.
 */

(function () {
    'use strict';

    // Global Namespace
    window.SpacearaNotification = window.SpacearaNotification || {};

    const CONFIG = {
        hubUrl: '/hubs/notification',
        apiBaseUrl: '/api/notifications',
        toastDuration: 5000
    };

    let hubConnection = null;
    let audioContext = null;

    // Initialize Web Audio API for subtle chime sound
    function playSubtleChime() {
        try {
            const AudioContext = window.AudioContext || window.webkitAudioContext;
            if (!AudioContext) return;

            if (!audioContext) {
                audioContext = new AudioContext();
            }

            if (audioContext.state === 'suspended') {
                audioContext.resume();
            }

            const now = audioContext.currentTime;

            // First note (High subtle chime)
            const osc1 = audioContext.createOscillator();
            const gain1 = audioContext.createGain();
            osc1.type = 'sine';
            osc1.frequency.setValueAtTime(587.33, now); // D5
            gain1.gain.setValueAtTime(0.12, now);
            gain1.gain.exponentialRampToValueAtTime(0.001, now + 0.35);

            osc1.connect(gain1);
            gain1.connect(audioContext.destination);

            osc1.start(now);
            osc1.stop(now + 0.35);

            // Second note (Harmonic pitch)
            const osc2 = audioContext.createOscillator();
            const gain2 = audioContext.createGain();
            osc2.type = 'sine';
            osc2.frequency.setValueAtTime(880, now + 0.08); // A5
            gain2.gain.setValueAtTime(0.15, now + 0.08);
            gain2.gain.exponentialRampToValueAtTime(0.001, now + 0.5);

            osc2.connect(gain2);
            gain2.connect(audioContext.destination);

            osc2.start(now + 0.08);
            osc2.stop(now + 0.5);
        } catch (e) {
            console.warn('Audio play failed or disabled by browser policy:', e);
        }
    }

    // Initialize SignalR Connection
    function initSignalR() {
        if (typeof signalR === 'undefined') {
            console.warn('SignalR library not loaded.');
            return;
        }

        hubConnection = new signalR.HubConnectionBuilder()
            .withUrl(CONFIG.hubUrl)
            .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
            .configureLogging(signalR.LogLevel.Warning)
            .build();

        // Event: Receive Notification
        hubConnection.on('ReceiveNotification', function (notification) {
            onNotificationReceived(notification);
        });

        // Event: Update Unread Count
        hubConnection.on('UpdateUnreadCount', function (count) {
            updateUnreadBadge(count);
        });

        // Event: Notification Marked Read
        hubConnection.on('NotificationMarkedRead', function (id) {
            markItemAsReadUI(id);
        });

        // Event: All Notifications Marked Read
        hubConnection.on('AllNotificationsMarkedRead', function () {
            markAllItemsAsReadUI();
        });

        // Event: Notification Deleted
        hubConnection.on('NotificationDeleted', function (id) {
            removeItemUI(id);
        });

        // Connection Reconnecting
        hubConnection.onreconnecting(function () {
            showToast({
                title: 'تنبيه الاتصال',
                message: 'جارٍ إعادة الاتصال بخادم التنبيهات...',
                type: 'Warning',
                iconClass: 'sync'
            });
        });

        // Connection Reconnected
        hubConnection.onreconnected(function () {
            showToast({
                title: 'تم إعادة الاتصال',
                message: 'تم استعادة الاتصال بخادم التنبيهات بنجاح.',
                type: 'Success',
                iconClass: 'wifi'
            });
            fetchRecentNotifications();
        });

        // Start Connection
        hubConnection.start()
            .then(function () {
                console.log('SignalR NotificationHub connected.');
                fetchRecentNotifications();
            })
            .catch(function (err) {
                console.error('SignalR Connection Error:', err);
            });
    }

    // Handle Incoming Notification
    function onNotificationReceived(notification) {
        // 1. Play chime sound
        playSubtleChime();

        // 2. Shake Bell Icon
        shakeBell();

        // 3. Increment Badge
        incrementBadge();

        // 4. Show Bootstrap Toast
        showToast(notification);

        // 5. Prepend to Dropdown
        prependToDropdown(notification);
        tagDropdownRelativeTime(notification);

        // 6. Prepend to Notifications Page if active
        if (window.SpacearaNotificationPage && typeof window.SpacearaNotificationPage.onNewNotification === 'function') {
            window.SpacearaNotificationPage.onNewNotification(notification);
        }
    }

    // Shake Bell Animation
    function shakeBell() {
        const bellIcon = document.getElementById('notif-bell-icon');
        if (!bellIcon) return;

        bellIcon.classList.remove('bell-shake');
        void bellIcon.offsetWidth; // Trigger reflow
        bellIcon.classList.add('bell-shake');

        setTimeout(() => {
            bellIcon.classList.remove('bell-shake');
        }, 1000);
    }

    // Update Unread Badge Count
    function updateUnreadBadge(count) {
        const badge = document.getElementById('notif-unread-badge');
        const countText = document.getElementById('notif-unread-count-text');

        if (!badge) return;

        if (count > 0) {
            badge.innerText = count > 99 ? '99+' : count;
            badge.classList.remove('hidden');
            badge.classList.add('badge-pulse');
            setTimeout(() => badge.classList.remove('badge-pulse'), 1500);
        } else {
            badge.classList.add('hidden');
        }

        if (countText) {
            countText.innerText = count;
        }
    }

    function incrementBadge() {
        const badge = document.getElementById('notif-unread-badge');
        if (!badge) return;

        let current = parseInt(badge.innerText, 10) || 0;
        updateUnreadBadge(current + 1);
    }

    // Helper to render icon HTML (supports FontAwesome, Bootstrap Icons, and Material Symbols)
    function renderIconHtml(iconClass, extraCss) {
        extraCss = extraCss || 'text-xl';
        if (!iconClass) iconClass = 'notifications';

        if (iconClass.startsWith('fa-') || iconClass.startsWith('fa ') || iconClass.startsWith('fas ') || iconClass.startsWith('far ') || iconClass.startsWith('fab ') || iconClass.startsWith('bi-') || iconClass.startsWith('bi ')) {
            return `<i class="${iconClass} ${extraCss}"></i>`;
        }

        return `<span class="material-symbols-outlined ${extraCss}">${iconClass}</span>`;
    }

    // Show Toast Popup
    function showToast(notification) {
        const container = document.getElementById('notification-toast-container');
        if (!container) return;

        const toastId = 'toast-' + Date.now() + '-' + Math.floor(Math.random() * 1000);
        const icon = notification.iconClass || notification.icon || 'notifications';
        const colorClass = notification.colorClass || 'text-teal-600 bg-teal-50 border-teal-200';
        const toastBgClass = notification.toastBgClass || 'border-r-4 border-teal-500';

        const toastHtml = `
            <div id="${toastId}" class="custom-toast ${toastBgClass} flex items-start gap-3 relative" role="alert">
                <div class="w-10 h-10 rounded-xl ${colorClass} flex items-center justify-center shrink-0 mt-0.5">
                    ${renderIconHtml(icon, 'text-xl')}
                </div>
                <div class="flex-1 min-w-0 pr-1">
                    <div class="flex items-center justify-between">
                        <h4 class="text-sm font-bold text-slate-900 dark:text-white truncate">${escapeHtml(notification.title)}</h4>
                        <button type="button" class="text-slate-400 hover:text-slate-600 dark:hover:text-slate-200 p-1 rounded-lg transition-colors" onclick="document.getElementById('${toastId}').remove()">
                            <span class="material-symbols-outlined text-sm">close</span>
                        </button>
                    </div>
                    <p class="text-xs text-slate-600 dark:text-slate-300 mt-1 line-clamp-2">${escapeHtml(notification.message)}</p>
                    ${notification.url ? `<a href="${notification.url}" class="inline-flex items-center gap-1 text-xs font-bold text-primary hover:underline mt-2">عرض التفاصيل <span class="material-symbols-outlined text-xs">arrow_left</span></a>` : ''}
                </div>
                <div class="toast-progress"></div>
            </div>
        `;

        container.insertAdjacentHTML('afterbegin', toastHtml);

        const toastEl = document.getElementById(toastId);
        setTimeout(() => {
            if (toastEl && toastEl.parentNode) {
                toastEl.classList.add('hide');
                setTimeout(() => toastEl.remove(), 300);
            }
        }, CONFIG.toastDuration);
    }

    // Prepend Notification Item to Dropdown List
    function prependToDropdown(notification) {
        const list = document.getElementById('notif-dropdown-list');
        const emptyState = document.getElementById('notif-empty-state');

        if (emptyState) {
            emptyState.classList.add('hidden');
        }

        if (!list) return;

        const itemHtml = createNotificationItemHtml(notification);
        list.insertAdjacentHTML('afterbegin', itemHtml);
    }

    // Build HTML for a notification item
    function createNotificationItemHtml(item) {
        const isUnread = !item.isRead;
        const icon = item.iconClass || item.icon || 'notifications';
        const colorClass = item.colorClass || 'text-teal-600 bg-teal-50 border-teal-200';
        const url = item.url || 'javascript:void(0)';

        return `
            <div id="dropdown-item-${item.id}" class="notif-item ${isUnread ? 'unread' : ''} p-3.5 border-b border-slate-100 dark:border-slate-800 flex items-start gap-3">
                <div class="w-10 h-10 rounded-xl ${colorClass} flex items-center justify-center shrink-0 mt-0.5">
                    ${renderIconHtml(icon, 'text-xl')}
                </div>
                <div class="flex-1 min-w-0">
                    <a href="${url}" onclick="SpacearaNotification.onItemClick(${item.id}, '${item.url || ''}')" class="block group">
                        <h4 class="text-xs font-bold text-slate-900 dark:text-white group-hover:text-primary transition-colors line-clamp-1">${escapeHtml(item.title)}</h4>
                        <p class="text-xs text-slate-500 dark:text-slate-400 mt-0.5 line-clamp-2">${escapeHtml(item.message)}</p>
                    </a>
                    <div class="flex items-center justify-between mt-2 text-[11px] text-slate-400">
                        <span>${escapeHtml(item.relativeTime || 'الآن')}</span>
                        <div class="flex items-center gap-2">
                            ${isUnread ? `
                                <button type="button" onclick="SpacearaNotification.markAsRead(${item.id})" class="text-primary hover:underline text-[11px] font-bold">
                                    تحديد كمقروء
                                </button>
                            ` : ''}
                            <button type="button" onclick="SpacearaNotification.deleteNotification(${item.id})" class="text-slate-400 hover:text-rose-500 text-[11px]">
                                <span class="material-symbols-outlined text-sm">delete</span>
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        `;
    }

    // Fetch Recent Notifications for Dropdown
    function fetchRecentNotifications() {
        const list = document.getElementById('notif-dropdown-list');
        const emptyState = document.getElementById('notif-empty-state');
        const skeleton = document.getElementById('notif-skeleton-loader');

        if (!list) return;

        if (skeleton) skeleton.classList.remove('hidden');

        fetch(CONFIG.apiBaseUrl + '/recent?take=8')
            .then(res => res.json())
            .then(data => {
                if (skeleton) skeleton.classList.add('hidden');

                updateUnreadBadge(data.unreadCount || 0);

                if (!data.items || data.items.length === 0) {
                    list.innerHTML = '';
                    if (emptyState) emptyState.classList.remove('hidden');
                } else {
                    if (emptyState) emptyState.classList.add('hidden');
                    list.innerHTML = data.items.map(item => createNotificationItemHtml(item)).join('');
                    data.items.forEach(tagDropdownRelativeTime);
                }
            })
            .catch(err => {
                if (skeleton) skeleton.classList.add('hidden');
                console.error('Error fetching recent notifications:', err);
            });
    }

    // Actions
    function markAsRead(id) {
        fetch(`${CONFIG.apiBaseUrl}/${id}/mark-read`, { method: 'POST' })
            .then(res => res.json())
            .then(data => {
                if (data.success) {
                    markItemAsReadUI(id);
                    updateUnreadBadge(data.unreadCount);
                }
            })
            .catch(err => console.error('Error marking as read:', err));
    }

    function markAllAsRead() {
        fetch(`${CONFIG.apiBaseUrl}/mark-all-read`, { method: 'POST' })
            .then(res => res.json())
            .then(data => {
                if (data.success) {
                    markAllItemsAsReadUI();
                    updateUnreadBadge(0);
                }
            })
            .catch(err => console.error('Error marking all as read:', err));
    }

    function deleteNotification(id) {
        fetch(`${CONFIG.apiBaseUrl}/${id}`, { method: 'DELETE' })
            .then(res => res.json())
            .then(data => {
                if (data.success) {
                    removeItemUI(id);
                    updateUnreadBadge(data.unreadCount);
                }
            })
            .catch(err => console.error('Error deleting notification:', err));
    }

    function onItemClick(id, url) {
        markAsRead(id);
        if (url && url !== 'javascript:void(0)') {
            window.location.href = url;
        }
    }

    // UI Updates
    function markItemAsReadUI(id) {
        const item = document.getElementById(`dropdown-item-${id}`);
        if (item) {
            item.classList.remove('unread');
            const btn = item.querySelector('button[onclick*="markAsRead"]');
            if (btn) btn.remove();
        }
    }

    function markAllItemsAsReadUI() {
        const unreadItems = document.querySelectorAll('#notif-dropdown-list .notif-item.unread');
        unreadItems.forEach(item => {
            item.classList.remove('unread');
            const btn = item.querySelector('button[onclick*="markAsRead"]');
            if (btn) btn.remove();
        });
    }

    function removeItemUI(id) {
        const item = document.getElementById(`dropdown-item-${id}`);
        if (item) {
            item.remove();
        }

        const list = document.getElementById('notif-dropdown-list');
        const emptyState = document.getElementById('notif-empty-state');
        if (list && list.children.length === 0 && emptyState) {
            emptyState.classList.remove('hidden');
        }
    }

    function escapeHtml(str) {
        if (!str) return '';
        return str
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#039;');
    }

    function relativeTime(createdAt) {
        const date = new Date(createdAt);
        if (Number.isNaN(date.getTime())) return 'الآن';
        const minutes = Math.floor((Date.now() - date.getTime()) / 60000);
        if (minutes < 1) return 'الآن';
        if (minutes < 2) return 'منذ دقيقة';
        if (minutes < 60) return `منذ ${minutes} دقائق`;
        const hours = Math.floor(minutes / 60);
        if (hours < 2) return 'منذ ساعة';
        if (hours < 3) return 'منذ ساعتين';
        if (hours < 24) return `منذ ${hours} ساعات`;
        const days = Math.floor(hours / 24);
        if (days < 2) return 'منذ يوم';
        if (days < 7) return `منذ ${days} أيام`;
        const weeks = Math.floor(days / 7);
        if (weeks < 2) return 'منذ أسبوع';
        if (days < 30) return `منذ ${weeks} أسابيع`;
        const months = Math.floor(days / 30);
        return months < 2 ? 'منذ شهر' : `منذ ${months} أشهر`;
    }

    function tagDropdownRelativeTime(item) {
        const time = document.querySelector(`#dropdown-item-${item.id} .flex.items-center.justify-between.mt-2 span`);
        if (time && item.createdAt) time.dataset.createdAt = item.createdAt;
    }

    function refreshRelativeTimes() {
        document.querySelectorAll('[data-created-at]').forEach(element => {
            element.textContent = relativeTime(element.dataset.createdAt);
        });
    }

    // Export API to global namespace
    window.SpacearaNotification = {
        init: initSignalR,
        markAsRead: markAsRead,
        markAllAsRead: markAllAsRead,
        deleteNotification: deleteNotification,
        onItemClick: onItemClick,
        fetchRecentNotifications: fetchRecentNotifications,
        playChime: playSubtleChime
    };

    // Auto initialize on DOM Content Loaded
    document.addEventListener('DOMContentLoaded', function () {
        initSignalR();
        refreshRelativeTimes();
        setInterval(refreshRelativeTimes, 60000);
    });
})();
