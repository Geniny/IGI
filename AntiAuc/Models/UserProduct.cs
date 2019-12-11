using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiAuc.Models
{
    public class UserProduct
    {
        public int? UserId { get; set; }
        public User User { get; set; }

        public int? ProductId { get; set; }
        public Product Product { get; set; }
    }
}
