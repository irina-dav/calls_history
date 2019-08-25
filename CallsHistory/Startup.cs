using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CallsHistory.Models;
using CallsHistory.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CallsHistory.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using CallsHistory.Infrastucture;

namespace CallsHistory
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<LdapConfig>(Configuration.GetSection("ldap"));

            /*services.AddDbContext<AppDbContext>(options =>
               options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));

            services.AddDbContext<UserDbContext>(options =>
               options.UseMySql(Configuration.GetConnectionString("ConnectionAsterisk")));*/

            List<string> connStringsCdr = Configuration.GetSection("ConnectionStringsCdr").Get<List<string>>();
            List<string> connStringsAsterisk = Configuration.GetSection("ConnectionStringsAsrerisk").Get<List<string>>();
            Dictionary<string, int> serversOffsetsUTC = Configuration.GetSection("OffsetsUTC").Get<Dictionary<string, int>>();
            Dictionary<string, int> timeZonesOffsetUTC = Configuration.GetSection("TimeZones").Get<Dictionary<string, int>>();

            services.AddScoped<DateTimeService>(x => new DateTimeService(serversOffsetsUTC, timeZonesOffsetUTC));

            services.AddSingleton<IDbContextFactoryService>(x => 
                 new DbContextFactoryService(new Dictionary<string, List<string>>{{"cds", connStringsCdr },{ "asterisk", connStringsAsterisk}}, x.GetService<ILogger<DbContextFactoryService>>()));

            // DbContextFactory.AddListConnectionString("cds", connStringsCdr);
            // DbContextFactory.AddListConnectionString("asterisk", connStringsAsterisk);

            services.AddTransient<IRepository, EFRepository>();
            services.AddTransient<IUsersRepository, EFUserRepository>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
                options.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/Account/AccessDeniedPath");
            });

            services.AddScoped<TextService>();
            services.AddScoped<LdapService>();

            services.AddMvc();
            services.AddHttpContextAccessor();
            services.AddMemoryCache();
            services.AddSession();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile(Configuration.GetSection("Logging"));
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseDeveloperExceptionPage();
            app.UseStatusCodePages();
            app.UsePathBase(Configuration["PathBase"]);
            app.UseStaticFiles();

            app.UseSession();
            app.UseAuthentication();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "main",
                    template: "{controller=CallsHistory}/{action=Index}");
            });
        }
    }
}
