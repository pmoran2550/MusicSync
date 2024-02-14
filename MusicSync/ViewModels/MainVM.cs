﻿using System;
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
        const string goodImg = "Resources\\Good20c.png";
        const string errorImg = "Resources\\Error20.png";

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

        public MainVM() 
        {
            _musicFolder = "";
            _errorText = "";
            _musicCollection = new ObservableCollection<Artist>();
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
                    int artistCount = 0;
                    List<string> artistDirectories = new List<string>(Directory.EnumerateDirectories(MusicFolder));
                    artistDirectories.Sort();
                    foreach (string artistDirectory in artistDirectories)
                    {
                        if (artistCount++ % 10 == 0 && progress != null)
                            progress.Report(artistCount);
                        if (artistCount > 20)
                            break;
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
                }
            }
            catch (Exception ex)
            {
                ErrorText = "Exception getting local music: " + ex.Message;
            }
            OnPropertyChanged(nameof(MusicCollection));
        }

        public void ClearMusicCollection()
        {
            foreach (Artist artist in MusicCollection)
            {
                foreach(Album album in artist.Albums)
                {
                    album.Songs.Clear();
                }
                artist.Albums.Clear();
            }
            MusicCollection.Clear();
        }
    }
}
