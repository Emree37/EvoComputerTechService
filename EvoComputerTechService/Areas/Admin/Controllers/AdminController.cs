using EvoComputerTechService.Data;
using EvoComputerTechService.Models.Entities;
using EvoComputerTechService.Models.Identity;
using EvoComputerTechService.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EvoComputerTechService.Areas.Admin.Controllers
{
    public class AdminController : AdminBaseController
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly MyContext _dbContext;

        public AdminController(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, MyContext dbContext)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Users()
        {
            return View();
        }

        public IActionResult Products()
        {
            return View();
        }

        public IActionResult GetProducts()
        {
            var products = _dbContext.Products.Where(x => x.IsDeleted == false).ToList();

            return View(products);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(Product model,IFormFile file)
        {
            var product = new Product()
            {
                ProductName = model.ProductName,
                ProductDescription = model.ProductDescription,
                Price = model.Price
            };

            if (file != null)
            {
                string imageExtension = Path.GetExtension(file.FileName);

                string imageName = Guid.NewGuid() + imageExtension;

                //string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/images/{imageName}");

                string path = $"wwwroot/images/{imageName}";

                using var stream = new FileStream(path, FileMode.Create);

                await file.CopyToAsync(stream);

                product.ProductPicture = path;
            }

            _dbContext.Products.Add(product);
            _dbContext.SaveChanges();

            return RedirectToAction("GetProducts");
        }

        
        public IActionResult FindProduct(Guid id)
        {
            var product = _dbContext.Products.Find(id);
            return Json(product);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProduct(Product model,IFormFile file)
        {
            Product product = _dbContext.Products.Find(model.Id);
            product.ProductName = model.ProductName;
            product.ProductDescription = model.ProductDescription;
            product.Price = model.Price;

            if (file != null)
            {
                string imageExtension = Path.GetExtension(file.FileName);

                string imageName = Guid.NewGuid() + imageExtension;

                //string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/images/{imageName}");

                string path = $"wwwroot/images/{imageName}";

                using var stream = new FileStream(path, FileMode.Create);

                await file.CopyToAsync(stream);

                product.ProductPicture = path;
            }

            _dbContext.SaveChanges();
            
            return RedirectToAction("GetProducts");
        }

        [HttpPost]
        public IActionResult DeleteProduct(Guid id)
        {
            var product = _dbContext.Products.Find(id);
            product.IsDeleted = true;

            _dbContext.SaveChanges();

            return RedirectToAction("GetProducts");
        }


        public async Task<IActionResult> RoleAssign(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            List<ApplicationRole> allRoles = _roleManager.Roles.ToList();
            List<string> userRoles = await _userManager.GetRolesAsync(user) as List<string>;
            List<RoleAssignViewModel> assignRoles = new List<RoleAssignViewModel>();
            allRoles.ForEach(role => assignRoles.Add(new RoleAssignViewModel
            {
                HasAssign = userRoles.Contains(role.Name),
                RoleId = role.Id,
                RoleName = role.Name
            }));

            return View(assignRoles);
        }
        [HttpPost]
        public async Task<ActionResult> RoleAssign(List<RoleAssignViewModel> modelList, string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            foreach (RoleAssignViewModel role in modelList)
            {
                if (role.HasAssign)
                    await _userManager.AddToRoleAsync(user, role.RoleName);
                else
                    await _userManager.RemoveFromRoleAsync(user, role.RoleName);
            }
            return RedirectToAction("Users", "Admin");
        }
    }
}
