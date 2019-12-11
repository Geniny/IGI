using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AntiAuc.Models
{
    public class BetModel 
    {
        [Display(Name = "Enter your sum")]
        [Required(ErrorMessage = "* Enter sum")]
        public float NewPrice { get; set; } = 0;
    }
}
