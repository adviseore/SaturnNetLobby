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
        }

        private void BrowseMednafenButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Select Mednafen",
                Filter =
                    "Mednafen (mednafen.exe)|mednafen.exe|" +
                    "Executable files (*.exe)|*.exe"
            };

            if (dialog.ShowDialog() == true)
            {
                MednafenPathBox.Text = dialog.FileName;
                StatusText.Text = "Status: Mednafen selected";
            }
        }

        private void BrowseGameButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Select Saturn Game",
                Filter =
                    "Saturn Games (*.cue;*.ccd;*.toc;*.m3u)|" +
                    "*.cue;*.ccd;*.toc;*.m3u|" +
                    "All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                GamePathBox.Text = dialog.FileName;
                StatusText.Text = "Status: Saturn game selected";
            }
        }

        private bool ValidatePaths(
            out string mednafenPath,
            out string gamePath)
        {
            mednafenPath = MednafenPathBox.Text.Trim();
            gamePath = GamePathBox.Text.Trim();

            if (!File.Exists(mednafenPath))
            {
                MessageBox.Show(
                    "Please select mednafen.exe first.",
                    "Mednafen Not Found",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }

            if (!File.Exists(gamePath))
            {
                MessageBox.Show(
                    "Please select a Saturn game first.",
                    "Game Not Found",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }

            return true;
        }

        private void LaunchButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (!ValidatePaths(
                out string mednafenPath,
                out string gamePath))
            {
                return;
            }

            try
            {
                ProcessStartInfo startInfo =
                    new ProcessStartInfo
                    {
                        FileName = mednafenPath,
                        WorkingDirectory =
                            Path.GetDirectoryName(mednafenPath)!
                    };

                startInfo.ArgumentList.Add(gamePath);

                Process.Start(startInfo);

                StatusText.Text =
                    "Status: Saturn launched offline!";
            }
            catch (Exception ex)
            {
                ShowLaunchError(ex);
            }
        }

        private void ConnectButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (!ValidatePaths(
                out string mednafenPath,
                out string gamePath))
            {
                return;
            }

            string nickname = NicknameBox.Text.Trim();
            string server = ServerBox.Text.Trim();
            string portText = PortBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(nickname))
            {
                MessageBox.Show(
                    "Please enter a nickname.",
                    "Nickname Required",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (string.IsNullOrWhiteSpace(server))
            {
                MessageBox.Show(
                    "Please enter a netplay server.",
                    "Server Required",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (!int.TryParse(portText, out int port) ||
                port < 1 ||
                port > 65535)
            {
                MessageBox.Show(
                    "Please enter a valid port between 1 and 65535.",
                    "Invalid Port",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            try
            {
                StatusText.Text =
                    $"Status: Launching netplay to {server}:{port}...";

                ProcessStartInfo startInfo =
                    new ProcessStartInfo
                    {
                        FileName = mednafenPath,
                        WorkingDirectory =
                            Path.GetDirectoryName(mednafenPath)!
                    };

                startInfo.ArgumentList.Add("-netplay.host");
                startInfo.ArgumentList.Add(server);

                startInfo.ArgumentList.Add("-netplay.port");
                startInfo.ArgumentList.Add(port.ToString());

                startInfo.ArgumentList.Add("-netplay.nick");
                startInfo.ArgumentList.Add(nickname);

                startInfo.ArgumentList.Add("-connect");

                startInfo.ArgumentList.Add(gamePath);

                Process process = Process.Start(startInfo);

                if (process == null)
                {
                    MessageBox.Show(
                        "Windows could not start Mednafen.",
                        "Launch Failed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    StatusText.Text =
                        "Status: Mednafen did not start";

                    return;
                }

                StatusText.Text =
                    $"Status: Netplay launched - {server}:{port}";
            }
            catch (Exception ex)
            {
                ShowLaunchError(ex);
            }
        }

        private void ShowLaunchError(Exception ex)
        {
            MessageBox.Show(
                $"Could not launch Mednafen:\n\n{ex.Message}",
                "Launch Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            StatusText.Text =
                "Status: Launch failed";
        }
    }
}