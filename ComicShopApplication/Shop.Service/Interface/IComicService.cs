using Shop.Domain.DomainModels;
using Shop.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Service.Interface
{
    public interface IComicService
    {
        List<Comic> GetAllComics();
        Comic GetDetailsForComic(Guid? id);
        void CreateNewComic(Comic c);
        void UpdateExistingComic(Comic c);
        AddToShoppingCartDto GetShoppingCartInfo(Guid? id);
        void DeleteComic(Guid id);
        bool AddToShoppingCart(AddToShoppingCartDto item, string userID);
    }
}
