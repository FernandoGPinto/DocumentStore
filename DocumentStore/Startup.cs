using DocumentStore.Core.Interfaces;
using DocumentStore.Data;
using DocumentStore.Data.Repositories;
using DocumentStore.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentStore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextFactory<FileStoreDBContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("FileStoreDbConnection"),
                    sqlServerOptions => sqlServerOptions.CommandTimeout(60))
                );

            services.AddAutoMapper(c => c.AddProfile<AutoMapperProfile>(), typeof(Startup));

            services.AddScoped<IFileStoreRepository, FileStoreRepository>();
            services.AddScoped<IFileStoreService, FileStoreService>();

            services.AddRazorPages()
                .AddRazorPagesOptions(options =>
                {
                    // CSRF (Antiforgery) tokens aren't involved in a server-side Blazor circuit, since CSRF is about protecting from unexpected third-party HTTP requests, whereas a server-side Blazor circuit exists within a Websocket connection.
                    options.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());

                    // Force authentication upon navigating to the website.
                    options.Conventions.AuthorizePage("/_Host", "RequireAuthentication");
                });

            services.AddServerSideBlazor();

            // The default authentication scheme is Identity.Application. An Identity.Application cookie is created and is then used to sign in, load and sign out the current user.
            // Only override when an alternative auth scheme is being used. This will generate a different cookie which may require alternative classes to SignInManager and UserManager.
            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
                .AddCookie(options =>
                {
                    options.Cookie.Name = "DocumentStore";
                    // Set the secure flag, which Chrome's changes will require for SameSite none.
                    // Note this will also require you to be running on HTTPS.
                    //options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

                    // Add the SameSite attribute, this will emit the attribute with a value of none.
                    // To not emit the attribute at all set
                    // SameSite = (SameSiteMode)(-1)
                    //options.Cookie.SameSite = SameSiteMode.Lax;

                    // Set the cookie to HTTP only which is good practice unless you really do need
                    // to access it client side in scripts.
                    options.Cookie.HttpOnly = true;

                    // Configure the client application to use sliding sessions i.e. session will remain active as long as the user is actively using the client application.
                    options.SlidingExpiration = false;

                    // Expire the session after 15 minutes of inactivity.
                    // Sets expiration for the ticket that is stored inside the cookie.
                    // The middleware that runs on the server will detect that the ticket is no longer valid and redirect to the login page.
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
                })
                .AddOpenIdConnect(options =>
                {
                    // MetadataAddress represents the Active Directory instance used to authenticate users.
                    options.MetadataAddress = "https://login.microsoftonline.com/tenantid/v2.0/.well-known/openid-configuration";
                    options.Authority = "";
                    options.AuthenticationMethod = OpenIdConnectRedirectBehavior.RedirectGet;
                    options.ClientId = "ClientId";
                    options.ClientSecret = "ClientSecret";
                    options.ResponseMode = "form_post";
                    options.ResponseType = "id_token";
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = "https://login.microsoftonline.com/tenantid/v2.0"
                    };

                });

            // Add policies to manage access to different parts of the application
            services.AddAuthorization(options => {
                options.AddPolicy("RequireAdminRole", policy => policy.RequireClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Admin"));
                options.AddPolicy("RequireAuthentication", policy =>
                {
                    policy.RequireAuthenticatedUser();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
