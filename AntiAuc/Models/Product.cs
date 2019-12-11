using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AntiAuc.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public float CurrentPrice { get; set; }
        public bool IsAvailable { get; set; } = true;
        public DateTime DateOfCreation { get; set; }
        public DateTime DateOfEnding { get; set; }
        public string Image { get; set; }
        public List<UserProduct> Users { get; set; }
        public Product()
        {
            Users = new List<UserProduct>();
        }
    }
}
