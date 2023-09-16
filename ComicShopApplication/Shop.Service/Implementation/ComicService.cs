using Shop.Domain.DomainModels;
using Shop.Domain.DTO;
using Shop.Repository.Interface;
using Shop.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Service.Implementation
{
    public class ComicService : IComicService
    {
        private readonly IRepository<Comic> _comicRepository;
        private readonly IRepository<ComicInShoppingCart> _comicInShoppingCartRepository;
        private readonly IUserRepository _userRepository;

        public ComicService(IRepository<Comic> comicRepository, IRepository<ComicInShoppingCart> comicInShoppingCartRepository, IUserRepository userRepository)
        {
            _comicRepository = comicRepository;
            _userRepository = userRepository;
            _comicInShoppingCartRepository = comicInShoppingCartRepository;
        }


        public bool AddToShoppingCart(AddToShoppingCartDto item, string userID)
        {
            var user = this._userRepository.Get(userID);

            var userShoppingCart = user.UserCart;

            if (item.SelectedComicId != null && userShoppingCart != null)
            {
                var comic = this.GetDetailsForComic(item.SelectedComicId);

                if (comic != null)
                {
                    ComicInShoppingCart itemToAdd = new ComicInShoppingCart
                    {
                        Id = Guid.NewGuid(),
                        CurrentComic = comic,
                        ComicId = comic.Id,
                        UserCart = userShoppingCart,
                        ShoppingCartId = userShoppingCart.Id,
                        Quantity = item.Quantity
                    };

                    var existing = userShoppingCart.ComicInShoppingCart.Where(z => z.ShoppingCartId == userShoppingCart.Id && z.ComicId == itemToAdd.ComicId).FirstOrDefault();

                    if (existing != null)
                    {
                        existing.Quantity += itemToAdd.Quantity;
                        this._comicInShoppingCartRepository.Update(existing);
                    }
                    else
                    {
                        this._comicInShoppingCartRepository.Insert(itemToAdd);
                    }
                    return true;
                }
                return false;
            }
            return false;
        }

        public void CreateNewComic(Comic c)
        {
            this._comicRepository.Insert(c);
        }

        public void DeleteComic(Guid id)
        {
            var comic = this.GetDetailsForComic(id);
            this._comicRepository.Delete(comic);
        }

        public List<Comic> GetAllComics()
        {
            return this._comicRepository.GetAll().ToList();
        }

        public Comic GetDetailsForComic(Guid? id)
        {
            return this._comicRepository.Get(id);
        }

        public AddToShoppingCartDto GetShoppingCartInfo(Guid? id)
        {
            var comic = this.GetDetailsForComic(id);
            AddToShoppingCartDto model = new AddToShoppingCartDto
            {
                SelectedComic = comic,
                SelectedComicId = comic.Id,
                Quantity = 1
            };

            return model;
        }

        public void UpdateExistingComic(Comic c)
        {
            this._comicRepository.Update(c);
        }
    }
}
