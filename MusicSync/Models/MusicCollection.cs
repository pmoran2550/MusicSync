using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicSync
{
    public class MusicCollection
    {
        public string Title { get; set; }
        public ObservableCollection<Artist> Artists { get; set; }

        public MusicCollection()
        {
            Title = "Local Music Collection";
            Artists = new ObservableCollection<Artist>();
        }
    }
}
