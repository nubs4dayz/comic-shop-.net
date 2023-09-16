using Microsoft.AspNetCore.Identity;
using Shop.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.Identity
{
    public class ShopApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public virtual ShoppingCart UserCart { get; set; }
    }
}
