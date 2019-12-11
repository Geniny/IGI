using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AntiAuc.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public float Deposit { get; set; } = 0f;
        public List<UserProduct> Products { get; set; }
        public int? RoleId { get; set; }
        public Role Role { get; set; }
        
        public List<Message> Messages { get; set; }
        public User()
        {
            Products = new List<UserProduct>();
            Messages = new List<Message>();
        }
    }
}
