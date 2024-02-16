using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicSync
{
    public class Album
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public ObservableCollection<Song> Songs { get; set; }
        public ErrorStatus AlbumError { get; set; }

        public Album()
        {
            Title = "";
            Artist = "";
            Songs = new ObservableCollection<Song>();
            AlbumError = ErrorStatus.No_Error;
        }
    }
}
