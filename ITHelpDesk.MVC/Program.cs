using FluentValidation;
using ITHelpDesk.Application;
using ITHelpDesk.Infrastructure.Data;
using ITHelpDesk.Infrastructure.Identity;
using Karakeeb.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ITHelpDesk.MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //Add Services of MediatR and Validation Behavior and Pipelines
            builder.Services.AddMediatR(options =>
            {
                options.RegisterServicesFromAssembly(typeof(IAssemblyMarker).Assembly);
            });

            builder.Services.AddValidatorsFromAssembly(typeof(IAssemblyMarker).Assembly);

            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            //Add Services of Infrastructure Layer
            builder.Services.AddInfrastructure(builder.Configuration);

            //Add Services of Application Layer
            builder.Services.AddApplication();

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy =>
                policy.RequireRole("Admin"));

                options.AddPolicy("User", policy =>
                policy.RequireRole("User"));

                options.AddPolicy("UserOrAdmin", policy =>
                policy.RequireRole("Admin", "User"));

                options.AddPolicy("UserAndAdmin", policy =>
                policy.RequireRole("Admin").RequireRole("User"));
            });

            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                // apply any pending migrations to the database
                db.Database.Migrate();

                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                AppDataSeeder.SeedAsync(dbContext, userManager, roleManager).GetAwaiter().GetResult();
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapStaticAssets();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
