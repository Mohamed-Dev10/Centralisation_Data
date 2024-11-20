using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using CentralisationV0.Models.Entities;


using CentralisationdeDonnee.Models; // Assurez-vous que l'espace de noms correspondant est importé


using CentralisationV0.Services; // Ajout de l'espace de noms pour le service de données
using CentralisationV0.Models.Entities.ViewModels;

namespace CentralisationV0.Controllers
{
    public class ClientController : Controller
    {
        private readonly IDataService _dataService;
        private CentralisationContext db = new CentralisationContext();

        public ClientController()
        {
            _dataService = new DataService();
        }

        public ActionResult Index()
        {
            // Utilisation du service pour récupérer les données si nécessaire
            var clients = _dataService.GetClients();
            var contactClients = db.ContactClients.ToList();
            var arcgisSolutions = db.ArcgisSolutions.ToList();

            var viewModel = new IndexAndDetailsViewModel
            {
                Clients = clients, // Liste des clients obtenue via le service
                ContactClients = contactClients, // Liste des contacts clients obtenue via le contexte
                ArcgisSolutions = arcgisSolutions // Liste des solutions ArcGIS obtenue via le contexte
            };

            ViewBag.Themes = db.Themes.ToList(); // Exemple d'utilisation de ViewBag pour passer des données supplémentaires à la vue
            return View("~/Views/Client/Index.cshtml", viewModel); // Retourne la vue avec le ViewModel
        }
        [HttpPost]
        public JsonResult AddContactClient(string contactNom, string contactAdresse, string contactEmail)
        {
            if (ModelState.IsValid)
            {
                var contactClient = new ContactClient
                {
                    clientName = contactNom,
                    clientAddress = contactAdresse,
                    clientEmail = contactEmail,
                    idClient = null // Aucun client associé au départ
                };

                db.ContactClients.Add(contactClient);
                db.SaveChanges();

                return Json(new { success = true, contactId = contactClient.idContactClient, message = "Contact ajouté avec succès." });
            }

            return Json(new { success = false, message = "Erreur lors de l'ajout du contact client." });
        }


        [HttpPost]
        
        public JsonResult AddArcgisSolution(string TitredArcgisSolution, string Description)
        {
            if (ModelState.IsValid)
            {
                var newSolution = new ArcgisSolution
                {
                    TitredArcgisSolution = TitredArcgisSolution,
                    DescriptionRole = Description
                };

                db.ArcgisSolutions.Add(newSolution);
                db.SaveChanges();

                return Json(new { success = true, arcgisSolutionId = newSolution.idArcgisSolution });
            }

            return Json(new { success = false });
        }


        [HttpPost]
        public JsonResult AjouterClient(string clientNom, string clientEmail, string clientIndustrie, string clientType, string clientAdresse, List<int> SelectedArcgisSolutions, List<int> selectedContacts)
        {
            if (ModelState.IsValid)
            {
                // Créer une nouvelle instance de Client
                var client = new Client
                {
                    clientName = clientNom,
                    clientAddress = clientAdresse,
                    clientEmail = clientEmail,
                    clientIndustry = clientIndustrie,
                    clientType = clientType,
                    ArcgisSolutions = new List<ArcgisSolution>()
                };

                // Ajouter les solutions ArcGIS sélectionnées
                if (SelectedArcgisSolutions != null && SelectedArcgisSolutions.Any())
                {
                    foreach (var solutionId in SelectedArcgisSolutions)
                    {
                        var arcgisSolution = db.ArcgisSolutions.Find(solutionId);
                        if (arcgisSolution != null)
                        {
                            client.ArcgisSolutions.Add(arcgisSolution);
                        }
                    }
                }

                // Ajouter le client à la base de données
                db.Clients.Add(client);
                db.SaveChanges(); // Ici, le client ID est généré après la sauvegarde.

                // Mettre à jour les contacts clients avec l'ID du client nouvellement créé
                if (selectedContacts != null && selectedContacts.Any())
                {
                    foreach (var contactId in selectedContacts)
                    {
                        var contactClient = db.ContactClients.Find(contactId);
                        if (contactClient != null)
                        {
                            contactClient.idClient = client.idClient; // Remplacer l'ID client temporaire par le nouvel ID client
                        }
                    }

                    db.SaveChanges(); // Sauvegarder les changements apportés aux contacts clients
                }

                return Json(new { success = true, clientId = client.idClient, message = "Client ajouté avec succès." });
            }

            return Json(new { success = false, message = "Erreur lors de l'ajout du client. Modèle invalide." });
        }

        [HttpGet]
        public JsonResult GetContacts(int? clientId)
        {
            if (!clientId.HasValue)
            {
                return Json(new { success = false, message = "clientId est manquant." }, JsonRequestBehavior.AllowGet);
            }

            var contacts = db.ContactClients
                            .Where(c => c.idClient == clientId.Value)
                            .Select(c => new { c.clientName, c.clientEmail })
                            .ToList();

            return Json(new { success = true, contacts = contacts }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetArcgisSolutions(int? clientId)
        {
            if (!clientId.HasValue)
            {
                return Json(new { success = false, message = "clientId est manquant." }, JsonRequestBehavior.AllowGet);
            }

            var arcgisSolutions = db.Clients
                                    .Where(c => c.idClient == clientId.Value)
                                    .SelectMany(c => c.ArcgisSolutions)
                                    .Select(s => new { s.TitredArcgisSolution, s.DescriptionRole })
                                    .ToList();

            return Json(new { success = true, arcgisSolutions = arcgisSolutions }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetClientById(int id)
        {
            if (id == 0)
            {
                return Json(new { success = false, message = "ID client non fourni." }, JsonRequestBehavior.AllowGet);
            }

            var client = db.Clients
                           .Include(c => c.ContactClients)
                           .Include(c => c.ArcgisSolutions)
                           .FirstOrDefault(c => c.idClient == id);

            if (client == null)
            {
                return Json(new { success = false, message = "Client non trouvé." }, JsonRequestBehavior.AllowGet);
            }
            // Récupérer tous les contacts disponibles
            var allContacts = db.ContactClients.Select(c => new { id = c.idContactClient, name = c.clientName }).ToList();

            // Récupérer toutes les solutions ArcGIS disponibles (similaire à ce que vous avez fait pour les contacts)
            var allSolutions = db.ArcgisSolutions.Select(s => new { id = s.idArcgisSolution, name = s.TitredArcgisSolution }).ToList();

            var clientData = new
            {
                success = true,
                data = new
                {
                    id = client.idClient,
                    name = client.clientName,
                    address = client.clientAddress,
                    email = client.clientEmail,
                    industry = client.clientIndustry,
                    type = client.clientType,
                    contacts = client.ContactClients.Select(c => new { id = c.idContactClient, name = c.clientName }).ToList(),
                    arcgisSolutions = client.ArcgisSolutions.Select(s => new { id = s.idArcgisSolution, name = s.TitredArcgisSolution }).ToList(),
                    allContacts = allContacts,  // Inclure tous les contacts disponibles
                    allSolutions = allSolutions  // Inclure toutes les solutions ArcGIS disponibles
                }
            };

            return Json(clientData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateClient(int idClient, string clientName, string clientAddress, string clientEmail, string clientIndustry, string clientType, string selectedContacts, string selectedSolutions)
        {
            try
            {
                if (idClient <= 0)
                {
                    return Json(new { success = false, message = "ID client non valide." });
                }

                var client = db.Clients
                               .Include(c => c.ContactClients)
                               .Include(c => c.ArcgisSolutions)
                               .FirstOrDefault(c => c.idClient == idClient);

                if (client == null)
                {
                    return Json(new { success = false, message = "Client non trouvé." });
                }

                // Mettre à jour les informations de base du client
                client.clientName = clientName;
                client.clientAddress = clientAddress;
                client.clientEmail = clientEmail;
                client.clientIndustry = clientIndustry;
                client.clientType = clientType;

                // Gérer les contacts sélectionnés
                if (!string.IsNullOrEmpty(selectedContacts))
                {
                    var selectedContactIds = selectedContacts.Split(',')
                                                             .Select(id =>
                                                             {
                                                                 int contactId;
                                                                 return int.TryParse(id, out contactId) ? contactId : (int?)null;
                                                             })
                                                             .Where(id => id.HasValue)
                                                             .Select(id => id.Value)
                                                             .ToList();

                    // Obtenez les IDs des contacts existants avant d'ajouter les nouveaux
                    var existingContactIds = client.ContactClients.Select(c => c.idContactClient).ToList();

                    // Ajouter les nouveaux contacts
                    var contactsToAdd = selectedContactIds.Except(existingContactIds).ToList();
                    foreach (var contactId in contactsToAdd)
                    {
                        var contact = db.ContactClients.Find(contactId);
                        if (contact != null)
                        {
                            client.ContactClients.Add(contact);
                        }
                    }

                    // Supprimer les contacts non sélectionnés
                    var contactsToRemove = existingContactIds.Except(selectedContactIds).ToList();
                    foreach (var contactId in contactsToRemove)
                    {
                        var contact = client.ContactClients.FirstOrDefault(c => c.idContactClient == contactId);
                        if (contact != null)
                        {
                            client.ContactClients.Remove(contact);
                        }
                    }
                }
                else
                {
                    // Si aucun contact n'est sélectionné, supprimer tous les contacts associés
                    client.ContactClients.Clear();
                }

                // Gérer les solutions ArcGIS sélectionnées
                if (!string.IsNullOrEmpty(selectedSolutions))
                {
                    var selectedSolutionIds = selectedSolutions.Split(',')
                                                               .Select(id =>
                                                               {
                                                                   int solutionId;
                                                                   return int.TryParse(id, out solutionId) ? solutionId : (int?)null;
                                                               })
                                                               .Where(id => id.HasValue)
                                                               .Select(id => id.Value)
                                                               .ToList();

                    // Obtenez les IDs des solutions ArcGIS existantes avant d'ajouter les nouvelles
                    var existingSolutionIds = client.ArcgisSolutions.Select(a => a.idArcgisSolution).ToList();

                    // Ajouter les nouvelles solutions ArcGIS
                    var solutionsToAdd = selectedSolutionIds.Except(existingSolutionIds).ToList();
                    foreach (var solutionId in solutionsToAdd)
                    {
                        var solution = db.ArcgisSolutions.Find(solutionId);
                        if (solution != null)
                        {
                            client.ArcgisSolutions.Add(solution);
                        }
                    }

                    // Supprimer les solutions ArcGIS non sélectionnées
                    var solutionsToRemove = existingSolutionIds.Except(selectedSolutionIds).ToList();
                    foreach (var solutionId in solutionsToRemove)
                    {
                        var solution = client.ArcgisSolutions.FirstOrDefault(a => a.idArcgisSolution == solutionId);
                        if (solution != null)
                        {
                            client.ArcgisSolutions.Remove(solution);
                        }
                    }
                }
                else
                {
                    // Si aucune solution n'est sélectionnée, supprimer toutes les solutions associées
                    client.ArcgisSolutions.Clear();
                }

                // Enregistrer les modifications dans la base de données
                db.SaveChanges();

                return Json(new { success = true, message = "Client mis à jour avec succès." });
            }
            catch (Exception ex)
            {
                // Log the exception (non affiché ici)
                return Json(new { success = false, message = "Une erreur est survenue lors de la mise à jour du client." });
            }
        }

        [HttpPost]
        public JsonResult Delete(int idClient)
        {
            var client = db.Clients
                           .Include(c => c.ContactClients)
                           .Include(c => c.ArcgisSolutions)
                           .FirstOrDefault(c => c.idClient == idClient);

            if (client == null)
            {
                return Json(new { success = false, message = "Client non trouvé." });
            }

            // Supprimer le client
            db.Clients.Remove(client);
            db.SaveChanges();

            return Json(new { success = true, message = "Client supprimé avec succès." });
        }

        [HttpGet]
        public ActionResult ClientDetailsPartial(int id)
        {
            var client = db.Clients
                .Include(c => c.ContactClients)
                .Include(c => c.ArcgisSolutions)
                .FirstOrDefault(c => c.idClient == id);

            if (client == null)
            {
                return HttpNotFound();
            }

            return PartialView("_ClientDetailsPartial", client);
        }

       


        public ActionResult GetArcgisSolutionsload()
        {
            var arcgisSolutions = db.ArcgisSolutions.Select(s => new
            {
                idArcgisSolution=s.idArcgisSolution,
                TitredArcgisSolution=s.TitredArcgisSolution,
                s.DescriptionRole,
            }).ToList();

            return Json(arcgisSolutions, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetContactClients()
        {
            var clients = db.ContactClients.Select(c => new
            {
                IdClient = c.idContactClient,
                nom = c.clientName,
                c.clientEmail,
                c.clientAddress,
                c.clientIndustry,
            }).ToList();

            return Json(clients, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> GetClientDetails(int id)
        {
            var client = await db.Clients
                .Where(c => c.idClient == id)
                .Select(c => new
                {
                    Nom = c.clientName,
                    Adresse = c.clientAddress,  // Assurez-vous que le nom du champ correspond à votre modèle
                    Email = c.clientEmail,
                    Industrie = c.clientIndustry,  // Assurez-vous que le nom du champ correspond à votre modèle
            TypeClient = c.clientType // Suppose que TypeClient est une relation avec une autre entité
        })
                .FirstOrDefaultAsync();

            if (client == null)
            {
                return HttpNotFound();
            }

            return Json(client, JsonRequestBehavior.AllowGet);
        }

      




    }
}