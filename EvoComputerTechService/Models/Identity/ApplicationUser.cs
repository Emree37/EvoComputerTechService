using EvoComputerTechService.Models.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EvoComputerTechService.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Surname { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public virtual List<Issue> Issues { get; set; }
    }
}
