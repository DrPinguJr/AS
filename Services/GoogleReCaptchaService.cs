using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace dit230680c_AS.Services
{
    public class GoogleReCaptchaService
    {
        private readonly string _secretKey;

        public GoogleReCaptchaService(IConfiguration configuration)
        {
            // Get Secret Key from configuration
            _secretKey = configuration["GoogleReCaptcha:SecretKey"];
        }

        public async Task<bool> VerifyCaptchaAsync(string captchaToken)
        {
            if (string.IsNullOrEmpty(captchaToken))
            {
                return false;
            }

            var client = new HttpClient();
            var response = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("secret", _secretKey),
                new KeyValuePair<string, string>("response", captchaToken)
            }));

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ReCaptchaResponse>(jsonResponse);
            return result.Success;
        }
    }

    public class ReCaptchaResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("error-codes")]
        public string[] ErrorCodes { get; set; }
    }
}
