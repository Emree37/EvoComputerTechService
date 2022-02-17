using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EvoComputerTechService.Models.Identity
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole()
        {

        }

        public ApplicationRole(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }

        [StringLength(100)]
        public string Description { get; set; }

    }
}
 


