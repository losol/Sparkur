using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sparkur.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spark.Engine;
using Spark.Mongo;
using Spark.Engine.Extensions;
using Sparkur.Hubs;
using Sparkur.Config;
using Sparkur.Models;

namespace Sparkur
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
            SparkSettings sparkSettings = new SparkSettings();
            Configuration.Bind("SparkSettings", sparkSettings);
            
            // TODO: Update mong=>mongo after next alpha
            MongoStoreSettings storeSettings = new MongoStoreSettings();
            Configuration.Bind("MongoStoreSettings", storeSettings);

            ExamplesSettings examplesSettings = new ExamplesSettings();
            Configuration.Bind("ExamplesSettings", examplesSettings);

            services.Configure<ExamplesSettings>(options => Configuration.GetSection("ExamplesSettings").Bind(options));
            
            services.AddMongoFhirStore(storeSettings);
            services.AddFhir(sparkSettings);


            services.AddSingleton<SparkSettings>(sparkSettings);
            services.AddSingleton<ExamplesSettings>(examplesSettings);

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options => 
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection"))
            );

            // services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(
            //        Configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdministratorRole",
                    policy => policy.RequireRole("Admin", "SuperAdmin"));
            });

            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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
                // app.UseHsts();
            }

            // app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseSignalR(routes =>
            {
                routes.MapHub<MaintenanceHub>("/maintenanceHub");
            });

            app.UseFhir(routes =>
            {
                routes.MapRoute(
                    name: "Areas",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseMvc();
        }
    }
}
