using Teklounge.Models;
using System.Data.SqlClient;

IConfiguration configuration = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json")
                            .Build();

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", async () =>
{
    var str = configuration["SQL_DB"];

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

    return customers;
});

app.Run();
