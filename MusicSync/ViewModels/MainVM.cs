using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicSync
{
    public class MainVM : DispatchNotifyPropertyChanged
    {
        LocalMusicVM _localMusicVM;
        public MainVM() 
        {
            _localMusicVM = new LocalMusicVM();
        }

        public LocalMusicVM LocalMusicVM { get { return _localMusicVM; } }
    }
}
