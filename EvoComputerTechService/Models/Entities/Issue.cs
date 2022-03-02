using EvoComputerTechService.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EvoComputerTechService.Models.Entities
{
    public class Issue : BaseEntity
    {
        public string IssueName { get; set; }

        public string Description { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string AddressDetail { get; set; }

        public string IssuePicture { get; set; }

        public IssueStates IssueState { get; set; }

        public bool IsDeleted { get; set; } = false;

        [StringLength(450)]
        public string TechnicianId { get; set; }

        [StringLength(450)]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey(nameof(TechnicianId))]
        public virtual ApplicationUser Technician { get; set; }

        public virtual List<IssueProducts> IssueProducts { get; set; }
    }

    public enum IssueStates
    {
        Beklemede,
        Kuyrukta,
        Islemde,
        Tamamlandi,
        Odendi
    }
}
