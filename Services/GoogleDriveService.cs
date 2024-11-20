using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace CentralisationV0.Services
{
    public class GoogleDriveService
    {
        private readonly DriveService _service;

        public GoogleDriveService()
        {
            string credentialsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configuration", "client_secret_529217040387-90o4i8eol3l7rfeoaaoft0mse22457ln.apps.googleusercontent.com.json");

            UserCredential credential;
            using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
            {
                string credPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configuration", "token.json");
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] { DriveService.Scope.DriveFile },
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            _service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Centralisation",
            });
        }

        public async Task UploadFileAsync(string fileName, Stream fileStream, string contentType)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = fileName
            };

            FilesResource.CreateMediaUpload request;
            request = _service.Files.Create(fileMetadata, fileStream, contentType);
            request.Fields = "id";
            await request.UploadAsync();
        }
    }
}