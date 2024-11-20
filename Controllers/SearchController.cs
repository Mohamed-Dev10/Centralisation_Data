using System;
using System.Linq;
using System.Web.Mvc;
using CentralisationV0.Models.Entities;
using System.Data.Entity;


namespace CentralisationV0.Controllers
{
    public class SearchController : Controller
    {
        private readonly CentralisationContext _context;

        public SearchController()
        {
            _context = new CentralisationContext();
        }

        // Recherche Globale
        [HttpGet]
        public JsonResult Global(string query)
        {
            // Vérifiez si la requête est vide
            bool isQueryEmpty = string.IsNullOrWhiteSpace(query);

            var dataResults = isQueryEmpty
                ? _context.Datas.Include(d => d.Theme).ToList()
                : _context.Datas
                    .Include(d => d.Theme)
                    .Where(d => d.Title.Contains(query) ||
                                d.Description.Contains(query) ||
                                d.Keywords.Contains(query) ||
                                d.Category.Contains(query) ||
                                d.Telecommunication.Contains(query) ||
                                d.Coverage.Contains(query) ||
                                d.Summary.Contains(query))
                    .ToList();

            var databaseResults = isQueryEmpty
                ? _context.DataBases.ToList()
                : _context.DataBases
                    .Where(db => db.DataBaseName.Contains(query) || db.Owner.Contains(query) || db.description.Contains(query) || db.Keywords.Contains(query))
                    .ToList();

            var results = new
            {
                DataResults = dataResults.Select(d => new
                {
                    d.IdData,
                    d.Title,
                    AcquisitionDate = d.AcquisitionDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                    PublicationDate = d.PublicationDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                    LastUpdatedDate = d.LastUpdatedDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                    d.Description,
                    d.Category,
                    ThemeName = d.Theme.nom,
                    d.Coverage,
                    d.SpatialResolution,
                    d.Summary
                }).ToList(),
                DatabaseResults = databaseResults.Select(db => new
                {
                    db.idDataBase,
                    db.DataBaseName,
                    db.Owner,
                    db.description,
                    db.Keywords,
                    createdDate = db.createdDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                }).ToList()
            };

            return Json(results, JsonRequestBehavior.AllowGet);
        }

        // Recherche dans les Données
        [HttpGet]
        public JsonResult Data(string query)
        {
            // Vérifier si la requête est vide ou contient seulement des espaces blancs
            bool isQueryEmpty = string.IsNullOrWhiteSpace(query);

            var dataResults = _context.Datas
                .Include(d => d.Theme)
                .Where(d => isQueryEmpty ||
                            d.Title.Contains(query) ||
                            d.Description.Contains(query) ||
                            d.Keywords.Contains(query) ||
                            d.Category.Contains(query) ||
                            d.Telecommunication.Contains(query) ||
                            d.Coverage.Contains(query) ||
                            d.Summary.Contains(query))
                .ToList()
                .Select(d => new
                {
                    d.IdData,
                    d.Title,
                    AcquisitionDate = d.AcquisitionDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                    PublicationDate = d.PublicationDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                    LastUpdatedDate = d.LastUpdatedDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                    d.Description,
                    d.Category,
                    ThemeName = d.Theme.nom,
                    d.Coverage,
                    d.SpatialResolution,
                    d.Summary
                })
                .ToList();

            return Json(dataResults, JsonRequestBehavior.AllowGet);
        }


        // Recherche dans les Bases de Données
        [HttpGet]
        public JsonResult Database(string query)
        {
            var databaseResults = _context.DataBases
                .Where(db => db.DataBaseName.Contains(query) || db.Owner.Contains(query) || db.description.Contains(query) || db.Keywords.Contains(query))
                .ToList()
                .Select(db => new
                {
                    db.idDataBase,
                    db.DataBaseName,
                    db.Owner,
                    db.description,
                    db.Keywords,
                    createdDate = db.createdDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                })
                .ToList();

            return Json(databaseResults, JsonRequestBehavior.AllowGet);
        }

        // Récupérer toutes les Données
        [HttpGet]
        public JsonResult AllData(string query)
        {
            string lowerQuery = query?.ToLower() ?? string.Empty;
            // Filtrer les données en fonction de la requête de recherche
            var dataResults = _context.Datas
                .Include(d => d.Theme)
                .AsEnumerable()
                .Where(d => string.IsNullOrEmpty(lowerQuery) ||
                            d.Title.ToLower().Contains(lowerQuery) ||
                            d.Description.ToLower().Contains(lowerQuery) ||
                            d.Category.ToLower().Contains(lowerQuery) ||
                            (d.Theme != null && d.Theme.nom.ToLower().Contains(lowerQuery)))
                .Select(d => new
                {
                    d.IdData,
                    d.Title,
                    AcquisitionDate = d.AcquisitionDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                    PublicationDate = d.PublicationDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                    LastUpdatedDate = d.LastUpdatedDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                    d.Description,
                    d.Category,
                    ThemeName = d.Theme != null ? d.Theme.nom : "",
                    d.Coverage,
                    d.SpatialResolution,
                    d.Summary
                })
                .ToList();

            return Json(dataResults, JsonRequestBehavior.AllowGet);
        }


        // Récupérer toutes les Bases de Données
        [HttpGet]
        public JsonResult AllDatabases()
        {
            var databaseResults = _context.DataBases
                .AsEnumerable()
                .Select(db => new
                {
                    db.idDataBase,
                    db.DataBaseName,
                    db.Owner,
                    db.description,
                    db.Keywords,
                    createdDate = db.createdDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                })
                .ToList();

            return Json(databaseResults, JsonRequestBehavior.AllowGet);
        }
    }
}
