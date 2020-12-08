using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace NorthwindConsole.Model
{
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        [Required(ErrorMessage = "CategoryName cannot be null!")]
        public string Description { get; set; }
        
        public virtual ICollection<Product> Products { get; set; }
    }
}
