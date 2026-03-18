using Microsoft.AspNetCore.Mvc;
using DBFirstASPCoreApp.Models;
using System.Security.Cryptography.Xml;

namespace DBFirstASPCoreApp.Controllers
{
    public class NorthwindController : Controller
    {
        public IActionResult SpainCustomers()
        {
            NorthwindContext cnt = new NorthwindContext();
            var spainCustomers = cnt.Customers
    .Where(x => x.Country == "Spain")
    .Select(x => new SpainCustomerViewModel
    {
        Cid = x.CustomerId,
        Cname = x.ContactName,
        Comname = x.CompanyName
    })
    .ToList();


            return View(spainCustomers);
        }

        public IActionResult ProductsinCategory(string CategoryName)
        {
            NorthwindContext cnt = new NorthwindContext();
            var prdtincaty = cnt.Products.
                Where(x => x.Category.CategoryName == CategoryName).
                Select(x => new ProdCat
                {
                    prodname = x.ProductName,
                    catname = x.Category.CategoryName
                });
            return View(prdtincaty);
        }

        public ActionResult OrderRange(string range)
        {
            NorthwindContext cnt = new NorthwindContext();
            var range1 = Convert.ToInt16(range);
            var custOrderCount = cnt.Customers.
                Where(x => x.Orders.Count() > range1).
                Select(x => new Customer
                {
                    CustomerId = x.CustomerId,
                    CompanyName = x.CompanyName,
                    ContactName = x.ContactName,
                    ContactTitle = x.ContactTitle,
                });
            return View(custOrderCount);
        }
        public IActionResult CustomerOrderDetails(string id)
        {
            NorthwindContext cnt = new NorthwindContext();

            var orders = cnt.Orders
                .Where(o => o.CustomerId == id)
                .Select(o => new Order
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    RequiredDate = o.RequiredDate,
                    ShippedDate = o.ShippedDate
                }).ToList();

            ViewBag.CustomerId = id;

            return View(orders);
        }
    }
}
