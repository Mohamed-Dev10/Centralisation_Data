using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CentralisationV0.Models.Entities;
using CentralisationV0.Services;
using CentralisationdeDonnee.Models;

using CentralisationV0.Models.Entities.ViewModels;



namespace CentralisationV0.Controllers
{
    public class DonneeController : Controller
    {
        private readonly IDataService _dataService;
        private CentralisationContext db = new CentralisationContext();
        public DonneeController()
        {
            _dataService = new DataService();
        }
        public ActionResult Index()
        {
            var dataList = _dataService.GetData();
            var databaseList = db.DataBases.ToList();
            var coordinateSystems = db.CoordinateSystems.ToList();
            var dataTyps = db.DataTyps.ToList();
            var themes = db.Themes.ToList();
            var industry = db.Industries.ToList();


            var viewModel = new DataAndDatabaseViewModel
            {
                DataList = dataList,
                DatabaseList = databaseList,
                 CoordinateSystems = coordinateSystems, // Ajout de cette ligne
                DataTypes = dataTyps,
                Themes=themes,
                Industries=industry

            };

            ViewBag.Themes = db.Themes.ToList();
            return View(viewModel);
        }
        // GET: Data/GetById/5
        public JsonResult GetById(int id)
        {
            var data = db.Datas.Include(d => d.Theme).FirstOrDefault(d => d.IdData == id);
            if (data == null)
            {
                return Json(new { success = false, message = "Donnée non trouvée" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
        }



        // GET: Donnee/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }



        // POST: Donnee/Create
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

        // GET: Donnee/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Donnee/Edit/5
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

        // GET: Donnee/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Donnee/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
