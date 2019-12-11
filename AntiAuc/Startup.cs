using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using AntiAuc.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using AntiAuc.Hubs;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AntiAuc
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            string connection = "Server=(localdb)\\mssqllocaldb;Database=aucdb;Trusted_Connection=True;MultipleActiveResultSets=True;";
            services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
                    options.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
                });

            services.AddSignalR();
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddMvc()
                .AddDataAnnotationsLocalization()
                .AddViewLocalization();
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en"),
                    new CultureInfo("be"),
                    new CultureInfo("ru")
                };

                options.DefaultRequestCulture = new RequestCulture("ru");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);



            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseSignalR(routes =>
            {
                routes.MapHub<ChatHub>("/chatHub");
            });
            app.UseMvc(routes =>
            {         
                routes.MapRoute("default", "{controller}/{action}/{sort:alpha?}", new { controller = "Home", action = "Index" });
                routes.MapRoute("setlan", "{controller}/{action}/{culture:alpha?}", new { Controller = "Home", action = "SetLanguage" });
                routes.MapRoute("about", "{controller}/{action}", new { controller = "Home", action = "About" });
                routes.MapRoute("addproduct", "{controller}/{action}", new { controller = "Product", action = "AddProduct" });
                routes.MapRoute("editproduct", "{controller}/{action}/{id?}", new { Controller = "Product", action = "Edit" });
                routes.MapRoute("productinfo", "{controller}/{action}/{id?}", new { Controller = "Product", action = "Info" });
                routes.MapRoute("accountedit", "{controller}/{action}/{id?}", new { Controller = "Account", action = "Edit" });
                routes.MapRoute("accountlogin", "{controller}/{action}", new { Controller = "Account", action = "Login" });
                routes.MapRoute("accountregister", "{controller}/{action}", new { Controller = "Account", action = "Register" });
                routes.MapRoute("accountdelete", "{controller}/{action}/{id?}", new { Controller = "Account", action = "Delete" });
                routes.MapRoute("accountprofile", "{controller}/{action}/{id?}", new { Controller = "Account", action = "Profile" });
                
            });
        }
    }
}
