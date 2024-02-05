using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicSync
{
    public class Song
    {
        public string Title { get; set; }
        public int TrackNumber { get; set; }

        public Song() 
        { 
            Title = string.Empty;
            TrackNumber = 0;
        }
    }
}
