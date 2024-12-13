using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using project_web2.Models;

namespace project_web2.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class getnameController : ControllerBase
    {
        [HttpGet("{cat}")]
        public IEnumerable<allusers> Get(string cat)
        {
            List<allusers> li = new List<allusers>();
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("project_web2Context");
            SqlConnection conn1 = new SqlConnection(conStr);
            string sql;
            sql = "SELECT * FROM usersaccounts where role ='" + cat + "' ";
            SqlCommand comm = new SqlCommand(sql, conn1);
            conn1.Open();
            SqlDataReader reader = comm.ExecuteReader();

            while (reader.Read())
            {
                li.Add(new allusers
                {
                    name = (string)reader["name"],
                });

            }

            reader.Close();
            conn1.Close();
            return li;
        }
    }
}