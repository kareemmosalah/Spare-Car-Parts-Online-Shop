using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

using Microsoft.Data.SqlClient;

namespace Front.Pages
{
    public class IndexModel : PageModel
{
    public List<BranchInfo> ListBranches = new List<BranchInfo>();

    public void OnGet()
    {
        try
        {   // note to my team: this is the connection string to the database put yours here
            string connectionString = "Server=localhost;Database=shop_Database;User Id=SA;Password=KaK123456B;TrustServerCertificate=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM Dbo.Branch"; // note to my team: select the table you want to display here(Branch)

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            BranchInfo branch = new BranchInfo();
                            branch.Name = Convert.ToString(dataReader["Name"]);
                            branch.Location = Convert.ToString(dataReader["Location"]);
                            ListBranches.Add(branch);
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
public class BranchInfo
{
    public string Name { get; set; }
    public string Location { get; set; }
}
