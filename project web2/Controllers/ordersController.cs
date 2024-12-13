using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using project_web2.Data;
using project_web2.Models;

namespace project_web2.Controllers
{
    public class ordersController : Controller
    {
        private readonly project_web2Context _context;
        List<buyitems> Bbks = new List<buyitems>();

        public ordersController(project_web2Context context)
        {
            _context = context;
        }

        // GET: orders
        public async Task<IActionResult> Index()
        {
            return View(await _context.orders.ToListAsync());
        }
        public async Task<IActionResult> report()
        {
 var orItems=await _context.report.FromSqlRaw("SELECT custname,SUM(total) as total FROM orders GROUP BY custname").ToListAsync();
            return View(orItems);
        }
        public async Task<IActionResult> ordersdetail(String? custname)
        {
var orItems=await _context.orders.FromSqlRaw("SELECT * from orders where custname ='"+custname+"'  ").ToListAsync();   
            return View(orItems);   
        }
        public async Task<IActionResult> orderline(int? orid)
        {
            var buyit = await _context.orderline.FromSqlRaw("SELECT * from orderline where orderid ='" + orid + "'  ").ToListAsync();
            return View(buyit);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orders = await _context.orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }

        // GET: orders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,custname,orderdate,total")] orders orders)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orders);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(orders);
        }

        // GET: orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orders = await _context.orders.FindAsync(id);
            if (orders == null)
            {
                return NotFound();
            }
            return View(orders);
        }
        public async Task<IActionResult> CatalogueBuy()
        {
            HttpContext.Session.LoadAsync();
            String ss = HttpContext.Session.GetString("Role");
            if (ss != "customer")
            {
                return RedirectToAction("login", "usersaccounts");

            }
            return View(await _context.items.ToListAsync());



        }
        public async Task<IActionResult> ItemBuyDetail(int? id)
        {
            HttpContext.Session.LoadAsync();
            String ss = HttpContext.Session.GetString("Role");
            if (ss != "customer")
            {
                return RedirectToAction("login", "usersaccounts");

            }
            var items = await _context.items.FindAsync(id);
            return View(items);
        }

        [HttpPost]

        public async Task<IActionResult> cartadd(int Id, int quantity)
        {
            await HttpContext.Session.LoadAsync();

            var sessionString = HttpContext.Session.GetString("Cart");
            if (sessionString is not null) {

                Bbks = JsonSerializer.Deserialize<List<buyitems>>(sessionString);
                }

            var item = await _context.items.FromSqlRaw("SELECT * FROM items WHERE Id = Id", Id).FirstOrDefaultAsync();

            // Check if the item exists
            if (item == null)
            {
                ViewData["Error"] = "Item not found.";
                return View("ItemBuyDetail", item);
            }

            // Check if the requested quantity is available
            if (quantity > item.quantity)
            {
                ViewData["Error"] = "Requested quantity exceeds available stock.";
                return View("ItemBuyDetail", item);
            }

            // Add the item to the cart
            Bbks.Add(new buyitems
            {
                name = item.name,
                price = item.price,
                quant = quantity
            });

            // Save the updated cart back to the session
            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(Bbks));

            return RedirectToAction("CartBuy");
        }


      
        public async Task<IActionResult> CartBuy()
        {

            await HttpContext.Session.LoadAsync();
            var sessionString = HttpContext.Session.GetString("Cart");
            if (sessionString is not null)
            {
                Bbks = JsonSerializer.Deserialize<List<buyitems>>(sessionString);
            }
            return View(Bbks);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,custname,orderdate,total")] orders orders)
        {
            if (id != orders.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orders);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ordersExists(orders.Id))
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
            return View(orders);
        }
        public async Task<IActionResult> Buy()
        {
            await HttpContext.Session.LoadAsync();
            var sessionString = HttpContext.Session.GetString("Cart");
            if (sessionString is not null)
            {
                Bbks = JsonSerializer.Deserialize<List<buyitems>>(sessionString);
            }

            string ctname = HttpContext.Session.GetString("Name");
            orders bkorder = new orders();
            bkorder.total = 0;
            bkorder.custname = ctname;
            bkorder.orderdate = DateTime.Today;
            _context.orders.Add(bkorder);
            await _context.SaveChangesAsync();
            var bord = await _context.orders.FromSqlRaw("select * from orders  where custname= '" + ctname + "' ").OrderByDescending(e => e.Id).FirstOrDefaultAsync();
            int ordid = bord.Id;
            decimal tot = 0;
            foreach (var bk in Bbks.ToList())
            {
                orderline oline = new orderline();
                oline.orderid = ordid;
                oline.itemname = bk.name;
                oline.itemquant = bk.quant;
                oline.itemprice = bk.price;
                _context.orderline.Add(oline);
                await _context.SaveChangesAsync();

                var bkk = await _context.items.FromSqlRaw("select * from items  where name= '" + bk.name + "' ").FirstOrDefaultAsync();
                bkk.quantity = bkk.quantity - bk.quant;
                _context.Update(bkk);
                await _context.SaveChangesAsync();

                tot = tot + (bk.quant * bk.price);
            }
            bord.total = Convert.ToInt16(tot);
            _context.Update(bord);
            await _context.SaveChangesAsync();

            ViewData["Message"] = "Thank you See you again";
            Bbks = new List<buyitems>();
            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(Bbks));
            return RedirectToAction("MyOrder");
        }
        public async Task<IActionResult> MyOrder()
        {
            string ctname = HttpContext.Session.GetString("Name");
            return View(await _context.orders.FromSqlRaw("select * from orders  where custname = '" + ctname + "' ").ToListAsync());
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orders = await _context.orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }

        // POST: orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orders = await _context.orders.FindAsync(id);
            if (orders != null)
            {
                _context.orders.Remove(orders);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ordersExists(int id)
        {
            return _context.orders.Any(e => e.Id == id);
        }
    }
}
