using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using Microsoft.Data.SqlClient;

namespace Front.Pages
{
    public class single_productModel : PageModel
    {
        public ClientInfo Employee { get; set; }

        public IActionResult OnGet(string Name)
        {
            try
            {
                // Note: Ensure to replace the connection string with the correct one
                string connectionString = "Server=localhost;Database=shop_Database;User Id=SA;Password=KaK123456B;TrustServerCertificate=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT sp.[Name], sp.[Type], sp.[Price], sp.[Quantity], ct.[Definition] " +
                                 "FROM Dbo.SpareParts sp " +
                                 "INNER JOIN Dbo.Catalog ct ON sp.PartId = ct.PartId " +
                                 "WHERE sp.[Name] = @Name";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Name", Name);

                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            if (dataReader.Read())
                            {
                                Employee = new ClientInfo
                                {
                                    Name = Convert.ToString(dataReader["Name"]),
                                    Type = Convert.ToString(dataReader["Type"]),
                                    Definition = Convert.ToString(dataReader["Definition"]),
                                    Price = Convert.ToInt32(dataReader["Price"]),
                                    Quantity = Convert.ToInt32(dataReader["Quantity"]),
                                    ImageUrl = "/path/to/employee/image.jpg"
                                };
                            }
                        }
                    }
                }

                return Page();
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                throw;
            }
        }

        public IActionResult OnPostAddToCart(string productName)
        {
            try
            {
                string connectionString = "Server=localhost;Database=shop_Database;User Id=SA;Password=KaK123456B;TrustServerCertificate=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Check if the item is already in the cart
                    string checkSql = "SELECT [Quantity] FROM [shop_Database].[dbo].[cartdata] WHERE [Name] = @Name";

                    using (SqlCommand checkCommand = new SqlCommand(checkSql, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@Name", productName);

                        var existingQuantity = checkCommand.ExecuteScalar();
                        if (existingQuantity != null)
                        {
                            // Item already exists, update the quantity
                            string updateSql = "UPDATE [shop_Database].[dbo].[cartdata] " +
                                               "SET [Quantity] = [Quantity] + 1 " +
                                               "WHERE [Name] = @Name";

                            using (SqlCommand updateCommand = new SqlCommand(updateSql, connection))
                            {
                                updateCommand.Parameters.AddWithValue("@Name", productName);
                                updateCommand.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // Item does not exist, insert a new record
                            string insertSql = "INSERT INTO [shop_Database].[dbo].[cartdata] ([PartId], [Type], [Price], [Name], [Quantity]) " +
                                               "SELECT [PartId], [Type], [Price], [Name], 1 " +
                                               "FROM Dbo.SpareParts " +
                                               "WHERE [Name] = @Name";

                            using (SqlCommand insertCommand = new SqlCommand(insertSql, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@Name", productName);
                                insertCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }

                return RedirectToPage("/Cart"); // Redirect to the cart page after adding to the cart
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                throw;
            }
        }
    }
}
