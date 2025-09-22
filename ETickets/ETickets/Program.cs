using ETickets.Utility.DBInitializer;

namespace ETickets
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Add services to the container

            // Enable MVC controllers and views
            builder.Services.AddControllersWithViews();

            // Configure database context with SQL Server
            builder.Services.AddDbContext<CineBookContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("UserDatabase")));

            // Configure Identity for authentication and authorization
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(option =>
            {
                option.Password.RequiredLength = 8;      // Minimum password length
                option.User.RequireUniqueEmail = true;   // Require unique email per user
            })
            .AddEntityFrameworkStores<CineBookContext>()
            .AddDefaultTokenProviders();
            builder.Services.AddScoped<IDBInitializer, DBInitializer>();

            // Register custom services
            builder.Services.AddTransient<IEmailSender, EmailSender>();             // Email sending service
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>)); // Generic repository pattern

            //builder.Services.AddTransient<IEmailSender, EmailSender>();
            //builder.Services.AddScoped<IRepository<Category>, Repository<Category>>();
            //builder.Services.AddScoped<IRepository<Actor>, Repository<Actor>>();
            //builder.Services.AddScoped<IRepository<Movie>, Repository<Movie>>();

            #endregion

            var app = builder.Build();

            #region Configure middleware pipeline

            // Error handling and HSTS for production
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Enforce HTTPS
            app.UseHttpsRedirection();

            // Routing
            app.UseRouting();

            // Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            using (var scope = app.Services.CreateScope())
            {
                var dbInitializer = scope.ServiceProvider.GetRequiredService<IDBInitializer>();
                // use dbInitializer
                dbInitializer.Initialize();
            }

            // Map static files and default route
            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            #endregion

            // Run the application
            app.Run();
        }
    }
}
