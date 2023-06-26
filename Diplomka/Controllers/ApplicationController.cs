using Diplomka.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Diplomka.Controllers
{
    public class ApplicationController : Controller
    {
        MyBaseContext db;
        public ApplicationController(MyBaseContext context)
        {
            db = context;
        }
        [Authorize(Roles = "Администратор, Планировщик")]
        public ActionResult Applications()
        {
            IQueryable<Application> applications = db.Applications.Include(p => p.Factory)
                                                                  .Include(p => p.Warehouse)
                                                                  .Include(p => p.Depot)
                                                                  .Include(p => p.Car)
                                                                  .Include(p => p.Driver);

            return View(applications.ToList());
        }





            public IActionResult CreatePlan()
        {
            
            ViewBag.Factory = new SelectList(db.Factories.ToList(), "FactoryID", "Name");
            ViewBag.Warehouse = new SelectList(db.Warehouses.ToList(), "WarehouseID", "Name");
            ViewBag.Depot = new SelectList(db.Depots.ToList(), "DepotID", "Name");
            ViewBag.Car = new SelectList(db.Cars.ToList(), "CarID", "Name");
            ViewBag.Driver = new SelectList(db.Drivers.ToList(), "DriverID", "FIO");


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlan(Application application)
        {
           

            db.Applications.Add(application);
            await db.SaveChangesAsync();
            return RedirectToAction("Applications");


            
        }
        public ActionResult BackToHome()
        {
            // Переход на главную страницу приложения
            return RedirectToAction("Applications");
        }
        public async Task<IActionResult> Edit(int? id)
        {
            
            ViewBag.Factory = new SelectList(db.Factories.ToList(), "FactoryID", "Name");
            ViewBag.Warehouse = new SelectList(db.Warehouses.ToList(), "WarehouseID", "Name");
            ViewBag.Depot = new SelectList(db.Depots.ToList(), "DepotID", "Name");
            ViewBag.Car = new SelectList(db.Cars.ToList(), "CarID", "Name");
            ViewBag.Driver = new SelectList(db.Drivers.ToList(), "DriverID", "FIO");

            if (id != null)
            {
                Application applications = await db.Applications.FirstOrDefaultAsync(a => a.ApplicationID == id);
                if (applications != null)
                    return View(applications);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Application application)
        {
            db.Applications.Update(application);
            await db.SaveChangesAsync();
            return RedirectToAction("Applications");
        }

        //=======================================================

        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null)
            {
                Application application = await db.Applications.FirstOrDefaultAsync(p => p.ApplicationID == id);
                if (application != null)
                    return View(application);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                Application application = await db.Applications.FirstOrDefaultAsync(p => p.ApplicationID == id);
                if (application != null)
                {
                    db.Applications.Remove(application);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Applications");
                }
            }
            return NotFound();
        }
        //public async Task<IActionResult> Print(int? id)
        //{

        //    ViewBag.Factory = new SelectList(db.Factories.ToList(), "FactoryID", "Name");
        //    ViewBag.Warehouse = new SelectList(db.Warehouses.ToList(), "WarehouseID", "Name");
        //    ViewBag.Depot = new SelectList(db.Depots.ToList(), "DepotID", "Name");
        //    ViewBag.Car = new SelectList(db.Cars.ToList(), "CarID", "Name");
        //    ViewBag.Driver = new SelectList(db.Drivers.ToList(), "DriverID", "FIO");


        //    if (id != null)
        //    {
        //        Application applications = await db.Applications.FirstOrDefaultAsync(a => a.ApplicationID == id);
        //        if (applications != null)
        //            return View(applications);
        //    }
        //    //return View();
        //    return NotFound();

        //}
        public async Task<IActionResult> Print2(int? id)
        {
            ViewBag.Factory = new SelectList(db.Factories.ToList(), "FactoryID", "Name");
            ViewBag.Grain = new SelectList(db.Grains.ToList(), "GrainID", "Name");

            if (id != null)
            {
                Order orders = await db.Orders.FirstOrDefaultAsync(p => p.OrderID == id);
                if (orders != null)
                    return View(orders);
            }
           
            return NotFound();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
