using Microsoft.EntityFrameworkCore;
using Csharp_periode_4_Groep_24_2025.Data;

namespace Csharp_periode_4_Groep_24_2025

    //S1162353
    //Delano Delgado
    //https://github.com/JunctionParadox/Csharp_periode_4_Groep_24_2025
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Connect to database
            builder.Services.AddDbContext<DbContext24>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("MachineDefault"));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
