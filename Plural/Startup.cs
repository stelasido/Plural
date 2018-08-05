using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Plural.Data;
using Plural.Data.Entities;
using Plural.Services;

namespace Plural
{
    public class Startup
    {
        private readonly IConfiguration _config;
        private readonly IHostingEnvironment _env;
        public Startup(IConfiguration config, IHostingEnvironment env)
        {
            _env = env;
            _config = config;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentity<StoreUser, IdentityRole>(cfg => {
                cfg.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<PluralContext>();

            services.AddAuthentication()
                .AddCookie()
                .AddJwtBearer(cfg => {
                    cfg.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = _config["Tokens:Issuer"],
                        ValidAudience = _config["Tokens:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]))
                    };
                });
            // WE ADD TWO KINDS OF AUTH.

            services.AddDbContext<PluralContext>(cfg => {
                cfg.UseSqlServer(_config.GetConnectionString("PluralConnectionString"));
            });
            services.AddAutoMapper();
            // THIS SERVICE IS FOR FAKE LOGGING INSTEAD SENDING EMAIL
            services.AddTransient<IMailService, NullMailService>();
            services.AddTransient<PluralSeeder>();
            services.AddScoped<IPluralRepository, PluralRepository>();
            services.AddMvc(opt => {
                if(_env.IsProduction())
                {
                    // ENABLE HTTPS FOR PRODUCTION
                    opt.Filters.Add(new RequireHttpsAttribute());
                }
            }).AddJsonOptions(opt => opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment()) // LOOKS FOR WORD DEVELOPMENT isEnvironment("Development")
            {
                app.UseDeveloperExceptionPage(); // SHOW EXCEPTIONS TO THE BROWSER | THIS MUST BE NOT USED IN PRODUCTION
            } else
            {
                app.UseExceptionHandler("/error");
            }
            // MUST BE IN THIS ORDER
            // WE REMOVES USE DEFAULT AND NOW INSTEAD INDEX APP/INDEX WILL BE SHOWN

            app.UseAuthentication();
            app.UseStaticFiles();

            app.UseMvc(cfg => {
                //cfg.MapRoute("Foo", "users/manage", new { controller = "UserManagement", Action = "Index" });
                cfg.MapRoute("Default", "{controller}/{action}/{id?}", new { controller = "App", Action = "Index" }); // THIS ACTION CONTROLLER S BY DEFAULT
            }); // MIDDLEWARE

            if(env.IsDevelopment())
            {
                using (var scope = app.ApplicationServices.CreateScope())
                {
                    var seeder = scope.ServiceProvider.GetService<PluralSeeder>();
                    seeder.Seed().Wait();
                }
            }
            
        }
    }
}
