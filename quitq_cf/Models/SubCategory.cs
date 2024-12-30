using System;
using System.Collections.Generic;

namespace quitq_cf.Models
{
    public partial class SubCategory
    {
        public SubCategory()
        {
            Products = new HashSet<Product>();
        }

        public int SubcategoryId { get; set; }
        public int? CategoryId { get; set; }
        public string SubcategoryName { get; set; } = null!;

        public virtual Category? Category { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
