using CentralisationdeDonnee.Models;
using CentralisationV0.Models.Entities;
using CentralisationV0.Services;
using Dropbox.Api;
using Dropbox.Api.Files;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity;





using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using System.Web.Mvc;





namespace CentralisationV0.Controllers
{
    public class ProjetController : Controller
    {
        private CentralisationContext db = new CentralisationContext();

        private const string RedirectUri = "https://www.centralisation.com/Projet/DropboxCallback";
        private const string ClientId = "ywueyd5rq7mtwdl";
        private const string ClientSecret = "7y8w87228xkieqi";
        private static string DropboxAccessToken = ""; // Store access token securely
        private static string DropboxRefreshToken = ""; // Store refresh token securely






        //// Your method to initiate the OAuth flow
        //public ActionResult Authorize()
        //{
        //    var clientId = "ywueyd5rq7mtwdl"; // Your Dropbox App Client ID
        //    var redirectUri = "https://www.centralisation.com/Projet/DropboxCallback"; // Redirect URI configured in Dropbox
        //    var authUrl = $"https://www.dropbox.com/oauth2/authorize?client_id={clientId}&response_type=code&redirect_uri={redirectUri}";

        //    return Redirect(authUrl);
        //}

        public ActionResult Authorize()
        {
            var clientId = "529217040387-8263l5m1oc9uooes2ajrki98f92jmeqs.apps.googleusercontent.com";
            var redirectUri = "https://www.centralisation.com"; // Ensure this matches your Google Cloud Console
            var scopes = "https://www.googleapis.com/auth/drive.file";

            // Construct the authorization URL
            var authorizationUrl = $"https://accounts.google.com/o/oauth2/auth?response_type=code&client_id={clientId}&redirect_uri={Uri.EscapeDataString(redirectUri)}&scope={Uri.EscapeDataString(scopes)}";

            return Redirect(authorizationUrl);
        }

        public async Task<ActionResult> Callback(string code)
        {
            var clientId = "529217040387-8263l5m1oc9uooes2ajrki98f92jmeqs.apps.googleusercontent.com";
            var clientSecret = "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";
            var redirectUri = "https://www.centralisation.com"; // Ensure this matches your Google Cloud Console

            // Create the token request content
            var tokenRequest = new FormUrlEncodedContent(new Dictionary<string, string>
    {
        { "code", code },
        { "grant_type", "authorization_code" },
        { "client_id", clientId },
        { "client_secret", clientSecret },
        { "redirect_uri", redirectUri }
    });

            try
            {
                // Exchange the authorization code for an access token
                var httpClient = new HttpClient();
                var response = await httpClient.PostAsync("https://oauth2.googleapis.com/token", tokenRequest);
                var responseString = await response.Content.ReadAsStringAsync();

                // Handle the response
                if (response.IsSuccessStatusCode)
                {
                    // Deserialize the response (assuming JSON format)
                    var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseString);

                    // TODO: Save tokens and redirect to a different page or show a message
                    return RedirectToAction("Success"); // Example redirect to a success page
                }
                else
                {
                    // Handle errors
                    ViewBag.ErrorMessage = "Failed to exchange code for token.";
                    return View("Error"); // Example view for errors
                }
            }
            catch (Exception ex)
            {
                // Log and handle exceptions
                ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
                return View("Error"); // Example view for errors
            }
        }

        // TokenResponse class for deserialization
        public class TokenResponse
        {
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
            public string TokenType { get; set; }
            public int ExpiresIn { get; set; }
        }



        [HttpGet]
        public async Task<ActionResult> DropboxCallback(string code)
        {

            var projets = db.Collaborations.ToList();
            var clients = db.Clients.ToList();
            var model=new IndexViewModel();

            System.Diagnostics.Debug.WriteLine("DropboxCallback  !!");
            var tokenService = new DropboxTokenService(ClientId, ClientSecret, RedirectUri);
            try
            {
                var tokens = await tokenService.GetTokensAsync(code);
                var accessToken = tokens.AccessToken;
                var refreshToken = tokens.RefreshToken;
                System.Diagnostics.Debug.WriteLine("after refreshToken");
                // Save tokens in a secure place
                DropboxAccessToken = accessToken;
                DropboxRefreshToken = refreshToken;

               

                model = new IndexViewModel
                {
                    Collaborations = projets,
                    Clients = clients
                };

                return View("Index", model);
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error obtaining tokens: {ex.Message}";
            }

             projets = db.Collaborations.ToList();
             clients = db.Clients.ToList();

             model = new IndexViewModel
            {
                Collaborations = projets,
                Clients = clients
            };

            return View("Index", model);
        }



        [HttpGet]
        public JsonResult GetById(int id)
        {
            var project = db.Collaborations.FirstOrDefault(c => c.idCollaborateur == id);

            if (project == null)
            {
                return Json(new { success = false, message = "Projet non trouvé." }, JsonRequestBehavior.AllowGet);
            }

            // Chargement explicite des données associées
            db.Entry(project).Reference(c => c.Client).Load();
            db.Entry(project).Reference(c => c.TypeCollaboration).Load();
            db.Entry(project).Collection(c => c.Datas).Load();

            // Traitement des fichiers
            var files = project.LivrablesPaths != null
                ? project.LivrablesPaths.Split(',')
                    .Select(fp => new IndexAndDetailsViewModel.FileViewModel
                    {
                        FileName = Path.GetFileName(fp),
                        FilePath = fp
                    })
                        .ToList()
                : new List<IndexAndDetailsViewModel.FileViewModel>();

            var clients = db.Clients.Select(c => new { c.idClient, c.clientName }).ToList();
            var typeCollaborations = db.typeCollaborations.Select(t => new { t.idTypeCollaboration, t.NomType }).ToList();

            var projectData = new
            {
                success = true,
                data = new
                {
                    Titre = project.Titre,
                    dataId = project.idCollaborateur,
                    ClientId = project.Client?.idClient,
                    ClientName = project.Client?.clientName,
                    StartDate = project.StartDate.ToString("yyyy-MM-dd"),
                    EndDate = project.EndDate.ToString("yyyy-MM-dd"),
                    Duration = project.Duration,
                    Description = project.description,
                    Status = project.Status,
                    TypeCollaborationId = project.TypeCollaboration?.idTypeCollaboration,
                    TypeCollaborationName = project.TypeCollaboration?.NomType,

                },
                clients = clients,
                typeCollaborations = typeCollaborations,
                files = files, // Ajout des fichiers à la réponse
            };

            return Json(projectData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult AjouterProjet(AjouterProjetModel model, HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data." });
            }

            // Convertir les identifiants en entiers
            int clientId;
            if (!int.TryParse(model.ClientId, out clientId))
            {
                return Json(new { success = false, message = "Client ID is invalid." });
            }

            // Créez une instance de votre modèle Collaboration
            var collaboration = new Collaboration
            {
                Titre = model.Titre,
                Client = db.Clients.Find(clientId),
                EndDate = model.EndDate,
                Duration = model.Duration,
                StartDate = model.StartDate,
                description = model.Description,
                Status = model.Status,  // Assigner le statut
                Datas = new List<Data>()
            };

            // Gestion du téléversement du fichier
            if (file != null && file.ContentLength > 0)
            {
                string collaborationName = ExtractCollaborationName(collaboration);
                if (string.IsNullOrEmpty(collaborationName))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Collaboration name could not be determined.");
                }

                var directoryPath = Server.MapPath($"~/App_Data/LivrablesFiles/{collaborationName}");
                if (directoryPath == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Failed to determine the server path.");
                }

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                var fileName = Path.GetFileName(file.FileName);
                if (string.IsNullOrEmpty(fileName))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid file name.");
                }

                var filePath = Path.Combine(directoryPath, fileName);

                int count = 1;
                string fileNameOnly = Path.GetFileNameWithoutExtension(fileName);
                string extension = Path.GetExtension(fileName);
                while (System.IO.File.Exists(filePath))
                {
                    fileName = $"{fileNameOnly}({count++}){extension}";
                    filePath = Path.Combine(directoryPath, fileName);
                }

                try
                {
                    file.SaveAs(filePath);
                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, $"Failed to save the file: {ex.Message}");
                }

                // Ajouter le chemin du fichier à LivrablesPaths
                if (string.IsNullOrEmpty(collaboration.LivrablesPaths))
                {
                    collaboration.LivrablesPaths = filePath;
                }
                else
                {
                    collaboration.LivrablesPaths += "," + filePath;
                }
            }
            else
            {
                return Json(new { success = false, message = "Aucun fichier téléversé." });
            }

            // Rechercher le type de collaboration par son nom
            var typeCollaboration = db.typeCollaborations.FirstOrDefault(t => t.NomType == model.TypeCollaboration);
            if (typeCollaboration == null)
            {
                typeCollaboration = new TypeCollaboration
                {
                    NomType = model.TypeCollaboration
                };
                db.typeCollaborations.Add(typeCollaboration);
                db.SaveChanges();
            }

            // Associer l'ID du type de collaboration à l'objet Collaboration
            collaboration.idTypeCollaboration = typeCollaboration.idTypeCollaboration;

            // Ajoutez les Donnees sélectionnées
            if (model.SelectedDonnees != null)
            {
                foreach (var donneeId in model.SelectedDonnees)
                {
                    if (int.TryParse(donneeId, out int donneeIdInt))
                    {
                        Data donnee = db.Datas.Find(donneeIdInt);
                        if (donnee != null)
                        {

                            if (!collaboration.Datas.Contains(donnee))
                            {
                                collaboration.Datas.Add(donnee);
                            }
                        }
                    }
                    else
                    {
                        // Optionnel : Vous pouvez gérer les identifiants invalides ici, par exemple en enregistrant des erreurs
                    }
                }
            }

            // Enregistrez l'objet dans la base de données
            db.Collaborations.Add(collaboration);
            db.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult Update(Collaboration updatedProject)
        {
            if (ModelState.IsValid)
            {
                var project = db.Collaborations.FirstOrDefault(c => c.idCollaborateur == updatedProject.idCollaborateur);

                if (project != null)
                {
                    project.Titre = updatedProject.Titre;
                    project.StartDate = updatedProject.StartDate;
                    project.EndDate = updatedProject.EndDate;
                    project.Duration = updatedProject.Duration;
                    project.description = updatedProject.description;
                    project.Status = updatedProject.Status;

                    // Mettre à jour idClient
                    if (updatedProject.idClient != 0)
                    {
                        project.idClient = updatedProject.idClient;
                    }

                    // Mettre à jour idTypeCollaboration
                    if (updatedProject.idTypeCollaboration != 0)
                    {
                        project.idTypeCollaboration = updatedProject.idTypeCollaboration;
                    }

                    db.Entry(project).State = EntityState.Modified;
                    db.SaveChanges();

                    return Json(new { success = true, message = "Projet mis à jour avec succès." });
                }

                return Json(new { success = false, message = "Projet non trouvé." });
            }

            return Json(new { success = false, message = "État du modèle invalide." });
        }
        [HttpPost]
        public JsonResult Delete(int id)
        {
            var project = db.Collaborations.FirstOrDefault(c => c.idCollaborateur == id);

            if (project != null)
            {
                db.Collaborations.Remove(project);
                db.SaveChanges();
                return Json(new { success = true, message = "Projet supprimé avec succès." });
            }

            return Json(new { success = false, message = "Projet non trouvé." });
        }

        [HttpPost]
        public async Task<ActionResult> UploadFile(HttpPostedFileBase file, int collaborationId)
        {
            if (file == null || file.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "File not selected.");
            }

            var collaboration = await db.Collaborations.FindAsync(collaborationId);
            if (collaboration == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Collaboration not found.");
            }

            string collaborationName = ExtractCollaborationName(collaboration);
            if (string.IsNullOrEmpty(collaborationName))
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Collaboration name could not be determined.");
            }

            var directoryPath = Server.MapPath($"~/App_Data/LivrablesFiles/{collaborationName}");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var fileName = Path.GetFileName(file.FileName);
            if (string.IsNullOrEmpty(fileName))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid file name.");
            }

            var filePath = Path.Combine(directoryPath, fileName);

            int count = 1;
            string fileNameOnly = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);
            while (System.IO.File.Exists(filePath))
            {
                fileName = $"{fileNameOnly}({count++}){extension}";
                filePath = Path.Combine(directoryPath, fileName);
            }

            try
            {
                file.SaveAs(filePath);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, $"Failed to save the file: {ex.Message}");
            }

            // Mettre à jour la propriété LivrablesPaths pour ajouter le nouveau fichier
            if (string.IsNullOrEmpty(collaboration.LivrablesPaths))
            {
                collaboration.LivrablesPaths = filePath;
            }
            else
            {
                collaboration.LivrablesPaths += "," + filePath;
            }

            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, $"Failed to update the database: {ex.Message}");
            }

            return Json(new { success = true, message = "File uploaded and Collaboration updated successfully." });
        }

        private string ExtractCollaborationName(Collaboration collaboration)
        {
            return collaboration.Titre ?? "DefaultName";
        }

        [HttpPost]
        public async Task<JsonResult> DeleteFile(string fileName, int collaborationId)
        {
            if (string.IsNullOrEmpty(fileName) || collaborationId <= 0)
            {
                return Json(new { success = false, message = "Invalid input." });
            }

            var collaboration = await db.Collaborations.FindAsync(collaborationId);
            if (collaboration == null)
            {
                return Json(new { success = false, message = "Collaboration not found." });
            }

            // Get the full path of the file
            string collaborationName = ExtractCollaborationName(collaboration);
            var directoryPath = Server.MapPath($"~/App_Data/LivrablesFiles/{collaborationName}");
            var filePath = Path.Combine(directoryPath, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return Json(new { success = false, message = "File not found." });
            }

            try
            {
                // Delete the file
                System.IO.File.Delete(filePath);

                // Update the `LivrablesPaths` property to remove the file path
                var filePaths = collaboration.LivrablesPaths.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var updatedPaths = filePaths.Where(path => !path.EndsWith(fileName)).ToList();
                collaboration.LivrablesPaths = string.Join(",", updatedPaths);

                // Save changes to the database
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Failed to delete the file: {ex.Message}" });
            }

            return Json(new { success = true, collaborationId = collaborationId,message = "File deleted successfully." });
        }

        



        //[HttpPost]
        //public async Task<ActionResult> UploadFiles(HttpPostedFileBase file)
        //{


        //    var clientId = "ywueyd5rq7mtwdl";
        //    var clientSecret = "7y8w87228xkieqi";
        //    var redirectUri = "https://www.centralisation.com/Projet/DropboxCallback";

        //    // Check if the user is authorized
        //    if (string.IsNullOrEmpty(DropboxAccessToken))
        //    {
        //        System.Diagnostics.Debug.WriteLine("if (string.IsNullOrEmpty(DropboxAccessToken))");
        //        // Redirect to Dropbox authorization if not authorized
        //        var scopes = "files.content.write files.content.read";
        //        var authorizationUrl = $"https://www.dropbox.com/oauth2/authorize?client_id={clientId}&response_type=code&redirect_uri={Uri.EscapeDataString(redirectUri)}&scope={Uri.EscapeDataString(scopes)}";
        //        return Redirect(authorizationUrl);
        //    }

        //    if (file != null && file.ContentLength > 0)
        //    {
        //        try
        //        {
        //            using (var memoryStream = new MemoryStream())
        //            {
        //                await file.InputStream.CopyToAsync(memoryStream);
        //                memoryStream.Position = 0;

        //                // Use the access token to upload the file
        //                var dropboxService = new DropboxTokenService(DropboxAccessToken);
        //                var fileName = Path.GetFileName(file.FileName);
        //                await dropboxService.UploadFileAsync("/ProjetsGeomatic", fileName, memoryStream);

        //                ViewBag.Message = "Fichier téléchargé avec succès sur Dropbox!";
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            ViewBag.Message = $"Erreur lors du téléchargement du fichier : {ex.Message}";
        //        }
        //    }
        //    else
        //    {
        //        ViewBag.Message = "Aucun fichier sélectionné ou le fichier est vide.";
        //    }

        //    var projets = db.Collaborations.ToList();
        //    var clients = db.Clients.ToList();

        //    var model = new IndexViewModel
        //    {
        //        Collaborations = projets,
        //        Clients = clients
        //    };

        //    return View("Index", model);
        //}


        public ActionResult Index()
        {
            // Récupère les données nécessaires
            var dataList = db.Datas.ToList();
            var projets = db.Collaborations.ToList();
            var clients = db.Clients.ToList();
            var themes = db.Themes.ToList();

            // Crée un modèle de type IndexAndDetailsViewModel
            var model = new IndexAndDetailsViewModel
            {
                DataList = dataList,
                Collaborations = projets,
                Clients = clients,
                Themes = themes
            };

            // Passe le modèle à la vue
            return View(model);
        }


        public ActionResult GetClients()
        {
            var clients = db.Clients.Select(t => new
            {
                idClient = t.idClient,
                clientName = t.clientName,
                t.clientEmail,
                t.clientAddress,
                t.clientIndustry,
            }).ToList();
            return Json(clients, JsonRequestBehavior.AllowGet);
        }




        [HttpPost]
        public JsonResult AddClient(string clientNom, string clientEmail, string clientIndustrie, string clientType, string clientAddress)
        {
            if (ModelState.IsValid)
            {
                var client = new Client
                {
                    clientName = clientNom,
                    clientEmail = clientEmail,
                    clientIndustry = clientIndustrie,
                    clientType = clientType,
                    clientAddress = clientAddress,
                    Keywords = $"{clientNom} {clientEmail} {clientType} {clientIndustrie} {clientAddress}"
                };

                db.Clients.Add(client);
                db.SaveChanges();

                return Json(new { success = true, clientId = client.idClient, clientNom = client.clientName });
            }

            return Json(new { success = false, message = "Erreur lors de l'ajout du client." });
        }
        [HttpPost]
        public async Task<ActionResult> UploadFiles(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await file.InputStream.CopyToAsync(memoryStream);
                        memoryStream.Position = 0;

                        var googleDriveService = new GoogleDriveService();
                        await googleDriveService.UploadFileAsync(file.FileName, memoryStream, file.ContentType);

                        ViewBag.Message = "File uploaded successfully to Google Drive!";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = $"Error uploading file: {ex.Message}";
                }
            }
            else
            {
                ViewBag.Message = "No file selected or the file is empty.";
            }

            // Return to the view or redirect as needed
            return View("Index");
        }


        [HttpGet]
        public ActionResult Details(int id)
        {
            // Récupère la collaboration par ID
            var collaboration = db.Collaborations
                .Include(c => c.Client)
                .Include(c => c.TypeCollaboration)
                .FirstOrDefault(c => c.idCollaborateur == id);

            if (collaboration == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Collaboration not found.");
            }

            // Récupère les chemins de fichiers et prépare le ViewModel
            var filePaths = collaboration.LivrablesPaths?.Split(',') ?? new string[0];
            var viewModel = new IndexAndDetailsViewModel
            {
                CollaborationDetails = new IndexAndDetailsViewModel.CollaborationDetailsViewModel
                {
                    Id = collaboration.idCollaborateur,
                    Titre = collaboration.Titre,
                    Description = collaboration.description,
                    ClientName = collaboration.Client?.clientName,
                    TypeCollaborationName = collaboration.TypeCollaboration?.NomType,
                    StartDate = collaboration.StartDate.ToString("yyyy-MM-dd"),
                    EndDate = collaboration.EndDate.ToString("yyyy-MM-dd"),
                    Status = collaboration.Status,
                    Duration = collaboration.Duration,
                    
                    Files = filePaths.Select(path => new IndexAndDetailsViewModel.FileViewModel
                    {
                        FileName = Path.GetFileName(path),
                        FilePath = path
                    }).ToList()
                },
                // Ajoutez d'autres propriétés si nécessaire
            };


            return View(viewModel); // Envoie les données à la vue
        }


        //[HttpPost]
        //public async Task<ActionResult> UploadFiles(HttpPostedFileBase file)
        //{
        //    // Check if the user is authorized
        //    if (string.IsNullOrEmpty(DropboxAccessToken))
        //    {
        //        // Redirect to Dropbox authorization if not authorized
        //        var clientId = "ywueyd5rq7mtwdl";
        //        var redirectUri = "https://www.centralisation.com/Projet/DropboxCallback";
        //        var scopes = "files.content.write files.content.read";
        //        var authorizationUrl = $"https://www.dropbox.com/oauth2/authorize?client_id={clientId}&response_type=code&redirect_uri={Uri.EscapeDataString(redirectUri)}&scope={Uri.EscapeDataString(scopes)}";
        //        return Redirect(authorizationUrl);
        //    }

        //    if (file != null && file.ContentLength > 0)
        //    {
        //        try
        //        {
        //            using (var memoryStream = new MemoryStream())
        //            {
        //                await file.InputStream.CopyToAsync(memoryStream);
        //                memoryStream.Position = 0;

        //                // Use the access token to upload the file
        //                var dropboxService = new DropboxTokenService(DropboxAccessToken);
        //                var fileName = Path.GetFileName(file.FileName);
        //                var dropboxFolderPath = "/ProjetsGeomatic"; // Your Dropbox folder path

        //                await dropboxService.UploadFileAsync(dropboxFolderPath, fileName, memoryStream);

        //                ViewBag.Message = "Fichier téléchargé avec succès sur Dropbox!";
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            ViewBag.Message = $"Erreur lors du téléchargement du fichier : {ex.Message}";
        //            System.Diagnostics.Debug.WriteLine($"Erreur lors du téléchargement du fichier : {ex.Message}");
        //            if (ex.InnerException != null)
        //            {
        //                System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
        //            }
        //        }
        //    }
        //    else
        //    {
        //        ViewBag.Message = "Aucun fichier sélectionné ou le fichier est vide.";
        //    }

        //    var projets = db.Collaborations.ToList();
        //    var clients = db.Clients.ToList();

        //    var model = new IndexViewModel
        //    {
        //        Collaborations = projets,
        //        Clients = clients
        //    };

        //    return View("Index", model);
        //}



        // GET: Projet/Creat
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projet/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Projet/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Projet/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

       






    }
}
