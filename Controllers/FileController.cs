using System;


using System.IO;
using System.IO.Compression;

using System.Linq; // Ajoutez cette ligne pour utiliser les méthodes LINQ
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CentralisationV0.Models.Entities;
using CentralisationdeDonnee.Models;



namespace CentralisationV0.Controllers
{
    public class FileController : Controller
    {
        private readonly CentralisationContext db = new CentralisationContext();

        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> UploadFile(HttpPostedFileBase file, int dataId)
        {
            if (file == null || file.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "No file selected.");
            }

            var data = await db.Datas.FindAsync(dataId);
            if (data == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Data not found.");
            }

            // Extraire le nom de la base de données du nom du fichier
            string databaseName = ExtractDatabaseName(file.FileName);

            // Créer le répertoire si nécessaire
            var directoryPath = Server.MapPath($"~/App_Data/DataFiles/{databaseName}");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(directoryPath, fileName);

            // Renommer le fichier s'il existe déjà
            int count = 1;
            string fileNameOnly = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);
            while (System.IO.File.Exists(filePath))
            {
                fileName = $"{fileNameOnly}({count++}){extension}";
                filePath = Path.Combine(directoryPath, fileName);
            }

            file.SaveAs(filePath);

            // Mettre à jour le champ UrlData
            data.UrlData = filePath;

            // Sauvegarder les modifications
            await db.SaveChangesAsync();

            return Json(new { success = true, message = "File uploaded and Data updated successfully." });
        }

        // Méthode pour extraire le nom de la base de données à partir du nom du fichier
        private string ExtractDatabaseName(string fileName)
        {
            // Exemple : Extraire "ONCF" de "ONCF_shapefile.shp"
            string[] parts = fileName.Split('_');
            if (parts.Length > 1)
            {
                return parts[0];
            }
            return "Other";
        }

        [HttpGet]
        public ActionResult DownloadFile(string databaseName)
        {
            if (string.IsNullOrEmpty(databaseName))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid database name.");
            }

            var directoryPath = Server.MapPath($"~/App_Data/DataFiles/{databaseName}");
            if (!Directory.Exists(directoryPath))
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Directory not found.");
            }

            using (var memoryStream = new MemoryStream())
            {
                using (var zip = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var filePath in Directory.GetFiles(directoryPath))
                    {
                        var fileName = Path.GetFileName(filePath);
                        var entry = zip.CreateEntry(fileName, CompressionLevel.Optimal);

                        using (var entryStream = entry.Open())
                        using (var fileStream = System.IO.File.OpenRead(filePath))
                        {
                            fileStream.CopyTo(entryStream);
                        }
                    }
                }

                memoryStream.Seek(0, SeekOrigin.Begin);
                return File(memoryStream.ToArray(), "application/zip", $"{databaseName}_files.zip");
            }
        }




    }
}
