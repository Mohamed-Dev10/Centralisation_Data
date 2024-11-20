document.getElementById('loginForm').addEventListener('submit', function(event) {
    event.preventDefault(); // Empêche le formulaire de se soumettre normalement

    // Récupère les valeurs des champs de saisie
    const email = document.getElementById('email').value;
    const password = document.getElementById('password').value;

    // Vérifie que les champs ne sont pas vides
    if (email === '' || password === '') {
        alert('Veuillez remplir tous les champs.');
        return;
    }

    // Listes d'admin et d'utilisateurs
    const admin = [
        { email: 'admin@gmail.com', password: 'admin' },
    ];
    const users = [
        { email: 'user1@gmail.com', password: 'user1pass' },
    ];

    // Vérifie les informations d'identification pour admin
    const isAdmin = admin.find(admin => admin.email === email && admin.password === password);
    // Vérifie les informations d'identification pour user
    const isUser = users.find(user => user.email === email && user.password === password);

    if (isAdmin) {
        // Redirection vers une autre page après connexion réussie pour admin
        window.location.href = 'index.html';
    } else if (isUser) {
        // Redirection vers une autre page après connexion réussie pour user
        window.location.href = 'Users/indexUser.html';
    } else {
        // Affiche une alerte si les informations d'identification sont incorrectes
        alert('Adresse e-mail ou mot de passe incorrect.');
    }
});