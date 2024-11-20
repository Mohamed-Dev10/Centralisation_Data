using System;
using System.IO;
using System.Linq;
using System.Net; // Assure-toi que ce namespace est inclus pour HttpStatusCode
using System.Threading.Tasks;
using System.Web;
using System.Data.Entity;
using System.Collections.Generic;
using System.Web.Mvc;
using CentralisationV0.Models.Entities;

using CentralisationdeDonnee.Models;



namespace CentralisationV0.Controllers
{
    public class LivrableController : Controller
    {

        private readonly CentralisationContext db = new CentralisationContext();


        [HttpPost]
        public async Task<ActionResult> UploadLivrable(HttpPostedFileBase file, int dataId)
        {
            if (file == null || file.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "File not selected.");
            }

            var collaboration = await db.Collaborations.FindAsync(dataId);
            if (collaboration == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Collaboration not found.");
            }

            // ExtractCollaborationName could be null if it is not implemented correctly
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
            // Utilisez la propriété Titre ou toute autre propriété pertinente pour extraire le nom
            return collaboration.Titre ?? "DefaultName";
        }
        [HttpGet]
        public ActionResult DownloadLivrable(string fileName, string collaborationName)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(collaborationName))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid file name or collaboration name.");
            }

            var directoryPath = Server.MapPath($"~/App_Data/LivrablesFiles/{collaborationName}");
            var filePath = Path.Combine(directoryPath, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "File not found.");
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            string contentType = MimeMapping.GetMimeMapping(filePath);

            return File(fileBytes, contentType, fileName);
        }

        [HttpGet]
        public async Task<ActionResult> GetCollaborationDetails(int dataId)
        {
            var collaboration = await db.Collaborations
                .Where(c => c.idCollaborateur == dataId)
                .Select(c => new
                {
                    c.idCollaborateur,
                    c.Titre,
                    c.description,
                    c.StartDate,
                    c.EndDate,
                    c.Status,
                    c.Duration,
                    ClientName = c.Client != null ? c.Client.clientName : null,
                    TypeCollaborationName = c.TypeCollaboration != null ? c.TypeCollaboration.NomType : null,
                    LivrablesPaths = c.LivrablesPaths
                })
                .FirstOrDefaultAsync();

            if (collaboration == null)
            {
                return HttpNotFound();
            }

            // Traitement en mémoire
            var detailsViewModel = new IndexAndDetailsViewModel.CollaborationDetailsViewModel
            {
                Id = collaboration.idCollaborateur,
                Titre = collaboration.Titre,
                Description = collaboration.description,
                ClientName = collaboration.ClientName,
                TypeCollaborationName = collaboration.TypeCollaborationName,
                StartDate = collaboration.StartDate.ToString("yyyy-MM-dd"), // Formattez la date selon vos besoins
                EndDate = collaboration.EndDate.ToString("yyyy-MM-dd"),
                
                Status = collaboration.Status,
                Duration = collaboration.Duration,
                Files = collaboration.LivrablesPaths != null
                    ? collaboration.LivrablesPaths.Split(',')
                        .Select(fp => new IndexAndDetailsViewModel.FileViewModel
                        {
                            FileName = Path.GetFileName(fp),
                            FilePath = fp
                        })
                        .ToList()
                    : new List<IndexAndDetailsViewModel.FileViewModel>()
            };

            return Json(detailsViewModel, JsonRequestBehavior.AllowGet);
        }






    }
}
