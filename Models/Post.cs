using AdminBlog.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AdminBlog.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string userId { get; set; }

        public virtual BlogUser user { get; set; }

        [Required]
        public string title { get; set; }

        public string Subtitle { get; set; }

        [Required]
        public string content { get; set; }

        public string imagePath { get; set; }

        public Category category { get; set; }

        [Required]
        public int categoryId { get; set; }

        [Required]
        public DateTime creationTime { get; set; } = DateTime.Now;

        [Required]
        public int isPublished { get; set; }
    }
}
