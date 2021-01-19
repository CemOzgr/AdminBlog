using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AdminBlog.Data;
using AdminBlog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminBlog.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly AdminBlogContext _db;

        [BindProperty]
        public Category Category { get; set; }

        public CategoryController(AdminBlogContext db)
        {
            _db = db;
        }


        public IActionResult Index()
        {
            ViewBag.Categories = _db.Categories.ToList();
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            Category = new Category();
            if (id == null)
            {
                return View(Category);
            }
            Category = _db.Categories.FirstOrDefault(c => c.Id == id);
            if (Category == null)
            {
                return NotFound();
            }
            return View(Category);
        }

        #region API Calls

        [HttpGet]
        public IActionResult GetCategories()
        {
            var fetched = _db.Categories.ToList();
            return Json(new { data = fetched });
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Upsert()
        {
            if (ModelState.IsValid)
            {
                //Save file to vernumBlog application if the file is given
                if (Request.Form.Files.Count() != 0)
                {
                    
                    var file = Request.Form.Files.First();

                    //savePath has to be changed to vernumBlog solution folder in the computer
                    string savePath = Path.Combine("C:", "Users", "cemoz", "OneDrive",
                        "Masaüstü", "Programlama", "ASP.NET", "VernumBlog", "wwwroot", "img");

                    //In order to avoid overriding files with same names. We set file names to creation date
                    var fileName = $"{DateTime.Now:MMddHHmmss}.{file.FileName.Split(".").Last()}";
                    var fileUrl = Path.Combine(savePath, fileName);
                    using (var fileStream = new FileStream(fileUrl, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    Category.imagePath = fileName;
                }

                if(Category.Posts == null)
                {
                    Category.Posts = await _db.Posts.Where(p => p.categoryId == Category.Id).ToListAsync();
                }

                //If its a new category create new category
                if (Category.Id == 0)
                {
                    await _db.Categories.AddAsync(Category);
                }
                //Otherwise update it
                else
                {
                    _db.Categories.Update(Category);
                }

                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(Category);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            // When deleting a category erase its image file. 
            // We delete all of its posts as well as their images files.

            var fetched = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (fetched == null)
            {
                return Json(new { success = false, message = "404 category not found" });
            }

            //savePath has to be changed to vernumBlog solution folder in the computer
            string folder = Path.Combine("C:", "Users", "cemoz", "OneDrive",
                        "Masaüstü", "Programlama", "ASP.NET", "VernumBlog", "wwwroot", "img");

            List<Post> categoryPosts = await _db.Posts.Where(p => p.categoryId == id).ToListAsync();

            //Get image path of posts in current category 
            List<string> postfilePaths = categoryPosts.Select(p => p.imagePath).ToList();

            //Added imagePath of the current category for convenience
            postfilePaths.Add(fetched.imagePath);

            try
            {   
                foreach(string imagePath in postfilePaths)
                {
                    if (System.IO.File.Exists(Path.Combine(folder, imagePath)))
                    {
                        System.IO.File.Delete(Path.Combine(folder, imagePath));
                    }
                    else
                    {
                        Console.WriteLine("File not found");
                    }
                }
                
            }
            catch (IOException ioExp)
            {
                Console.WriteLine(ioExp.Message);
            }

            _db.Categories.Remove(fetched);
            _db.SaveChanges();

            return Json(new { success = true, message = "Delete successful" });
        }

        #endregion
    }
}
