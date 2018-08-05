using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Plural.Data;
using Plural.Data.Entities;
using Plural.ViewModels;

namespace Plural.Controllers
{
    [Produces("application/json")]
    [Route("api/[Controller]")]
    [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
    public class OrdersController : Controller
    {
        private readonly IPluralRepository _repository;
        private readonly ILogger<OrdersController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<StoreUser> _userManager;
        public OrdersController(IPluralRepository repository, 
            ILogger<OrdersController> logger,
            IMapper mapper, UserManager<StoreUser> userManager
            )
        {
            _userManager = userManager;
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public IActionResult Get(bool includeItems = true)
        {
            try
            {
                var username = User.Identity.Name;
                var results = _repository.GetAllOrdersByUser(username, includeItems);
                return Ok(_mapper.Map<IEnumerable<Order>, IEnumerable<OrderViewModel>>(results));
            }
            catch (Exception)
            {
                _logger.LogError("ERROR ORDERS");
                return BadRequest("ORDERS BAD");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]OrderViewModel model) // FROMBODY MEANS NOT FROM QUERY STRING
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var newOrder = _mapper.Map<OrderViewModel, Order>(model);
                    if(newOrder.OrderDate == DateTime.MinValue)
                    {
                        newOrder.OrderDate = DateTime.Now;
                    }
                    var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
                    newOrder.User = currentUser;
                    _repository.AddEntity(newOrder);
                    if (_repository.SaveAll())
                    {
                        return Created($"/api/orders/{newOrder.Id}", _mapper.Map<Order, OrderViewModel>(newOrder));
                    }
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception)
            {
                _logger.LogError("ERROR ORDERS");
            }
            return BadRequest("FAILED TO SAVE");
        }

        [HttpGet("{id:int}")]
        public IActionResult GET(int id)
        {
            try
            {
                var order = _repository.GetOrderById(User.Identity.Name, id);
                if (order != null)
                {
                    return Ok(_mapper.Map<Order, OrderViewModel>(order));
                } else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                _logger.LogError("ERROR ORDERS");
                return BadRequest("ORDERS BAD");
            }
        }
    }
}