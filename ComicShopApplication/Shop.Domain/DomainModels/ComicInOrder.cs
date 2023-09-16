using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.DomainModels
{
    public class ComicInOrder : BaseEntity
    {
        public Guid OrderId { get; set; }

        public Order Order { get; set; }

        public Guid ComicId { get; set; }

        public Comic Comic { get; set; }

        public int Quantity { get; set; }
    }
}
