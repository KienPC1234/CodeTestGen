using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows.Forms;
using CodeTestGenV1;
using Markdig;
using Microsoft.Web.WebView2.Core;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CodeTestGen
{
    public partial class FormChangeLog : MaterialSkin.Controls.MaterialForm
    {
        private string _Version;
        private string _DownloadLink;
        private SettingsData _Settings;
        private readonly string AppVersion = Hotro.version;
        private UpdateManager _updateManager;
        private UpdateData _updateData;
        private bool _isDownloading = false;
        private int _lastSelectedIndex = -1;
        private CancellationTokenSource _cancellationTokenSource;
        private string _HtmlPath;

        public FormChangeLog(SettingsData Settings)
        {
            InitializeComponent();
            _Settings = Settings;
            _updateManager = new UpdateManager(AppVersion);
            _cancellationTokenSource = new CancellationTokenSource();
            InitForm();
        }

        private async void InitForm()
        {
            try
            {
                // Load ChangeLogLoader.html path
                _HtmlPath = Path.Combine(Hotro.AppPath, "Update", "ChangeLogLoader.html");
                // Fetch latest version
                var latestVersion = await _updateManager.GetLatestVersionAsync();
                _Version = latestVersion.Version;
                _DownloadLink = latestVersion.DownloadLink;
                foreverButton1.Text = "Download v" + _Version;
                SetTheme();
                await InitializeWebView2Async();
                await LoadUpdateDataAsync();
                await PopulateListBoxAndCheckUpdatesAsync();
                // Start background task to monitor SelectedIndex
                StartSelectionMonitoringTask();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing form: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetTheme()
        {
            if (_Settings != null)
            {
                string Mode = _Settings.Mode;
                if (Mode == "Dark")
                {
                    this.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
                    foreverListBox1.BackColor = System.Drawing.Color.FromArgb(45, 45, 45);
                    foreverListBox1.ForeColor = System.Drawing.Color.White;
                }
                else
                {
                    this.BackColor = System.Drawing.Color.White;
                    foreverListBox1.BackColor = System.Drawing.Color.White;
                    foreverListBox1.ForeColor = System.Drawing.Color.Black;
                }
            }
        }

        private async Task InitializeWebView2Async()
        {
            await webView21.EnsureCoreWebView2Async(null);
            webView21.CoreWebView2.Settings.IsScriptEnabled = true;
            // Load HTML file using file URI
            webView21.Source = new Uri($"file:///{_HtmlPath.Replace("\\", "/")}");
            // Sync HTML theme with SettingsData.Mode
            if (_Settings?.Mode == "Dark")
            {
                await webView21.CoreWebView2.ExecuteScriptAsync("toggleTheme(true);");
            }
            else
            {
                await webView21.CoreWebView2.ExecuteScriptAsync("toggleTheme(false);");
            }
        }

        private async Task LoadHtmlContentAsync(string htmlContent)
        {
            // Escape HTML content for JSON
            string jsonSafeContent = JsonConvert.SerializeObject(htmlContent);
            // Call LoadHTMLs JavaScript function
            await webView21.CoreWebView2.ExecuteScriptAsync($"LoadHTMLs({jsonSafeContent});");
        }

        private async Task LoadUpdateDataAsync()
        {
            _updateData = await _updateManager.GetUpdateHistoryAsync();
            var items = new List<string> { "Latest: " + _updateData.LatestVersion.Version };
            foreach (var update in _updateData.History)
            {
                items.Add(update.Version);
            }
            foreverListBox1.Items = items.ToArray();
        }

        private async Task PopulateListBoxAndCheckUpdatesAsync()
        {
            bool isUpdateAvailable = await _updateManager.IsUpdateAvailableAsync();
            if (isUpdateAvailable)
            {
                MessageBox.Show("A new version is available!", "Update Available", MessageBoxButtons.OK, MessageBoxIcon.Information);
                foreverListBox1.SelectedIndex = 0; // Auto-select latest version
            }
            else
            {
                // Find current version index
                int currentVersionIndex = -1;
                for (int i = 0; i < foreverListBox1.Items.Length; i++)
                {
                    if (foreverListBox1.Items[i].Replace("Latest: ", "") == AppVersion)
                    {
                        currentVersionIndex = i;
                        break;
                    }
                }
                foreverListBox1.SelectedIndex = currentVersionIndex >= 0 ? currentVersionIndex : 0;
            }
            _lastSelectedIndex = foreverListBox1.SelectedIndex;
            // Load initial changelog
            await CheckSelectionChangeAsync();
        }

        private void StartSelectionMonitoringTask()
        {
            Task.Factory.StartNew(async () =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    if (InvokeRequired)
                    {
                        Invoke(new Action(async () =>
                        {
                            await CheckSelectionChangeAsync();
                        }));
                    }
                    else
                    {
                        await CheckSelectionChangeAsync();
                    }
                    await Task.Delay(100); // Check every 100ms
                }
            }, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private async Task CheckSelectionChangeAsync()
        {
            if (foreverListBox1.SelectedIndex != _lastSelectedIndex && foreverListBox1.SelectedIndex >= 0)
            {
                _lastSelectedIndex = foreverListBox1.SelectedIndex;
                string selectedVersion = foreverListBox1.Items[foreverListBox1.SelectedIndex].Replace("Latest: ", "");
                UpdateInfo selectedUpdate = selectedVersion == _updateData.LatestVersion.Version
                    ? _updateData.LatestVersion
                    : _updateData.History.FirstOrDefault(h => h.Version == selectedVersion);

                if (selectedUpdate != null)
                {
                    string htmlContent = Markdown.ToHtml(selectedUpdate.ChangeLogs);
                    await LoadHtmlContentAsync(htmlContent);
                    _Version = selectedUpdate.Version;
                    _DownloadLink = selectedUpdate.DownloadLink;
                    foreverButton1.Text = "Download v" + _Version;
                    foreverButton1.Enabled = _Version != AppVersion && !_isDownloading;
                }
            }
        }

        private void foreverButton1_Click(object sender, EventArgs e)
        {
            if (_isDownloading || _Version == AppVersion)
            {
                if (_Version == AppVersion)
                    MessageBox.Show("You are already on this version.", "No Update Needed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _isDownloading = true;
            foreverButton1.Enabled = false;
            foreverButton1.Text = "Downloading 0%";

            WebClient client = new WebClient();
            client.DownloadProgressChanged += (s, args) =>
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => foreverButton1.Text = "Downloading " + args.ProgressPercentage + "%"));
                }
                else
                {
                    foreverButton1.Text = "Downloading " + args.ProgressPercentage + "%";
                }
            };
            client.DownloadFileCompleted += (s, args) =>
            {
                _isDownloading = false;
                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                    {
                        HandleDownloadCompletion(args);
                    }));
                }
                else
                {
                    HandleDownloadCompletion(args);
                }
                client.Dispose();
            };

            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), "Setup_" + _Version + ".exe");
                client.DownloadFileAsync(new Uri(_DownloadLink), tempPath);
            }
            catch (Exception ex)
            {
                _isDownloading = false;
                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                    {
                        foreverButton1.Enabled = true;
                        foreverButton1.Text = "Download v" + _Version;
                        MessageBox.Show("Download failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }));
                }
                else
                {
                    foreverButton1.Enabled = true;
                    foreverButton1.Text = "Download v" + _Version;
                    MessageBox.Show("Download failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                client.Dispose();
            }
        }

        private void HandleDownloadCompletion(System.ComponentModel.AsyncCompletedEventArgs args)
        {
            _isDownloading = false;
            if (args.Error != null)
            {
                foreverButton1.Enabled = true;
                foreverButton1.Text = "Download v" + _Version;
                MessageBox.Show("Download failed: " + args.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                foreverButton1.Text = "Download Complete";
                string tempPath = Path.Combine(Path.GetTempPath(), "Setup_" + _Version + ".exe");
                Process.Start(tempPath);
                Application.Exit();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            base.OnFormClosing(e);
        }
    }
}