using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EvoComputerTechService.Models.Entities
{
    public class IssueProducts
    {
        public Guid IssueId { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; } = Guid.NewGuid();

        [ForeignKey(nameof(IssueId))]
        public Issue Issue { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }
    }
}
