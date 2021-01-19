using System;
using AdminBlog.Areas.Identity.Data;
using AdminBlog.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(AdminBlog.Areas.Identity.IdentityHostingStartup))]
namespace AdminBlog.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<AdminBlogContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("AdminBlogContextConnection")));

                services.AddDefaultIdentity<BlogUser>(options => {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                })
                    .AddEntityFrameworkStores<AdminBlogContext>();
            });
        }
    }
}