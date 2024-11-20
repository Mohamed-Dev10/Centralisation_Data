$(document).ready(function () {
    $('#saveChangesBtn').click(function (e) {
        e.preventDefault(); // Prevent the default form submission

        var form = $('#editUserForm');
        $.ajax({
            type: 'POST',
            url: '/Account/EditUser', // Ensure this URL is correct
            data: form.serialize(),
            success: function (response) {
                if (response.success) {
                    swal("Succès !", "Utilisateur  mises à jour avec succès!", "success").then(function () {
                        $('#ModalEdit').modal('hide');

                    });
                   
                   
                } else {
                   
                    swal("information", response.message, "error").then(function () {
                       


                    });
                    $.each(response.errors, function (index, error) {
                        console.log(error);
                    });
                }
            },
            error: function (xhr, status, error) {
                alert('Une erreur est survenue lors de la mise à jour.');
                console.log('Status: ' + status);
                console.log('Error: ' + error);
                console.log('Response: ' + xhr.responseText);
            }
        });
    });
});
