using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using TeamBins.Common.ViewModels;
using TeamBins.DataAccessCore;
using TeamBins.Infrastrucutre;
using TeamBins.Infrastrucutre.Cache;
using TeamBins.Infrastrucutre.Filters;
using TeamBins.Infrastrucutre.Services;
using TeamBins.Services;
using TeamBins.DataAccess;
using Microsoft.AspNetCore.Server.Kestrel.Internal.Http;



namespace TeamBins.Web
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
            Configuration = builder.Build();
        }
        public static ConnectionManager ConnectionManager;
        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCompression();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddTransient<ICommentManager, CommentManager>();
            services.AddTransient<IUserAuthHelper, SessionUserAuthHelper>();
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
            services.AddTransient<IUploadHandler, AzureBlobStorageHandler>(); //AzureBlobStorageHandler
            services.AddTransient<IUploadManager, UploadManager>();
            services.AddTransient<IUploadRepository, UploadRepository>();
            //services.AddScoped<>()
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddTransient<ICache, InMemoryCache>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddMemoryCache();

            services.Configure<AppSettings>(Configuration);
            services.Configure<TeamBinsAppSettings>(Configuration.GetSection("TeamBins"));
            services.AddSession(s => s.IdleTimeout = TimeSpan.FromMinutes(30));

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>(); //will work


            services.AddApplicationInsightsTelemetry(Configuration);

            // Add framework services.
            services.AddMvc(o =>
            {
                o.Filters.Add(new ReqProcessFilter());
            });
            //services.AddMvc(o =>
            //{
               
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {

                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();

            }
            else
            {
                app.UseApplicationInsightsRequestTelemetry();
                app.UseApplicationInsightsExceptionTelemetry();
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
