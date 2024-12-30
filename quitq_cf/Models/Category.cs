using System;
using System.Collections.Generic;

namespace quitq_cf.Models
{
    public partial class Category
    {
        public Category()
        {
            SubCategories = new HashSet<SubCategory>();
        }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;

        public virtual ICollection<SubCategory> SubCategories { get; set; }
    }
}
