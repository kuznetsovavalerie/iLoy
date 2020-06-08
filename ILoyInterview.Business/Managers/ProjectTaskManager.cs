using ILoyInterview.Contracts.ApiModels;
using ILoyInterview.Data;
using ILoyInterview.Data.Entities;
using ILoyInterview.Data.Entities.Enums;
using ILoyInterview.Domain.Abstract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ILoyInterview.Domain.Managers
{
    public class ProjectTaskManager : ICrudManager<ProjectTaskModel>
    {
        private readonly ILoyInterviewContext _context;

        public ProjectTaskManager(ILoyInterviewContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAsync(ProjectTaskModel model, CancellationToken token)
        {
            if (model.ParentTaskId.HasValue)
            {
                await AssureProjectTaskExistsAsync(model.ParentTaskId.Value, nameof(ProjectTaskModel.ParentTaskId), token);
            }

            if (model.ProjectId.HasValue)
            {
                await AssureProjectExistsAsync(model.ProjectId.Value, nameof(ProjectTaskModel.ProjectId), token);
            }

            var projectTask = new ProjectTask
            {
                Name = model.Name,
                Description = model.Description,
                StartDate = model.StartDate.Value,
                FinishDate = model.FinishDate.Value,
                ProjectId = model.ProjectId.Value,
                ParentTaskId = model.ParentTaskId,
                State = (StateEnum)model.State.Value
            };

            _context.ProjectTasks.Add(projectTask);

            await _context.SaveChangesAsync(token);

            return projectTask.Id;
        }

        public async Task UpdateAsync(int id, ProjectTaskModel model, CancellationToken token)
        {
            await AssureProjectTaskExistsAsync(id, nameof(ProjectTask), token);

            if (model.ParentTaskId.HasValue)
            {
                await AssureProjectTaskExistsAsync(model.ParentTaskId.Value, nameof(ProjectTaskModel.ParentTaskId), token);
            }

            if (model.ProjectId.HasValue)
            {
                await AssureProjectExistsAsync(model.ProjectId.Value, nameof(ProjectTaskModel.ProjectId), token);
            }

            var projectTask = await _context.ProjectTasks
                .Where(x => x.Id == id)
                .FirstAsync(token);

            projectTask.Name = model.Name;
            projectTask.Description = model.Description;
            projectTask.StartDate = model.StartDate.Value;
            projectTask.FinishDate = model.FinishDate.Value;
            projectTask.ProjectId = model.ProjectId.Value;
            projectTask.ParentTaskId = model.ParentTaskId;
            projectTask.State = (StateEnum)model.State.Value;

            await _context.SaveChangesAsync(token);
        }

        public async Task DeleteAsync(int id, CancellationToken token)
        {
            var projectTask = await _context.ProjectTasks
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync(token);

            if (projectTask == null)
            {
                throw new ArgumentException($"{nameof(ProjectTask)} doesn't exists.");
            }

            _context.ProjectTasks.Remove(projectTask);

            await _context.SaveChangesAsync(token);
        }

        public async Task<ProjectTaskModel> GetAsync(int id, CancellationToken token)
        {
            var model = await _context.ProjectTasks
                .Where(x => x.Id == id)
                .Select(x => new ProjectTaskModel
                {
                    Name = x.Name,
                    Description = x.Description,
                    StartDate = x.StartDate,
                    FinishDate = x.FinishDate,
                    ProjectId = x.ProjectId,
                    ParentTaskId = x.ParentTaskId,
                    State = (ModelStateEnum?)x.State
                })
                .FirstOrDefaultAsync(token);

            if (model == null)
            {
                throw new ArgumentException($"{nameof(ProjectTask)} doesn't exists.");
            }

            return model;
        }

        public async Task<List<ProjectTask>> GetTasksWithProjectsAsync(DateTime reportDate, CancellationToken token)
        {
            var tasks = await _context.ProjectTasks
                .Where(x => x.StartDate <= reportDate && x.FinishDate >= reportDate && x.State == StateEnum.InProgress)
                .Include(x => x.Project)
                .ToListAsync(token);

            return tasks;
        }

        private async Task AssureProjectExistsAsync(int projectId, string propertyName, CancellationToken token)
        {
            if (!(await _context.Projects.AnyAsync(x => x.Id == projectId, token)))
            {
                throw new ArgumentException($"{propertyName} doesn't exists.");
            }
        }

        private async Task AssureProjectTaskExistsAsync(int projectTaskId, string propertyName, CancellationToken token)
        {
            if (!(await _context.ProjectTasks.AnyAsync(x => x.Id == projectTaskId, token)))
            {
                throw new ArgumentException($"{propertyName} doesn't exists.");
            }
        }
    }
}
