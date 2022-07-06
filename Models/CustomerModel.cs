using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Champions.Models
{
    public class CustomerModel
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int AdminCode { get; set; }
        public string ResetPasswordCode { get; set; }
        public string Image { get; set; }
        public string WinTeam { get; set; }
        public string WinPlayer { get; set; }
        [NotMapped]
        public int WinPlayerGoals { get; set; }
    }
}