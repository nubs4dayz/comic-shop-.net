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
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IRepository<ShoppingCart> _shoppingCartRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<ComicInOrder> _comicInOrderRepository;
        private readonly IUserRepository _userRepository;

        public ShoppingCartService(IRepository<ShoppingCart> shoppingCartRepository, IUserRepository userRepository, IRepository<Order> orderRepository, IRepository<ComicInOrder> comicInOrderRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _comicInOrderRepository = comicInOrderRepository;
        }


        public bool deleteComicFromShoppingCart(string userId, Guid comicId)
        {
            if (!string.IsNullOrEmpty(userId) && comicId != null)
            {
                var loggedInUser = this._userRepository.Get(userId);

                var userShoppingCart = loggedInUser.UserCart;

                var itemToDelete = userShoppingCart.ComicInShoppingCart.Where(z => z.ComicId.Equals(comicId)).FirstOrDefault();

                userShoppingCart.ComicInShoppingCart.Remove(itemToDelete);

                this._shoppingCartRepository.Update(userShoppingCart);

                return true;
            }
            return false;
        }

        public ShoppingCartDto getShoppingCartInfo(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var loggedInUser = this._userRepository.Get(userId);

                var userCart = loggedInUser.UserCart;

                var allComics = userCart.ComicInShoppingCart.ToList();

                var allComicPrices = allComics.Select(z => new
                {
                    ComicPrice = z.CurrentComic.ComicPrice,
                    Quantity = z.Quantity
                }).ToList();

                double totalPrice = 0.0;

                foreach (var item in allComicPrices)
                {
                    totalPrice += item.Quantity * item.ComicPrice;
                }

                var result = new ShoppingCartDto
                {
                    Comics = allComics,
                    TotalPrice = totalPrice
                };

                return result;
            }
            return new ShoppingCartDto();
        }

        public bool order(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var loggedInUser = this._userRepository.Get(userId);
                var userCart = loggedInUser.UserCart;

                Order order = new Order
                {
                    Id = Guid.NewGuid(),
                    User = loggedInUser,
                    UserId = userId
                };

                this._orderRepository.Insert(order);

                List<ComicInOrder> comicInOrders = new List<ComicInOrder>();

                var result = userCart.ComicInShoppingCart.Select(z => new ComicInOrder
                {
                    Id = Guid.NewGuid(),
                    ComicId = z.CurrentComic.Id,
                    Comic = z.CurrentComic,
                    OrderId = order.Id,
                    Order = order,
                    Quantity = z.Quantity
                }).ToList();

                comicInOrders.AddRange(result);

                foreach (var item in comicInOrders)
                {
                    this._comicInOrderRepository.Insert(item);
                }

                loggedInUser.UserCart.ComicInShoppingCart.Clear();

                this._userRepository.Update(loggedInUser);

                return true;
            }

            return false;
        }
    }
}
