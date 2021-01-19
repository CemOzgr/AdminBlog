using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using AdminBlog.Models;
using Microsoft.AspNetCore.Identity;

namespace AdminBlog.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the BlogUser class
    public class BlogUser : IdentityUser
    {
        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string FirstName { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string LastName { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
}
