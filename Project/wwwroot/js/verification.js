
document.addEventListener('DOMContentLoaded', () => {
    const inputs = document.querySelectorAll('.otp-input');
    const fullCodeInput = document.getElementById('fullCode');
    const form = document.querySelector('form');

    function updateFullCode() {
        let code = '';

        inputs.forEach(input => {
            code += input.value;
        });

        fullCodeInput.value = code;

        if (code.length === 6) {
            inputs.forEach(input => {
                input.classList.add('otp-complete');
            });
            //form.submit();
        }
        else {

            inputs.forEach(input => {
                input.classList.remove('otp-complete');
            });
        }
    }

    inputs.forEach((input, index) => {
        input.addEventListener('focus', function () {
            this.select();
        });
        input.addEventListener('input', function () {
            this.classList.remove('otp-error')

            this.classList.remove('otp-shake')

            this.value = this.value.replace(/\D/g, '');
            if (this.value && index < inputs.length - 1) {
                inputs[index + 1].focus();
            }
            updateFullCode();
        });

        input.addEventListener('keydown', function (e) {
            if (e.key === 'Backspace' && !this.value && index > 0) {
                inputs[index - 1].focus();
            }

            if (e.key === 'ArrowRight' && index < inputs.length - 1) {
                inputs[index + 1].focus();
            }

            if (e.key === 'ArrowLeft' && index > 0) {
                inputs[index - 1].focus();
            }

        });
        input.addEventListener('paste', function (e) {
            e.preventDefault();

            const pastedData = e.clipboardData
                .getData('text')
                .trim()
                .replace(/\D/g, '');

            const digits = pastedData.slice(0, 6).split('');

            digits.forEach((digit, i) => {
                if (inputs[i]) {
                    inputs[i].value = digit;
                }
            });
            updateFullCode();

            if (digits.length < 6) {
                inputs[digits.length].focus();

            } else {
                inputs[5].blur();
            }

        });

    });

    let timeLeft = 10 * 60;

    const timerElement = document.getElementById('timer');
    const verifyBtn = document.getElementById('verifyBtn');

    const countdown = setInterval(() => {
        const minutes = Math.floor(timeLeft / 60);
        const seconds = timeLeft % 60;

        timerElement.textContent = `${minutes}:${seconds.toString().padStart(2, '0')}`;

        if (timeLeft <= 0) {
            clearInterval(countdown);
            timerElement.textContent = 'Expired';

            inputs.forEach(input => {
                input.disabled = true;

            });
            verifyBtn.disabled = true;

            verifyBtn.classList.remove('btn-color');
            verifyBtn.classList.add('btn-danger')
            return;
        } timeLeft--;
    }, 1000)

    const resendBtn = document.getElementById('resendBtn');
    let resendSeconds = 60;
    let resendInterval;
    function startResendTimer() {
        resendBtn.disabled = true;
        resendInterval = setInterval(() => {
            resendBtn.textContent = `Resend Code After (${resendSeconds}s)`;
            resendSeconds--;
            if (resendSeconds <= 0) {
                clearInterval(resendInterval);

                resendBtn.disabled = false;
                resendBtn.textContent = 'Resend Code';

                resendSeconds = 60;
                return;
            }

        }, 1000);
    }

    startResendTimer();



    resendBtn.addEventListener('click', function () {

        const email = document.getElementById('emailHidden').value;

        resendSeconds = 60;
        clearInterval(resendInterval);
        startResendTimer();
        
        fetch('/Identity/Account/VerifyCodeEmail?handler=Resend', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken':
                    document.querySelector('input[name="__RequestVerificationToken"]').value
            },
            body: `Email=${encodeURIComponent(email)}`
        })

    });

    const validationError = document.querySelector('.validation-summary-errors');
    if (validationError) {
        inputs.forEach(input => {
            input.value = '';
            input.classList.remove('otp-complete');
            input.classList.add('otp-error');
            input.classList.add('otp-shake');
            setTimeout(() => {
                input.classList.remove('otp-shake');
            }, 350);
        });

        inputs[0].focus();

        fullCodeInput.value = '';
    }

});




