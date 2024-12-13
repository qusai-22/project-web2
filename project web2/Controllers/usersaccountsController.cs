using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using project_web2.Data;
using project_web2.Models;

namespace project_web2.Controllers
{
    public class usersaccountsController : Controller
    {
        private readonly project_web2Context _context;

        public usersaccountsController(project_web2Context context)
        {
            _context = context;
        }
        public IActionResult Users_Search()
        {
            string role = HttpContext.Session.GetString("Role");

            if (role != "admin")
            {
                return RedirectToAction("login", "usersaccounts");
            }
            usersaccounts users = new usersaccounts();
            return View(users);
        }

            [HttpPost]

        public async Task<IActionResult> Users_Search(string name)
        {
            string role = HttpContext.Session.GetString("Role");

            if (role != "admin")
            {
                return RedirectToAction("login", "usersaccounts");
            }
            var uuss = await _context.usersaccounts.FromSqlRaw("SELECT * FROM usersaccounts where name ='" + name + "' ").FirstOrDefaultAsync();

            return View(uuss);
        }
        public IActionResult AddAdmin()
        {
            string role = HttpContext.Session.GetString("Role");

            if (role != "admin")
            {
                return RedirectToAction("login", "useraccounts");
            }
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAdmin([Bind("name,pass")] usersaccounts acc)
        {
            string role = HttpContext.Session.GetString("Role");


            MD5 md5 = new MD5CryptoServiceProvider();
            string paa = Encoding.ASCII.GetString(md5.ComputeHash(ASCIIEncoding.Default.GetBytes(acc.pass)));


            if (role != "admin")
            {
                return RedirectToAction("login", "usersaccounts");
            }

            var existingUser = await _context.usersaccounts.FirstOrDefaultAsync(u => u.name == acc.name);
            if (existingUser != null)
            {
                ViewData["Message"] = "Username already exists.";
                return View();
            }

            acc.pass = paa;
            acc.role = "admin";
            _context.Add(acc);
            await _context.SaveChangesAsync();

            ViewData["Message"] = "Admin added successfully!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> email(int? id)
        {


            return View();
        }


        [HttpPost, ActionName("email")]
        [ValidateAntiForgeryToken]
        public IActionResult email(string address, string subject, string body)
        {
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            var mail = new MailMessage();
            mail.From = new MailAddress("theking99968@gmail.com");
            mail.To.Add(address);
            mail.Subject = subject;
            mail.IsBodyHtml = true;
            mail.Body = body;
            SmtpServer.Port = 587;
            SmtpServer.UseDefaultCredentials = false;
            SmtpServer.Credentials = new System.Net.NetworkCredential("theking99968@gmail.com", "xifbhisxejlancpf");
            SmtpServer.EnableSsl = true;
            SmtpServer.Send(mail);
            ViewData["Message"] = "Email sent.";
            return View();
        }
        public ActionResult Home()
        {
            HttpContext.Session.LoadAsync();
            String ss = HttpContext.Session.GetString("Role");
            if (ss == "admin")
            {
                ViewData["name"] = HttpContext.Session.GetString("Name");
                return View();
            }
            else
                return RedirectToAction("login", "usersaccounts");
        }
        

        public IActionResult Login()
        {
            if (!HttpContext.Request.Cookies.ContainsKey("Name"))
                return View();
            else
            {
                string na = HttpContext.Request.Cookies["Name"].ToString();
                string ro = HttpContext.Request.Cookies["Role"].ToString();
                HttpContext.Session.SetString("Name", na);
                HttpContext.Session.SetString("Role", ro);
                if (ro == "customer")
                    return RedirectToAction(nameof(customerhome));
                else
                    return RedirectToAction("adminhome", "usersaccounts");
            }
        }
        [HttpPost, ActionName("login")]
        public async Task<IActionResult> login(string na, string pa, string auto)
        {
            var ur = await _context.usersaccounts.FromSqlRaw("SELECT * FROM usersaccounts where name ='" + na + "' and  pass ='" + pa + "' ").FirstOrDefaultAsync();

            if (ur != null)
            {

                int id = ur.Id;
                string na1 = ur.name;
                string ro = ur.role;
                HttpContext.Session.SetString("userid", Convert.ToString(id));
                HttpContext.Session.SetString("Name", na1);
                HttpContext.Session.SetString("Role", ro);
                if (auto == "on")
                {
                    HttpContext.Response.Cookies.Append("Name", na1);
                    HttpContext.Response.Cookies.Append("Role", ro);
                }
                if (ro == "customer")
                    return RedirectToAction("customerhome", "usersaccounts");
                else if (ro == "admin")
                    return RedirectToAction("adminhome", "usersaccounts");
                else
                    return View();
            }
            else
            {
                ViewData["Message"] = "wrong user name password";
                return View();
            }
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Name");
            HttpContext.Session.Remove("Role");

            HttpContext.Response.Cookies.Delete("Name");
            HttpContext.Response.Cookies.Delete("Role");

            return RedirectToAction(nameof(login));
        }
        public IActionResult registration()
        {
            return View();
        }


        public async Task<IActionResult> adminhome()
        {
           HttpContext.Session.LoadAsync();
            String ss =HttpContext.Session.GetString("Role");
            if (ss=="admin")
            {
                ViewData["name"]=HttpContext.Session.GetString("Name");
                return View();
            }
            else
                return RedirectToAction("login","usersaccounts");
        }
        public async Task<IActionResult> customerhome()
        {
            HttpContext.Session.LoadAsync();
            String ss = HttpContext.Session.GetString("Role");
            if (ss == "customer")
            {
                ViewData["name"] = HttpContext.Session.GetString("Name");
                var discount = await _context.items.Where(b => b.discount == "yes").ToListAsync();
                return View(discount);
            }
            else
                return RedirectToAction("login", "usersaccounts");
        }

        [HttpPost]
        public IActionResult registration([Bind("name,email,job,gender,married, location")] customer cust, [Bind("name,pass,role")] usersaccounts acc)
        {
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("project_web2Context");
            SqlConnection conn = new SqlConnection(conStr);
            MD5 sec = new MD5CryptoServiceProvider();
            string paa = Encoding.ASCII.GetString(sec.ComputeHash(ASCIIEncoding.Default.GetBytes(acc.pass)));

            conn.Open();
            string sql = "select * from usersaccounts  where name = '" + acc.name + "' and pass = '" + paa + "'";
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader reader = comm.ExecuteReader();
            if (reader.Read())
            {
                ViewData["message"] = "name and password already exists";
                reader.Close();
            }
            else
            {
                reader.Close();
                sql = "insert into customer (name,email,job,married,gender,location)  values  ('" + cust.name + "','" + cust.email + "','" + cust.job + "','" + cust.married + "' ,'" + cust.gender + "','" + cust.location + "')";
                comm = new SqlCommand(sql, conn);
                comm.ExecuteNonQuery();

                acc.role = "customer";
                sql = "insert into usersaccounts (name,pass,role)  values  ('" + acc.name + "','" + paa + "','" + acc.role + "')";
                comm = new SqlCommand(sql, conn);
                comm.ExecuteNonQuery();

                ViewData["message"] = "Sucessfully added";
                return RedirectToAction("login", "usersaccounts");
            }
            conn.Close();
            return View();
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.usersaccounts.ToListAsync());
        }

        // GET: usersaccounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.usersaccounts == null)
            {
                return NotFound();
            }

            var users = await _context.usersaccounts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (users == null)
            {
                return NotFound();
            }

            return View(users);
        }

        // GET: usersaccounts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: usersaccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,name,pass,role")] usersaccounts usersaccounts)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usersaccounts);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(usersaccounts);
        }

        // GET: usersaccounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usersaccounts = await _context.usersaccounts.FindAsync(id);
            if (usersaccounts == null)
            {
                return NotFound();
            }
            return View(usersaccounts);
        }

        // POST: usersaccounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,name,pass,role")] usersaccounts usersaccounts)
        {
            if (id != usersaccounts.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usersaccounts);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!usersaccountsExists(usersaccounts.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(usersaccounts);
        }

        // GET: usersaccounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usersaccounts = await _context.usersaccounts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usersaccounts == null)
            {
                return NotFound();
            }

            return View(usersaccounts);
        }

        // POST: usersaccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usersaccounts = await _context.usersaccounts.FindAsync(id);
            if (usersaccounts != null)
            {
                _context.usersaccounts.Remove(usersaccounts);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool usersaccountsExists(int id)
        {
            return _context.usersaccounts.Any(e => e.Id == id);
        }
    }
}
