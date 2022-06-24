using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using StartUpProject.DAL;
using StartUpProject.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StartUpProject.Areas.Manage.Controllers
{
    [Area("Manage"),Authorize]
    public class TestimonialController : Controller
    {
        readonly Context _context;
        readonly IWebHostEnvironment _env;

        public TestimonialController(Context context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            return View(_context.Testimonials.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Edit(int id)
        {
            return View(_context.Testimonials.FirstOrDefault(x => x.Id == id));
        }

        [HttpPost, AutoValidateAntiforgeryToken]
        public IActionResult Create(Testimonial testimonial)
        {
            if (!ModelState.IsValid) return View(testimonial);
            if (testimonial.ImageFile != null)
            {
                if (testimonial.ImageFile.ContentType != "image/jpeg" && testimonial.ImageFile.ContentType != "image/png" && testimonial.ImageFile.ContentType != "image/webp")
                {
                    ModelState.AddModelError(testimonial.ImageFile.FileName, "This is not image format");
                    return View(testimonial);
                }
                if (testimonial.ImageFile.Length / 1024 > 2000)
                {
                    ModelState.AddModelError("ImageFile", "Image size must be lower than 2mb");
                    return View(testimonial);
                }
                string fileName = testimonial.ImageFile.FileName;
                if (testimonial.ImageFile.FileName.Length > 64)
                {
                    fileName.Substring(fileName.Length - 64, 64);
                }
                string newFileName = Guid.NewGuid().ToString() + fileName;
                string path = Path.Combine(_env.WebRootPath, "assets", "images", newFileName);
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    testimonial.ImageFile.CopyToAsync(fs);
                }
                testimonial.Image = newFileName;
                _context.Testimonials.Add(testimonial);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            ModelState.AddModelError(testimonial.ImageFile.FileName, "Image required");
            return View(testimonial);
        }
       

        [HttpPost]
        public IActionResult Edit(Testimonial customer)
        {
            var existTestimonial = _context.Testimonials.FirstOrDefault(x => x.Id == customer.Id);
            if (existTestimonial == null) return NotFound();
            string newFileName = null;

            if (customer.ImageFile != null)
            {
                if (customer.ImageFile.ContentType != "image/jpeg" && customer.ImageFile.ContentType != "image/png" && customer.ImageFile.ContentType != "image/webp")
                {
                    ModelState.AddModelError("ImageFile", "Image can be only .jpeg or .png");
                    return View();
                }
                if (customer.ImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("ImageFile", "Image size must be lower than 2mb");
                    return View();
                }
                string fileName = customer.ImageFile.FileName;
                if (fileName.Length > 64)
                {
                    fileName = fileName.Substring(fileName.Length - 64, 64);
                }
                newFileName = Guid.NewGuid().ToString() + fileName;

                string path = Path.Combine(_env.WebRootPath, "assets", "images", newFileName);
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    customer.ImageFile.CopyTo(stream);
                }
            }
            if (newFileName != null)
            {
                string deletePath = Path.Combine(_env.WebRootPath, "assets", "images", existTestimonial.Image);

                if (System.IO.File.Exists(deletePath))
                {
                    System.IO.File.Delete(deletePath);
                }

                existTestimonial.Image = newFileName;
            }

            existTestimonial.UserName = customer.UserName;
            existTestimonial.Description = customer.Description;
            existTestimonial.CompanyName = customer.CompanyName;
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            var testimonial = _context.Testimonials.FirstOrDefault(x => x.Id == id);
            _context.Testimonials.Remove(testimonial);
            _context.SaveChanges();
            string deletePath = Path.Combine(_env.WebRootPath, "assets", "images", testimonial.Image);

            if (System.IO.File.Exists(deletePath))
            {
                System.IO.File.Delete(deletePath);
            }
            return RedirectToAction("index");
        }
    }
}
