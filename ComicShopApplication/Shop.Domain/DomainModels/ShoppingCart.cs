using Shop.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.DomainModels
{
    public class ShoppingCart : BaseEntity
    {
        public string OwnerId { get; set; }

        public virtual ShopApplicationUser Owner { get; set; }

        public virtual ICollection<ComicInShoppingCart> ComicInShoppingCart { get; set; }
    }
}
