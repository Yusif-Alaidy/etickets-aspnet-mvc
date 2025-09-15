using ETickets.DataAccess;
using ETickets.Models;
using ETickets.Repositories;
using ETickets.Repositories.IRepositories;
using ETickets.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace ETickets
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<CineBookContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("UserDatabase")));
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(option =>
            {
                option.Password.RequiredLength = 8;
                option.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<CineBookContext>().AddDefaultTokenProviders();

            //builder.Services.AddScoped<IRepository<Movie>, Repository<Movie>>();
            //builder.Services.AddScoped<IRepository<Actor>, Repository<Actor>>();
            //builder.Services.AddScoped<IRepository<Cinema>, Repository<Cinema>>();
            //builder.Services.AddScoped<IRepository<Category>, Repository<Category>>();
            //builder.Services.AddScoped(typeof(Repository<>), typeof(Repository<>));

            builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));




            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
