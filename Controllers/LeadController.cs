using Microsoft.AspNetCore.Mvc;
using PortfolioApi.Models;
using Npgsql;

namespace PortfolioApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeadController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public LeadController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> SaveLead([FromBody] Lead lead)
        {
            try
            {
                using var connection =
                    new NpgsqlConnection(
                        _configuration.GetConnectionString("DefaultConnection"));

                await connection.OpenAsync();

                string query =
                    @"INSERT INTO leads(name,email,question)
                      VALUES(@name,@email,@question)";

                using var command =
                    new NpgsqlCommand(query, connection);

                command.Parameters.AddWithValue("@name", lead.Name);
                command.Parameters.AddWithValue("@email", lead.Email);
                command.Parameters.AddWithValue("@question", lead.Question);

                await command.ExecuteNonQueryAsync();

                return Ok(new
                {
                    Message = "Lead Saved Successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetLeads()
        {
            List<Lead> leads = new();

            using var connection =
                new NpgsqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

            await connection.OpenAsync();

            string query =
                "SELECT id,name,email,question FROM leads";

            using var command =
                new NpgsqlCommand(query, connection);

            using var reader =
                await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                leads.Add(new Lead
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Email = reader.GetString(2),
                    Question = reader.GetString(3)
                });
            }

            return Ok(leads);
        }

        [HttpGet("test-db")]
        public async Task<IActionResult> TestDb()
        {
            try
            {
                using var connection =
                    new NpgsqlConnection(
                        _configuration.GetConnectionString("DefaultConnection"));

                await connection.OpenAsync();

                return Ok("Database Connected Successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }
    }
}