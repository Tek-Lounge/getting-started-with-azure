using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Teklounge.Models;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Teklounge
{
    public static class DemoFunction
    {
        [FunctionName("DemoFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "customers")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("HTTP Trigger");

            var str = Environment.GetEnvironmentVariable("SQL_DB");

            var customers = new List<Customer>();

            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                var text = "SELECT * from [SalesLT].[Customer]";

                using (SqlCommand cmd = new SqlCommand(text, conn))
                {
                    var reader = await cmd.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        var customer = new Customer()
                        {
                            Id = Convert.ToInt32(reader["CustomerID"].ToString()),
                            Title = reader["Title"].ToString(),
                            FirstName = reader["FirstName"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            EmailAddress = reader["EmailAddress"].ToString()
                        };

                        customers.Add(customer);
                    }
                }
            }

            return new OkObjectResult(customers);
        }
    }
}
