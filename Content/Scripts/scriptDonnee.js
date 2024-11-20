function displaySuccessMessage() {
    // Obtenir les paramètres de la requête
    var params = new URLSearchParams(window.location.search);
    console.log("URL Params:", params.toString()); // Ajouté pour debug
    if (params.has('success')) {
        var message = "Les données ont été enregistrées avec succès.";
        var alertDiv = $('<div class="alert alert-success"></div>').text(message);
        $('body').prepend(alertDiv); // Ajoute le message au début du body ou à l'endroit désiré

        // Ajouter un délai pour faire disparaître le message après quelques secondes
        setTimeout(function () {
            alertDiv.fadeOut('slow', function () {
                $(this).remove();
            });
        }, 5000); // 5000 millisecondes = 5 secondes
    }
}

// Appel de la fonction après le chargement de la page
$(document).ready(function () {
    displaySuccessMessage();
});


   


function deleteData(button) {
    var id = $(button).data("id");
    if (confirm("Êtes-vous sûr de vouloir supprimer cet élément ?")) {
        $.ajax({
            url: '/Data/Delete', // Assurez-vous que cette URL correspond à l'action Delete dans votre contrôleur
            type: 'POST',
            data: { id: id },
            success: function (result) {
                if (result.success) {
                    alert("L'élément a été supprimé avec succès.");
                    location.reload(); // Recharger la page pour refléter les changements
                } else {
                    alert("La suppression a échoué : " + result.message);
                }
            },
            error: function (xhr, status, error) {
                // Gérer la réponse d'erreur
                alert("Une erreur est survenue lors de la suppression de l'élément.");
            }
        });
    }
}

document.getElementById('filterIconData').addEventListener('click', function () {
    var searchInput = document.getElementById('searchInputData');
    searchInput.classList.toggle('hidden');
});

document.getElementById('searchInputData').addEventListener('input', function () {
    var query = this.value;

    $.ajax({
        url: '/Search/Data',
        type: 'GET',
        data: { query: query },
        success: function (response) {
            console.log(response);
            var resultsTableBody = document.getElementById('dataBody');
            resultsTableBody.innerHTML = ''; // Vider les résultats précédents

            response.forEach(function (data) {
                var row = document.createElement('tr');
                row.innerHTML = `
                                                                                                                                            <td>${data.Title}</td>
                                                                                                                                            <td>${new Date(data.AcquisitionDate).toLocaleDateString()}</td>
                                                                                                                                            <td>${new Date(data.PublicationDate).toLocaleDateString()}</td>
                                                                                                                                            <td>${new Date(data.LastUpdatedDate).toLocaleDateString()}</td>
                                                                                                                                            <td>${data.Description}</td>
                                                                                                                                            <td>${data.Category}</td>
                                                                                                                                            <td>${data.ThemeName || ''}</td>
                                                                                                                                            <td>${data.Coverage}</td>
                                                                                                                                            <td>${data.SpatialResolution}</td>
                                                                                                                                            <td>${data.Summary}</td>
                                                                                                                                            <td>
                                                                                                                                                <span class="status completed downdata-button">
                                                                                                                                                    <i class='bx bxs-download' style="font-size: 14px;" onclick="triggerFileUpload(${data.IdData})"></i>
                                                                                                                                                </span>
                                                                                                                                                <button type="button" class="status completed edit-button" style="background-color: green;" data-toggle="modal" data-target="#ModalUpdating" data-id="${data.IdData}" onclick="populatedouni(this)">
                                                                                                                                                    <i class='bx bx-edit' style="font-size: 14px;"></i>
                                                                                                                                                </button>
                                                                                                                                                <span class="status completed show-button" style="background-color: rgb(224, 159, 125);" data-toggle="modal" data-target="#exampleModal">
                                                                                                                                                    <i class='bx bx-show' style="font-size: 14px;"></i>
                                                                                                                                                </span>
                                                                                                                                                <button type="button" class="status completed delete-button" style="background-color: red;" data-id="${data.IdData}" onclick="deleteData(this)">
                                                                                                                                                    <i class='bx bx-trash' style="font-size: 14px;"></i>
                                                                                                                                                </button>
                                                                                                                                            </td>
                                                                                                                                        `;
                resultsTableBody.appendChild(row);
            });

            document.getElementById('dataTable').classList.remove('hidden');
        },
        error: function () {
            alert("Une erreur s'est produite lors de la recherche dans les données.");
        }
    });
});

// Événement pour afficher ou masquer le champ de recherche lorsque l'icône de filtre est cliquée
document.getElementById('filterIconDatabase').addEventListener('click', function () {
    var searchInput = document.getElementById('searchInputDatabase');
    searchInput.classList.toggle('hidden');
});

// Événement pour effectuer la recherche lorsque l'utilisateur tape dans le champ de recherche
document.getElementById('searchInputDatabase').addEventListener('input', function () {
    var query = this.value;

    $.ajax({
        url: '/Search/Database',
        type: 'GET',
        data: { query: query },
        success: function (response) {
            var resultsTableBody = document.getElementById('searchResultsBody');
            resultsTableBody.innerHTML = ''; // Vider les résultats précédents

            if (response && response.length > 0) {
                response.forEach(function (database) {
                    var row = document.createElement('tr');
                    // Vérifier et parser la date de création
                    var createdDate = 'Non défini';
                    if (database.createdDate) {
                        var date = new Date(database.createdDate);
                        if (!isNaN(date.getTime())) {
                            createdDate = date.toLocaleDateString();
                        }
                    }
                    row.innerHTML = `
                                                                                                                                                <td>${database.DataBaseName || 'Non défini'}</td>
                                                                                                                                                <td>${database.Owner || 'Non défini'}</td>
                                                                                                                                                <td>${database.createdDate} </td>
                                                                                                                                                <td>${database.description || 'Non défini'}</td>
                                                                                                                                                <td style="text-align: center;">
                                                                                                                                                    <span class="status completed downdata-button" onclick="downloadFile('${database.DataBaseName}')">
                                                                                                                                                        <i class='bx bxs-download' style="font-size: 14px;"></i>
                                                                                                                                                    </span>
                                                                                                                                                    <span class="status completed edit-button" style="background-color: green;" data-toggle="modal" data-target="#ModalBasedonneeUpdating" data-id="${database.idDataBase}" onclick="EditBase(this)">
                                                                                                                                                        <i class='bx bx-edit' style="font-size: 14px;"></i>
                                                                                                                                                    </span>
                                                                                                                                                    <span class="status completed show-button" style="background-color: rgb(224, 159, 125);" data-toggle="modal" data-target="#exampleBasedonneeModal">
                                                                                                                                                        <i class='bx bx-show' style="font-size: 14px;"></i>
                                                                                                                                                    </span>
                                                                                                                                                </td>
                                                                                                                                                `;
                    resultsTableBody.appendChild(row);
                });

                // Afficher le tableau si des résultats sont présents
                document.getElementById('searchResultsTableDatabase').style.display = 'table';
            } else {
                // Cacher le tableau si aucun résultat n'est présent
                document.getElementById('searchResultsTableDatabase').style.display = 'none';
            }
        },
        error: function () {
            alert("Une erreur s'est produite lors de la recherche dans les bases de données.");
        }
    });
});





 
function displayRowInformation(row) {
    // Extraire les informations de la ligne cliquée
    var tdElement = row.find('td:first-child'); // Trouver l'élément <td> contenant l'image et le titre
    var titre = tdElement.find('p').text(); // Trouver la balise <p> à l'intérieur de l'élément <td> et obtenir son texte

    // Obtenez les informations des autres cellules de la ligne
    var dateRéception = row.find('td:eq(1)').text();
    var datePublication = row.find('td:eq(2)').text();
    var dateDernièreMiseàJour = row.find('td:eq(3)').text();
    var Description = row.find('td:eq(4)').text();
    var categorie = row.find('td:eq(5)').text();
    var theme = row.find('td:eq(6)').text();
    var couverture = row.find('td:eq(7)').text();
    var resolutionSpatial = row.find('td:eq(8)').text();
    var resume = row.find('td:eq(9)').text();

    // Afficher les informations dans le modal
    $('#labelTitre').text(titre).css('color', '#3C91E6');
    $('#labelDateRéception').text(dateRéception).css('color', '#3C91E6');
    $('#labelDatePublication').text(datePublication).css('color', '#3C91E6');
    $('#labelDateDernièreMiseàJour').text(dateDernièreMiseàJour).css('color', '#3C91E6');
    $('#labelDescription').text(Description).css('color', '#3C91E6');
    $('#labelCategorie').text(categorie).css('color', '#3C91E6');
    $('#labelTheme').text(theme).css('color', '#3C91E6');
    $('#labelCouverture').text(couverture).css('color', '#3C91E6');
    $('#labelResolutionSpatial').text(resolutionSpatial).css('color', '#3C91E6');
    $('#labelResume').text(resume).css('color', '#3C91E6');

    // Afficher le modal
    // $('#exampleModal').modal('show');
}

// Ajouter un gestionnaire d'événements pour les clics sur le bouton d'affichage
$(document).on('click', 'tbody tr', function() {
    var row = $(this);
    displayRowInformation(row);
});


function displayRowBaseInformation(row) {
    var baseDeDonnee = row.find('td:nth-child(1)').text();
    var proprietaire = row.find('td:nth-child(2)').text();
    var dateCreation = row.find('td:nth-child(3)').text();
    var description = row.find('td:nth-child(4)').text();

    $('#detailNom').text(baseDeDonnee).css('color', '#3C91E6');
    $('#detailProprietaire').text(proprietaire).css('color', '#3C91E6');
    $('#detailDateDeCreation').text(dateCreation).css('color', '#3C91E6');
    $('#detailDescription').text(description).css('color', '#3C91E6');

    //$('#exampleBasedonneeUserModal').modal('show');
}
// Ajouter un gestionnaire d'événements pour les clics sur le bouton d'affichage
$(document).on('click', 'tbody tr', function() {
    var row = $(this);
    displayRowBaseInformation(row);
});


function populateUpdateModal(editButton) {
    // Get the closest row to the clicked edit button
    var row = $(editButton).closest("tr");
    
    // Extract information from the row
    var titre = row.find("td:eq(0) p").text();
    var dateRéception = row.find("td:eq(1)").text();
    var datePublication = row.find("td:eq(2)").text();
    var dateDernièreMiseàJour = row.find("td:eq(3)").text();
    var Description = row.find("td:eq(4)").text();
    var categorie = row.find("td:eq(5)").text();
    var theme = row.find("td:eq(6)").text();
    var couverture = row.find("td:eq(7)").text();
    var resolutionSpatial = row.find("td:eq(8)").text();
    var resume = row.find("td:eq(9)").text();
    
    function formatDate(dateStr) {
        var parts = dateStr.split('/');
        return `${parts[2]}-${parts[1]}-${parts[0]}`;
    }
    
    
    // Populate input fields in the modal with extracted information
    $('#inputTitreUpdate').val(titre).css('color', '#3C91E6');
    $('#inputDateRéceptionUpdate').val(formatDate(dateRéception)).css('color', '#3C91E6');
    $('#inputDatePublicationUpdate').val(formatDate(datePublication)).css('color', '#3C91E6');
    $('#inputDateDernierMiseàJourUpdate').val(formatDate(dateDernièreMiseàJour)).css('color', '#3C91E6');
    $('#inputDescriptionUpdate').val(Description).css('color', '#3C91E6');
    $('#inputCategorieUpdate').val(categorie).css('color', '#3C91E6');
    $('#inputThemeUpdate').val(theme).css('color', '#3C91E6');
    $('#inputCouvertureUpdate').val(couverture).css('color', '#3C91E6');
    $('#inputResolutionSpatialUpdate').val(resolutionSpatial).css('color', '#3C91E6');
    $('#inputResumeUpdate').val(resume).css('color', '#3C91E6');
    
// Sélection de tous les éléments de case à cocher dans la liste déroulante
    var checkboxes = document.querySelectorAll('.dropdown-menu input[type="checkbox"]');
   

// Fonction pour mettre à jour le champ de recherche en fonction des cases cochées
function updateSearchFilter() {
    var selectedOptions = [];

    // Parcours de toutes les cases à cocher
    checkboxes.forEach(function(checkbox) {
        // Si la case est cochée, ajoute sa valeur à la liste des options sélectionnées
        if (checkbox.checked) {
            selectedOptions.push(checkbox.value);
        }
    });

    // Mise à jour du champ de recherche avec les options sélectionnées
    var searchFilter = selectedOptions.join(', ');
    document.getElementById('dropdownFilter').value = searchFilter;
}

// Ajout d'un écouteur d'événements pour chaque case à cocher
checkboxes.forEach(function(checkbox) {
    checkbox.addEventListener('change', function() {
        // Lorsqu'une case est cochée ou décochée, mettre à jour le champ de recherche
        updateSearchFilter();
    });
});
}
 // Stocker la ligne actuelle pour une utilisation ultérieure
 $('#updateDonneeForm').data('currentRow', row);

 // Afficher le modal de mise à jour du client
// $('#UpdateDonneeModal').modal('show');

// Ajouter un écouteur d'événements pour les clics sur le bouton d'édition
$(document).on('click', '.edit-button', function() {
 var row = $(this).closest('tr');
 populateUpdateModal(row);
});


function populateUpdateBaseModal(editButton) {
    // Get the closest row to the clicked edit button
    var row = $(editButton).closest("tr");
    
    // Extract information from the row
    var baseDeDonnee = row.find('td:eq(0)').text();
    var proprietaire = row.find('td:eq(1)').text();
    var dateCreation = row.find('td:eq(2)').text();
    var description = row.find('td:eq(3)').text();
    
    function formatDate(dateStr) {
        var parts = dateStr.split('/');
        return `${parts[2]}-${parts[1]}-${parts[0]}`;
    }
    
    
    // Populate input fields in the modal with extracted information
    $('#inputBasededonneeUpdate').val(baseDeDonnee).css('color', '#3C91E6');
    $('#inputProprietaireUpdate').val(proprietaire).css('color', '#3C91E6');
    $('#inputDateDeCreationUpdate').val(formatDate(dateCreation)).css('color', '#3C91E6');
    $('#inputDescriptionssUpdate').val(description).css('color', '#3C91E6');
}
 // Stocker la ligne actuelle pour une utilisation ultérieure
 $('#updateBaseForm').data('currentRow', row);

 // Afficher le modal de mise à jour du client
// $('#UpdateBaseeModal').modal('show');

// Ajouter un écouteur d'événements pour les clics sur le bouton d'édition
$(document).on('click', '.edit-button', function() {
    var row = $(this).closest('tr');
    populateUpdateBaseModal(row);
});

// Function to toggle visibility of search input field
document.getElementById("filterIcon").addEventListener("click", function() {
    var searchInput = document.getElementById("searchInput");
    if (searchInput.classList.contains("hidden")) {
        searchInput.classList.remove("hidden");
    } else {
        searchInput.classList.add("hidden");
        // Clear search input and reset table rows when hiding the input
        searchInput.value = "";
        filterTable();
    }
});

// Function to filter table rows based on search input
function filterTable() {
    var input, filter, table, tr, td, i, txtValue;
    input = document.getElementById("searchInput");
    filter = input.value.toUpperCase();
    table = document.querySelector('.order table');
    tr = table.getElementsByTagName("tr");
    // Loop through all table rows, and hide those that don't match the search query
    for (i = 0; i < tr.length; i++) {
        td = tr[i].getElementsByTagName("td");
        for (var j = 0; j < td.length; j++) {
            if (td[j]) {
                txtValue = td[j].textContent || td[j].innerText;
                if (txtValue.toUpperCase().indexOf(filter) > -1) {
                    tr[i].style.display = "";
                    break;
                } else {
                    tr[i].style.display = "none";
                }
            }
        }
    }
}

// Add event listener to search input field
document.getElementById("searchInput").addEventListener("input", filterTable);


// JavaScript to handle dropdown with checkboxes
$(document).ready(function() {
    // Toggle dropdown
    $('.dropdown-checkbox').on('click', function(event) {
        $(this).toggleClass('show');
        event.stopPropagation();
    });

    // Close dropdown when clicking outside
    $(document).on('click', function(event) {
        if (!$(event.target).closest('.dropdown-checkbox').length) {
            $('.dropdown-checkbox').removeClass('show');
        }
    });

    // Handle checkbox selection
    $('.dropdown-menu input[type="checkbox"]').on('change', function() {
        var selectedOptions = [];
        $('.dropdown-menu input[type="checkbox"]:checked').each(function() {
            selectedOptions.push($(this).val());
        });
        $('#inputCouvertureUpdate').val(selectedOptions.join(', '));
    });
});


(function($) {
    var CheckboxDropdown = function(el) {
        // JavaScript code for handling the dropdown checkboxes
        // You can place the provided JavaScript code here
    };

    // Initialize the checkbox dropdowns
    var checkboxesDropdowns = document.querySelectorAll('[data-control="checkbox-dropdown"]');
    for (var i = 0, length = checkboxesDropdowns.length; i < length; i++) {
        new CheckboxDropdown(checkboxesDropdowns[i]);
    }
})(jQuery);

function toggleDropdown() {
    var dropdownMenu = document.querySelector('.dropdown-menu');
    dropdownMenu.classList.toggle('show');
}

function filterCheckboxes() {
    var input, filter, ul, li, checkboxes, i;
    input = document.getElementById("dropdownFilter");
    filter = input.value.toUpperCase();
    checkboxes = document.querySelectorAll('.dropdown-menu input[type="checkbox"]');

    checkboxes.forEach(function(checkbox) {
        var label = checkbox.parentElement;
        if (label.textContent.toUpperCase().indexOf(filter) > -1) {
            label.style.display = "";
        } else {
            label.style.display = "none";
        }
    });
}

function getDateFromString(dateString) {
    // Split the date string into day, month, and year parts
    var parts = dateString.split('/');
    var day = parseInt(parts[0], 10);
    var month = parseInt(parts[1], 10);
    var year = parseInt(parts[2], 10);

    // Create a new Date object with the extracted parts
    return new Date(year, month - 1, day); // Month is zero-based in Date constructor
}

 


//function setDateValues() {
    var dateRéceptionInput = document.getElementById("inputDateRéceptionUpdate");
    var datePublicationInput = document.getElementById("inputDatePublicationUpdate");
    var dateDernièrMiseàJourInput = document.getElementById("inputDateDernièrMiseàJourUpdate");
    var dateDeCreationInput = document.getElementById("inputDateDeCreationUpdate");


    // Get the date strings from the table cells
    var dateRéceptionString = document.getElementById("dateRéceptionCell").textContent.trim();
    var datePublicationString = document.getElementById("datePublicationCell").textContent.trim();
    var dateDernièrMiseàJourString = document.getElementById("dateDernièrMiseàJourCell").textContent.trim();
    var dateDeCreationString = document.getElementById("dateDeCreationCell").textContent.trim();


    // Convert the date strings to Date objects
    var dateRéception = getDateFromString(dateRéceptionString);
    var datePublication = getDateFromString(datePublicationString);
    var dateDernièreMiseàJour = getDateFromString(dateDernièrMiseàJourString);
    var dateDeCreation = getDateFromString(dateDeCreationString);


    // Format the dates into yyyy-mm-dd format (required by input[type="date"])
    var formattedDateRéception = dateRéception.toISOString().split('T')[0];
    var formattedDatePublication = datePublication.toISOString().split('T')[0];
    var formattedDateDernièrMiseàJour = dateDernièreMiseàJour.toISOString().split('T')[0];
    var formattedDeCreation = dateDeCreation.toISOString().split('T')[0];


    // Set the formatted dates to the input fields
    dateAcquisitionInput.value = formattedDateAcquisition;
    datePublicationInput.value = formattedDatePublication;
    dateDernièrMiseàJourInput.value = formattedDateDernièrMiseàJour;
    dateDeCreationInput.value = formattedDateDeCreation;
//}


  

    // Close the modal
    $('#ModalBasedonneeAdding').modal('hide');


    // Close the modal
    $('#ModalBasedonneeUpdating').modal('hide');

    // Optionally, refresh the page or update the UI to reflect changes


// Call the function to set the date values when needed
setDateValues();




    

    


