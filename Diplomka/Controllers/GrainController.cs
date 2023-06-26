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
using Microsoft.AspNetCore.Http;
using System.IO;
using static System.Net.Mime.MediaTypeNames;

namespace Diplomka.Controllers
{
    public class GrainController : Controller
    {
        MyBaseContext db;

        public GrainController(MyBaseContext context)
        {
            db = context;
        }
        public ActionResult Grains()
        {
            IQueryable<Grain> grains = db.Grains;
            return View(grains.ToList());
        }

        public IActionResult Create()//добавление лекартсва
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string Name, int Price, List<IFormFile> Img)
        {


            foreach (var formfile in Img)
            {
                if (formfile.Length > 0)
                {
                    string fp = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ImgGrain", formfile.FileName);
                    using (var stream = System.IO.File.Create(fp))
                    {
                        formfile.CopyTo(stream);
                        db.Grains.Add(new Models.Grain
                        {
                            Name = Name,
                            Img = formfile.FileName,
                            Price = Price
                        });
                    }
                };

            }
            await db.SaveChangesAsync();
            return RedirectToAction("Grains");
        }
       
            public ActionResult BackToHome()
        {
            // Переход на главную страницу приложения
            return RedirectToAction("Grains");
        }

        public async Task<IActionResult> Edit(int? id)//изменение
        {
            if (id != null)
            {
                Grain grain = await db.Grains.FirstOrDefaultAsync(g => g.GrainID == id);
                if (grain != null)
                    return View(grain);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, string Name, int Price, List<IFormFile> Img)
        {
            foreach (var formfile in Img)
            {
                string FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ImgGrain", formfile.FileName);
                using (var stream = System.IO.File.Create(FilePath))
                {
                    formfile.CopyTo(stream);
                    var grain = db.Grains.Find(id);
                    if (grain != null)
                    {
                        grain.Name = Name;
                        grain.Img = formfile.FileName;
                        grain.Price = Price;
                       
                    }
                }
            }
            await db.SaveChangesAsync();
            return RedirectToAction("Grains");
        }

        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int? id)//удаление
        {
            if (id != null)
            {
               Grain grain = await db.Grains.FirstOrDefaultAsync(w => w.GrainID == id);
                if (grain != null)
                    return View(grain);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                Grain grain = await db.Grains.FirstOrDefaultAsync(p => p.GrainID == id);
                if (grain != null)
                {
                    db.Grains.Remove(grain);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Grains");
                }
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Search(string Name)//поиск лекарств
        {


            return View(db.Grains.ToList().Where(g => g.Name.Contains(Name)));
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}