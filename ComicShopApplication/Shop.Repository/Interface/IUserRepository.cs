using Shop.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Repository.Interface
{
    public interface IUserRepository
    {
        IEnumerable<ShopApplicationUser> GetAll();
        ShopApplicationUser Get(string id);
        void Insert(ShopApplicationUser entity);
        void Update(ShopApplicationUser entity);
        void Delete(ShopApplicationUser entity);
    }
}
