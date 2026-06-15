using System.Text;
using System.Text.Json;

namespace PortfolioApi
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public EmailService(IConfiguration configuration,
                            HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task SendLeadEmail(string name,
                                        string email,
                                        string question)
        {
            var apiKey = _configuration["Resend:ApiKey"];

            _httpClient.DefaultRequestHeaders.Clear();

            _httpClient.DefaultRequestHeaders.Add(
                "Authorization",
                $"Bearer {apiKey}");

            var body = new
            {
                from = "onboarding@resend.dev",

                to = new[]
                {
                    "aaabhishekmishra123@gmail.com"
                },

                subject = "🚀 New Portfolio Lead",

                html = $@"
                    <h2>New Portfolio Lead</h2>

                    <p><b>Name:</b> {name}</p>

                    <p><b>Email:</b> {email}</p>

                    <p><b>Question:</b></p>

                    <p>{question}</p>"
            };

            var json = JsonSerializer.Serialize(body);

            var response = await _httpClient.PostAsync(
                "https://api.resend.com/emails",
                new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                throw new Exception($"Resend Error: {error}");
            }

            Console.WriteLine("Email sent successfully.");
        }
    }
}