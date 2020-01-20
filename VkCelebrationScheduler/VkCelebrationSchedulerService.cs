using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using VkCelebrationScheduler.Logging;

namespace VkCelebrationScheduler
{
    internal class VkCelebrationSchedulerService : ServiceBase
    {
        private bool _disposed = false;

        private HttpClient _httpClient;
        private ISimpleLogger _logger;

        private Timer _timer;

        private IEnumerable<Account> _accounts;

        public VkCelebrationSchedulerService(IEnumerable<Account> accounts,
            ISimpleLogger logger)
        {
            _accounts = accounts;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://vk-celebration.azurewebsites.net/api/")
            };
            _logger = logger;
        }

        protected override async void OnStart(string[] args)
        {
            _timer = new Timer
            {
                Interval = TimeSpan.FromHours(1).TotalMilliseconds
            };
            _timer.Elapsed += async (s, e) => await ProcessAccounts();
            _timer.Enabled = true;

            await ProcessAccounts();
        }

        protected override void OnStop()
        {
            _timer.Enabled = false;
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _httpClient.Dispose();
                _timer?.Dispose();
            }

            _disposed = true;

            base.Dispose(disposing);
        }

        public async Task RunAsConsole(string[] args)
        {
            await ProcessAccounts();
        }

        private async Task ProcessAccounts()
        {
            _logger.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}] Started Process Accounts");

            foreach (var account in _accounts)
            {
                await DoWithCredentials(account.Login, account.Password, account.SearchParams);
            }
        }

        private async Task DoWithCredentials(string login, string password, SearchParams searchParams)
        {
            await Auth(login, password);

            _logger.WriteLine("-----------------------------------------------------");

            var statusCode = await SendCongratulation(searchParams);
            if (statusCode == HttpStatusCode.NotFound)
            {
                _logger.WriteLine("*****************************************************");
                searchParams.LastSeenMode = LastSeenMode.Last24Hours;
                await SendCongratulation(searchParams);
            }

            _logger.WriteLine("=====================================================");
        }

        private async Task<HttpStatusCode> SendCongratulation(SearchParams searchParams)
        {
            var jsonObject = JsonConvert.SerializeObject(searchParams, Formatting.Indented);

            _logger.WriteLine("Search Params: " + jsonObject);

            var content = new StringContent(jsonObject, Encoding.UTF8, "application/json");

            var res = await _httpClient.PostAsync("VkCelebration/SendRandomUserCongratulation", content);

            _logger.WriteLine("Status code: " + res.StatusCode);

            var resStr = await res.Content.ReadAsStringAsync();

            _logger.WriteLine("Result: " + resStr);

            return res.StatusCode;
        }

        private async Task Auth(string login, string password)
        {
            var credentials = new { Login = login, Password = password };
            var jsonObject = JsonConvert.SerializeObject(credentials, Formatting.Indented);

            _logger.WriteLine("Credentials: " + jsonObject);

            var content = new StringContent(jsonObject, Encoding.UTF8, "application/json");

            var res = await _httpClient.PostAsync("Auth", content);

            _logger.WriteLine("Status code: " + res.StatusCode);

            var resStr = await res.Content.ReadAsStringAsync();

            _logger.WriteLine("Result: " + resStr);

            var authResult = JsonConvert.DeserializeObject<AuthResult>(resStr);
            _httpClient.DefaultRequestHeaders.Authorization
                                     = new AuthenticationHeaderValue("Bearer", authResult.AuthToken);
        }

        private class AuthResult
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("auth_token")]
            public string AuthToken { get; set; }

            [JsonProperty("expires_in")]
            public long ExpiresIn { get; set; }
        }
    }
}
