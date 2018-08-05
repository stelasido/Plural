using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Plural.ViewModels
{
    public class ContactViewModel
    {
        [Required(ErrorMessage ="FK U REQUIRED")]
        [MinLength(2, ErrorMessage = "MIN LENGTH 2")]
        public string Name { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "FK U REQ"), MaxLength(100)]
        public string Message { get; set; }
    }
}
