using Shop.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.DomainModels
{
    public class Order : BaseEntity
    {
        public string UserId { get; set; }

        public ShopApplicationUser User { get; set; }

        public virtual ICollection<ComicInOrder> ComicInOrders { get; set; }
    }
}
