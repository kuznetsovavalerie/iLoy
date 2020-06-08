using ILoyInterview.Contracts.ApiModels;
using ILoyInterview.Data;
using ILoyInterview.Domain.Managers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ILoyInterview.Tests
{
    // more test cases should be described here, with all combinations of task states and count
    // and invalid model conditions
    public class ProjectManagerTest : IDisposable
    {
        private readonly ILoyInterviewContext _context;
        private readonly ProjectManager _manager;
        private readonly ProjectTaskManager _projectTaskManager;
        private readonly int _notExistsProjectId = 30;

        public ProjectManagerTest()
        {
            var options = new DbContextOptionsBuilder<ILoyInterviewContext>()
                .UseInMemoryDatabase(databaseName: "ILoyDbTest")
                .Options;

            _context = new ILoyInterviewContext(options);
            _manager = new ProjectManager(_context);
            _projectTaskManager = new ProjectTaskManager(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public async Task CreateSucceed()
        {
            var id = await _manager.CreateAsync(GetValidModel(), CancellationToken.None);

            Assert.True(await _context.Projects.AnyAsync(x => x.Id == id));
        }

        [Fact]
        public async Task CreateThrowsArgumentException()
        {
            var model = GetValidModel();

            model.ParentProjectId = _notExistsProjectId;

            await Assert.ThrowsAsync<ArgumentException>(() => _manager.CreateAsync(model, CancellationToken.None));
        }

        [Fact]
        public async Task UpdateSucceed()
        {
            var model = GetValidModel();

            var id = await _manager.CreateAsync(model, CancellationToken.None);
            model.Code = "TPU";

            await _manager.UpdateAsync(id, model, CancellationToken.None);

            Assert.True(await _context.Projects.AnyAsync(x => x.Id == id && x.Code == model.Code));
        }

        [Fact]
        public async Task UpdateThrowsArgumentException()
        {
            var model = GetValidModel();

            var id = _notExistsProjectId;

            await Assert.ThrowsAsync<ArgumentException>(() => _manager.UpdateAsync(id, model, CancellationToken.None));
        }

        [Fact]
        public async Task DeleteSucceed()
        {
            var id = await _manager.CreateAsync(GetValidModel(), CancellationToken.None);

            await _manager.DeleteAsync(id, CancellationToken.None);

            Assert.False(await _context.Projects.AnyAsync(x => x.Id == id));
        }

        [Fact]
        public async Task CascadeDeleteSucceed()
        {
            var parentId = await _manager.CreateAsync(GetValidModel(), CancellationToken.None);

            var child = GetValidModel();

            child.ParentProjectId = parentId;

            var childId = await _manager.CreateAsync(child, CancellationToken.None);
            var projectChildTaskId = await CreateProjectTaskAsync(childId);

            await _manager.DeleteAsync(parentId, CancellationToken.None);

            Assert.False(await _context.Projects.AnyAsync(x => x.Id == parentId || x.Id == childId));
            Assert.False(await _context.ProjectTasks.AnyAsync(x => x.Id == projectChildTaskId));
        }

        [Fact]
        public async Task DeleteThrowsArgumentException()
        {
            await _manager.CreateAsync(GetValidModel(), CancellationToken.None);

            await Assert.ThrowsAsync<ArgumentException>(() => _manager.DeleteAsync(_notExistsProjectId, CancellationToken.None));
        }

        [Fact]
        public async Task GetSucceed()
        {
            var model = GetValidModel();

            var id = await _manager.CreateAsync(model, CancellationToken.None);

            var getModel = await _manager.GetAsync(id, CancellationToken.None);

            Assert.True(getModel.Code == model.Code);
        }

        [Fact]
        public async Task GetThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _manager.GetAsync(_notExistsProjectId, CancellationToken.None));
        }

        private ProjectModel GetValidModel()
        {
            var model = new ProjectModel
            {
                Name = "Test Proj",
                Code = "TP",
                StartDate = new DateTime(2020, 5, 20),
                FinishDate = new DateTime(2020, 5, 30)
            };

            return model;
        }

        private Task<int> CreateProjectTaskAsync(int projectId, ModelStateEnum state = ModelStateEnum.Planned)
        {
            var model = new ProjectTaskModel
            {
                Name = "Test Task",
                Description = "Task description",
                State = state,
                ProjectId = projectId,
                StartDate = new DateTime(2020, 5, 20),
                FinishDate = new DateTime(2020, 5, 30)
            };

            return _projectTaskManager.CreateAsync(model, CancellationToken.None);
        }
    }
}
