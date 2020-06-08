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
    // more test cases should be here, with all combinations of task states and count
    // and invalid model conditions
    public class ProjectTaskManagerTest : IDisposable
    {
        private readonly ILoyInterviewContext _context;
        private readonly ProjectManager _projectManager;
        private readonly ProjectTaskManager _projectTaskManager;
        private readonly int _notExistsProjectId = 30;
        private readonly int _notExistsProjectTaskId = 30;

        public ProjectTaskManagerTest()
        {
            var options = new DbContextOptionsBuilder<ILoyInterviewContext>()
                .UseInMemoryDatabase(databaseName: "ILoyDbTest")
                .Options;

            _context = new ILoyInterviewContext(options);
            _projectManager = new ProjectManager(_context);
            _projectTaskManager = new ProjectTaskManager(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public async Task CreateSucceed()
        {
            var id = await _projectTaskManager.CreateAsync(await GetValidModel(), CancellationToken.None);

            Assert.True(await _context.ProjectTasks.AnyAsync(x => x.Id == id));
        }

        [Fact]
        public async Task CreateThrowsArgumentException()
        {
            var model = await GetValidModel();

            model.ProjectId = _notExistsProjectId;

            await Assert.ThrowsAsync<ArgumentException>(() => _projectTaskManager.CreateAsync(model, CancellationToken.None));
        }

        [Fact]
        public async Task UpdateSucceed()
        {
            var model = await GetValidModel();

            var id = await _projectTaskManager.CreateAsync(model, CancellationToken.None);
            model.Name = "New Name";

            await _projectTaskManager.UpdateAsync(id, model, CancellationToken.None);

            Assert.True(await _context.ProjectTasks.AnyAsync(x => x.Id == id && x.Name == model.Name));
        }

        [Fact]
        public async Task UpdateThrowsArgumentException()
        {
            var model = await GetValidModel();

            await Assert.ThrowsAsync<ArgumentException>(() => _projectTaskManager.UpdateAsync(_notExistsProjectTaskId, model, CancellationToken.None));
        }

        [Fact]
        public async Task DeleteSucceed()
        {
            var id = await _projectTaskManager.CreateAsync(await GetValidModel(), CancellationToken.None);

            await _projectTaskManager.DeleteAsync(id, CancellationToken.None);

            Assert.False(await _context.ProjectTasks.AnyAsync(x => x.Id == id));
        }

        [Fact]
        public async Task DeleteThrowsArgumentException()
        {
            await _projectTaskManager.CreateAsync(await GetValidModel(), CancellationToken.None);

            await Assert.ThrowsAsync<ArgumentException>(() => _projectTaskManager.DeleteAsync(_notExistsProjectTaskId, CancellationToken.None));
        }

        [Fact]
        public async Task GetSucceed()
        {
            var model = await GetValidModel();

            var id = await _projectTaskManager.CreateAsync(model, CancellationToken.None);

            var getModel = await _projectTaskManager.GetAsync(id, CancellationToken.None);

            Assert.True(getModel.Name == model.Name);
        }

        [Fact]
        public async Task GetThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _projectTaskManager.GetAsync(_notExistsProjectTaskId, CancellationToken.None));
        }

        private async Task<ProjectTaskModel> GetValidModel()
        {
            var model = new ProjectTaskModel
            {
                Name = "Test Task",
                Description = "Task description",
                State = ModelStateEnum.Planned,
                ProjectId = await CreateProject(),
                StartDate = new DateTime(2020, 5, 20),
                FinishDate = new DateTime(2020, 5, 30)
            };

            return model;
        }

        private Task<int> CreateProject()
        {
            var projectModel = new ProjectModel
            {
                Name = "Test Proj",
                Code = "TP",
                StartDate = new DateTime(2020, 5, 20),
                FinishDate = new DateTime(2020, 5, 30)
            };

            return _projectManager.CreateAsync(projectModel, CancellationToken.None);
        }
    }
}
