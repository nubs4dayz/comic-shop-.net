using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.DomainModels
{
    public class Comic : BaseEntity
    {
        [Required]
        public string ComicName { get; set; }

        [Required]
        public string ComicCover { get; set; }

        [Required]
        public string ComicDescription { get; set; }

        [Required]
        public string ComicGenre { get; set; }

        [Required]
        public string ComicAuthor { get; set; }

        [Required]
        public string ComicPublisher { get; set; }

        [Required]
        public double ComicPrice { get; set; }

        public virtual ICollection<ComicInShoppingCart>? ComicInShoppingCart { get; set; }

        public virtual ICollection<ComicInOrder>? ComicInOrders { get; set; }
    }
}
