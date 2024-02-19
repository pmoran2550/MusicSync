using System.Windows;
using Microsoft.Win32;

namespace MusicSync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainVM viewModel;

        public MainWindow()
        {
            InitializeComponent();
            viewModel = new MainVM();
            DataContext = viewModel;
            progressBar.Visibility = Visibility.Hidden;
            localMusicGrid.DataContext = viewModel.LocalMusicVM;
        }

        private void getMusicFolder_Click(object sender, RoutedEventArgs e)
        {
            viewModel.LocalMusicVM.ErrorText = "";
            OpenFolderDialog openFolderDialog = new OpenFolderDialog();
            openFolderDialog.Multiselect = false;
            openFolderDialog.Title = "Select Music Folder";

            bool? folder = openFolderDialog.ShowDialog();

            if (folder == true)
            {
                viewModel.LocalMusicVM.MusicFolder = openFolderDialog.FolderName;
            }
            else
            {
                viewModel.LocalMusicVM.ErrorText = "Unable to get music folder";
            }
        }

        private async void getMusic_Click(object sender, RoutedEventArgs e)
        {
            viewModel.LocalMusicVM.ClearMusicCollection();
            viewModel.LocalMusicVM.ErrorText = "";
            progressBar.Visibility = Visibility.Visible;
            await viewModel.LocalMusicVM.GetLocalMusicAsync();
            await viewModel.LocalMusicVM.CheckForErrorsAsync();
            progressBar.Visibility = Visibility.Hidden;
            viewModel.LocalMusicVM.ErrorText = "All done getting local music.";

            treeviewLocalMusic.ItemsSource = viewModel.LocalMusicVM.MusicCollection;
        }
    }
}