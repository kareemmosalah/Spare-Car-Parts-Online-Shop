using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

using Microsoft.Data.SqlClient;
namespace Front.Pages
{
    public class login_registerModel : PageModel
    {
        [BindProperty]
        public LoginModel LoginData { get; set; }

        [BindProperty]
        public RegisterModel RegisterData { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPostLogin()
        {
            try
            {
                string connectionString = "Server=localhost;Database=shop_Database;User Id=SA;Password=KaK123456B;TrustServerCertificate=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT PasswordHash FROM [dbo].[Cust] WHERE Email = @Email";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Email", LoginData.Email);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedHash = reader["PasswordHash"].ToString();
                                if (VerifyPassword(LoginData.Password, storedHash))
                                {
                                    // Login successful, handle session or redirect
                                    return RedirectToPage("/Index");
                                }
                                else
                                {
                                    ErrorMessage = "Invalid password.";
                                }
                            }
                            else
                            {
                                ErrorMessage = "User not found.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred. Please try again later.";
            }

            return Page();
        }

        public IActionResult OnPostRegister()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string passwordHash = HashPassword(RegisterData.Password);

                    string connectionString = "Server=localhost;Database=shop_Database;User Id=SA;Password=KaK123456B;TrustServerCertificate=True";

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string sql = "INSERT INTO [dbo].[Cust] (FirstName, LastName, Email, PasswordHash) " +
                                     "VALUES (@FirstName, @LastName, @Email, @PasswordHash)";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@FirstName", RegisterData.FirstName);
                            command.Parameters.AddWithValue("@LastName", RegisterData.LastName);
                            command.Parameters.AddWithValue("@Email", RegisterData.Email);
                            command.Parameters.AddWithValue("@PasswordHash", passwordHash);

                            command.ExecuteNonQuery();
                        }
                    }

                    return RedirectToPage("/LoginRegister"); // Or wherever you want to redirect after registration
                }
                catch (SqlException ex)
                {
                    ErrorMessage = "An error occurred while registering. Please try again.";
                }
            }

            return Page();
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            string enteredHash = HashPassword(enteredPassword);
            return enteredHash == storedHash;
        }
    }

    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RegisterModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
