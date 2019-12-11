using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AntiAuc.Models
{
    public class ProductModel
    {
        [Required(ErrorMessage = "* Enter product name")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "* Enter some words here")]
        [StringLength(30,MinimumLength = 5,ErrorMessage = "* Message's length 5 - 30 symbols")]
        public string ShortDescription { get; set; }

        public string Description { get; set; }

        public float StartPrice { get; set; } = 1f;

        public IFormFile File { get; set; }

        public DateTime DateOfCreation { get; set; } = DateTime.Now;

        [Range(1,3)]
        [Required(ErrorMessage = "* Enter last day")]
        public int DateOfEnding { get; set; } = 1;
    }
}
