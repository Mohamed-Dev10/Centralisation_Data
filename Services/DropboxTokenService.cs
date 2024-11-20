using Dropbox.Api;
using Dropbox.Api.Files;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace CentralisationV0.Services
{


    public class DropboxTokenService
    {
        private string _accessToken;
        private string _refreshToken;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _redirectUri;

        public DropboxTokenService(string clientId, string clientSecret, string redirectUri)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            _redirectUri = redirectUri;
        }

        public DropboxTokenService(string accessToken)
        {
            _accessToken = accessToken;
        }

        public async Task<(string AccessToken, string RefreshToken)> GetTokensAsync(string code)
        {
            using (var client = new HttpClient())
            {
                var tokenRequest = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "code", code },
                { "grant_type", "authorization_code" },
                { "client_id", _clientId },
                { "client_secret", _clientSecret },
                { "redirect_uri", _redirectUri }
            });

                var response = await client.PostAsync("https://api.dropboxapi.com/oauth2/token", tokenRequest);
                var jsonResponse = await response.Content.ReadAsStringAsync();
                dynamic tokenData = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse);
                return (tokenData.access_token.ToString(), tokenData.refresh_token.ToString());
            }
        }

        public async Task UploadFileAsync(string path, string fileName, Stream fileStream)
        {
            using (var dbx = new DropboxClient(_accessToken))
            {
                await dbx.Files.UploadAsync($"{path}/{fileName}", WriteMode.Overwrite.Instance, body: fileStream);
            }
        }

        public async Task RefreshAccessTokenAsync()
        {
            using (var client = new HttpClient())
            {
                var tokenRequest = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "refresh_token", _refreshToken },
                { "grant_type", "refresh_token" },
                { "client_id", _clientId },
                { "client_secret", _clientSecret }
            });

                var response = await client.PostAsync("https://api.dropboxapi.com/oauth2/token", tokenRequest);
                var jsonResponse = await response.Content.ReadAsStringAsync();
                dynamic tokenData = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse);
                _accessToken = tokenData.access_token.ToString();
            }
        }
    }




}