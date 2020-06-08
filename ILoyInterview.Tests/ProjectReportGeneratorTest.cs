using ILoyInterview.Contracts.ApiModels;
using ILoyInterview.Data;
using ILoyInterview.Domain.Managers;
using ILoyInterview.Domain.Reports;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ILoyInterview.Tests
{
    // This test will only ensure that report generation is processed without errors, so developer can manually check saved file
    public class ProjectReportGeneratorTest : IDisposable
    {
        private readonly ILoyInterviewContext _context;
        private readonly ProjectManager _manager;
        private readonly ProjectTaskManager _projectTaskManager;
        private readonly ReportGenerator _reportGenerator;

        public ProjectReportGeneratorTest()
        {
            var options = new DbContextOptionsBuilder<ILoyInterviewContext>()
                .UseInMemoryDatabase(databaseName: "ILoyDbTest")
                .Options;

            _context = new ILoyInterviewContext(options);
            _manager = new ProjectManager(_context);
            _projectTaskManager = new ProjectTaskManager(_context);
            _reportGenerator = new ReportGenerator(_projectTaskManager);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public async Task CreateSucceeded()
        {
            await GenerateTestDataAsync();

            var ms = await _reportGenerator.Generate(new DateTime(2020, 5, 7), CancellationToken.None);

            using (var file = new FileStream("E:\\test.xlsx", FileMode.Create, System.IO.FileAccess.Write))
            {
                var bytes = new byte[ms.Length];

                ms.Read(bytes, 0, (int)ms.Length);
                file.Write(bytes, 0, bytes.Length);
                ms.Close();
            }
        }

        private async Task GenerateTestDataAsync()
        {
            var model = new ProjectModel
            {
                Name = "Test Project ",
                Code = "TP",
                StartDate = new DateTime(2020, 5, 20),
                FinishDate = new DateTime(2020, 5, 30)
            };

            for (var i = 1; i < 10; i++)
            {
                model.Name += i;
                model.Code += i;
                model.StartDate = model.StartDate.Value.AddDays(1);
                model.FinishDate = model.FinishDate.Value.AddDays(1);

                var projId = await _manager.CreateAsync(model, CancellationToken.None);

                var taskModel = new ProjectTaskModel
                {
                    Name = "Test Task",
                    Description = "Task description",
                    State = ModelStateEnum.Planned,
                    ProjectId = projId,
                    StartDate = model.StartDate,
                    FinishDate = model.FinishDate
                };

                await _projectTaskManager.CreateAsync(taskModel, CancellationToken.None);

                taskModel.State = ModelStateEnum.InProgress;

                await _projectTaskManager.CreateAsync(taskModel, CancellationToken.None);
            }
        }
    }
}
