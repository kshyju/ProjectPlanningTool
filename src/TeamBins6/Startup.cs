using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Exceptional.Stores;
using TeamBins.DataAccess;
using TeamBins.Services;
using TeamBins6.Infrastrucutre.Filters;
using TeamBins6.Infrastrucutre.Services;

namespace TeamBins6
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

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
            services.AddTransient<IActivityRepository,ActivityRepository>();
            services.AddTransient<ITeamRepository, TeamRepository>();
            services.AddTransient<ICommentRepository, CommentRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUserAccountManager, UserAccountManager>();
            services.AddTransient<IEmailManager, EmailManager>();

            // Rechecking in

            services.AddCaching();
            services.AddSession(s => s.IdleTimeout = TimeSpan.FromMinutes(30));
            
            // Add framework services.
            services.AddMvc(o =>
            {
                o.Filters.Add(new ReqProcessFilter());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseIISPlatformHandler();

            app.UseStaticFiles();

            app.UseSession();

            StackExchange.Exceptional.ErrorStore.Setup("My Application", new SQLErrorStore("Data Source=DET-4082;Initial Catalog=Team;Integrated Security=true"));

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
