using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data.SqlClient;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
namespace Front.Pages
{
    public class my_accountModel : PageModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public void OnGet()
        {
            var userEmail = HttpContext.User.FindFirstValue(ClaimTypes.Email); // Get the user's email from claims

            if (userEmail == null)
            {
                // Handle the case where the user is not logged in
                RedirectToPage("/LoginRegister");
                return;
            }

            try
            {
                string connectionString = "Server=localhost;Database=shop_Database;User Id=SA;Password=KaK123456B;TrustServerCertificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT FirstName, LastName, Email FROM dbo.Cust WHERE Email = @Email";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Email", userEmail);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                FirstName = reader["FirstName"].ToString();
                                LastName = reader["LastName"].ToString();
                                Email = reader["Email"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                throw;
            }
        }
    }
}
