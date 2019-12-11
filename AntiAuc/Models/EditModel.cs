using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AntiAuc.Models
{
    public class EditModel
    {
        [Required(ErrorMessage = "* Old password is incorrect")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        
        //[DataType(DataType.Password)]
        //[Compare("NewPassword", ErrorMessage = "Wrong password")]
        //public string ConfirmPassword { get; set; }

        [Required]
        [Range(1,2)]
        public int? RoleId { get; set; }
        public float Deposit { get; set; }

        [Required(ErrorMessage = "* Email is incorrect")]
        [EmailAddress]
        public string Email { get; set; }
        public int UserId { get; set; }
    }
}
