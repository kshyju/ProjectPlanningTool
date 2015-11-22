using TeamBins.DataAccess;
using TeamBins.Services;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(TeamBins.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(TeamBins.App_Start.NinjectWebCommon), "Stop")]

namespace TeamBins.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<IUserAccountManager>().To<UserAccountManager>();
            kernel.Bind<IAccountRepository>().To<AccountRepository>();
            kernel.Bind<ITeamRepository>().To<TeamRepository>();

            kernel.Bind<IUserAccountEmailManager>().To<UserAccountEmailManager>();

            kernel.Bind<IEmailTemplateRepository>().To<EmailTemplateRepository>();
            kernel.Bind<IUserSessionHelper>().To<UserSessionHelper>();
            kernel.Bind<IProjectRepository>().To<ProjectRepository>();
            kernel.Bind<ITeamManager>().To<TeamManager>();
            kernel.Bind<IProjectManager>().To<ProjectManager>();
            kernel.Bind<IRepositary>().To<Repositary>();
            kernel.Bind<IIssueRepository>().To<IssueRepository>();
            kernel.Bind<IIssueManager>().To<IssueManager>();
            kernel.Bind<IActivityRepository>().To<ActivityRepository>();

        }        
    }
}
