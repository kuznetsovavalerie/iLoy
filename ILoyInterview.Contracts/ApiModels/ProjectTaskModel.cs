using ILoyInterview.Contracts.ApiModels.ValidationAttributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace ILoyInterview.Contracts.ApiModels
{
    public class ProjectTaskModel
    {
        [Required]
        public int? ProjectId { get; set; }

        public int? ParentTaskId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(255)]
        public string Description { get; set; }

        [Required]
        public DateTime? StartDate { get; set; }

        [Required]
        [DateAfter("StartDate")]
        public DateTime? FinishDate { get; set; }

        [Required]
        public ModelStateEnum? State { get; set; }
    }
}
