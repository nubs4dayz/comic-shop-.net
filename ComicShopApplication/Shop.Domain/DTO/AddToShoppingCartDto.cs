using Shop.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.DTO
{
    public class AddToShoppingCartDto
    {
        public Comic SelectedComic { get; set; }
        public Guid SelectedComicId { get; set; }
        public int Quantity { get; set; }
    }
}
