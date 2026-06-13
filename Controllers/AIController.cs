using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace PortfolioApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AIController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AIController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost]
    public async Task<IActionResult> Ask([FromBody] string question)
    {
        string resumeData = @"
        Name: Abhishek Mishra
        Role: Full Stack Developer

        Skills:
        ASP.NET Core MVC
        React.js
        SQL Server
        ADO.NET
        Entity Framework
        Web API

        Experience:
        Graduate Trainee at Motherson Technology Services
        Apprentice at Techpile Technology

        Projects:
        Course Selling Platform
        Task Management System
        Employee Management System

        Availability:
        Immediate Joiner
        ";

        var prompt =
            $"Answer only from this information:\n{resumeData}\n\nQuestion:{question}";

        using var client = new HttpClient();

        var apiKey =
            _configuration["Gemini:ApiKey"];

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new
                        {
                            text = prompt
                        }
                    }
                }
            }
        };

        var response =
            await client.PostAsync(
                $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}",
                new StringContent(
                    JsonConvert.SerializeObject(requestBody),
                    Encoding.UTF8,
                    "application/json"));

        var result =
    await response.Content.ReadAsStringAsync();

        Console.WriteLine(result);

        return Content(result, "application/json");
    }
}