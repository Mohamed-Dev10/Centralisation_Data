$(document).ready(function() {

  


    $('#resetPasswordLink').click(function (e) {

      
        e.preventDefault();

        var email = $('#emailPlaceholder').text();
        var token = $('input[name="__RequestVerificationToken"]').val();
      
        $.ajax({
            type: 'POST',
            url: '/Account/ForgotPassword',
            data: {
                __RequestVerificationToken: token,
                Email: email
            },
            success: function (response) {
                if (response.success) {
                    swal("Succès !", "le code du réinitialisation à été envoyé sur votre email avec succès!", "success").then(function () {
                        

                    });
                  
                } else {
                    alert(response.message);
                }
            }
        });
    });

    $('#verifyCodeButton').click(function (e) {
        e.preventDefault();

        var email = $('#emailPlaceholder').text();
        var verificationCode = $('#verificationCodeInput').val();
        var token = $('input[name="__RequestVerificationToken"]').val();

        $.ajax({
            type: 'POST',
            url: '/Account/VerifyCode',
            data: {
                __RequestVerificationToken: token,
                Email: email,
                VerificationCode: verificationCode
            },
            success: function (response) {
                if (response.success) {
                    $('#resetPasswordSection').show();
                } else {
                    alert(response.message);
                }
            }
        });
    });

    $('#resetPasswordButton').click(function (e) {
        e.preventDefault();

        var email = $('#emailPlaceholder').text();
        var password = $('#passwordInput').val();
        var confirmPassword = $('#confirmPasswordInput').val();
        var verificationCode = $('#verificationCodeInput').val();
        var token = $('input[name="__RequestVerificationToken"]').val();

        $.ajax({
            type: 'POST',
            url: '/Account/ResetPassword',
            data: {
                __RequestVerificationToken: token,
                Email: email,
                PasswordResetCode: verificationCode,
                Password: password,
                ConfirmPassword: confirmPassword
            },
            success: function (response) {
                if (response.success) {
                    $('#forgotPasswordModal').modal('hide');
                    alert('Password has been reset successfully.');
                } else {
                    alert(response.message);
                }
            }
        });
    });
});



