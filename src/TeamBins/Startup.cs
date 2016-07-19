using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TeamBins.Services;
using TeamBins.DataAccess;
using TeamBins6.Infrastrucutre.Services;
using TeamBins6.Infrastrucutre.Filters;
using TeamBins6.Infrastrucutre.Cache;
using Microsoft.AspNetCore.Http;

namespace TeamBins
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
           // Configuration = builder.Build();

            Configuration= builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

       // public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<ICommentManager, CommentManager>();
            services.AddTransient<IUserSessionHelper, UserSessionHelper>();
            services.AddTransient<IProjectManager, ProjectManager>();
            services.AddTransient<IProjectRepository, ProjectRepository>();
            services.AddTransient<IIssueRepository, IssueRepository>();
            services.AddTransient<IIssueManager, IssueManager>();
            services.AddTransient<ITeamManager, TeamManager>();
            services.AddTransient<IActivityRepository, ActivityRepository>();
            services.AddTransient<ITeamRepository, TeamRepository>();
            services.AddTransient<ICommentRepository, CommentRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUserAccountManager, UserAccountManager>();
            services.AddTransient<IEmailManager, EmailManager>();
            services.AddTransient<IEmailRepository, EmailRepository>();

            //services.AddScoped<>()
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddTransient<ICache, InMemoryCache>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddMemoryCache();
            
           

           services.AddSession(s => s.IdleTimeout = TimeSpan.FromMinutes(30));

            // Add framework services.
            services.AddMvc(o =>
            {
                o.Filters.Add(new ReqProcessFilter());
            });

            //services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseSession();
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
