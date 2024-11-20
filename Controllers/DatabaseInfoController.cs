using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CentralisationV0.Models.Entities;
using CentralisationdeDonnee.Models;
using System.Data.Entity;
using System.Web.Mvc;
using CentralisationV0.Services;
using CentralisationV0.Models.Entities.ViewModels;

namespace CentralisationV0.Controllers
{
    public class DatabaseInfoController : Controller
    {
        private readonly DatabaseInfoService _databaseInfoService = new DatabaseInfoService();

        private CentralisationContext db = new CentralisationContext();
        public ActionResult Index()
        {
            var dataList = db.Datas.ToList();
            var databaseList = _databaseInfoService.GetDatabases();

            var viewModel = new DataAndDatabaseViewModel
            {
                DataList = dataList,
                DatabaseList = databaseList
            };

            return View("~/Views/Donnee/Index.cshtml", viewModel);
        }






        // GET: DatabaseInfo/Create
        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "DatabaseName, Owner, createdDate, description")] DataBase database)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.DataBases.Add(database);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Les informations de la base de données ont été enregistrées avec succès." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Une erreur s'est produite lors de l'enregistrement des informations." });
                }
            }

            return Json(new { success = false, message = "Les informations de la base de données ne sont pas valides." });
        }
        // GET: DatabaseInfo/GetById/5
        public JsonResult GetById(int id)
        {
            var database = db.DataBases.FirstOrDefault(db => db.idDataBase == id);
            if (database == null)
            {
                return Json(new { success = false, message = "Base de données non trouvée" }, JsonRequestBehavior.AllowGet);
            }

            // Formater la date de création au format ISO8601
            var isoCreationDate = database.createdDate.ToString("yyyy-MM-ddTHH:mm:ss");

            // Construire l'objet JSON à retourner
            var jsonData = new
            {
                success = true,
                data = new
                {
                    idDataBase = database.idDataBase,
                    DataBaseName = database.DataBaseName,
                    Owner = database.Owner,
                    createdDate = isoCreationDate, // Format ISO8601
                    description = database.description,
                    Keywords = database.Keywords
                }
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Update(DataBase updatedDatabase)
        {
            if (ModelState.IsValid)
            {
                var database = db.DataBases.FirstOrDefault(db => db.idDataBase == updatedDatabase.idDataBase);

                if (database != null)
                {
                    database.DataBaseName = updatedDatabase.DataBaseName;
                    database.Owner = updatedDatabase.Owner;
                    database.createdDate = updatedDatabase.createdDate;
                    database.description = updatedDatabase.description;
                    database.Keywords = updatedDatabase.Keywords;

                    db.Entry(database).State = EntityState.Modified;
                    db.SaveChanges();

                    return Json(new { success = true, message = "Base de données mise à jour avec succès." });
                }

                return Json(new { success = false, message = "Base de données non trouvée." });
            }

            return Json(new { success = false, message = "État du modèle invalide." });
        }



    }
}