using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace Front.Pages
{
    public class cartModel : PageModel
    {
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();

        public void OnGet()
        {
            LoadCartItems();
        }

        private void LoadCartItems()
        {
            try
            {
                string connectionString = "Server=localhost;Database=shop_Database;User Id=SA;Password=KaK123456B;TrustServerCertificate=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT [PartId], [Name], [Type], [Price], [Quantity] FROM [shop_Database].[dbo].[cartdata]";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            CartItems.Clear(); // Clear existing items
                            while (dataReader.Read())
                            {
                                CartItems.Add(new CartItem
                                {
                                    PartId = Convert.ToInt32(dataReader["PartId"]),
                                    Name = Convert.ToString(dataReader["Name"]),
                                    Type = Convert.ToString(dataReader["Type"]),
                                    Price = Convert.ToDecimal(dataReader["Price"]),
                                    Quantity = Convert.ToInt32(dataReader["Quantity"])
                                });
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

        public IActionResult OnPostAddToCart(int partId, int quantity)
        {
            try
            {
                string connectionString = "Server=localhost;Database=shop_Database;User Id=SA;Password=KaK123456B;TrustServerCertificate=True";

                // Check if the item already exists in the cart
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string checkSql = "SELECT COUNT(*) FROM [shop_Database].[dbo].[cartdata] WHERE [PartId] = @PartId";

                    using (SqlCommand command = new SqlCommand(checkSql, connection))
                    {
                        command.Parameters.AddWithValue("@PartId", partId);
                        int count = (int)command.ExecuteScalar();

                        if (count > 0)
                        {
                            // Item exists, update quantity
                            string updateSql = "UPDATE [shop_Database].[dbo].[cartdata] SET [Quantity] = [Quantity] + @Quantity WHERE [PartId] = @PartId";
                            using (SqlCommand updateCommand = new SqlCommand(updateSql, connection))
                            {
                                updateCommand.Parameters.AddWithValue("@Quantity", quantity);
                                updateCommand.Parameters.AddWithValue("@PartId", partId);
                                updateCommand.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // Item does not exist, insert new item
                            // Retrieve item details from the database or other source
                            var itemDetails = GetProductDetails(partId); // Implement this method to fetch item details

                            string insertSql = "INSERT INTO [shop_Database].[dbo].[cartdata] ([PartId], [Name], [Type], [Price], [Quantity]) VALUES (@PartId, @Name, @Type, @Price, @Quantity)";

                            using (SqlCommand insertCommand = new SqlCommand(insertSql, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@PartId", partId);
                                insertCommand.Parameters.AddWithValue("@Name", itemDetails.Name);
                                insertCommand.Parameters.AddWithValue("@Type", itemDetails.Type);
                                insertCommand.Parameters.AddWithValue("@Price", itemDetails.Price);
                                insertCommand.Parameters.AddWithValue("@Quantity", quantity);
                                insertCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                throw;
            }
        }

        public IActionResult OnPostUpdateQuantity(int partId, int quantity)
        {
            try
            {
                string connectionString = "Server=localhost;Database=shop_Database;User Id=SA;Password=KaK123456B;TrustServerCertificate=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE [shop_Database].[dbo].[cartdata] SET [Quantity] = @Quantity WHERE [PartId] = @PartId";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Quantity", quantity);
                        command.Parameters.AddWithValue("@PartId", partId);
                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                throw;
            }
        }

        public IActionResult OnPostRemoveItem(int partId)
        {
            try
            {
                string connectionString = "Server=localhost;Database=shop_Database;User Id=SA;Password=KaK123456B;TrustServerCertificate=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "DELETE FROM [shop_Database].[dbo].[cartdata] WHERE [PartId] = @PartId";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@PartId", partId);
                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                throw;
            }
        }

        private CartItem GetProductDetails(int partId)
        {
            // Implement this method to retrieve product details based on partId
            // This is a placeholder implementation
            // Replace with your actual logic to fetch product details
            return new CartItem
            {
                PartId = partId,
                Name = "Sample Product",
                Type = "Sample Type",
                Price = 100.00m
            };
        }
    }

    public class CartItem
    {
        public int PartId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
