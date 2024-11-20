using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CentralisationV0.Models.Entities;


using CentralisationdeDonnee.Models; // Assurez-vous que l'espace de noms correspondant est importé


using CentralisationV0.Services; // Ajout de l'espace de noms pour le service de données
using CentralisationV0.Models.Entities.ViewModels;

namespace CentralisationV0.Controllers
{
    public class DataoiController : Controller
    {

        private readonly IDataService _dataService;
        private CentralisationContext db = new CentralisationContext();

        public DataoiController()
        {
            _dataService = new DataService();
        }

        public ActionResult Index()
        {
            var dataList = _dataService.GetData();
            var databaseList = db.DataBases.ToList();
            var coordinateSystems = db.CoordinateSystems.ToList();

            var viewModel = new DataAndDatabaseViewModel
            {
                DataList = dataList,
                DatabaseList = databaseList,
                CoordinateSystems = coordinateSystems  // Ajout de cette ligne
            };

            ViewBag.Themes = db.Themes.ToList();
            return View("~/Views/Donnee/Index.cshtml", viewModel);
        }


        // GET: Data/Create
        public ActionResult Create()
        {
            ViewBag.Themes = new SelectList(db.Themes, "IdTheme", "nom");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(Data model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Données invalides." });
            }

            try
            {
                // Convertir l'ID du système de coordonnées en entier
                int coordinateSystemId;
                if (!int.TryParse(model.CoordinateSystemId.ToString(), out coordinateSystemId))
                {
                    return Json(new { success = false, message = "ID du système de coordonnées invalide." });
                }

                // Créez une instance de votre modèle Data
                var data = new Data
                {
                    Title = model.Title,
                    AcquisitionDate = model.AcquisitionDate,
                    PublicationDate = model.PublicationDate,
                    LastUpdatedDate = model.LastUpdatedDate,
                    Description = model.Description,
                    Category = model.Category,
                    CoordinateSystemId = model.CoordinateSystemId,
                    DataTypeId = model.DataTypeId,
                    ThemeId = model.ThemeId,

                    Coverage = model.Coverage,
                    SpatialResolution = model.SpatialResolution,
                    Summary = model.Summary,


                };


                // Rechercher l'industrie par son nom
                var industry = db.Industries.FirstOrDefault(i => i.nom == model.IndustryName && i.ThemeId == data.ThemeId);
                if (industry == null)
                {
                    industry = new Industry
                    {
                        nom = model.IndustryName,
                        ThemeId = data.ThemeId

                    };
                    db.Industries.Add(industry);
                    db.SaveChanges();
                }

                // Associer l'ID de l'industrie à l'objet Data
                data.IndustryId = industry.IdUseConstraint;

                // Associer des valeurs par défaut pour les autres champs obligatoires
                if (data.DataTypeId == 0) data.DataTypeId = 1;

               

                // Ajouter l'objet Data à la base de données
                db.Datas.Add(data);
                db.SaveChanges();

                return Json(new { success = true, message = "Les données ont été enregistrées avec succès." });
            }
            catch (Exception ex)
            {
                // Log the exception details for debugging purposes
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");

                return Json(new { success = false, message = "Une erreur s'est produite lors de l'enregistrement des données." });
            }
        }


        // GET: Data/GetById/5
        public JsonResult GetById(int id)
        {
            var data = db.Datas.Include(d => d.Theme).FirstOrDefault(d => d.IdData == id);
            if (data == null)
            {
                return Json(new { success = false, message = "Donnée non trouvée" }, JsonRequestBehavior.AllowGet);
            }

            // Formater les dates au format ISO8601
            var isoAcquisitionDate = data.AcquisitionDate.ToString("yyyy-MM-ddTHH:mm:ss");
            var isoPublicationDate = data.PublicationDate.ToString("yyyy-MM-ddTHH:mm:ss");
            var isoLastUpdatedDate = data.LastUpdatedDate.ToString("yyyy-MM-ddTHH:mm:ss");

            // Construire l'objet JSON à retourner
            var jsonData = new
            {
                success = true,
                data = new
                {
                    IdData = data.IdData,
                    Title = data.Title,
                    AcquisitionDate = isoAcquisitionDate, // Format ISO8601
                    PublicationDate = isoPublicationDate, // Format ISO8601
                    LastUpdatedDate = isoLastUpdatedDate, // Format ISO8601
                    Description = data.Description,
                    Theme = data.Theme != null ? new { nom = data.Theme.nom } : null,
                    Coverage = data.Coverage,
                    SpatialResolution = data.SpatialResolution,
                    Summary = data.Summary,
                    Category = data.Category
                }
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
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






    }
}
