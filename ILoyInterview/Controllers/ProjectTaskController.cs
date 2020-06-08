using ILoyInterview.API.Controllers.Base;
using ILoyInterview.Contracts.ApiModels;
using ILoyInterview.Domain.Managers;
using ILoyInterview.Domain.Reports;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace ILoyInterview.API.Controllers
{
    [Route("api")]
    public class ProjectTaskController : ApiBaseController<ProjectTaskManager, ProjectTaskModel>
    {
        private readonly ReportGenerator _reportGenerator;

        public ProjectTaskController(ProjectTaskManager manager, ReportGenerator reportGenerator) 
            : base(manager) 
        {
            _reportGenerator = reportGenerator;
        }

        [HttpGet("[controller]/[action]/{date}")]
        public async Task<IActionResult> Report(DateTime date)
        {
            var stream = await _reportGenerator.Generate(date, HttpContext.RequestAborted);

            return File(stream, "application/octet-stream", "report.xlsx");
        }
    }
}
