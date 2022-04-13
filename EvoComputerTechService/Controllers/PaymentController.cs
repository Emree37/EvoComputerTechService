using AutoMapper;
using EvoComputerTechService.Data;
using EvoComputerTechService.Extensions;
using EvoComputerTechService.Models;
using EvoComputerTechService.Models.Entities;
using EvoComputerTechService.Models.Identity;
using EvoComputerTechService.Models.Payment;
using EvoComputerTechService.Services;
using EvoComputerTechService.ViewModels;
using Iyzipay.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EvoComputerTechService.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly MyContext _dbContext;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private decimal totalPrice;

        public PaymentController(IPaymentService paymentService, MyContext dbContext, IMapper mapper, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _paymentService = paymentService;
            _dbContext = dbContext;
            _mapper = mapper;
            _userManager = userManager;
            _emailSender = emailSender;
            var cultureInfo = CultureInfo.GetCultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
        }

        
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Index(PaymentViewModel model)
        {
            var paymentModel = new PaymentModel()
            {
                Installment = model.Installment,
                Address = new AddressModel(),
                BasketList = new List<BasketModel>(),
                Customer = new CustomerModel(),
                CardModel = model.CardModel,
                Price = 1000,
                UserId = HttpContext.GetUserId(),
                Ip = Request.HttpContext.Connection.RemoteIpAddress?.ToString(),
            };

            var installmentInfo = _paymentService.CheckInstallments(paymentModel.CardModel.CardNumber.Substring(0, 6), paymentModel.Price);

            var installmentNumber =
                installmentInfo.InstallmentPrices.FirstOrDefault(x => x.InstallmentNumber == model.Installment);

            paymentModel.PaidPrice = decimal.Parse(installmentNumber != null ? installmentNumber.TotalPrice.Replace('.', ',') : installmentInfo.InstallmentPrices[0].TotalPrice.Replace('.', ','));

            var result = _paymentService.Pay(paymentModel);
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult CheckInstallment(string binNumber, decimal price)
        {
            if (binNumber.Length < 6 || binNumber.Length > 16)
                return BadRequest(new
                {
                    Message = "Bad req."
                });

            var result = _paymentService.CheckInstallments(binNumber, price);
            return Ok(result);
        }




        [Authorize]
        public IActionResult Purchase(Guid id)//satın alma işlemi için
        {
            var issue = _dbContext.Issues.Find(id);
            if(issue == null)
            {
                //Doldur...
                return RedirectToAction("");
            }

            var productsInIssue = _dbContext.IssueProducts
                .Include(x => x.Product)
                .Where(x => x.IssueId == issue.Id)
                .ToList();

            foreach (var item in productsInIssue)
            {
                totalPrice += item.Price;
            }
            ViewBag.TotalPrice = totalPrice;

            var model2 = new PaymentViewModel()
            {
                BasketModel = new BasketModel()
                {
                    Category1 = issue.IssueName,
                    ItemType = BasketItemType.VIRTUAL.ToString(),
                    Id = issue.Id.ToString(),
                    Name = issue.IssueName,
                    Price = totalPrice.ToString(new CultureInfo("en-us")),
                    IssueProducts = productsInIssue
                }
            };

            return View(model2);
        }

        [HttpPost]
        public async Task<IActionResult> Purchase(PaymentViewModel model)
        {
            var issue = _dbContext.Issues.Find(Guid.Parse(model.BasketModel.Id));
            if (issue == null)
            {
                //Doldur...
                return RedirectToAction("");
            }

            var productsInIssue = _dbContext.IssueProducts
                .Include(x => x.Product)
                .Where(x => x.IssueId == issue.Id)
                .ToList();

            foreach (var item in productsInIssue)
            {
                totalPrice += item.Price;
            }
            ViewBag.TotalPrice = totalPrice;

            
            var basketModel = new BasketModel()
            {
                Category1 = issue.IssueName,
                ItemType = BasketItemType.VIRTUAL.ToString(),
                Id = issue.Id.ToString(),
                Name = issue.IssueName,
                Price = totalPrice.ToString(new CultureInfo("en-us")),
                IssueProducts = productsInIssue
            };


            var user = await _userManager.FindByIdAsync(HttpContext.GetUserId());


            var addressModel = new AddressModel()
            {
                City = "İstanbul",
                ContactName = $"{user.Name} {user.Surname}",
                Country = "Türkiye",
                Description = "Açıklama",
                ZipCode = "34000"
            };

            var customerModel = new CustomerModel()
            {
                City = "İstanbul",
                Country = "Turkiye",
                Email = user.Email,
                GsmNumber = user.PhoneNumber,
                Id = user.Id,
                IdentityNumber = user.Id,
                Ip = Request.HttpContext.Connection.RemoteIpAddress?.ToString(),
                Name = user.Name,
                Surname = user.Surname,
                ZipCode = addressModel.ZipCode,
                LastLoginDate = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                RegistrationDate = $"{user.CreatedDate:yyyy-MM-dd HH:mm:ss}",
                RegistrationAddress = "Adres"
            };

            var paymentModel = new PaymentModel()
            {
                Installment = model.Installment,
                Address = addressModel,
                BasketList = new List<BasketModel>() { basketModel },
                Customer = customerModel,
                CardModel = model.CardModel,
                Price = model.Amount,
                UserId = HttpContext.GetUserId(),
                Ip = Request.HttpContext.Connection.RemoteIpAddress?.ToString(),

            };

            var installmentInfo = _paymentService.CheckInstallments(paymentModel.CardModel.CardNumber.Substring(0, 6), paymentModel.Price);

            var installmentNumber = installmentInfo.InstallmentPrices.FirstOrDefault(x => x.InstallmentNumber == model.Installment);

            paymentModel.PaidPrice = decimal.Parse(installmentNumber != null ? installmentNumber.TotalPrice : installmentInfo.InstallmentPrices[0].TotalPrice);

            var result = _paymentService.Pay(paymentModel);

            if (result.Status == "success")
            {
                issue.IssueState = IssueStates.Odendi;
                _dbContext.SaveChanges();

                //MAİL AT ÖDENDİĞİNE DAİR...
                var emailMessage = new EmailMessage()
                {
                    //Contacts = new string[] { user.Email },
                    Contacts = new string[] { "manifestationoffate@gmail.com" },
                    Body = $"Merhaba {user.Name} {user.Surname}, <br/> -{issue.IssueName}- İsimli Arıza Kaydınıza Ait Ödemeniz Başarılı Bir Şekilde Gerçekleşmiştir.",
                    Subject = "Başarılı Ödeme"
                };

                await _emailSender.SendAsync(emailMessage);

            }
            
            return RedirectToAction("GetIssues","Issue");
        }
    }
}
