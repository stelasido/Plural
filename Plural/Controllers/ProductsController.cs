using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Plural.Data;
using Plural.Data.Entities;

namespace Plural.Controllers
{
    [Produces("application/json")]
    [Route("api/[Controller]")]
    public class ProductsController : Controller
    {
        private readonly IPluralRepository _repository;
        private readonly ILogger<ProductsController> _logger;
        public ProductsController(IPluralRepository repository, ILogger<ProductsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get() // CONTENT NEGOTIATION W WILL RETURN WAT CALLER FORMAT WANTS WITH Ok AND IActionResult
        {
            try
            {
                return Ok(_repository.GetAllProducts());
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.ToString());
                return BadRequest("Failed");
            }
            
        }
    }
}