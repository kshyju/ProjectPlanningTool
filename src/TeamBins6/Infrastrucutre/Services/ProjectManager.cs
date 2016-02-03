using System.Collections.Generic;
using TeamBins.Common;
using TeamBins.Common.ViewModels;
using TeamBins.DataAccess;
using TeamBins6.Infrastrucutre.Services;

namespace TeamBins.Services
{
    public interface IProjectManager
    {
        //GetProjects(int teamId);

        bool DoesProjectsExist();

        ProjectDto GetDefaultProjectForCurrentTeam();

        IEnumerable<ProjectDto> GetProjects();

        ProjectDto GetProject(int id);

        void Save(CreateProjectVM model);

        int GetIssueCountForProject(int projectId);
        void Delete(int id);
    }

    public class ProjectManager : IProjectManager
    {
        IProjectRepository projectRepository;
        private IUserSessionHelper userSessionHelper;

        public ProjectManager(IProjectRepository projectRepository, IUserSessionHelper userSessionHelper)
        {
            this.projectRepository = projectRepository;
            this.userSessionHelper = userSessionHelper;
        }

        public void Delete(int id)
        {
            this.projectRepository.Delete(id);
        }

        public bool DoesProjectsExist()
        {
            return this.projectRepository.DoesProjectsExist(this.userSessionHelper.TeamId);
        }

        public ProjectDto GetDefaultProjectForCurrentTeam()
        {
            return this.projectRepository.GetDefaultProjectForTeam(this.userSessionHelper.TeamId);
        }

        public IEnumerable<ProjectDto> GetProjects()
        {
            return this.projectRepository.GetProjects(this.userSessionHelper.TeamId);
        }

        public ProjectDto GetProject(int id)
        {
            return this.projectRepository.GetProject(id);
        }

        public void Save(CreateProjectVM model)
        {
            model.CreatedById = userSessionHelper.UserId;
            model.TeamId = userSessionHelper.TeamId;
            this.projectRepository.Save(model);
        }

        public int GetIssueCountForProject(int projectId)
        {
            return this.projectRepository.GetIssueCountForProject(projectId);
            ;
        }
    }
}