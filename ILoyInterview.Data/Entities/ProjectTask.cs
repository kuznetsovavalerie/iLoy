using ILoyInterview.Data.Entities.Enums;
using System;
using System.Collections.Generic;

namespace ILoyInterview.Data.Entities
{
    public class ProjectTask
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }

        public Project Project { get; set; }

        public int? ParentTaskId { get; set; }

        public ProjectTask ParentTask { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime FinishDate { get; set; }

        public StateEnum State { get; set; }

        public IEnumerable<ProjectTask> ChildTasks { get; set; }
    }
}
