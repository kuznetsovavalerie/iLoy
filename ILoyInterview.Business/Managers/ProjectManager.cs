using ILoyInterview.Domain.Abstract;
using ILoyInterview.Contracts.ApiModels;
using ILoyInterview.Data;
using ILoyInterview.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using ILoyInterview.Data.Entities.Enums;

namespace ILoyInterview.Domain.Managers
{
    public class ProjectManager : ICrudManager<ProjectModel>
    {
        private readonly ILoyInterviewContext _context;

        public ProjectManager(ILoyInterviewContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAsync(ProjectModel model, CancellationToken token)
        {
            if (model.ParentProjectId.HasValue && !(await _context.Projects.AnyAsync(x => x.Id == model.ParentProjectId, token)))
            {
                throw new ArgumentException($"{nameof(ProjectModel.ParentProjectId)} doesn't exists.");
            }

            var project = new Project
            {
                Name = model.Name,
                Code = model.Code,
                StartDate = model.StartDate.Value,
                FinishDate = model.FinishDate.Value,
                ParentProjectId = model.ParentProjectId
            };

            _context.Projects.Add(project);

            await _context.SaveChangesAsync(token);

            return project.Id;
        }

        public async Task UpdateAsync(int id, ProjectModel model, CancellationToken token)
        {
            if (!(await _context.Projects.AnyAsync(x => x.Id == id, token)))
            {
                throw new ArgumentException($"{nameof(Project)} doesn't exists.");
            }

            if (model.ParentProjectId.HasValue && !(await _context.Projects.AnyAsync(x => x.Id == model.ParentProjectId, token)))
            {
                throw new ArgumentException($"{nameof(ProjectModel.ParentProjectId)} doesn't exists.");
            }

            var project = await _context.Projects
                .Where(x => x.Id == id)
                .FirstAsync(token);

            project.Name = model.Name;
            project.Code = model.Code;
            project.StartDate = model.StartDate.Value;
            project.FinishDate = model.FinishDate.Value;
            project.ParentProjectId = model.ParentProjectId;

            await _context.SaveChangesAsync(token);
        }

        public async Task DeleteAsync(int id, CancellationToken token)
        {
            var project = await _context.Projects
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync(token);

            if (project == null)
            {
                throw new ArgumentException($"{nameof(Project)} doesn't exists.");
            }

            _context.Projects.Remove(project);

            await _context.SaveChangesAsync(token);
        }

        public async Task<ProjectModel> GetAsync(int id, CancellationToken token)
        {
            var model = await _context.Projects
                .Where(x => x.Id == id)
                .Select(x => new ProjectModel
                {
                    Name = x.Name,
                    Code = x.Code,
                    StartDate = x.StartDate,
                    FinishDate = x.FinishDate,
                    ParentProjectId = x.ParentProjectId,
                    State = (ModelStateEnum?)x.State
                })
                .FirstOrDefaultAsync(token);

            if (model == null)
            {
                throw new ArgumentException($"{nameof(Project)} doesn't exists.");
            }

            return model;
        }
    }
}
