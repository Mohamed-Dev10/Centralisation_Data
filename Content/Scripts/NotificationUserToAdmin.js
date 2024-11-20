
$(document).ready(function () {
    $('#notifyAdminButton').click(function () {
        var userId = $(this).data('user-id');
        var userName = $(this).data('user-name');

        $.ajax({
            url: '/Notification/NotifyUser',
            type: 'POST',
            data: {
                userId: userId,
                userName: userName
            },
            success: function (response) {
                if (response.success) {
                    // Store the notification in localStorage
                    if (response.newNotification) {
                        var newNotification = response.newNotification;
                        //   localStorage.setItem('newNotification', JSON.stringify(newNotification));
                        // alert("storage item  " + localStorage.getItem('newNotification'))
                        swal("Succès !", "votre demande d'un nouveau mot de passe est traitée avec succès", "success").then(function () {
                            // Optionally redirect to the parameters page
                            appendNotification(userName);

                        });
                    }
                } else {
                    swal("Information", response.message, "error");
                }
            },
            error: function (xhr, status, error) {
                alert('Une erreur est survenue lors de l\'envoi de la notification.');
            }
        });
    });
});







function appendNotification(userName) {

    alert("append !!!")
    // Construct the notification message
    var notificationMessage = `L'utilisateur ${userName} a demandé un nouveau mot de passe.`;

    // Find the notification list or counter and update it
    var notificationList = $('.notification-list'); // Assuming you have a list element to append to
    var notificationCount = $('.notification .num'); // Assuming this is where you display the count

    // Append the message
    notificationList.append('<li>' + notificationMessage + '</li>'); // Update to match your structure

    // Update the notification count
    var currentCount = parseInt(notificationCount.text());

    alert(currentCount);
    notificationCount.text(currentCount + 1);
}


