using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AdminBlog.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public DateTime creationTime { get; set; } = DateTime.Now;

        public string imagePath { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
}
