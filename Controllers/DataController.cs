using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.IO;
using System.IO;
using System.IO.Compression;

using System.Web.Mvc;
using Newtonsoft.Json;

using System.Linq; // Ajoutez cette ligne pour utiliser les méthodes LINQ
using System.Net;
using System.Text.Json;


using System.Threading.Tasks;

using CentralisationV0.Models.Entities;


using CentralisationdeDonnee.Models; // Assurez-vous que l'espace de noms correspondant est importé


using CentralisationV0.Services; // Ajout de l'espace de noms pour le service de données
using CentralisationV0.Models.Entities.ViewModels;

namespace CentralisationV0.Controllers
{
    public class DataController : Controller
    {

        private readonly IDataService _dataService;
        private CentralisationContext db = new CentralisationContext();

        public DataController()
        {
            _dataService = new DataService();
        }

        public ActionResult Index()
        {
            var dataList = _dataService.GetData();
            var databaseList = db.DataBases.ToList();
            var coordinateSystems = db.CoordinateSystems.ToList();
            var dataTypes = db.DataTyps.ToList();
           
            var viewModel = new DataAndDatabaseViewModel
            {
                DataList = dataList,
                DatabaseList = databaseList,
                CoordinateSystems = coordinateSystems,
                Themes = db.Themes.ToList(),  
                DataTypes = dataTypes
            };

            return View("~/Views/Donnee/Index.cshtml", viewModel);

        }


        // GET: Data/Create
        public ActionResult Create()
        {
            ViewBag.Themes = new SelectList(db.Themes, "IdTheme", "nom");
            return View();
        }

        public ActionResult GetThemes()
        {
            var themes = db.Themes.Select(t => new
            {
                themeId = t.IdTheme,
                t.nom,
                t.description,
                // Ajoutez ici toutes les autres propriétés dont vous avez besoin
            }).ToList();

            return Json(themes, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCoordinateSystems()
        {
            var coordinateSystems = db.CoordinateSystems.Select(t => new
            {
                coordinateSystemId = t.IdCoordinateSystem,
                name=t.Name,
                t.Description,
            }).ToList();
            return Json(coordinateSystems, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDataTypes()
        {
            var dataTypes =  db.DataTyps.Select(t => new
            {
                dataTypeId = t.ID,
                format = t.format,
                t.description,
            }).ToList();
            return Json(dataTypes, JsonRequestBehavior.AllowGet);
        }


        // Méthode pour convertir un chemin relatif en chemin absolu
        private string ConvertToAbsolutePath(string relativePath)
        {
            return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath));
        }

        [HttpPost]
        public ActionResult UploadFiles(HttpPostedFileBase[] files)
        {
            if (files == null || files.Length == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "No files selected.");
            }

            var filePaths = new List<string>();

            foreach (var file in files)
            {
                if (file != null && file.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string filePath = Path.Combine(Server.MapPath("~/App_Data/Uploads"), fileName);

                    // Save the file
                    file.SaveAs(filePath);
                    filePaths.Add(filePath);
                }
            }

            // Store the file paths in a session or database temporarily
            Session["UploadedFilePaths"] = filePaths;

            return Json(new { success = true, filePaths });
        }



        [HttpPost]
        public async Task<ActionResult> AjouterData(AjouterDataModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data." });
            }

            // Rechercher les entités associées par leur ID
            var theme = await db.Themes.FindAsync(model.ThemeId);
            var coordinateSystem = await db.CoordinateSystems.FindAsync(model.CoordinateSystemId);
            var dataType = await db.DataTyps.FindAsync(model.DataTypeId);

            if (theme == null)
            {
                return Json(new { success = false, message = "Theme not found." });
            }

            var data = new Data
            {
                Title = model.Title,
                AcquisitionDate = model.AcquisitionDate,
                PublicationDate = model.PublicationDate,
                LastUpdatedDate = model.LastUpdatedDate,
                Description = model.Description,
                Category = model.Category,
                Theme = theme,
                CoordinateSystem = coordinateSystem,
                DataType = dataType,
                Coverage = model.Coverage,
                SpatialResolution = model.SpatialResolution,
                Summary = model.Summary,
                DataBases = new List<DataBase>()
            };

            var industry = await db.Industries
                .FirstOrDefaultAsync(i => i.nom == model.IndustryName && i.ThemeId == model.ThemeId);

            if (industry == null)
            {
                if (await db.Themes.AnyAsync(t => t.IdTheme == model.ThemeId))
                {
                    industry = new Industry
                    {
                        nom = model.IndustryName,
                        ThemeId = model.ThemeId
                    };
                    db.Industries.Add(industry);
                    await db.SaveChangesAsync();
                }
                else
                {
                    return Json(new { success = false, message = "Invalid ThemeId for Industry." });
                }
            }

            data.IndustryId = industry.IdUseConstraint;

            var savedFilePaths = new List<string>();

            // Gérer les fichiers
            if (model.Files != null && model.Files.Any())
            {
                foreach (var file in model.Files)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        var originalFileName = Path.GetFileName(file.FileName);
                        var uniqueFileName = originalFileName; // Utilisez le nom de fichier d'origine

                        // Vérifiez l'existence du fichier dans le répertoire pour éviter les conflits
                        if (model.SelectedDataBases != null && model.SelectedDataBases.Any() && !model.SelectedDataBases.Contains("none"))
                        {
                            var selectedDatabaseIds = model.SelectedDataBases
                                .Where(id => int.TryParse(id, out _))
                                .Select(id => int.Parse(id))
                                .ToList();

                            foreach (var dbId in selectedDatabaseIds)
                            {
                                var selectedDatabase = await db.DataBases.FindAsync(dbId);
                                if (selectedDatabase != null)
                                {
                                    var uploadPath = Server.MapPath($"~/App_Data/Uploads/{selectedDatabase.DataBaseName}");
                                    if (!Directory.Exists(uploadPath))
                                    {
                                        Directory.CreateDirectory(uploadPath);
                                    }

                                    var filePath = Path.Combine(uploadPath, uniqueFileName);

                                    // Assurez-vous que le fichier n'existe pas déjà
                                    int counter = 1;
                                    while (System.IO.File.Exists(filePath))
                                    {
                                        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
                                        var extension = Path.GetExtension(originalFileName);
                                        filePath = Path.Combine(uploadPath, $"{fileNameWithoutExtension}_{counter}{extension}");
                                        counter++;
                                    }

                                    // Sauvegarde du fichier
                                    file.SaveAs(filePath);

                                    // Ajouter le chemin du fichier sauvegardé à la liste
                                    savedFilePaths.Add(filePath);

                                    // Associer la base de données à l'objet Data si ce n'est pas déjà fait
                                    if (!data.DataBases.Contains(selectedDatabase))
                                    {
                                        data.DataBases.Add(selectedDatabase);
                                    }
                                }
                                else
                                {
                                    return Json(new { success = false, message = $"Database with ID {dbId} not found." });
                                }
                            }
                        }
                        else
                        {
                            var defaultUploadPath = Server.MapPath($"~/App_Data/Uploads/{data.Title}");
                            if (!Directory.Exists(defaultUploadPath))
                            {
                                Directory.CreateDirectory(defaultUploadPath);
                            }

                            var filePath = Path.Combine(defaultUploadPath, uniqueFileName);

                            // Assurez-vous que le fichier n'existe pas déjà
                            int counter = 1;
                            while (System.IO.File.Exists(filePath))
                            {
                                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
                                var extension = Path.GetExtension(originalFileName);
                                filePath = Path.Combine(defaultUploadPath, $"{fileNameWithoutExtension}_{counter}{extension}");
                                counter++;
                            }

                            // Sauvegarde du fichier
                            file.SaveAs(filePath);

                            // Ajouter le chemin du fichier sauvegardé à la liste
                            savedFilePaths.Add(filePath);
                        }
                    }
                }

                // Conserver les chemins des fichiers sauvegardés en utilisant des virgules comme séparateurs
                data.UrlData = string.Join(",", savedFilePaths);
            }
            else
            {
                return Json(new { success = false, message = "No files were uploaded." });
            }

            // Ajouter l'objet Data à la base de données
            db.Datas.Add(data);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(e => e.ValidationErrors)
                    .Select(e => $"{e.PropertyName}: {e.ErrorMessage}");

                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = $"Validation failed for one or more entities. Details: {fullErrorMessage}";

                return Json(new { success = false, message = exceptionMessage });
            }

            return Json(new { success = true, message = "Data added successfully." });
        }

        [HttpGet]
        public JsonResult GetById(int id)
        {
            var debugMessages = new List<string>();

            // Récupération des données
            var data = db.Datas
                .Include(d => d.Theme)
                .Include(d => d.DataBases)
                .FirstOrDefault(d => d.IdData == id);

            if (data == null)
            {
                debugMessages.Add($"Donnée avec ID {id} non trouvée.");
                return Json(new { success = false, message = "Donnée non trouvée", debugMessages }, JsonRequestBehavior.AllowGet);
            }

            // Utiliser HashSet pour stocker les noms de fichiers uniques
            var files = new HashSet<IndexAndDetailsViewModel.FileViewModel>(new FileViewModelComparer());

            // Traiter les fichiers si UrlData n'est pas vide
            if (!string.IsNullOrEmpty(data.UrlData))
            {
                // Diviser les chemins par les virgules pour obtenir chaque groupe de fichiers concaténés
                var paths = data.UrlData.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var pathGroup in paths)
                {
                    // Diviser chaque groupe par les points-virgules pour obtenir des chemins individuels
                    var individualPaths = pathGroup.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var trimmedPath in individualPaths)
                    {
                        var fileName = Path.GetFileName(trimmedPath);

                        debugMessages.Add($"Vérification du fichier : {trimmedPath}");

                        // Ajouter le fichier à la liste sans répétition
                        if (!files.Any(f => f.FileName == fileName))
                        {
                            files.Add(new IndexAndDetailsViewModel.FileViewModel
                            {
                                FileName = fileName,
                                FilePath = trimmedPath
                            });
                            debugMessages.Add($"Fichier ajouté : {fileName}");
                        }
                        else
                        {
                            debugMessages.Add($"Fichier déjà ajouté : {fileName}");
                        }
                    }
                }
            }
            else
            {
                debugMessages.Add("Aucun fichier à récupérer.");
            }

            var isoAcquisitionDate = data.AcquisitionDate.ToString("yyyy-MM-ddTHH:mm:ss");
            var isoPublicationDate = data.PublicationDate.ToString("yyyy-MM-ddTHH:mm:ss");
            var isoLastUpdatedDate = data.LastUpdatedDate.ToString("yyyy-MM-ddTHH:mm:ss");

            var jsonData = new
            {
                success = true,
                data = new
                {
                    IdData = data.IdData,
                    Title = data.Title,
                    AcquisitionDate = isoAcquisitionDate,
                    PublicationDate = isoPublicationDate,
                    LastUpdatedDate = isoLastUpdatedDate,
                    Description = data.Description,
                    Theme = data.Theme != null ? new { nom = data.Theme.nom } : null,
                    Coverage = data.Coverage,
                    SpatialResolution = data.SpatialResolution,
                    Summary = data.Summary,
                    Category = data.Category
                },
                files = files.ToList(), // Convertir HashSet en liste pour la réponse JSON
                debugMessages // Inclure les messages de débogage dans la réponse JSON
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        // Comparateur pour éviter les doublons dans le HashSet
        public class FileViewModelComparer : IEqualityComparer<IndexAndDetailsViewModel.FileViewModel>
        {
            public bool Equals(IndexAndDetailsViewModel.FileViewModel x, IndexAndDetailsViewModel.FileViewModel y)
            {
                return x.FileName == y.FileName; // Comparer uniquement sur la base du nom de fichier
            }

            public int GetHashCode(IndexAndDetailsViewModel.FileViewModel obj)
            {
                return obj.FileName.GetHashCode();
            }
        }


        [HttpPost]
        public JsonResult Update(Data updatedData)
        {
            if (ModelState.IsValid)
            {
                var data = db.Datas.Include(d => d.Theme).FirstOrDefault(d => d.IdData == updatedData.IdData);

                if (data != null)
                {
                    data.Title = updatedData.Title;
                    data.AcquisitionDate = updatedData.AcquisitionDate;
                    data.PublicationDate = updatedData.PublicationDate;
                    data.LastUpdatedDate = updatedData.LastUpdatedDate;
                    data.Description = updatedData.Description;
                    data.Category = updatedData.Category;
                    data.Coverage = updatedData.Coverage;
                    data.SpatialResolution = updatedData.SpatialResolution;
                    data.Summary = updatedData.Summary;

                    // Update Theme
                    var theme = db.Themes.FirstOrDefault(t => t.nom == updatedData.ThemeName);
                    if (theme == null)
                    {
                        theme = new Theme
                        {
                            nom = updatedData.ThemeName
                        };
                        db.Themes.Add(theme);
                        db.SaveChanges();
                    }

                    data.ThemeId = theme.IdTheme;

                    db.Entry(data).State = EntityState.Modified;
                    db.SaveChanges();

                    return Json(new { success = true, message = "Donnée mise à jour avec succès." });
                }

                return Json(new { success = false, message = "Donnée non trouvée." });
            }

            return Json(new { success = false, message = "État du modèle invalide." });
        }
        [HttpPost]
        public JsonResult Delete(int id)
        {
            var data = db.Datas.FirstOrDefault(d => d.IdData == id);
            if (data != null)
            {
                db.Datas.Remove(data);
                db.SaveChanges();
                return Json(new { success = true, message = "Donnée supprimée avec succès." });
            }
            return Json(new { success = false, message = "Donnée non trouvée." });
        }
        [HttpPost]
        public JsonResult AddCoordinateSystem(string name, string description)
        {
            if (ModelState.IsValid)
            {
                var coordinateSystem = new CoordinateSystem
                {
                    Name = name,
                    Description = description
                };

                db.CoordinateSystems.Add(coordinateSystem);
                db.SaveChanges();

                return Json(new { success = true, message = "Système de coordonnées ajouté avec succès !", coordinateSystemId = coordinateSystem.IdCoordinateSystem, name = coordinateSystem.Name });
            }

            return Json(new { success = false, message = "Erreur lors de l'ajout du système de coordonnées." });
        }
        [HttpPost]
        public JsonResult AddDataType(string format, string description)
        {
            if (ModelState.IsValid)
            {
                // Création d'une nouvelle instance de DataType
                var dataType = new DataTyp
                {
                    format = format,
                    description = description
                };

                // Ajout du DataType au contexte de la base de données
                db.DataTyps.Add(dataType);
                db.SaveChanges();

                // Retourner une réponse JSON avec succès
                return Json(new { success = true, message = "Type de données ajouté avec succès !", dataTypeId = dataType.ID, format = dataType.format });
            }

            // Retourner une réponse JSON avec erreur si la validation échoue
            return Json(new { success = false, message = "Erreur lors de l'ajout du type de données." });
        }
        [HttpPost]
        public JsonResult AddTheme(string nom, string description)
        {
            if (ModelState.IsValid)
            {
                // Création d'une nouvelle instance de Theme
                var theme = new Theme
                {
                    nom = nom,
                    description = description
                };

                // Ajout du Theme au contexte de la base de données
                db.Themes.Add(theme);
                db.SaveChanges();

                // Retourner une réponse JSON avec succès
                return Json(new { success = true, message = "Thème ajouté avec succès !", themeId = theme.IdTheme, nom = theme.nom });
            }

            // Retourner une réponse JSON avec erreur si la validation échoue
            return Json(new { success = false, message = "Erreur lors de l'ajout du thème." });
        }

        [HttpPost]
        public JsonResult AddIndustry(string nom)
        {
            if (ModelState.IsValid)
            {
                // Création d'une nouvelle instance de Industry
                var industry = new Industry
                {
                    nom = nom
                   
                };

                // Ajout de l'Industry au contexte de la base de données
                db.Industries.Add(industry);
                db.SaveChanges();

                // Retourner une réponse JSON avec succès
                return Json(new { success = true, message = "Industry ajoutée avec succès !", industryId = industry, nom = industry.nom });
            }

            // Retourner une réponse JSON avec erreur si la validation échoue
            return Json(new { success = false, message = "Erreur lors de l'ajout de l'industry." });
        }
        [HttpGet]
        public async Task<ActionResult> GetDataDetails(int dataId)
        {
            // Récupérer les données de la base de données
            var data = await db.Datas
                .Where(d => d.IdData == dataId)
                .Select(d => new
                {
                    d.IdData,
                    d.Title,
                    AcquisitionDate = d.AcquisitionDate,
                    PublicationDate = d.PublicationDate,
                    LastUpdatedDate = d.LastUpdatedDate,
                    d.Description,
                    d.Category,
                    ThemeName = d.Theme != null ? d.Theme.nom : null,
                    d.Coverage,
                    d.SpatialResolution,
                    d.Summary,
                    d.UrlData
                })
                .FirstOrDefaultAsync();

            if (data == null)
            {
                return HttpNotFound();
            }

            // Liste pour stocker tous les fichiers sans doublon
            var uniqueFiles = new HashSet<FileViewModel>();

            // Récupérer tous les fichiers s'ils existent dans UrlData
            if (!string.IsNullOrEmpty(data.UrlData))
            {
                // Supposons que les chemins des fichiers soient séparés par des virgules ou des points-virgules
                var filePaths = data.UrlData.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var filePath in filePaths)
                {
                    var fileName = Path.GetFileName(filePath.Trim());

                    // Ajouter les fichiers à la liste sans doublon
                    if (!uniqueFiles.Any(f => f.FileName == fileName))
                    {
                        uniqueFiles.Add(new FileViewModel
                        {
                            FileName = fileName,
                            FilePath = filePath.Trim()
                        });
                    }
                }
            }

            // Créer le modèle de vue avec les fichiers uniques
            var detailsViewModel = new DataDetailsViewModel
            {
                IdData = data.IdData,
                Title = data.Title,
                AcquisitionDate = data.AcquisitionDate.ToString("yyyy-MM-dd"),
                PublicationDate = data.PublicationDate.ToString("yyyy-MM-dd"),
                LastUpdatedDate = data.LastUpdatedDate.ToString("yyyy-MM-dd"),
                Description = data.Description,
                Category = data.Category,
                ThemeName = data.ThemeName,
                Coverage = data.Coverage,
                SpatialResolution = data.SpatialResolution,
                Summary = data.Summary,
                Files = uniqueFiles.ToList() // Convertir en liste pour la vue
            };

            return Json(detailsViewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult DownloadFile(string fileName, int dataId)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid file name.");
            }

            var data = db.Datas.Include(d => d.DataBases).FirstOrDefault(d => d.IdData == dataId);
            if (data == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Data not found.");
            }

            // Liste des chemins potentiels où le fichier pourrait être stocké
            var potentialFilePaths = new List<string>();

            // Chemin dans le dossier portant le nom du titre de la donnée
            var titleDirectoryPath = Server.MapPath($"~/App_Data/Uploads/{data.Title}");
            var titleFilePath = Path.Combine(titleDirectoryPath, fileName);
            potentialFilePaths.Add(titleFilePath);

            // Chemins dans les dossiers portant les noms des bases de données associées
            foreach (var database in data.DataBases)
            {
                var dbDirectoryPath = Server.MapPath($"~/App_Data/Uploads/{database.DataBaseName}");
                var dbFilePath = Path.Combine(dbDirectoryPath, fileName);
                potentialFilePaths.Add(dbFilePath);
            }

            // Rechercher le fichier dans les chemins potentiels
            string foundFilePath = potentialFilePaths.FirstOrDefault(System.IO.File.Exists);

            if (foundFilePath == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "File not found.");
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(foundFilePath);
            string contentType = MimeMapping.GetMimeMapping(foundFilePath);

            return File(fileBytes, contentType, fileName);
        }
        [HttpPost]
        public async Task<ActionResult> UploadDataFile(HttpPostedFileBase file, int dataId)
        {
            // Créer une liste pour stocker les messages de débogage
            var debugMessages = new List<string>();

            // Vérification du fichier
            if (file == null || file.ContentLength == 0)
            {
                debugMessages.Add("Erreur: Aucun fichier sélectionné ou fichier vide.");
                return Json(new { success = false, message = "File not selected.", debug = debugMessages });
            }

            // Récupération des données associées à l'ID
            var data = await db.Datas.Include(d => d.DataBases).FirstOrDefaultAsync(d => d.IdData == dataId);
            if (data == null)
            {
                debugMessages.Add($"Erreur: Données avec ID {dataId} introuvables.");
                return Json(new { success = false, message = "Data not found.", debug = debugMessages });
            }

            // Nom du fichier
            string fileName = Path.GetFileName(file.FileName);
            if (string.IsNullOrEmpty(fileName))
            {
                debugMessages.Add("Erreur: Nom de fichier non valide.");
                return Json(new { success = false, message = "Invalid file name.", debug = debugMessages });
            }

            // Chemins de fichiers sauvegardés
            var savedFilePaths = new List<string>();

            foreach (var database in data.DataBases)
            {
                var dbDirectoryPath = Server.MapPath($"~/App_Data/Uploads/{database.DataBaseName}");

                // Vérification et création du répertoire si nécessaire
                if (!Directory.Exists(dbDirectoryPath))
                {
                    debugMessages.Add($"Création du répertoire: {dbDirectoryPath}");
                    Directory.CreateDirectory(dbDirectoryPath);
                }

                // Chemin complet du fichier à sauvegarder
                string dbFilePath = Path.Combine(dbDirectoryPath, fileName);

                // Sauvegarde du fichier si non existant
                if (!System.IO.File.Exists(dbFilePath))
                {
                    file.SaveAs(dbFilePath);
                    debugMessages.Add($"Fichier sauvegardé dans: {dbFilePath}");
                }
                else
                {
                    debugMessages.Add($"Le fichier existe déjà: {dbFilePath}");
                }

                savedFilePaths.Add(dbFilePath);
            }

            // Mise à jour du champ UrlData
            if (!string.IsNullOrEmpty(data.UrlData))
            {
                data.UrlData = $"{data.UrlData},{string.Join(",", savedFilePaths)}";
            }
            else
            {
                data.UrlData = string.Join(",", savedFilePaths);
            }

            debugMessages.Add($"UrlData mis à jour avec les chemins: {data.UrlData}");

            try
            {
                // Sauvegarde dans la base de données
                await db.SaveChangesAsync();
                debugMessages.Add("Changements sauvegardés dans la base de données.");
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(e => e.ValidationErrors)
                    .Select(e => $"{e.PropertyName}: {e.ErrorMessage}");

                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = $"Échec de la validation pour une ou plusieurs entités. Détails: {fullErrorMessage}";

                debugMessages.Add($"Erreur lors de la sauvegarde: {exceptionMessage}");
                return Json(new { success = false, message = exceptionMessage, debug = debugMessages });
            }

            // Récupérer la liste des fichiers mis à jour
            var updatedFiles = data.DataBases
                .SelectMany(db => Directory.GetFiles(Server.MapPath($"~/App_Data/Uploads/{db.DataBaseName}")))
                .Select(filePath => new { FileName = Path.GetFileName(filePath) })
                .ToList();

            // Retourner le succès et les messages de débogage
            return Json(new { success = true, message = "File uploaded successfully.", fileName = fileName, files = updatedFiles, debug = debugMessages });
        }
        [HttpPost]
        public async Task<JsonResult> DeleteFile(string fileName, int dataId)
        {
            if (string.IsNullOrEmpty(fileName) || dataId <= 0)
            {
                return Json(new { success = false, message = "Invalid input." });
            }

            // Récupérer l'objet Data et ses bases de données associées
            var data = await db.Datas.Include(d => d.DataBases).FirstOrDefaultAsync(d => d.IdData == dataId);
            if (data == null)
            {
                return Json(new { success = false, message = "Data not found." });
            }

         

            foreach (var database in data.DataBases)
            {
                var dbDirectoryPath = Server.MapPath($"~/App_Data/Uploads/{database.DataBaseName}");
                var dbFilePath = Path.Combine(dbDirectoryPath, fileName);

                if (System.IO.File.Exists(dbFilePath))
                {
                    try
                    {
                        // Supprimer le fichier du répertoire
                        System.IO.File.Delete(dbFilePath);

                        var filePaths = data.UrlData.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        var updatedPaths = filePaths.Where(path => !path.EndsWith(fileName)).ToList();
                        data.UrlData = string.Join(",", updatedPaths);
                        await db.SaveChangesAsync();

                    }
                    catch (Exception ex)
                    {
                        return Json(new { success = false, message = $"Failed to delete the file: {ex.Message}" });
                    }
                }
            }



           


            

            return Json(new { success = true, message = "File deleted successfully." });
        }


    }
}
