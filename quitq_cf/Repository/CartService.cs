using Microsoft.EntityFrameworkCore;
using quitq_cf.Data;
using quitq_cf.Models;
using quitq_cf.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace quitq_cf.Repository
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _appDBContext;

        public CartService(ApplicationDbContext appDBContext)
        {
            _appDBContext = appDBContext;
        }

        public async Task<Response> AddToCartAsync(string userId, CartItemDTO item)
        {
            try
            {
                var existingCartItem = await _appDBContext.Carts
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == item.ProductId);

                if (existingCartItem != null)
                {
                    existingCartItem.Quantity += item.Quantity;
                }
                else
                {
                    var newCartItem = new Cart
                    {
                        UserId = userId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    };
                    await _appDBContext.Carts.AddAsync(newCartItem);
                }

                await _appDBContext.SaveChangesAsync();
                return new Response { Status = "Success", Message = "Item added to cart successfully." };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new Response { Status = "Failure", Message = "Error occurred while adding item to cart." };
            }
        }

        public async Task<Response> ClearCartAsync(string userId)
        {
            try
            {
                var cartItems = _appDBContext.Carts.Where(c => c.UserId == userId);
                _appDBContext.Carts.RemoveRange(cartItems);

                await _appDBContext.SaveChangesAsync();
                return new Response { Status = "Success", Message = "Cart cleared successfully." };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new Response { Status = "Failure", Message = "Error occurred while clearing cart." };
            }
        }

        public async Task<IEnumerable<CartItemDTO>> GetCartItemsAsync(string userId)
        {
            try
            {
                var cartItems = await _appDBContext.Carts
                    .Where(c => c.UserId == userId)
                    .Join(
                        _appDBContext.Products,
                        cart => cart.ProductId,
                        product => product.ProductId,
                        (cart, product) => new CartItemDTO
                        {
                            ProductId = cart.ProductId,
                            Quantity = cart.Quantity,
                            ProductName = product.ProductName,
                            Price = product.Price // Fetch the price from the Product table
                        }
                    )
                    .ToListAsync();

                return cartItems;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while fetching cart items.", ex);
            }
        }



        public async Task<Response> RemoveFromCartAsync(string userId, int productId)
        {
            try
            {
                var cartItem = await _appDBContext.Carts
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

                if (cartItem == null)
                {
                    return new Response { Status = "Failure", Message = "Item not found in the cart." };
                }

                _appDBContext.Carts.Remove(cartItem);
                await _appDBContext.SaveChangesAsync();
                return new Response { Status = "Success", Message = "Item removed from cart successfully." };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new Response { Status = "Failure", Message = "Error occurred while removing item from cart." };
            }
        }

        public async Task<Response> UpdateCartItemAsync(string userId, CartItemDTO item)
        {
            try
            {
                var existingCartItem = await _appDBContext.Carts
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == item.ProductId);

                if (existingCartItem == null)
                {
                    return new Response { Status = "Failure", Message = "Item not found in the cart." };
                }

                existingCartItem.Quantity = item.Quantity;
                await _appDBContext.SaveChangesAsync();

                return new Response { Status = "Success", Message = "Cart item updated successfully." };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new Response { Status = "Failure", Message = "Error occurred while updating cart item." };
            }
        }
    }
}
