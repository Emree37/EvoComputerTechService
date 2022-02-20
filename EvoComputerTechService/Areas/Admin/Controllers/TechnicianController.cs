﻿using EvoComputerTechService.Data;
using EvoComputerTechService.Extensions;
using EvoComputerTechService.Models.Entities;
using EvoComputerTechService.Models.Identity;
using EvoComputerTechService.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvoComputerTechService.Areas.Admin.Controllers
{
    public class TechnicianController : TechnicianBaseController
    {
        private readonly MyContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public TechnicianController(MyContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> MyIssues()
        {
            var user = await _userManager.FindByIdAsync(HttpContext.GetUserId());

            var myIssues = _dbContext.Issues.Where(x => x.TechnicianId == user.Id && x.IssueState == IssueStates.Atandi).ToList();

            return View(myIssues);
        }

        [HttpGet]
        public async Task<IActionResult> CompletedIssues()
        {
            var user = await _userManager.FindByIdAsync(HttpContext.GetUserId());

            var completedIssues = _dbContext.Issues.Where(x => x.TechnicianId == user.Id && x.IssueState == IssueStates.Tamamlandi).ToList();

            return View(completedIssues);
        }


        public IActionResult AcceptIssue(Guid id)
        {
            var issue = _dbContext.Issues.Find(id);
            issue.IssueState = IssueStates.Islemde;
            _dbContext.SaveChanges();

            return RedirectToAction("AcceptedIssues");
        }

        [HttpGet]
        public async Task<IActionResult> AcceptedIssues()
        {
            var user = await _userManager.FindByIdAsync(HttpContext.GetUserId());

            var acceptedIssues = _dbContext.Issues.Where(x => x.TechnicianId == user.Id &&
                x.IssueState == IssueStates.Islemde)
                .ToList();

            return View(acceptedIssues);
        }

        [HttpGet]
        public IActionResult IssueDetail(Guid id)
        {
            TempData["IssueId"] = id;

            var products = _dbContext.Products.ToList();

            var productsInIssue = _dbContext.IssueProducts
                .Include(x=>x.Product)
                .Where(x => x.IssueId == id)
                .ToList();

            IssueDetailViewModel model = new IssueDetailViewModel()
            {
                IssueProducts = productsInIssue,
                Products = products
            };


            return View(model);
        }

        
        public IActionResult AddProduct(Guid id)
        {
            var issueid = TempData["IssueId"];
            var issue = _dbContext.Issues.Find(issueid);
            var product = _dbContext.Products.Find(id);

            var productsInIssue = _dbContext.IssueProducts
                .Include(x => x.Product)
                .Where(x => x.IssueId == issue.Id)
                .ToList();


            var control = productsInIssue.SingleOrDefault(x=>x.ProductId == product.Id);
            if(control == null)
            {
                //Ürün yok
                IssueProducts newProduct = new IssueProducts()
                {
                    IssueId = issue.Id,
                    ProductId = product.Id,
                    Quantity = 1,
                    Price = product.Price
                };
                _dbContext.IssueProducts.Add(newProduct);
            }
            else
            {
                control.Quantity++;
                control.Price = control.Quantity * product.Price;
            }

            _dbContext.SaveChanges();

            return RedirectToAction("IssueDetail", new { id=issueid });
        }

        [HttpGet]
        public IActionResult DeleteProduct(Guid id)
        {
            var issueid = TempData["IssueId"];
            var issue = _dbContext.Issues.Find(issueid);
            var control = _dbContext.IssueProducts
                .SingleOrDefault(x => x.ProductId == id && x.IssueId == issue.Id);

            if(control == null)
            {
                return RedirectToAction("IssueDetail", new { id = issueid });
            }
            else
            {
                _dbContext.IssueProducts.Remove(control);
                _dbContext.SaveChanges();
            }

            return RedirectToAction("IssueDetail", new { id = issueid });

        }

        [HttpGet]
        public IActionResult CompleteIssue()
        {
            var issueid = TempData["IssueId"];
            var issue = _dbContext.Issues.Find(issueid);
            issue.IssueState = IssueStates.Tamamlandi;
            _dbContext.SaveChanges();

            return RedirectToAction("CompletedIssues");

        }
    }
}
