using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Champions.Models
{
    public class Teams
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Img { get; set; }
        public string Group { get; set; }
        public int Points { get; set; }
    }
}