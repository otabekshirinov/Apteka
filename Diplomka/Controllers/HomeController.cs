using Diplomka.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Diplomka.Controllers
{
    public class HomeController : Controller
    {
        MyBaseContext db;

        public HomeController(MyBaseContext context)
        {
            db = context;
        }

        public ActionResult Index()
        {           
            return View();
        }

        public ActionResult Users()
        {
            IQueryable<User> users = db.Users.Include(u => u.Role);
            return View(users.ToList());
        }

        [Authorize(Roles = "Администратор")]
        public IActionResult Create()
        {
            ViewBag.Role = new SelectList(db.Roles.ToList(), "RoleId", "Name");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Create(User user)
        {      
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return RedirectToAction("Users");
        }

        //=======================================================
        [Authorize(Roles = "Администратор")]
        public ActionResult BackToHome()
        {
            // Переход на главную страницу приложения
            return RedirectToAction("Users");
        }

        //=======================================================

        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.Role = new SelectList(db.Roles.ToList(), "RoleId", "Name");

            if (id != null)
            {
                User user = await db.Users.FirstOrDefaultAsync(c => c.Id == id);
                if (user != null)
                    return View(user);
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Edit(User user)
        {
            db.Users.Update(user);
            await db.SaveChangesAsync();
            return RedirectToAction("Users");
        }

        //=======================================================

        [HttpGet]
        [Authorize(Roles = "Администратор")]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null)
            {
                User user = await db.Users.FirstOrDefaultAsync(d => d.Id == id);
                if (user != null)
                    return View(user);
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                User user = await db.Users.FirstOrDefaultAsync(d => d.Id == id);
                if (user != null)
                {
                    db.Users.Remove(user);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Users");
                }
            }
            return NotFound();
        }
        //=======================================================

      



        public IActionResult Privacy()
        {
            return View();
        }

      
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}