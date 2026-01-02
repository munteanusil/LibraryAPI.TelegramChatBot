using Library.Application.Interfaces;
using Library.Infrastructure.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Library.Infrastructure.ExternalServices
{
    public class GoogleService : IGoogleService
    {
        private readonly HttpClient _httpClient;
        private readonly GoogleConfigurations _configuration; 

        public GoogleService(HttpClient httpClient,IOptions<GoogleConfigurations> configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration.Value;
        }
        public async Task<string> GetIdTocken(string code, CancellationToken ct = default)
        {
            var tokenRequest = new HttpRequestMessage(HttpMethod.Post,"/token");
            tokenRequest.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "code", code },
                {"client_id", _configuration.ClientId },
                {"client_secret", _configuration.ClientSecret },
                {"redirect_uri", _configuration.RedirectPath },
                {"grant_type", "authorization_code" }
            });
            var tokenResponse = await _httpClient.SendAsync(tokenRequest,ct);
            var tokenResponseContent = await tokenResponse.Content.ReadAsStringAsync(ct);
            var tokenData = JsonSerializer.Deserialize<JsonElement>(tokenResponseContent);
            var idToken = tokenData.GetProperty("id_token").GetString();
            return idToken;
        }

        public string GetRedirectLink()
        {
            var scope = "openid email profile";

            var url =
                $"{_configuration.BaseUrl}" +
                $"client_id={_configuration.ClientId}" +
                $"&redirect_uri={Uri.EscapeDataString(_configuration.RedirectPath)}" +
                $"&response_type=code" +
                $"&scope={Uri.EscapeDataString(scope)}";

            return url;
        }
    }
}
