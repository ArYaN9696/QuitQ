using AutoMapper;
using Microsoft.EntityFrameworkCore;
using quitq_cf.Data;
using quitq_cf.DTO;
using quitq_cf.Models;
using quitq_cf.Repository;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    

    public OrderService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    //public async Task<OrderDTO> CreateOrderAsync(string userId, CreateOrderDTO orderDto)
    //{
    //    using var transaction = await _context.Database.BeginTransactionAsync();
    //    try
    //    {
    //        var cartItems = await _context.Carts
    //            .Include(c => c.Product)
    //            .Where(c => c.UserId == userId)
    //            .ToListAsync();

    //        if (!cartItems.Any())
    //        {
    //            throw new Exception("Cart is empty");
    //        }

    //        decimal totalAmount = cartItems.Sum(ci => ci.Product.Price * ci.Quantity);

    //        var order = new Order
    //        {
    //            UserId = userId,
    //            TotalAmount = totalAmount,
    //            ShippingAddress = orderDto.ShippingAddress,
    //            StatusId = 1
    //        };

    //        await _context.Orders.AddAsync(order);
    //        await _context.SaveChangesAsync();

    //        var orderDetails = cartItems.Select(ci => new OrderDetail
    //        {
    //            OrderId = order.OrderId,
    //            ProductId = ci.ProductId,
    //            Quantity = ci.Quantity,
    //            Price = ci.Product.Price,
    //            Total = ci.Product.Price * ci.Quantity
    //        }).ToList();

    //        await _context.OrderDetails.AddRangeAsync(orderDetails);

    //        foreach (var item in cartItems)
    //        {
    //            item.Product.Stock -= item.Quantity;
    //            if (item.Product.Stock < 0)
    //            {
    //                throw new Exception($"Insufficient stock for product: {item.Product.ProductName}");
    //            }
    //        }

    //        _context.Carts.RemoveRange(cartItems);
    //        await _context.SaveChangesAsync();
    //        await transaction.CommitAsync();

    //        var orderDtoResponse = _mapper.Map<OrderDTO>(order);
    //        orderDtoResponse.OrderDetails = _mapper.Map<List<OrderDetailDTO>>(orderDetails);

    //        return orderDtoResponse;
    //    }
    //    catch (Exception)
    //    {
    //        await transaction.RollbackAsync();
    //        throw;
    //    }
    //}

    //public async Task<OrderDTO> CreateOrderAsync(string userId, CreateOrderDTO orderDto)
    //{
    //    await using var transaction = await _context.Database.BeginTransactionAsync();
    //    try
    //    {
    //        var cartItems = await _context.Carts
    //        .Include(c => c.Product)
    //        .Where(c => c.UserId == userId)
    //        .ToListAsync();


    //        if (!cartItems.Any())
    //        {
    //            throw new InvalidOperationException("Cart is empty");
    //        }

    //        var productIds = cartItems.Select(c => c.ProductId).ToList();
    //        var productsForUpdate = await _context.Products
    //            .Where(p => productIds.Contains(p.ProductId))
    //            .ToListAsync();

    //        foreach (var item in cartItems)
    //        {
    //            var product = productsForUpdate.First(p => p.ProductId == item.ProductId);
    //            if (product.Stock < item.Quantity)
    //            {
    //                throw new InvalidOperationException($"Insufficient stock for product: {product.ProductName}");
    //            }
    //        }

    //        var order = new Order
    //        {
    //            UserId = userId,
    //            // OrderDate = DateTime.UtcNow,
    //            TotalAmount = cartItems.Sum(ci => ci.Product.Price * ci.Quantity),
    //            ShippingAddress = orderDto.ShippingAddress,
    //            PaymentMethod = orderDto.PaymentMethod,
    //            StatusId = 1,
    //            PaymentDate = DateTime.UtcNow
    //        };

    //        _context.Orders.Add(order);

    //        var orderDetails = cartItems.Select(ci => new OrderDetail
    //        {
    //            OrderId = order.OrderId,  
    //            ProductId = ci.ProductId,
    //            Quantity = ci.Quantity,
    //            Price = ci.Product.Price,
    //            Total = ci.Product.Price * ci.Quantity
    //        }).ToList();

    //        _context.OrderDetails.AddRange(orderDetails);

    //        foreach (var item in cartItems)
    //        {
    //            var product = productsForUpdate.First(p => p.ProductId == item.ProductId);
    //            product.Stock -= item.Quantity;
    //            _context.Products.Update(product);
    //        }

    //        _context.Carts.RemoveRange(cartItems);

    //        await _context.SaveChangesAsync();
    //        await transaction.CommitAsync();

    //        var orderDtoResponse = _mapper.Map<OrderDTO>(order);
    //        orderDtoResponse.OrderDetails = _mapper.Map<List<OrderDetailDTO>>(orderDetails);

    //        return orderDtoResponse;
    //    }
    //    catch (Exception ex)
    //    {
    //        await transaction.RollbackAsync();
    //        Console.WriteLine($"Error creating order: {ex.Message}");
    //        if (ex.InnerException != null)
    //        {
    //            Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
    //        }
    //        throw new Exception($"Error creating order: {ex.Message}");
    //    }



    //}

    public async Task<OrderDTO> CreateOrderAsync(string userId, CreateOrderDTO orderDto)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var cartItems = await _context.Carts
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                throw new InvalidOperationException("Cart is empty");
            }

            var productIds = cartItems.Select(c => c.ProductId).ToList();
            var productsForUpdate = await _context.Products
                .Where(p => productIds.Contains(p.ProductId))
                .ToListAsync();

            foreach (var item in cartItems)
            {
                var product = productsForUpdate.First(p => p.ProductId == item.ProductId);
                if (product.Stock < item.Quantity)
                {
                    throw new InvalidOperationException($"Insufficient stock for product: {product.ProductName}");
                }
            }

            var order = new Order
            {
                UserId = userId,
                TotalAmount = cartItems.Sum(ci => ci.Product.Price * ci.Quantity),
                ShippingAddress = orderDto.ShippingAddress,
                PaymentMethod = orderDto.PaymentMethod,
                StatusId = 1,
                PaymentDate = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var orderDetails = cartItems.Select(ci => new OrderDetail
            {
                OrderId = order.OrderId,
                ProductId = ci.ProductId,
                Quantity = ci.Quantity,
                Price = ci.Product.Price,
                Total = ci.Product.Price * ci.Quantity
            }).ToList();

            _context.OrderDetails.AddRange(orderDetails);

            foreach (var item in cartItems)
            {
                var product = productsForUpdate.First(p => p.ProductId == item.ProductId);
                product.Stock -= item.Quantity;
                _context.Products.Update(product);
            }

            _context.Carts.RemoveRange(cartItems);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            var orderDtoResponse = _mapper.Map<OrderDTO>(order);
            orderDtoResponse.OrderDetails = _mapper.Map<List<OrderDetailDTO>>(orderDetails);

            return orderDtoResponse;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error creating order: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            throw new Exception($"Error creating order: {ex.Message}");
        }
    }

    public async Task<OrderDTO> GetOrderByIdAsync(int orderId)
    {
        var order = await _context.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);
        return _mapper.Map<OrderDTO>(order);
    }

    public async Task<IEnumerable<OrderDTO>> GetUserOrdersAsync(string userId)
    {
        var orders = await _context.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product)
            .Where(o => o.UserId == userId)
            .ToListAsync();

        foreach (var order in orders)
        {
            Console.WriteLine(order.PaymentDate?.ToString("yyyy-MM-dd") ?? "Payment date not available");
        }


        return _mapper.Map<IEnumerable<OrderDTO>>(orders);
    }


    public async Task<IEnumerable<OrderDTO>> GetSellerOrdersAsync(string sellerId)
    {
        var orders = await _context.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product)
            .Where(o => o.OrderDetails.Any(od => od.Product.SellerId == sellerId))
            .ToListAsync();

        return _mapper.Map<IEnumerable<OrderDTO>>(orders);
    }

    public async Task<Response> UpdateOrderStatusAsync(int orderId, int statusId)
    {
        try
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return new Response
                {
                    Status = "Failed",
                    Message = "Order not found"
                };
            }

            order.StatusId = statusId;
            await _context.SaveChangesAsync();

            return new Response
            {
                Status = "Success",
                Message = "Order status updated successfully"
            };
        }
        catch (Exception ex)
        {
            return new Response
            {
                Status = "Failed",
                Message = $"Error updating order status: {ex.Message}"
            };
        }
    }

    public async Task<Response> CancelOrderAsync(int orderId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return new Response
                {
                    Status = "Failed",
                    Message = "Order not found"
                };
            }

            foreach (var detail in order.OrderDetails)
            {
                detail.Product.Stock += detail.Quantity;
            }

            order.StatusId = 5;
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new Response
            {
                Status = "Success",
                Message = "Order cancelled successfully"
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return new Response
            {
                Status = "Failed",
                Message = $"Error cancelling order: {ex.Message}"
            };
        }
    }

    public async Task<List<Order>> GetAllOrdersAsync()
    {
        try
        {
            return await _context.Orders
                         .Include(o => o.Status)
                         .ToListAsync();
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw new NotImplementedException();
        }
    }
}
