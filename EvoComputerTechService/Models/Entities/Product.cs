using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EvoComputerTechService.Models.Entities
{
    public class Product :  BaseEntity
    {
        public string ProductName { get; set; }

        public string ProductDescription { get; set; }

        public decimal Price { get; set; }

        [StringLength(450)]
        public Guid IssueId { get; set; } = Guid.NewGuid();

        [ForeignKey(nameof(IssueId))]
        public virtual Issue Issue { get; set; }
    }
}
