$(document).ready(function () {
    let isDropdownOpen = false; // État pour suivre si le dropdown est ouvert

    $('#notificationBell').on('click', function (e) {
        e.preventDefault();

        
    });

    // Retirer l'écouteur global pour empêcher la fermeture automatique
});


$(document).ready(function () {
    function fetchNotifications() {

        console.log('Notification111:');
        $.ajax({
            url: '/Notification/GetLatestNotifications', // Use Url.Action to generate the correct URL
            type: 'GET',
            dataType: 'json',
            success: function (response) {
                if (response.success) {
                    console.log('0000:');
                    console.log('Notifications:', response.notifications);
                    var notificationsHtml = '';
                    $.each(response.notifications, function (index, notification) {
                        notificationsHtml += `
                            <div class="notification-item">
                                <div class="notification-header">
                                    ${notification.UserNames.map(name => `<span class="username">${name}</span>`).join('')}
                                </div>
                                <p class="message">${notification.Message}</p>
                                <span class="date">${notification.CreatedDate}</span>
                            </div>
                        `;
                        console.log("333333")
                    });
                    console.log("4444")
                    $('#notificationDropdown').html(notificationsHtml);
                    $('#notificationCount').text(response.notifications.length);
                } else {
                    console.error('Error fetching notifications: ' + response.message);
                }
            },
            error: function (xhr, status, error) {
                console.error('Error: ' + xhr.status + ' - ' + xhr.statusText);
            }
        });
    }

    // Call fetchNotifications every 2 seconds
    setInterval(fetchNotifications, 2000);

    // Optionally, fetch notifications immediately when the page loads
    fetchNotifications();
});








function toggleAccountStatus(userId, isActivated) {

    $.ajax({
        url: isActivated ? '/Account/ActivateAccount' : '/Account/DeactivateAccount',
        type: 'POST',
        data: {
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
            userId: userId
        },
        success: function (response) {
            if (response.success) {
                swal("Succès !", "Vous avez changé Le Status du Compte", "success").then(function () {


                });
            } else {

                swal("information", response.message, "error").then(function () {


                });
            }
        },
        error: function () {
            alert('An error occurred while updating account status.');
        }
    });
}




function EmailForgotModal(row) {
    // Extract information from the clicked row

    var email = row.find('td:eq(3)').text().trim();




    $('#emailPlaceholder').text(email).css('color', '#3C91E6');

}

// Event listener for edit button clicks
$(document).on('click', '.forgot-button', function () {

    var row = $(this).closest('tr');
    EmailForgotModal(row);
});

function displayRowInformation(row) {

    // Find the <td> element containing the image and title
    var tdElement = row.find('td:first-child');

    // Find the <p> tag within the <td> element and get its text
    var titre = tdElement.find('p').text();

    // Get the alt attribute of the image
    var nom = row.find('td:eq(1)').text();
    var prenom = row.find('td:eq(2)').text();
    var profession = row.find('td:eq(3)').text(); // Get the text of the third td element
    var email = row.find('td:eq(4)').text();
    // var dateAjout = row.find('td:eq(4)').text();
    var role = row.find('td:eq(5)').text(); // Get the text of the fourth td element
  
    // Display information in the modal
    $('#labelNom').text(nom).css('color', '#3C91E6');
    $('#labelPrenom').text(prenom).css('color', '#3C91E6');
    $('#labelProfession').text(profession).css('color', '#3C91E6');
    $('#labelEmail').text(email).css('color', '#3C91E6');
    //$('#labelDateAjout').text(dateAjout).css('color', '#3C91E6');
    $('#labelRole').text(role).css('color', '#3C91E6');

    // Show the modal
    $('#exampleModal').modal('show');
}

$(document).on('click', 'tbody tr', function () {
    var row = $(this);
    displayRowInformation(row);
});

function populateUpdateModal(row) {
    // Extract information from the clicked row
    var nom = row.find('td:eq(1)').text().trim();
    var profession = row.find('td:eq(2)').text().trim();
    var email = row.find('td:eq(3)').text().trim();
    var role = row.find('td:eq(5) select').val().trim();
    var userId = row.find('td:eq(6)').text().trim();
    var dateAjout = row.find('td:eq(4)').text().trim(); // e.g., "20/11/2024 00:00:00"

    // Function to format the date for input (yyyy-mm-dd) and display (dd/mm/yyyy)
    function formatDateForInput(dateStr) {
        var parts = dateStr.split(" ")[0].split('/'); // Extract date part and split by "/"
        var day = parts[0];
        var month = parts[1];
        var year = parts[2];
        return {
            displayDate: `${day}/${month}/${year}`, // For display in dd/mm/yyyy format
            inputDate: `${year}-${month}-${day}`    // For <input type="date"> in yyyy-mm-dd format
        };
    }

    var formattedDate = formatDateForInput(dateAjout);

    // Fill the update modal with the extracted information
    $('#inputNomUpdate').val(nom).css('color', '#3C91E6');
    $('#inputProfessionUpdate').val(profession).css('color', '#3C91E6');
    $('#inputEmailUpdate').val(email).css('color', '#3C91E6');
    $('#UserIdUpdateModel').val(userId).css('color', '#3C91E6');
    $('#inputDateAjoutUpdate').val(formattedDate.inputDate).css('color', '#3C91E6');

    // Show the display format in a separate field or as a placeholder if needed
    $('#inputDateAjoutUpdate').attr('placeholder', formattedDate.displayDate); // Optional placeholder

    // Reset radio buttons to unchecked state before checking the correct one
    $('#inputRoleAdmin').prop('checked', false);
    $('#inputRoleUser').prop('checked', false);

    // Set the correct radio button based on the role
    if (role === "Admin") {
        $('#inputRoleAdmin').prop('checked', true);
    } else if (role === "User") {
        $('#inputRoleUser').prop('checked', true);
    }
}

// Event listener for edit button clicks
$(document).on('click', '.edit-button', function () {
    var row = $(this).closest('tr');
    populateUpdateModal(row);
});



$(document).ready(function () {
    // Intercept the form submission
    $('.editForm').submit(function (event) {
        event.preventDefault(); // Prevent default form submission
      
        // Get the selected role value from the radio buttons
        var role = $("input[name='Role']:checked").val(); // Get the selected radio button value
       
        var data = {
            UserId: $('#UserIdUpdateModel').val(),
            Nom: $('#inputNomUpdate').val(),
            DateNaissance: $('#inputDateAjoutUpdate').val(), // Make sure the date is in the correct format (e.g., YYYY-MM-DD)
            Proffession: $('#inputProfessionUpdate').val(),
            Email: $('#inputEmailUpdate').val(),
            Role: role, // Set the role based on the selected radio button
            __RequestVerificationToken: $('[name=__RequestVerificationToken]').val() // Include the AntiForgeryToken
        };
       
        $.ajax({
            url: '/Account/EditUser',
            type: 'POST',
            data: data, // Send the data as is
            success: function (response) {
                if (response.success) {
                    swal("Succès !", response.message, "success").then(function () {
                        $('#UpdateUtilisateurModal').modal('hide');
                        var updatedUser = response.updatedUser;
                        var row = $('tr[data-user-id="' + updatedUser.Id + '"]'); // Assuming each row has a data attribute for user ID

                        // Update each cell in the row with the updated data
                        row.find('td:eq(1)').text(updatedUser.Nom); // Nom Complet
                        row.find('td:eq(2)').text(updatedUser.Profession); // Profession
                        row.find('td:eq(3)').text(updatedUser.Email); // Email
                        row.find('td:eq(4)').text(new Date(updatedUser.DateNaissance).toLocaleDateString('fr-FR')); // Date de Naissance
                        row.find('td:eq(5)').text(updatedUser.Role); // Ro
             
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


//user can update info




function getDateFromString(dateString) {
    // Split the date string into day, month, and year parts
    var parts = dateString.split('/');
    var day = parseInt(parts[0], 10);
    var month = parseInt(parts[1], 10);
    var year = parseInt(parts[2], 10);

    // Create a new Date object with the extracted parts
    return new Date(year, month - 1, day); // Month is zero-based in Date constructor
}

function setDateValues() {
    var dateReceptionInput = document.getElementById("inputDateReceptionUpdate");
    var datePublicationInput = document.getElementById("inputDatePublicationUpdate");
    var dateDerniereMiseAJourInput = document.getElementById("inputDateDerniereMiseAJourUpdate");
    var dateAjoutInput = document.getElementById("inputDateAjoutUpdate");


    // Get the date strings from the table cells
    var dateReceptionString = document.getElementById("dateReceptionCell").textContent.trim();
    var datePublicationString = document.getElementById("datePublicationCell").textContent.trim();
    var dateDerniereMiseAJourString = document.getElementById("dateDerniereMiseAJourCell").textContent.trim();
    var dateAjoutString = document.getElementById("dateAjoutCell").textContent.trim();


    // Convert the date strings to Date objects
    var dateReception = getDateFromString(dateReceptionString);
    var datePublication = getDateFromString(datePublicationString);
    var dateDerniereMiseAJour = getDateFromString(dateDerniereMiseAJourString);
    var dateAjout = getDateFromString(dateAjoutString);


    // Format the dates into yyyy-mm-dd format (required by input[type="date"])
    var formattedDateReception = dateReception.toISOString().split('T')[0];
    var formattedDatePublication = datePublication.toISOString().split('T')[0];
    var formattedDateDerniereMiseAJour = dateDerniereMiseAJour.toISOString().split('T')[0];
    var formattedDateAjout = dateAjout.toISOString().split('T')[0];


    // Set the formatted dates to the input fields
    dateReceptionInput.value = formattedDateReception;
    datePublicationInput.value = formattedDatePublication;
    dateDerniereMiseAJourInput.value = formattedDateDerniereMiseAJour;
    dateAjoutInput.value = formattedDateAjout;

}

// Call the function to set the date values when needed
setDateValues();


//get notification

function getUserInfo(userId, callback) {
    $.ajax({
        url: '/Notification/GetUserInfo', // Update with your actual controller name and action
        type: 'GET',
        data: { userId: userId },
        success: function (data) {
            if (callback) callback(data);
        },
        error: function () {
            alert('Une erreur est survenue lors de la récupération des informations de l\'utilisateur');
        }
    });
}