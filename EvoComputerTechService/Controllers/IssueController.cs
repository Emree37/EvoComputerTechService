using AutoMapper;
using EvoComputerTechService.Models.Identity;
using EvoComputerTechService.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvoComputerTechService.Controllers
{
    public class IssueController : Controller
    {

        [HttpGet]
        public IActionResult GetIssues()
        {
            return View();
        }
    }
}
