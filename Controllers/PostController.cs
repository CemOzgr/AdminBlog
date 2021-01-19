using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AdminBlog.Areas.Identity.Data;
using AdminBlog.Data;
using AdminBlog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace AdminBlog.Controllers
{
    [Authorize]
    public class PostController : Controller
    {

        private readonly AdminBlogContext _db;
        private UserManager<BlogUser> _userManager;

        [BindProperty]
        public Post Post { get; set; }

        public PostController(AdminBlogContext db, UserManager<BlogUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            Post = new Post();
            ViewBag.Categories = _db.Categories.Select(c =>
               new SelectListItem
               {
                   Text = c.Name,
                   Value = c.Id.ToString()
               }
            ).ToList();

            if (id == null)
            {
                if (Post.userId == null)
                {
                    BlogUser currentUser = await _userManager.GetUserAsync(HttpContext.User);
                    Post.userId = currentUser.Id;
                    Post.user = currentUser;
                }
                
                
                return View(Post);
            }
            Post = _db.Posts.FirstOrDefault(p => p.Id == id);
            if (Post == null)
            {
                return NotFound();
            }
            return View(Post);
        }

        #region API Calls
        public async Task<IActionResult> GetPosts()
        {
            string userEmail = User.Identity.Name;
            return Json(new { 
                data = await _db.Posts.Where(p=> p.user.Email== userEmail).ToListAsync() 
            });
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Upsert()
        {
            if (ModelState.IsValid)
            {
                //TODO: Add security controls
                if(Request.Form.Files.Count() != 0)
                {
                    var file = Request.Form.Files.First();
                    string savePath = Path.Combine("C:", "Users", "cemoz", "OneDrive",
                        "Masaüstü", "Programlama", "ASP.NET", "VernumBlog", "wwwroot", "img");
                    var fileName = $"{DateTime.Now:MMddHHmmss}.{file.FileName.Split(".").Last()}";
                    var fileUrl = Path.Combine(savePath, fileName);
                    using (var fileStream = new FileStream(fileUrl, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    Post.imagePath = fileName;
                }
                
                if (Post.Id == 0)
                {
                    _db.Posts.Add(Post);
                }
                else
                {
                    _db.Posts.Update(Post);
                }
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(Post);

        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var fetchedPost = await _db.Posts.FirstOrDefaultAsync(p => p.Id == id);
            if (fetchedPost == null)
            {
                return Json(new { succes = false, message = "404 post not found" });
            }

            string folder = Path.Combine("C:", "Users", "cemoz", "OneDrive",
                        "Masaüstü", "Programlama", "ASP.NET", "VernumBlog", "wwwroot", "img");
            string filename = fetchedPost.imagePath;
            string filePath = Path.Combine(folder, filename);
            
            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                else Console.WriteLine("File not found");
            }
            catch (IOException ioExp)
            {
                Console.WriteLine(ioExp.Message);
            }

            _db.Posts.Remove(fetchedPost);
            _db.SaveChanges();

            return Json(new { success = true, message = "Delete successful" });
        }
        #endregion

    }
}
