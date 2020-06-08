using ILoyInterview.Contracts.ApiModels.ValidationAttributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace ILoyInterview.Contracts.ApiModels
{
    public class ProjectModel
    {
        public int? ParentProjectId { get; set; }

        [Required]
        [StringLength(4)]
        public string Code { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        public DateTime? StartDate { get; set; }

        [Required]
        [DateAfter("StartDate")]
        public DateTime? FinishDate { get; set; }

        public ModelStateEnum? State { get; set; }
    }
}
