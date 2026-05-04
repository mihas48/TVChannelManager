using System;
using System.ComponentModel;

namespace TVChannelManager.Library.Models
{
    public class TVChannel
    {
        public string Name { get; set; }
        public double Rating { get; set; }
        public int MedianViewersAge { get; set; }
        public DateTime FoundingDate { get; set; }
        public TimeSpan BroadcastStartTime { get; set; }
        public bool IsHD { get; set; }
        public string Genre { get; set; }
        public string LogoImage { get; set; } // base64 строка логотипа
    }
}