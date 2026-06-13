using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using PortfolioApi.Models;

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
                using (SqlConnection connection =
                    new SqlConnection(
                        _configuration.GetConnectionString("DefaultConnection")))
                {
                    SqlCommand command =
                        new SqlCommand("SP_INSERT_LEAD", connection);

                    command.CommandType =
                        CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Name", lead.Name);
                    command.Parameters.AddWithValue("@Email", lead.Email);
                    command.Parameters.AddWithValue("@Question", lead.Question);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();

                   
                   var emailService = new EmailService();
                    await emailService.SendLeadEmail(
                        lead.Name,
                       lead.Email,
                        lead.Question);
                }

                return Ok(new
                {
                    Message = "Lead Saved Successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while saving the lead.",
                    Error = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetLeads()
        {
            List<Lead> leads = new List<Lead>();

            using (SqlConnection connection =
                new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand command =
                    new SqlCommand("SP_GET_LEADS", connection);

                command.CommandType =
                    CommandType.StoredProcedure;

                await connection.OpenAsync();

                using (SqlDataReader reader =
                    await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        leads.Add(new Lead
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString(),
                            Email = reader["Email"].ToString(),
                            Question = reader["Question"].ToString()
                        });
                    }
                }
            }

            return Ok(leads);
        }
    }
}