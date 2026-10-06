using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace SaturnNetLobby
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            BrowseMednafenButton.Click += BrowseMednafenButton_Click;
            BrowseGameButton.Click += BrowseGameButton_Click;
            LaunchButton.Click += LaunchButton_Click;
        }

        private void BrowseMednafenButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Title = "Select Mednafen";
            dialog.Filter = "Mednafen (mednafen.exe)|mednafen.exe|Executable files (*.exe)|*.exe";

            if (dialog.ShowDialog() == true)
            {
                MednafenPathBox.Text = dialog.FileName;
                StatusText.Text = "Status: Mednafen selected";
            }
        }

        private void BrowseGameButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Title = "Select Saturn Game";
            dialog.Filter =
                "Saturn Games (*.cue;*.ccd;*.toc;*.m3u)|*.cue;*.ccd;*.toc;*.m3u|" +
                "All files (*.*)|*.*";

            if (dialog.ShowDialog() == true)
            {
                GamePathBox.Text = dialog.FileName;
                StatusText.Text = "Status: Saturn game selected";
            }
        }

        private void LaunchButton_Click(object sender, RoutedEventArgs e)
        {
            string mednafenPath = MednafenPathBox.Text.Trim();
            string gamePath = GamePathBox.Text.Trim();

            if (!File.Exists(mednafenPath))
            {
                MessageBox.Show(
                    "Please select mednafen.exe first.",
                    "Mednafen Not Found",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (!File.Exists(gamePath))
            {
                MessageBox.Show(
                    "Please select a Saturn game first.",
                    "Game Not Found",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = mednafenPath,
                    WorkingDirectory = Path.GetDirectoryName(mednafenPath)!
                };

                startInfo.ArgumentList.Add(gamePath);

                Process.Start(startInfo);

                StatusText.Text = "Status: Saturn launched!";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Could not launch Mednafen:\n\n{ex.Message}",
                    "Launch Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                StatusText.Text = "Status: Launch failed";
            }
        }
    }
}