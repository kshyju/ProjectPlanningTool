using System;
using System.Collections.Generic;
using System.Linq;
using TeamBins.Common;

namespace TeamBins.DataAccess
{
    public interface IProjectRepository
    {
        IEnumerable<ProjectDto> GetProjects(int teamId);
        bool DoesProjectsExist(int teamId);
    }

    public class ProjectRepository : IProjectRepository
    {
        private readonly TeamEntities db;
        public ProjectRepository()
        {
            db = new TeamEntities();
        }

        public bool DoesProjectsExist(int teamId)
        {
            return db.Projects.Any(s => s.TeamID == teamId);
        }

        public IEnumerable<ProjectDto> GetProjects(int teamId)
        {
            return db.Projects.Where(s => s.TeamID == teamId)
                .Select(s => new ProjectDto
                {
                    Name = s.Name,
                    Id = s.ID
                }).ToList();
        }
    }
}