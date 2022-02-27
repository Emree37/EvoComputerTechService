using EvoComputerTechService.Data;
using EvoComputerTechService.Extensions;
using EvoComputerTechService.Models;
using EvoComputerTechService.Models.Entities;
using EvoComputerTechService.Models.Identity;
using EvoComputerTechService.Services;
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
        private readonly IEmailSender _emailSender;

        public TechnicianController(MyContext dbContext, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _emailSender = emailSender;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> MyIssues()
        {
            var user = await _userManager.FindByIdAsync(HttpContext.GetUserId());

            var myIssues = _dbContext.Issues.Where(x => x.TechnicianId == user.Id && (x.IssueState == IssueStates.Kuyrukta || x.IssueState == IssueStates.Islemde)).OrderBy(x=>x.CreatedDate).ToList();

            for (int i = 0; i < myIssues.Count; i++)
            {
                if(i == 0)
                {
                    myIssues[i].IssueState = IssueStates.Islemde;
                }
            }

            _dbContext.SaveChanges();

            return View(myIssues);
        }

        [HttpGet]
        public async Task<IActionResult> CompletedIssues()
        {
            var user = await _userManager.FindByIdAsync(HttpContext.GetUserId());

            var completedIssues = _dbContext.Issues.Where(x => x.TechnicianId == user.Id && x.IssueState == IssueStates.Tamamlandi).ToList();

            return View(completedIssues);
        }

        [HttpGet]
        public IActionResult IssueDetail(Guid id)
        {
            TempData["IssueId"] = id;
            var issue = _dbContext.Issues.Find(id);
            ViewBag.IssueState = issue.IssueState;


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
        public async Task<IActionResult> CompleteIssue()
        {
            var issueid = TempData["IssueId"];
            var issue = _dbContext.Issues.Find(issueid);
            issue.IssueState = IssueStates.Tamamlandi;
            _dbContext.SaveChanges();

            var user = _dbContext.Users.Find(issue.UserId);

            //Kullanıcıya Mail Gönderme
            var emailMessage = new EmailMessage()
            {
                //Contacts = new string[] { user.Email },
                Contacts = new string[] { "vedataydinkayaa@gmail.com" },
                Body = $"Arıza Kaydınıza Ait Ödeme.",
                Subject = "Arıza Kaydı Ödemesi"
            };

            await _emailSender.SendAsync(emailMessage);


            return RedirectToAction("CompletedIssues");

        }
    }
}
