using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plural.Data;
using Plural.Services;
using Plural.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plural.Controllers
{
    public class AppController : Controller
    {
        // INJECT THE SERVICE
        private readonly IMailService _mailService;
        private readonly IPluralRepository _repository;

        public AppController(IMailService mailService, IPluralRepository repository)
        {
            _mailService = mailService;
            _repository = repository;
        }

        [Authorize]
        public IActionResult Shop()
        {
            return View();
        }
        
        public IActionResult Index()
        {

            return View();
        }

        [HttpGet("contact")]
        public IActionResult Contact()
        {
            ViewBag.Title = "Contact Us";

            return View();
        }
        [HttpPost("contact")]
        public IActionResult Contact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                _mailService.SendMessage("vsi1@abv.bg", "SUBJECT", $"From: {model.Email}, Message: {model.Message}");
                ViewBag.UserMessage = "Mail send";
                // CLEAR THE FORM
                ModelState.Clear();
            } 
           
            return View();
        }
        [HttpGet("about")]
        public IActionResult About()
        {
            ViewBag.Title = "About";

            return View();
        }
    }
    
}
