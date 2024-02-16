using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicSync
{
    public class Artist
    {
        public string Name { get; set; }
        public ObservableCollection<Album> Albums { get; set; }
        public ErrorStatus ArtistError { get; set; }

        public Artist()
        {
            Name = "";
            Albums = new ObservableCollection<Album>();
            ArtistError = ErrorStatus.No_Error;
        }
    }
}
