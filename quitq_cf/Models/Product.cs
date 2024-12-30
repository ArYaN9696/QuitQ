using System;
using System.Collections.Generic;

namespace quitq_cf.Models
{
    public partial class Product
    {
        public Product()
        {
            Carts = new HashSet<Cart>();
            OrderDetails = new HashSet<OrderDetail>();
            StockInfos = new HashSet<StockInfo>();
        }

        public int ProductId { get; set; }
        public string? SellerId { get; set; }
        public int? SubcategoryId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }

        public virtual Seller? Seller { get; set; }
        public virtual SubCategory? Subcategory { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<StockInfo> StockInfos { get; set; }
    }
}
