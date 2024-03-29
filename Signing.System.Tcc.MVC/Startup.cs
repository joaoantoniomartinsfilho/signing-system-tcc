﻿using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Signing.System.Tcc.DependencyConfiguration;
using NToastNotify;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using System;
using Signing.System.Tcc.MVC.Services;
using Signing.System.Tcc.MVC.Interfaces;

namespace Signing.System.Tcc.MVC
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
            // Dependency Injection Configuration
            services.AddDepencyInjectionSigningSystem();

            services.AddScoped<IAuthenticantionService, AuthService>();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthentication(Helpers.Helpers.SigningSystemScheme)
            .AddCookie(Helpers.Helpers.SigningSystemScheme);

            services.AddMvc(options =>
            {
                var policyAuthAllControllers = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                options.Filters.Add(new AuthorizeFilter(policyAuthAllControllers));
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            .AddNToastNotifyToastr(new ToastrOptions
            {
                ProgressBar = false,
                PositionClass = ToastPositions.TopCenter,
                TimeOut = 5000
            });

            services.ConfigureApplicationCookie(options =>
            {                
                options.Cookie.MaxAge = TimeSpan.FromMinutes(30);

                options.AccessDeniedPath = "/Account/AccessDenied";

                options.Cookie.Name = Helpers.Helpers.SigningSystemScheme;

                options.Cookie.Expiration = TimeSpan.FromHours(2);

                options.ExpireTimeSpan = TimeSpan.FromHours(2);

                options.LoginPath = "/Account/Login";

                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;

                options.SlidingExpiration = true;                
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            //{
            //    using (var context = scope.ServiceProvider.GetService<DbContext>())
            //    {
            //        context.Database.Migrate();
            //        context.Database.ExecuteSqlCommand("create extension if not exists unaccent;");
            //    }
            //}

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseNToastNotify();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
