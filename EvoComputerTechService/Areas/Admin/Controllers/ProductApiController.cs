using DevExtreme.AspNet.Data;
using EvoComputerTechService.Data;
using EvoComputerTechService.Extensions;
using EvoComputerTechService.Models.Entities;
using EvoComputerTechService.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvoComputerTechService.Areas.Admin.Controllers
{
    [Route("api/[controller]/[action]")]
    [Authorize(Roles = "Admin")]
    public class ProductApiController : ControllerBase
    {
        private readonly MyContext _dbContext;

        public ProductApiController(MyContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetProducts(DataSourceLoadOptions loadOptions)
        {
            var data = _dbContext.Products.Where(x=>x.IsDeleted == false);

            return Ok(DataSourceLoader.Load(data, loadOptions));
        }

        [HttpPut]
        public IActionResult UpdateProducts(Guid key, string values)
        {
            var data = _dbContext.Products.Find(key);
            if(data == null)
            {
                return BadRequest(new JSonResponseViewModel()
                {
                    IsSuccess = false,
                    ErrorMessage = ModelState.ToFullErrorString()
                });
            }

            JsonConvert.PopulateObject(values, data);
            if (!TryValidateModel(data))
                return BadRequest(ModelState.ToFullErrorString());

            var result = _dbContext.SaveChanges();
            if (result == 0)
                return BadRequest(new JSonResponseViewModel()
                {
                    IsSuccess = false,
                    ErrorMessage = "Ürün Güncellenemedi"
                });
            return Ok(new JSonResponseViewModel());
        }

        [HttpPost]
        public IActionResult InsertProduct(string values)
        {
            var data = new Product();
            JsonConvert.PopulateObject(values, data);

            if (!TryValidateModel(data))
                return BadRequest(new JSonResponseViewModel()
                {
                    IsSuccess = false,
                    ErrorMessage = ModelState.ToFullErrorString()
                });
            _dbContext.Products.Add(data);

            var result = _dbContext.SaveChanges();
            if (result == 0)
                return BadRequest(new JSonResponseViewModel
                {
                    IsSuccess = false,
                    ErrorMessage = "Yeni Ürün Kaydedilemedi."
                });
            return Ok(new JSonResponseViewModel());
        }

        [HttpDelete]
        public IActionResult DeleteProduct(Guid key)
        {
            var product = _dbContext.Products.Find(key);
            if(product == null)
            {
                return StatusCode(StatusCodes.Status409Conflict, "Ürün bulunamadı");
            }

            product.IsDeleted = true;
            var result = _dbContext.SaveChanges();
            if(result == 0)
            {
                return BadRequest("Silme işlemi başarısız");
            }

            return Ok(new JSonResponseViewModel());

        }
    }
}
