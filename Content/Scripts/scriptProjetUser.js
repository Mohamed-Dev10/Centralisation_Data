
   

function displayProjectDetails(row) {
    // Extraire les informations de la ligne cliquée
    var projet = row.find('td:nth-child(1)').text();
    var imgSrc = row.find('td:nth-child(2) img').attr('src');
    var dateDebut = row.find('td:nth-child(3)').text();
    var dateFin = row.find('td:nth-child(4)').text();
    var typeCollaboration = row.find('td:nth-child(5)').text();
    var duree = row.find('td:nth-child(6)').text();
    var description = row.find('td:nth-child(7)').text();
    var status = row.find('td:nth-child(8)').text();

    // Remplir le modal de détails avec les informations extraites
    $('#labelProjet').text(projet).css('color', '#3C91E6');
    $('#labelDateDebut').text(dateDebut).css('color', '#3C91E6');
    $('#labelDateFin').text(dateFin).css('color', '#3C91E6');
    $('#labelTypeCollaboration').text(typeCollaboration).css('color', '#3C91E6');
    $('#labelDuree').text(duree).css('color', '#3C91E6');
    $('#labelDescription').text(description).css('color', '#3C91E6');
    $('#labelStatus').text(status).css('color', '#3C91E6');
   

    // Afficher le modal de détails
   // $('#ModalAdding').modal('show');
}

 


$(document).on('click', 'tbody tr', function() {
    var row = $(this);
    displayProjectDetails(row);
});


function updateProjectModal(row){

    var projet = row.find('td:nth-child(1)').text();
    var dateDebut = row.find('td:nth-child(3)').text();
    var dateFin = row.find('td:nth-child(4)').text();
    var typeCollaboration = row.find('td:nth-child(5)').text();
    var duree = row.find('td:nth-child(6)').text();
    var description = row.find('td:nth-child(7)').text();
    var status = row.find('td:nth-child(8) .status').text();

    dateDebut = convertDateFormat(dateDebut);
    dateFin = convertDateFormat(dateFin);
    
    // Remplir le modal de détails avec les informations extraites
    $('#inputProjetUpdate').val(projet).css('color', '#3C91E6');
    $('#inputDateDebutUpdate').val(dateDebut).css('color', '#3C91E6');
    $('#inputDateFinUpdate').val(dateFin).css('color', '#3C91E6');
    $('#inputTypeCollaborationUpdate').val(typeCollaboration).css('color', '#3C91E6');
    $('#inputDureeUpdate').val(duree).css('color', '#3C91E6');
    $('#inputDescriptionUpdate').val(description).css('color', '#3C91E6');
    $('#inputStatusUpdate').val(status).css('color', '#3C91E6');

}
function convertDateFormat(dateStr) {

    //la date est au format DD/MM/YYYY
    var parts = dateStr.split('/');
    return `${parts[2]}-${parts[1]}-${parts[0]}`;
}

// Ajouter un écouteur d'événements pour les clics sur le bouton d'édition
$(document).on('click', 'tbody tr', function() {
    var row = $(this);
    updateProjectModal(row);
});
    
// Assume selectedFiles is an array containing the names of selected files
var selectedFiles = ["file1.pdf", "file2.doc", "file3.docx"];

// Get the container element
var fileSelectionContainer = document.getElementById("labelFileSelection");

// Clear any existing content
fileSelectionContainer.innerHTML = "";

// Loop through the selectedFiles array and create list items
selectedFiles.forEach(function(fileName) {
    var listItem = document.createElement("li");
    listItem.textContent = fileName;
    fileSelectionContainer.appendChild(listItem);
});


function populateUpdateModal(editButton) {
    // Get the closest row to the clicked edit button
    var row = $(editButton).closest("tr");
    
    // Extract information from the row
    var projet = row.find("td:eq(0) p").text(); // Project name is inside a <p> tag within the first <td>
    var dateDebut = row.find("td:eq(1)").text();
    var dateFin = row.find("td:eq(2)").text();
    var typeCollaboration = row.find("td:eq(3)").text();
    var duree = row.find("td:eq(4)").text();
    var description = row.find("td:eq(5)").text();
    var status = row.find("td:eq(6) span").text(); // Assuming the status is inside a <span> tag within the seventh <td>
    
    // Helper function to format date
    function formatDate(dateStr) {
        var parts = dateStr.split('/');
        return `${parts[2]}-${parts[1]}-${parts[0]}`; // Format as yyyy-mm-dd
    }

    // Log the extracted values for debugging
    console.log("Projet:", projet);
    console.log("Date de Début:", dateDebut);
    console.log("Date de Fin:", dateFin);
    console.log("Type de collaboration:", typeCollaboration);
    console.log("Durée (Mois):", duree);
    console.log("Description:", description);
    console.log("Statut:", status);
    
    // Populate input fields in the modal with extracted information
    $('#inputProjetUpdate').val(projet);
    $('#inputDateDebutUpdate').val(formatDate(dateDebut));
    $('#inputDateFinUpdate').val(formatDate(dateFin));
    $('#inputTypeCollaborationUpdate').val(typeCollaboration);
    $('#inputDureeUpdate').val(duree);
    $('#inputDescriptionUpdate').val(description);
    $('#inputStatusUpdate').val(status);
}

// Event listener for edit button clicks
$(document).on("click", ".edit-button", function() {
    populateUpdateModal(this);
   // $('#ModalUpdateProjet').modal('show');
});

document.addEventListener('DOMContentLoaded', function() {
    const dataButtons = document.querySelectorAll('.data-button');

    dataButtons.forEach(button => {
        button.addEventListener('click', function() {
            // Get the project name from the clicked row in projetinfo
            const projectRow = button.closest('tr');
            const projectName = projectRow.querySelector('td p').textContent.trim();

            // Get all rows from donneInfo table
            const donneInfoRows = document.querySelectorAll('#donneInfo tbody tr');

            // Find the matching row in donneInfo
            let matchingRow;
            donneInfoRows.forEach(row => {
                const dataName = row.querySelector('td p').textContent.trim();
                if (dataName === projectName) {
                    matchingRow = row;
                }
            });

            // Populate and show the modal with the matching row information
            if (matchingRow) {
                const modalContent = document.getElementById('modalDataContent');
                modalContent.innerHTML = ''; // Clear previous content

                // Labels array to map to the corresponding cells
                const labels = [
                    'Titre',
                    'Date Réception',
                    'Date de Publication',
                    'Date Derniers Mise à Jour',
                    'Catégorie',
                    'Thème',
                    'Couverture',
                    'Résolution Spatial',
                    'Résumé'
                ];

                // Populate modal with label-value pairs
                for (let i = 0; i < labels.length; i++) {
                    const label = labels[i];
                    const value = matchingRow.cells[i].textContent.trim();

                    const formGroup = document.createElement('div');
                    formGroup.classList.add('form-group', 'row');

                    const labelElement = document.createElement('label');
                    labelElement.classList.add('col-sm-6');
                    labelElement.textContent = label + ':';
                    formGroup.appendChild(labelElement);

                    const valueElement = document.createElement('p');
                    valueElement.classList.add('col-sm-2');
                    valueElement.id = 'modal' + label.replace(/\s+/g, ''); // Remove spaces for ID
                    valueElement.textContent = value;
                    valueElement.style.color="#3C91E6";
                    formGroup.appendChild(valueElement);
                    modalContent.appendChild(formGroup);
                }

                // Show the modal
              //  $('#dataModal').modal('show');
            } else {
               // alert('No matching data found!');
            }
            
        });
    });
});





    


    

    


