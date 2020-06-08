using ILoyInterview.Data.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ILoyInterview.Data.Entities
{
    public class Project
    {
        public int Id { get; set; }

        public int? ParentProjectId { get; set; }

        public Project ParentProject { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime FinishDate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public StateEnum State { get; set; }

        public IEnumerable<ProjectTask> Tasks { get; set; }

        public IEnumerable<Project> ChildProjects { get; set; }
    }
}
