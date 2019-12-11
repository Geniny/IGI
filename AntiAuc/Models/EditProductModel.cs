using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AntiAuc.Models
{
    public class EditProductModel
    {
        [Required]
        public int? ProductId { get; set; }
        [Required]
        [Display(Name = "Product name")]
        public string ProductName { get; set; }
        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Short description")]
        public string ShortDescription { get; set; }
        [Required]
        [Display(Name = "Product name")]
        public bool IsAvailable { get; set; } 
        [Required]
        [Display(Name = "Current price")]
        public float CurrentPrice { get; set; }
        [Required]
        [Display(Name = "Date of creation")]
        public DateTime DateOfCreation { get; set; }
        [Required]
        [Display(Name = "Date of ending")]
        public DateTime DateOfEnding { get; set; }
        public string Image { get; set; }

        public IFormFile File { get; set; }
    }
}
