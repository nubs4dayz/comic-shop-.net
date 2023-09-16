using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.DomainModels
{
    public class ComicInShoppingCart : BaseEntity
    {
        public Guid ComicId { get; set; }

        public virtual Comic CurrentComic { get; set; }

        public Guid ShoppingCartId { get; set; }

        public virtual ShoppingCart UserCart { get; set; }

        public int Quantity { get; set; }
    }
}
