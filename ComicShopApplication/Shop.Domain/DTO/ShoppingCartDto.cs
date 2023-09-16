using Shop.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.DTO
{
    public class ShoppingCartDto
    {
        public List<ComicInShoppingCart> Comics { get; set; }

        public double TotalPrice { get; set; }
    }
}
