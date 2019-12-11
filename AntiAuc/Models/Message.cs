using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AntiAuc.Models
{
    public class Message
    {
        public int? Id { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public DateTime When { get; set; }

        public int? SenderId { get; set;}
        public User Sender { get; set; }
    }
}
