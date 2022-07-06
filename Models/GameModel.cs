using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Champions.Models
{
    public class GameModel
    {
        public int ID { get; set; }
        public string Team1 { get; set; }
        public string Team2 { get; set; }
        public int Team1_Score { get; set; }
        public int Team2_Score { get; set; }
        public int Drow_Score { get; set; }
        public int Team1_Goal { get; set; }
        public int Team2_Goal { get; set; }
        public DateTime Time { get; set; }
        public string Team1_img { get; set; }
        public string Team2_img { get; set; }
    }
}