function displayRowInformation(row) {
    // Extraire les informations de la ligne cliquée

    var imgSrc = row.find('img').attr('src');
    var nom = row.find('p').text();
    var adresse = row.find('td:nth-child(2)').text();
    var email = row.find('td:nth-child(3)').text();
    var type = row.find('td:nth-child(4)').text();
    var categorie = row.find('td:nth-child(5)').text();
    var solutionArcGIS = row.find('td:nth-child(6)').text();
    var contactClient = row.find('td:nth-child(7)').text();
    var nomContact = row.find('td:nth-child(8)').text();

   // Remplir le modal de détails avec les informations extraites

    $('#detailNom').text(nom).css('color', '#3C91E6');
    $('#detailAdresse').text(adresse).css('color', '#3C91E6');
    $('#detailEmail').text(email).css('color', '#3C91E6');
    $('#detailType').text(type).css('color', '#3C91E6');
    $('#detailCategorie').text(categorie).css('color', '#3C91E6');
    $('#detailSolution').text(solutionArcGIS).css('color', '#3C91E6');
    $('#detailContactClient').text(contactClient).css('color', '#3C91E6');
    $('#detailNomContact').text(nomContact).css('color', '#3C91E6');

   // Afficher le modal de détails

   // $('#ModalDetailClient').modal('show');
}

// Ajouter un écouteur d'événements pour les clics sur les lignes du tableau

$(document).on('click', 'tbody tr', function() {
    var row = $(this);
    displayRowInformation(row);
});


function populateUpdateClientModal(row) {
    // Extraire les informations de la ligne cliquée
    var nom = row.find('p').text();
    var adresse = row.find('td:nth-child(2)').text();
    var email = row.find('td:nth-child(3)').text();
    var type = row.find('td:nth-child(4)').text();
    var categorie = row.find('td:nth-child(5)').text();
    var solutionArcGIS = row.find('td:nth-child(6)').text();
    var contactClient = row.find('td:nth-child(7)').text();
    var nomContact = row.find('td:nth-child(8)').text();

    // Remplir le modal de mise à jour du client avec les informations extraites
    $('#updateNom').val(nom).css('color', '#3C91E6');
    $('#updateAdresse').val(adresse).css('color', '#3C91E6');
    $('#updateEmail').val(email).css('color', '#3C91E6');
    $('#updateType').val(type).css('color', '#3C91E6');
    $('#updateCategorie').val(categorie).css('color', '#3C91E6');
    $('#updateSolution').val(solutionArcGIS).css('color', '#3C91E6');
    $('#updateContactClient').val(contactClient).css('color', '#3C91E6');
    $('#updateNomContact').val(nomContact).css('color', '#3C91E6');

    // Stocker la ligne actuelle pour une utilisation ultérieure
    $('#updateClientForm').data('currentRow', row);

    // Afficher le modal de mise à jour du client
   // $('#UpdateClientModal').modal('show');
}

// Ajouter un écouteur d'événements pour les clics sur le bouton d'édition
$(document).on('click', '.edit-button', function() {
    var row = $(this).closest('tr');
    populateUpdateClientModal(row);
});