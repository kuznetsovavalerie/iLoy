using ILoyInterview.API.Controllers.Base;
using ILoyInterview.Contracts.ApiModels;
using ILoyInterview.Domain.Managers;
using Microsoft.AspNetCore.Mvc;

namespace ILoyInterview.API.Controllers
{
    [Route("api")]
    public class ProjectController : ApiBaseController<ProjectManager, ProjectModel>
    {
        public ProjectController(ProjectManager manager) : base(manager) { }
    }
}
