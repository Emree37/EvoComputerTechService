using AutoMapper;
using EvoComputerTechService.Data;
using EvoComputerTechService.Extensions;
using EvoComputerTechService.Models.Identity;
using EvoComputerTechService.Models.Payment;
using EvoComputerTechService.Services;
using EvoComputerTechService.ViewModels;
using Iyzipay.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public PaymentController(IPaymentService paymentService, MyContext dbContext, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _paymentService = paymentService;
            _dbContext = dbContext;
            _mapper = mapper;
            _userManager = userManager;
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

            //Burada price 1000di...
            var result = _paymentService.CheckInstallments(binNumber, price);
            return Ok(result);
        }

        [Authorize]
        public IActionResult Purchase(Guid id)//satın alma işlemi için
        {
            var data = _dbContext.Issues.Find(id);
            if(data == null)
            {
                //Doldur...
                return RedirectToAction("");
            }

            //var model = _mapper.Map<SubscriptionTypeViewModel>(data);
            ViewBag.Subs = data;

            //var model2 = new PaymentViewModel()
            //{
            //    BasketModel = new BasketModel()
            //    {
            //        Category1 = data.Name,
            //        ItemType = BasketItemType.VIRTUAL.ToString(),
            //        Id = data.Id.ToString(),
            //        Name = data.Name,
            //        Price = data.Price.ToString(new CultureInfo("en-us"))
            //    }
            //};

            //return View(model2);
            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> Purchase(PaymentViewModel model)
        //{
        //    var type = _dbContext.SubscriptionTypes.Find(Guid.Parse(model.BasketModel.Id));
        //    var basketModel = new BasketModel()
        //    {
        //        Category1 = type.Name,
        //        ItemType = BasketItemType.VIRTUAL.ToString(),
        //        Id = type.Id.ToString(),
        //        Name = type.Name,
        //        Price = type.Price.ToString(new CultureInfo("en-us"))
        //    };



        //    var data2 = _dbContext.SubscriptionTypes.Find(Guid.Parse(basketModel.Id));
        //    var model2 = _mapper.Map<SubscriptionTypeViewModel>(data2);
        //    ViewBag.Subs = model2;
        //    var addresses = _dbContext.Addresses.Where(x => x.UserId == HttpContext.GetUserId())
        //        .ToList()
        //        .Select(x => _mapper.Map<AddressViewModel>(x))
        //        .ToList();

        //    ViewBag.Addresses = addresses;




        //    var user = await _userManager.FindByIdAsync(HttpContext.GetUserId());

        //    var address = _dbContext.Addresses
        //        .Include(x => x.State.City)
        //        .First(x => x.Id == Guid.Parse(model.AddressModel.Id));

        //    var addressModel = new AddressModel()
        //    {
        //        City = address.State.City.Name,
        //        ContactName = $"{user.Name} {user.Surname}",
        //        Country = "Türkiye",
        //        Description = address.Line,
        //        ZipCode = address.PostCode
        //    };

        //    var customerModel = new CustomerModel()
        //    {
        //        City = address.State.City.Name,
        //        Country = "Turkiye",
        //        Email = user.Email,
        //        GsmNumber = user.PhoneNumber,
        //        Id = user.Id,
        //        IdentityNumber = user.Id,
        //        Ip = Request.HttpContext.Connection.RemoteIpAddress?.ToString(),
        //        Name = user.Name,
        //        Surname = user.Surname,
        //        ZipCode = addressModel.ZipCode,
        //        LastLoginDate = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",
        //        RegistrationDate = $"{user.CreatedDate:yyyy-MM-dd HH:mm:ss}",
        //        RegistrationAddress = address.Line
        //    };

        //    var paymentModel = new PaymentModel()
        //    {
        //        Installment = model.Installment,
        //        Address = addressModel,
        //        BasketList = new List<BasketModel>() { basketModel },
        //        Customer = customerModel,
        //        CardModel = model.CardModel,
        //        Price = model.Amount,
        //        UserId = HttpContext.GetUserId(),
        //        Ip = Request.HttpContext.Connection.RemoteIpAddress?.ToString(),

        //    };

        //    var installmentInfo = _paymentService.CheckInstallments(paymentModel.CardModel.CardNumber.Substring(0, 6), paymentModel.Price);

        //    var installmentNumber = installmentInfo.InstallmentPrices.FirstOrDefault(x => x.InstallmentNumber == model.Installment);

        //    paymentModel.PaidPrice = decimal.Parse(installmentNumber != null ? installmentNumber.TotalPrice : installmentInfo.InstallmentPrices[0].TotalPrice);

        //    var result = _paymentService.Pay(paymentModel);
        //    return View();
        //}
    }
}
