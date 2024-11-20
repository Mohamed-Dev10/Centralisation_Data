

function addDatadon() {
    alert("1");
    var form = $('#dataForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    alert("2sss");

    var newData = {
        __RequestVerificationToken: token,
        Title: $('#inputTitre').val(),
        AcquisitionDate: $('#inputDateRéception').val(),
        PublicationDate: $('#inputDatePublication').val(),
        LastUpdatedDate: $('#inputDateDernièreMiseàJour').val(),
        Description: $('#inputDescription').val(),
        Category: $('#inputCategorie').val(),
        ThemeName: $('#inputTheme').val(),
        Coverage: $('input[name="Coverage"]:checked').map(function () { return this.value; }).get().join(','),
        SpatialResolution: $('#inputResolutionSpatial').val(),
        Summary: $('#inputResume').val()
    };

    console.log('newData:', newData);
    alert("3");

    $.ajax({
        url: '/Data/Create',
        type: 'POST',
        data: newData,
        success: function (response) {
            if (response.success) {
               // $('#ModalAdding').modal('hide'); // Close the modal after a successful add
                alert(response.message); // Show a success message
              //  location.reload(); // Reload the page to reflect changes
            } else {
                alert(response.message); // Show an error message if the add failed
            }
        },
        error: function () {
            alert("Une erreur s'est produite lors de l'ajout de la donnée.");
        }
    });
}

function EditBase(button) {
    var databaseId = $(button).attr('data-id');
    $('#databaseId').val(databaseId);

    $.ajax({
        url: '/DatabaseInfo/GetById/' + databaseId,
        type: 'GET',
        success: function (response) {
            if (response.success) {
                var data = response.data;

                $('#inputDataBaseNameUpdate').val(data.DataBaseName);
                $('#inputOwnerUpdate').val(data.Owner);
                $('#inputCreationDateUpdate').val(data.createdDate ? data.createdDate.split('T')[0] : '');
                $('#inputDescriptionBaseUpdate').val(data.description);
                $('#inputKeywordsUpdate').val(data.Keywords);
                $('#ModalBasedonneeUpdating').modal('hide');
            } else {
                alert(response.message);
            }
        },
        error: function (xhr, status, error) {
            alert("Une erreur est survenue lors de la récupération des données.");
        }
    });
}

function downloadFile(databaseName) {
    fetch(`/Data/DownloadFilezip?databaseName=${databaseName}`, {
        method: 'GET',
    })
        .then(response => {
            if (response.ok) {
                return response.blob(); // Convert the response to a blob
            } else {
                throw new Error('Failed to download files');
            }
        })
        .then(blob => {
            // Create a link element and set its URL to the blob
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.style.display = 'none';
            a.href = url;
            a.download = `${databaseName}_files.zip`; // File name to download
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url); // Release the object URL
        })
        .catch(error => {
            alert('Error downloading files: ' + error.message);
        });
}

function showDataDetails(element) {
    var $tr = $(element).closest('tr'); // Trouve la ligne de tableau la plus proche
    var dataId = $tr.data('id');

    $.ajax({
        url: '/Data/GetDataDetails',
        type: 'GET',
        data: { dataId: dataId },
        success: function (data) {
            $('#labelTitre').text(data.Title);
            $('#labelDateRéception').text(data.AcquisitionDate);
            $('#labelDatePublication').text(data.PublicationDate);
            $('#labelDateDernièreMiseàJour').text(data.LastUpdatedDate);
            $('#labelDescription').text(data.Description);
            $('#labelCategorie').text(data.Category);
            $('#labelTheme').text(data.ThemeName);
            $('#labelCouverture').text(data.Coverage);
            $('#labelResolutionSpatial').text(data.SpatialResolution);
            $('#labelResume').text(data.Summary);

            var fileSelectionHtml = '';

            if (data.Files && data.Files.length > 0) {
                data.Files.forEach(function (file) {
                    fileSelectionHtml += '<div><a href="/Data/DownloadFile?fileName=' + encodeURIComponent(file.FileName) + '&dataId=' + encodeURIComponent(dataId) + '" download>' + file.FileName + '</a></div>';
                });
            } else {
                fileSelectionHtml = '<p>Aucun fichier disponible.</p>';
            }
            $('#labelFileSelection').html(fileSelectionHtml);

            $('#exampleModal').modal('hide');
        },
        error: function (xhr, status, error) {
            alert('Error fetching data details: ' + error);
        }
    });
}
// Suppression du fond sombre après la fermeture du modal
$('#exampleModal').on('hidden.bs.modal', function () {
    $('body').removeClass('modal-open');
    $('.modal-backdrop').remove();
});

$(document).ready(function () {
    var fileList = [];  // Liste pour stocker les fichiers sélectionnés

    // Gestion de la sélection des fichiers
    $('#fileUploadInput').on('change', function () {
        var files = this.files;

        for (var i = 0; i < files.length; i++) {
            // Ajouter chaque fichier sélectionné à la liste fileList
            fileList.push(files[i]);

            // Console log pour vérifier les fichiers ajoutés
            console.log("Fichier ajouté :", files[i].name);

            // Ajouter le fichier dans le tableau pour l'afficher
            var newRow = `<tr>
                <td>${files[i].name}</td>
                <td style="text-align: center;">
                    <button type="button" class="status completed delete-file-btn" data-index="${fileList.length - 1}">
                        <i class="fas fa-trash"></i>
                    </button>
                </td>
            </tr>`;
            $('#existingFilesBody').append(newRow);
        }

        // Réinitialiser l'input file pour permettre de sélectionner à nouveau
        $('#fileUploadInput').val('');
    });

    // Suppression d'un fichier de la liste et du tableau
    $('#existingFilesBody').on('click', '.delete-file-btn', function () {
        var index = $(this).data('index');
        console.log("Fichier supprimé :", fileList[index].name);  // Console log lors de la suppression
        fileList.splice(index, 1);  // Supprimer le fichier de la liste
        $(this).closest('tr').remove();  // Supprimer la ligne du tableau
    });

    // Gestion de la soumission du formulaire
    $('#formAjouterData').on('submit', function (e) {
        e.preventDefault();

        var formData = new FormData();

        // Ajouter les fichiers accumulés dans fileList
        for (var i = 0; i < fileList.length; i++) {
            formData.append('Files', fileList[i]);
            console.log("Fichier soumis :", fileList[i].name);  // Console log pour vérifier les fichiers soumis
        }

        // Ajouter les autres champs du formulaire
        formData.append('Title', $('#inputTitre').val());
        formData.append('AcquisitionDate', $('#inputDateRéception').val());
        formData.append('PublicationDate', $('#inputDatePublication').val());
        formData.append('LastUpdatedDate', $('#inputDateDernièreMiseàJour').val());
        formData.append('Description', $('#inputDescription').val());
        formData.append('Category', $('#inputCategorie').val());
        formData.append('ThemeId', $('#inputTheme').val());
        formData.append('IndustryName', $('#inputIndustryName').val());
        formData.append('Coverage', $('input[name="Coverage"]:checked').map(function () { return this.value; }).get());
        formData.append('SpatialResolution', $('#inputResolutionSpatial').val());
        formData.append('CoordinateSystemId', $('#inputCoordinateSystem').val());
        formData.append('DataTypeId', $('#inputDataType').val());
        formData.append('Summary', $('#inputResume').val());

        var selectedDatabases = $('input[name="SelectedDataBases"]:checked').map(function () {
            return this.value;
        }).get();

        selectedDatabases.forEach(function (db) {
            formData.append('SelectedDataBases[]', db);
        });

        // Envoi de l'Ajax avec les fichiers et les données
        $.ajax({
            url: '/Data/AjouterData',
            type: 'POST',
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.success) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Succès',
                        text: 'Donnée ajoutée avec succès !',
                        timer: 2000,
                        showConfirmButton: false
                    });
                    $('#ModalAdding').modal('hide');
                    // Redirigez ou rechargez la page selon vos besoins
                    window.location.href = '/Data/Index';
                }
             else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Erreur',
                        text: response.message
                    });
                }
            },
            error: function (xhr, status, error) {
                Swal.fire({
                    icon: 'error',
                    title: 'Erreur',
                    text: "Une erreur s'est produite : " + error
                });
            }
        });
    });
});

document.addEventListener('DOMContentLoaded', function () {
    var globalSearchInput = document.getElementById('globalSearchInput');

    // Ajoute un écouteur d'événement 'input' pour déclencher la recherche à chaque changement
    globalSearchInput.addEventListener('input', function () {
        globalSearch();
    });

    // Fonction pour gérer la recherche
    function globalSearch(event) {
        if (event) {
            event.preventDefault();
        }

        var query = globalSearchInput.value.trim();
        var dataUrl = query === "" ? '/Search/AllData' : '/Search/Data';
        var databaseUrl = query === "" ? '/Search/AllDatabases' : '/Search/Database';

        // Recherche dans les données
        $.ajax({
            url: dataUrl,
            type: 'GET',
            data: { query: query },
            success: function (response) {
                var resultsTableBody = document.getElementById('dataBody');
                resultsTableBody.innerHTML = ''; // Vider les résultats précédents

                if (response.length === 0) {
                    resultsTableBody.innerHTML = '<tr><td colspan="11">Aucun résultat trouvé dans les données</td></tr>';
                } else {
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
                }

                document.getElementById('dataTable').classList.remove('hidden');
            },
            error: function () {
                alert("Une erreur s'est produite lors de la recherche dans les données.");
            }
        });

        // Recherche dans les bases de données
        $.ajax({
            url: databaseUrl,
            type: 'GET',
            data: { query: query },
            success: function (response) {
                var resultsTableBody = document.getElementById('searchResultsBody');
                resultsTableBody.innerHTML = ''; // Vider les résultats précédents

                if (response.length === 0) {
                    resultsTableBody.innerHTML = '<tr><td colspan="6">Aucun résultat trouvé dans les bases de données</td></tr>';
                } else {
                    response.forEach(function (database) {
                        var row = document.createElement('tr');
                        row.innerHTML = `
                                                                                                                            <td>${database.DataBaseName}</td>
                                                                                                                            <td>${database.Owner}</td>
                                                                                                                            <td>${database.description}</td>
                                                                                                                            <td>${database.Keywords}</td>
                                                                                                                            <td>${new Date(database.createdDate).toLocaleDateString()}</td>
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
                }

                document.getElementById('searchResultsTable').classList.remove('hidden');
            },
            error: function () {
                alert("Une erreur s'est produite lors de la recherche dans les bases de données.");
            }
        });
    }
});

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


function triggerFileUpload(dataId) {
    var fileInput = document.getElementById('fileInput');
    document.getElementById('dataId').value = dataId;
    fileInput.click();
    fileInput.onchange = function () {
        uploadFile();
    };
}

function uploadFile() {
    var form = document.getElementById('fileUploadForm');
    var formData = new FormData(form);

    fetch('/File/UploadFile', {
        method: 'POST',
        body: formData
    })
        .then(response => {
            if (response.ok) {
                alert('File uploaded successfully!');
                location.reload();
            } else {
                response.text().then(text => { alert('File upload failed: ' + text); });
            }
        })
        .catch(error => {
            alert('File upload failed: ' + error.message);
        });
}

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










// Fonction pour soumettre la mise à jour
function submitUpdate() {

    var updatedData = {

        IdData: $('#dataId').val(), // Assurez-vous que #dataId contient l'ID de la donnée à mettre à jour
        Title: $('#inputTitreUpdate').val(),
        AcquisitionDate: $('#inputDateRéceptionUpdate').val(),
        PublicationDate: $('#inputDatePublicationUpdate').val(),
        LastUpdatedDate: $('#inputDateDernierMiseàJourUpdate').val(),
        Description: $('#inputDescriptionUpdate').val(),
        ThemeName: $('#inputThemeUpdate').val(),
        Coverage: $('#inputCouvertureUpdate').val(),
        SpatialResolution: $('#inputResolutionSpatialUpdate').val(),
        Summary: $('#inputResumeUpdate').val(),
        Category: $('#inputCategorieUpdate').val()
    };

    $.ajax({
        url: '/Data/Update',
        type: 'POST',
        data: updatedData,
        success: function (response) {
            if (response.success) {
                $('#ModalUpdating').modal('hide'); // Ferme le modal après la mise à jour réussie
                alert(response.message); // Affiche un message de succès
                location.reload(); // Recharge la page pour refléter les modifications
            } else {
                alert(response.message); // Affiche un message d'erreur si la mise à jour a échoué
            }
        }
    });
}


function addDatabase() {
    var form = $('#databaseForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    var newDatabase = {
        __RequestVerificationToken: token,
        DataBaseName: $('#inputBasededonnee').val(),
        Owner: $('#inputPropriete').val(),
        createdDate: $('#inputDateDeCreation').val(),
        description: $('#inputDescriptionadd').val(),
        Keywords: $('#inputKeywords').val()
    };

    $.ajax({
        url: form.attr('action'),
        type: 'POST',
        data: newDatabase,
        success: function (response) {
            if (response.success) {
                $('#ModalBasedonneeAdding').modal('hide'); // Ferme le modal après l'ajout réussi
                alert(response.message); // Affiche un message de succès
                location.reload(); // Recharge la page pour refléter les modifications
            } else {
                alert(response.message); // Affiche un message d'erreur si l'ajout a échoué
            }
        },
        error: function () {
            alert("Une erreur s'est produite lors de l'ajout de la base de données.");
        }
    });
}

function updateDatabase() {
    var updatedDatabase = {
        idDataBase: $('#databaseId').val(),
        DataBaseName: $('#inputDataBaseNameUpdate').val(),
        Owner: $('#inputOwnerUpdate').val(),
        createdDate: $('#inputCreationDateUpdate').val(),
        description: $('#inputDescriptionBaseUpdate').val(),
        Keywords: $('#inputKeywordsUpdate').val() // Assurez-vous que ce champ existe dans votre modèle
    };

    $.ajax({
        url: '/DatabaseInfo/Update',
        type: 'POST',
        data: updatedDatabase,
        success: function (response) {
            if (response.success) {
                $('#ModalBasedonneeUpdating').modal('hide'); // Ferme le modal après la mise à jour réussie
                alert(response.message); // Affiche un message de succès
                location.reload(); // Recharge la page pour refléter les modifications
            } else {
                alert(response.message); // Affiche un message d'erreur si la mise à jour a échoué
            }
        },
        error: function () {
            alert("Une erreur s'est produite lors de la mise à jour de la base de données.");
        }
    });
}
