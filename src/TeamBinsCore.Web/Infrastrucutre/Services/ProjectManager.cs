using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TeamBins.Common;
using TeamBins.Common.ViewModels;
using TeamBins.DataAccessCore;
using TeamBins6.Infrastrucutre;
using TeamBins6.Infrastrucutre.Cache;
using TeamBins6.Infrastrucutre.Services;
using TeamBinsCore.DataAccess;

namespace TeamBins.Services
{
    public interface IProjectManager
    {
        //GetProjects(int teamId);

        bool DoesProjectsExist();

        Task<ProjectDto> GetDefaultProjectForCurrentTeam();

        IEnumerable<ProjectDto> GetProjects();

        ProjectDto GetProject(int id);

        void Save(CreateProjectVM model);

        int GetIssueCountForProject(int projectId);
        void Delete(int id);
    }

    public class ProjectManager : IProjectManager
    {
        IProjectRepository projectRepository;
        private IUserAuthHelper userSessionHelper;
        private readonly ICache cache;

        public ProjectManager(IProjectRepository projectRepository, IUserAuthHelper userSessionHelper,ICache cache)
        {
            this.projectRepository = projectRepository;
            this.userSessionHelper = userSessionHelper;
            this.cache = cache;
        }

        public void Delete(int id)
        {
            this.projectRepository.Delete(id);
        }

        public bool DoesProjectsExist()
        {
            return this.projectRepository.DoesProjectsExist(this.userSessionHelper.TeamId);
        }

        public async Task<ProjectDto> GetDefaultProjectForCurrentTeam()
        {
            return await this.projectRepository.GetDefaultProjectForTeamMember(this.userSessionHelper.TeamId,
                this.userSessionHelper.UserId);
          
        }


        public IEnumerable<ProjectDto> GetProjects()
        {
            var cacheKey = CacheKey.GetKey(CacheKey.Projects, userSessionHelper.TeamId);
            return this.cache.Get(cacheKey, () => this.projectRepository.GetProjects(this.userSessionHelper.TeamId),60*60);
        }

        public ProjectDto GetProject(int id)
        {
            var cacheKey = CacheKey.GetKey(CacheKey.Projects, userSessionHelper.TeamId);
            var projects = this.cache.Get(cacheKey, () => this.projectRepository.GetProjects(this.userSessionHelper.TeamId), 60 * 60);
            return projects.FirstOrDefault(s => s.Id == id);
        }

        public void Save(CreateProjectVM model)
        {
            model.CreatedById = userSessionHelper.UserId;
            model.TeamId = userSessionHelper.TeamId;
            this.projectRepository.Save(model);

            // Clear the cache so that new data will be read and stored to cache when needed
            cache.Clear(CacheKey.GetKey(CacheKey.Projects, userSessionHelper.TeamId));

        }

         
        public int GetIssueCountForProject(int projectId)
        {
            return this.projectRepository.GetIssueCountForProject(projectId);
        }
    }
}