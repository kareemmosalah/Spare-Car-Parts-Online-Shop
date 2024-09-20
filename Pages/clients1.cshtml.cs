using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace Front.Pages
{
    public class clients1Model : PageModel
{
        public List<ClientInfo> ListClients = new List<ClientInfo>();

        public void OnGet()
        {
            try

            {   // note to my team: this is the connection string to the database put yours here
                string connectionString = "Server=localhost;Database=shop_Database;User Id=SA;Password=KaK123456B;TrustServerCertificate=True";


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Dbo.SpareParts " + "SELECT * FROM Dbo.cartdata"; // note to my team: select the table you want to display here(spareparts)

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                ClientInfo client = new ClientInfo();
                                client.Name = Convert.ToString(dataReader["Name"]);
                                client.Type = Convert.ToString(dataReader["Type"]);
                                client.Price = Convert.ToInt32(dataReader["Price"]);
                                ListClients.Add(client);
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

    public class ClientInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int Price { get; set; }
        public string ImageUrl { get; set; }
        public string Definition { get; set; }
        public int Quantity { get; set; }
        public int PartId { get; set; }
    }
}
