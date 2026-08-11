using System.Threading.Tasks;
using Project.Models.Enums;

namespace Project.Services
{
    /// <summary>
    /// Production usage examples showing how to invoke INotificationService across all core workflow events in Spaceara.
    /// </summary>
    public class NotificationExamples
    {
        private readonly INotificationService _notificationService;

        public NotificationExamples(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // 1. Student submits application -> Notify Department Head & University Admin
        public async Task OnStudentSubmitApplicationAsync(string studentId, string studentName, int applicationId, string opportunityTitle, string departmentHeadUserId)
        {
            // Notify Department Head
            await _notificationService.NotifyUserAsync(
                userId: departmentHeadUserId,
                title: "طلب تدريب جديد",
                message: $"قام الطالب {studentName} بتقديم طلب تدريب جديد على الفرصة: '{opportunityTitle}'.",
                type: NotificationType.Application,
                url: $"/University/UniAdminTrainigRequests?id={applicationId}",
                icon: "assignment_ind",
                senderId: studentId,
                referenceId: applicationId.ToString(),
                referenceType: "TrainingApplication"
            );

            // Notify University Training Admins
            await _notificationService.NotifyRoleAsync(
                roleName: "UniversityTrainingAdmin",
                title: "طلب تدريب جديد وارد",
                message: $"تم تقديم طلب جديد من قبل الطالب {studentName} للتخصص المحول.",
                type: NotificationType.Application,
                url: $"/University/UniAdminTrainigRequests?id={applicationId}",
                icon: "assignment_ind",
                referenceId: applicationId.ToString(),
                referenceType: "TrainingApplication"
            );
        }

        // 2. Application Accepted -> Notify Student
        public async Task OnApplicationAcceptedAsync(string studentUserId, int applicationId, string opportunityTitle, string reviewerName)
        {
            await _notificationService.NotifyUserAsync(
                userId: studentUserId,
                title: "تمت الموافقة على طلبك 🎉",
                message: $"تهانينا! تمت الموافقة على طلب التدريب الميداني للفرصة '{opportunityTitle}'.",
                type: NotificationType.Success,
                url: $"/Student/MyApplications",
                icon: "task_alt",
                referenceId: applicationId.ToString(),
                referenceType: "TrainingApplication"
            );
        }

        // 3. Application Rejected -> Notify Student
        public async Task OnApplicationRejectedAsync(string studentUserId, int applicationId, string opportunityTitle, string reason)
        {
            await _notificationService.NotifyUserAsync(
                userId: studentUserId,
                title: "حالة طلب التدريب الميداني",
                message: $"نحيطكم علماً بأنه لم يتم قبول طلبكم للفرصة '{opportunityTitle}'. السبب: {reason}",
                type: NotificationType.Warning,
                url: $"/Student/MyApplications",
                icon: "cancel",
                referenceId: applicationId.ToString(),
                referenceType: "TrainingApplication"
            );
        }

        // 4. Training Assigned -> Notify Student & Institution Supervisor
        public async Task OnTrainingAssignedAsync(string studentUserId, string supervisorUserId, string institutionName, string startDate)
        {
            // Notify Student
            await _notificationService.NotifyUserAsync(
                userId: studentUserId,
                title: "تنسيق موقع التدريب",
                message: $"تم اعتماد تدريبك الميداني في '{institutionName}' ابتداءً من تاريخ {startDate}.",
                type: NotificationType.Training,
                url: "/Student/CurrentTraining",
                icon: "business_center",
                referenceType: "TrainingPlacement"
            );

            // Notify Supervisor
            await _notificationService.NotifyUserAsync(
                userId: supervisorUserId,
                title: "تعيين طالب جديد للتدريب",
                message: $"تم إسناد طالب جديد لإشرافك في جهة التدريب '{institutionName}'.",
                type: NotificationType.Training,
                url: "/Institution/CurrentTraining",
                icon: "group_add",
                referenceType: "TrainingPlacement"
            );
        }

        // 5. Evaluation Submitted -> Notify University Admin & Supervisor
        public async Task OnEvaluationSubmittedAsync(string evaluatorName, string studentName, string studentUserId, int evaluationId)
        {
            // Notify Student
            await _notificationService.NotifyUserAsync(
                userId: studentUserId,
                title: "إيداع تقييم ميداني جديد",
                message: $"قام المشرف {evaluatorName} بإرسال التقييم الميداني الخاص بك.",
                type: NotificationType.Evaluation,
                url: "/Student/Evaluations",
                icon: "grading",
                referenceId: evaluationId.ToString(),
                referenceType: "StudentEvaluation"
            );
        }

        // 6. Evaluation Completed -> Notify Student
        public async Task OnEvaluationCompletedAsync(string studentUserId, double finalScore, string evaluationTitle)
        {
            await _notificationService.NotifyUserAsync(
                userId: studentUserId,
                title: "إكتمال التقييم النهـائي",
                message: $"تم اكتمال تقييم '{evaluationTitle}' بنجاح. الدرجة الإجمالية: {finalScore}%.",
                type: NotificationType.Success,
                url: "/Student/Evaluations",
                icon: "workspace_premium",
                referenceType: "StudentEvaluation"
            );
        }

        // 7. Supervisor Comment -> Notify Student
        public async Task OnSupervisorCommentAddedAsync(string studentUserId, string supervisorName, string reportTitle, string commentSnippet)
        {
            await _notificationService.NotifyUserAsync(
                userId: studentUserId,
                title: "ملاحظة جديدة من المشرف",
                message: $"أضاف المشرف {supervisorName} تعليقاً على التقرير '{reportTitle}': \"{commentSnippet}\"",
                type: NotificationType.Message,
                url: "/Student/Reports",
                icon: "insert_comment",
                referenceType: "StudentReport"
            );
        }

        // 8. System Announcement -> Broadcast to All Users or Specific Role
        public async Task OnSystemAnnouncementAsync(string title, string announcementBody, string? roleTarget = null)
        {
            if (string.IsNullOrEmpty(roleTarget))
            {
                // Broadcast to Everyone
                await _notificationService.NotifyAllAsync(
                    title: $"📢 إعلان نظام: {title}",
                    message: announcementBody,
                    type: NotificationType.System,
                    url: null,
                    icon: "campaign",
                    referenceType: "SystemAnnouncement"
                );
            }
            else
            {
                // Target Specific Role
                await _notificationService.NotifyRoleAsync(
                    roleName: roleTarget,
                    title: $"📢 إعلان مهم: {title}",
                    message: announcementBody,
                    type: NotificationType.System,
                    url: null,
                    icon: "campaign",
                    referenceType: "SystemAnnouncement"
                );
            }
        }
    }
}
