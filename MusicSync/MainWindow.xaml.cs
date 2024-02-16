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
        }

        private void getMusicFolder_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ErrorText = "";
            OpenFolderDialog openFolderDialog = new OpenFolderDialog();
            openFolderDialog.Multiselect = false;
            openFolderDialog.Title = "Select Music Folder";

            bool? folder = openFolderDialog.ShowDialog();

            if (folder == true)
            {
                viewModel.MusicFolder = openFolderDialog.FolderName;
            }
            else
            {
                viewModel.ErrorText = "Unable to get music folder";
            }
        }

        private async void getMusic_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ClearMusicCollection();
            viewModel.ErrorText = "";
            progressBar.Visibility = Visibility.Visible;
            await viewModel.GetLocalMusicAsync();
            await viewModel.CheckForErrorsAsync();
            progressBar.Visibility = Visibility.Hidden;
            viewModel.ErrorText = "All done getting local music.";

            treeviewLocalMusic.ItemsSource = viewModel.MusicCollection;
        }
    }
}