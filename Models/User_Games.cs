using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace Champions.Models
{
    public class User_Games
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public int GameID { get; set; }
        public int Team1_Bet { get; set; }
        public int Team2_Bet { get; set; }
        public int Score { get; set; }
        [NotMapped]
        public string Img { get; set; }
    }
}