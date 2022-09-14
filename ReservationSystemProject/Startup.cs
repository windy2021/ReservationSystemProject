using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReservationSystemProject.Data;
using ReservationSystemProject.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemProject
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<PersonService>();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddCors();
            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors(x =>
            {
                x.AllowAnyMethod()
                 .AllowAnyHeader()
                 .SetIsOriginAllowed(o => true)
                 .AllowCredentials();
            });

            //Profiler
            app.UseMiddleware<StackifyMiddleware.RequestTracerMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "area-route",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
            createRoles(serviceProvider);
            createAdmin(serviceProvider);
        }
        private void createRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roleNames = { "Manager", "Staff", "Member" };
            foreach (var roleName in roleNames)
            {
                Task<bool> roleExists = roleManager.RoleExistsAsync(roleName);
                roleExists.Wait();
                if (!roleExists.Result)
                {
                    Task<IdentityResult> result = roleManager.CreateAsync(new IdentityRole(roleName));
                    result.Wait();
                }
            }
        }

        private void createAdmin(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var personService = serviceProvider.GetRequiredService<PersonService>();
            var user = new IdentityUser { Id = "0bd0b4dd-a90b-468d-8a53-5cb14a04dad8", UserName = "admin@email.com", Email = "admin@email.com" };
            Task<IdentityUser> getUser = userManager.FindByIdAsync("0bd0b4dd-a90b-468d-8a53-5cb14a04dad8");
            getUser.Wait();
            if(getUser.Result == null)
            {
                Task<IdentityResult> userResult = userManager.CreateAsync(user, "Passw0rd_");
                userResult.Wait();
                if(userResult.IsCompletedSuccessfully)
                {
                    var roleResult = userManager.AddToRoleAsync(user, "Manager");
                    roleResult.Wait();
                }
            }
            personService.UpsertPersonAsync(new Person { Email = user.Email, Id = 1, FullName = "Admin", MobileNumber = "0404525343", UserId = user.Id  }, false).Wait();
        }
    }
}
