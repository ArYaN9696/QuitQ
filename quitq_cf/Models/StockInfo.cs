using System;
using System.Collections.Generic;

namespace quitq_cf.Models
{
    public partial class StockInfo
    {
        public int StockId { get; set; }
        public int? ProductId { get; set; }
        public string? SellerLocation { get; set; }
        public int Quantity { get; set; }

        public virtual Product? Product { get; set; }
    }
}
