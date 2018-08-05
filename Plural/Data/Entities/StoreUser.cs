using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plural.Data.Entities
{
    public class StoreUser : IdentityUser
    {
        // WE WILL GET ALL PROPS FOR INDENTITYUSER + THESE
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
