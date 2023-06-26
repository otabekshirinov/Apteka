﻿using Diplomka.Models;
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
    public class OrderController : Controller
    {
        MyBaseContext db;

        public OrderController(MyBaseContext context)
        {
            db = context;
        }

        [Authorize(Roles = "Администратор, Планировщик, Заказчик")]
        public ActionResult Orders(string status)
        {
            IQueryable<Order> orders = db.Orders.Include(o => o.Factory)
                                                .Include(o => o.Grain);
            return View(orders.ToList());
        }

        //=======================================================
        [Authorize(Roles = "Заказчик")]

        public IActionResult Create()
        {
            ViewBag.Factory = new SelectList(db.Factories.ToList(), "FactoryID", "Name");
            ViewBag.Grain = new SelectList(db.Grains.ToList(), "GrainID", "Name");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Заказчик")]
        public async Task<IActionResult> Create(Order order)
        {
            Grain grain = await db.Grains.FirstOrDefaultAsync(g => g.GrainID == order.GrainID);
          
            order.Price = order.Volume * grain.Price;

            var cargoRemnant = db.CargoRemnants.Where(c => c.GrainID == order.GrainID).FirstOrDefault();

            cargoRemnant.Volume -= order.Volume;
            
            db.Orders.Add(order);
            await db.SaveChangesAsync();
            return RedirectToAction("Orders");
        }


        //=======================================================
        public ActionResult BackToHome()
        {
            // Переход на главную страницу приложения
            return RedirectToAction("Orders");
        }

        //=======================================================

        [Authorize(Roles = "Заказчик")]
        public async Task<IActionResult> Edit(int? id)
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

        [HttpPost]
        [Authorize(Roles = "Заказчик")]
        public async Task<IActionResult> Edit(Order orders)
        {
            Grain grain = await db.Grains.FirstOrDefaultAsync(g => g.GrainID == orders.GrainID);
            
            orders.Price = orders.Volume * grain.Price;
            db.Orders.Update(orders);
            await db.SaveChangesAsync();
            return RedirectToAction("Orders");
        }

        //=======================================================

        [HttpGet]
        [ActionName("Delete")]
        [Authorize(Roles = "Заказчик")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null)
            {
                Order order = await db.Orders.FirstOrDefaultAsync(p => p.OrderID == id);
                if (order != null)
                    return View(order);
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Заказчик")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                Order order = await db.Orders.FirstOrDefaultAsync(p => p.OrderID == id);
                if (order != null)
                {
                    db.Orders.Remove(order);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Orders");
                }
            }
            return NotFound();
        }

        

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult AdminStatus( int OrderID) 
        { 
            
            var order = db.Orders.FirstOrDefault(p => p.OrderID == OrderID);
            if (order != null)
            {
                order.status = true;
            }
            db.SaveChanges();

            return RedirectToAction("Orders", "Order");
        }
    }
}