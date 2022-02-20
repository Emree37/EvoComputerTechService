using EvoComputerTechService.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvoComputerTechService.ViewModels
{
    public class IssueDetailViewModel
    {
        public List<IssueProducts> IssueProducts { get; set; }
        public List<Product> Products { get; set; }
    }
}
