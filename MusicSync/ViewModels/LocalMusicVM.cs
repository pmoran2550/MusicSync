using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicSync
{
    public class LocalMusicVM : DispatchNotifyPropertyChanged
    {
        private static readonly log4net.ILog localMusicLog = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        const string goodImg = "Resources\\Good20c.png";
        const string errorImg = "Resources\\Error20.png";

        public LocalMusicVM()
        {
            _musicFolder = "";
            _errorText = "";
            _musicCollection = new ObservableCollection<Artist>();
            _artist = new ObservableCollection<Album>();
            _album = new ObservableCollection<Song>();
            _song = new Song();
        }

        // Properties
        private string _musicFolder;
        public string MusicFolder
        {
            get { return _musicFolder; }
            set
            {
                if (value != _musicFolder)
                {
                    _musicFolder = value;
                    OnPropertyChanged(nameof(MusicFolder));
                }
            }
        }

        private string _errorText;
        public string ErrorText
        {
            get { return _errorText; }
            set
            {
                if (value != _errorText)
                {
                    _errorText = value;
                    OnPropertyChanged(nameof(ErrorText));
                }
            }
        }

        private int _filesFound;
        public int FilesFound
        {
            get { return _filesFound; }
            set
            {
                if (value != _filesFound)
                {
                    _filesFound = value;
                    OnPropertyChanged(nameof(FilesFound));
                }
            }
        }

        private Song _song;
        public Song Song
        {
            get { return _song; }
            set
            {
                if (_song != value)
                {
                    _song = value;
                    OnPropertyChanged(nameof(Song));
                }
            }
        }

        private ObservableCollection<Song> _album;
        public ObservableCollection<Song> Album
        {
            get { return _album; }
            set
            {
                if (_album != value)
                {
                    _album = value;
                    OnPropertyChanged(nameof(Album));
                }
            }
        }

        private ObservableCollection<Album> _artist;
        public ObservableCollection<Album> Artist
        {
            get { return _artist; }
            set
            {
                if (_artist != value)
                {
                    _artist = value;
                    OnPropertyChanged(nameof(Artist));
                }
            }
        }

        private ObservableCollection<Artist> _musicCollection;
        public ObservableCollection<Artist> MusicCollection
        {
            get { return _musicCollection; }
            set
            {
                if (_musicCollection != value)
                {
                    _musicCollection = value;
                    OnPropertyChanged(nameof(MusicCollection));
                }
            }
        }

        public async Task GetLocalMusicAsync()
        {
            IProgress<int> progress = new Progress<int>(filesDone =>
            {
                ErrorText = "Files found: " + filesDone.ToString();
            });
            await Task.Run(() =>
            {
                GetLocalMusic(progress);
            });
        }

        public void GetLocalMusic(IProgress<int> progress)
        {
            ErrorText = string.Empty;
            if (progress != null) { progress.Report(0); }

            try
            {
                if (Directory.Exists(MusicFolder))
                {
                    localMusicLog.Error("Getting local music");
                    int artistCount = 0;
                    List<string> artistDirectories = new List<string>(Directory.EnumerateDirectories(MusicFolder));
                    artistDirectories.Sort();
                    foreach (string artistDirectory in artistDirectories)
                    {
                        artistCount++;
                        if (artistCount % 10 == 0 && progress != null)
                            progress.Report(artistCount);
                        //if (artistCount > 20) // for debugging
                        //    break;
                        if (!string.IsNullOrEmpty(artistDirectory))
                        {
                            string shortArtistName = new DirectoryInfo(artistDirectory).Name;
                            Artist newArtist = new Artist();
                            newArtist.Name = shortArtistName;
                            MusicCollection.Add(newArtist);

                            List<string> albumDirectories = new List<string>(Directory.EnumerateDirectories(artistDirectory));
                            albumDirectories.Sort();
                            foreach (string albumDirectory in albumDirectories)
                            {
                                if (!string.IsNullOrEmpty(albumDirectory))
                                {
                                    string shortAlbumName = new DirectoryInfo(albumDirectory).Name;
                                    Album newAlbum = new Album();
                                    newAlbum.Title = shortAlbumName;
                                    newAlbum.Artist = shortArtistName;
                                    newArtist.Albums.Add(newAlbum);

                                    ObservableCollection<Song> songList = new ObservableCollection<Song>();
                                    List<string> songFiles = new List<string>(Directory.EnumerateFiles(albumDirectory, "*.m4a").Union(Directory.EnumerateFiles(albumDirectory, "*.mp3")));
                                    foreach (string songFile in songFiles)
                                    {
                                        if (!string.IsNullOrEmpty(songFile))
                                        {
                                            string songTrackName = new FileInfo(songFile).Name;
                                            int trackNumber = 0;
                                            string commonName = songTrackName;
                                            if (songTrackName.Length > 7)
                                            {
                                                if (!int.TryParse(songTrackName.Substring(0, 3), out trackNumber))
                                                    trackNumber = 0;
                                                commonName = songTrackName.Substring(3, songTrackName.Length - 7);
                                            }
                                            Song newSong = new Song();
                                            newSong.Title = commonName;
                                            newSong.TrackNumber = trackNumber;
                                            songList.Add(newSong);
                                        }
                                    }
                                    ObservableCollection<Song> sortedSongs = new ObservableCollection<Song>(songList.OrderBy(song => song.TrackNumber));
                                    newAlbum.Songs = sortedSongs;
                                }
                            }
                        }
                    }
                }
                else
                {
                    ErrorText = "Given directory doesn't exist";
                    localMusicLog.Error(ErrorText);
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception getting local music: " + ex.Message;
                localMusicLog.Error(ErrorText);
            }
            OnPropertyChanged(nameof(MusicCollection));
        }

        public async Task CheckForErrorsAsync()
        {
            IProgress<int> progress = new Progress<int>(filesDone =>
            {
                ErrorText = "Files checked for errors: " + filesDone.ToString();
            });
            await Task.Run(() =>
            {
                CheckForErrors(progress);
            });
        }
        public void CheckForErrors(IProgress<int> progress)
        {
            ErrorText = string.Empty;
            if (progress != null) { progress.Report(0); }

            try
            {
                int artistCount = 0;
                foreach (Artist artist in MusicCollection)
                {
                    artistCount++;
                    if (artistCount % 10 == 0 && progress != null)
                        progress.Report(artistCount);

                    if (artist.Albums.Count == 0)
                    {
                        artist.ArtistError = ErrorStatus.Missing_Albums;
                        localMusicLog.ErrorFormat("Missing album error on {0}", artist.Name);
                    }
                    else
                    {
                        foreach (Album album in artist.Albums)
                        {
                            if (album.Songs.Count == 0)
                            {
                                album.AlbumError = ErrorStatus.Missing_Songs;
                                artist.ArtistError = ErrorStatus.Missing_Songs;
                                localMusicLog.ErrorFormat("Missing songs error on {0}, {1}", album.Title, artist.Name);
                            }
                            else
                            {

                                foreach (Song song in album.Songs)
                                {
                                    List<Song> songMatches = album.Songs.Where(item => item.Title == song.Title).ToList();
                                    if (songMatches.Count > 1)
                                    {
                                        song.SongError = ErrorStatus.Duplicate;
                                        album.AlbumError = ErrorStatus.Duplicate;
                                        artist.ArtistError = ErrorStatus.Duplicate;
                                        localMusicLog.ErrorFormat("Duplicate error on {0}, {1}, {2}", song.Title, album.Title, artist.Name);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception checking for local music errors: " + ex.Message;
                localMusicLog.ErrorFormat(ErrorText);
            }
            OnPropertyChanged(nameof(MusicCollection));
        }


        public void ClearMusicCollection()
        {
            foreach (Artist artist in MusicCollection)
            {
                foreach (Album album in artist.Albums)
                {
                    album.Songs.Clear();
                }
                artist.Albums.Clear();
            }
            MusicCollection.Clear();
        }
    }
}
