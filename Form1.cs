using System;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpDX;
using SharpDX.XInput;
using System.Diagnostics;

namespace Hub_Joystick
{
    public partial class Form1 : Form
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private Controller controller;
        private Button[] buttons;
        private Button powerButton;
        private Button turnOffNotebookButton;
        private int selectedIndex = 0;
        private bool isPowerButtonSelected = false;

        private string[] exePaths = new string[]
        {
            @"C:\Program Files\WindowsApps\Microsoft.XboxGamingOverlay_7.124.5142.0_x64__8wekyb3d8bbwe\GameBar.exe",
            @"C:\Program Files (x86)\Steam\Steam.exe -bigpicture",
            "firefox",
            "firefox"
        };

        private string[] appArguments = new string[]
        {
            "-bigpicture",
            "",
            "https://www.netflix.com",
            "https://www.primevideo.com"
        };

        public Form1()
        {
            InitializeComponent();
            LoadImages();
            this.Resize += Form1_Resize;
            this.controller = new Controller(UserIndex.One);
        }

        private void LoadImages()
        {
            string[] imagePaths = new string[]
            {
                @"assets\game_bar.png",
                @"assets\steam.png",
                @"assets\netflix.png",
                @"assets\primevideo.png"
            };

            buttons = new Button[imagePaths.Length];

            for (int i = 0; i < imagePaths.Length; i++)
            {
                Button btn = new Button
                {
                    Size = new Size(120, 120), 
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = { BorderSize = 0 },
                    BackgroundImageLayout = ImageLayout.Zoom 
                };

                try
                {
                    string fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imagePaths[i]);
                    btn.BackgroundImage = Image.FromFile(fullPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading image from {imagePaths[i]}: {ex.Message}");
                }

                this.Controls.Add(btn);
                buttons[i] = btn;
                btn.Tag = i;
                btn.Click += Button_Click;
            }

            powerButton = new Button
            {
                Size = new Size(200, 75),
                Location = new Point(10, 10),
                Text = "Turn Off Controller",
                BackColor = Color.LightGray
            };
            powerButton.Click += PowerButton_Click;
            this.Controls.Add(powerButton);

            turnOffNotebookButton = new Button
            {
                Size = new Size(200, 75),
                Location = new Point(powerButton.Right + 10, 10),
                Text = "Turn Off Notebook",
                BackColor = Color.LightGray
            };
            turnOffNotebookButton.Click += TurnOffNotebookButton_Click;
            this.Controls.Add(turnOffNotebookButton);

            PositionButtons();
            HighlightButton(selectedIndex);
            StartControllerMonitoring();
        }

        private void TurnOffNotebookButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Notebook turned off.");
        }

        private void PositionButtons()
        {
            int buttonWidth = 150;
            int buttonHeight = 100;
            int spacing = 20;
            int totalWidth = (buttonWidth * buttons.Length) + (spacing * (buttons.Length - 1));
            int startX = (this.ClientSize.Width - totalWidth) / 2;

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].Location = new Point(startX + i * (buttonWidth + spacing), (this.ClientSize.Height - buttonHeight) / 2 + 50);
            }

            powerButton.Location = new Point(10, 10);
            turnOffNotebookButton.Location = new Point(powerButton.Right + 10, 10);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            PositionButtons();
        }

        private async void StartControllerMonitoring()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        if (controller != null && controller.IsConnected) 
                        {
                            var state = controller.GetState();

                            if (state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadLeft))
                            {
                                Invoke((Action)(() => MoveSelection(-1)));
                            }
                            if (state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadRight))
                            {
                                Invoke((Action)(() => MoveSelection(1)));
                            }
                            if (state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadUp))
                            {
                                Invoke((Action)(SelectPowerButton));
                            }
                            if (state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadDown))
                            {
                                Invoke((Action)(SelectAppButtons));
                            }
                            if (state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.A))
                            {
                                Invoke((Action)ExecuteAction);
                            }
                        }
                        else
                        {
                            controller = new Controller(UserIndex.One);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error in controller monitoring: {ex.Message}");
                    }

                    Task.Delay(100).Wait();
                }
            });
        }

        private void MoveSelection(int direction)
        {
            if (isPowerButtonSelected)
            {
                if (direction == -1)
                {
                    selectedIndex = (selectedIndex + direction + buttons.Length) % buttons.Length;
                    isPowerButtonSelected = false;
                    HighlightButton(selectedIndex);
                }
            }
            else
            {
                selectedIndex = (selectedIndex + direction + buttons.Length) % buttons.Length;
                HighlightButton(selectedIndex);
            }
        }

        private void HighlightButton(int index)
        {
            if (index >= 0 && index < buttons.Length)
            {
                int defaultWidth = 150;
                int defaultHeight = 100;
                int highlightedWidth = 170;
                int highlightedHeight = 120;

                foreach (Button btn in buttons)
                {
                    btn.Size = new Size(defaultWidth, defaultHeight);
                    btn.BackColor = Color.Transparent;
                }

                buttons[index].Size = new Size(highlightedWidth, highlightedHeight);
                buttons[index].BackColor = Color.LightGray;
            }
        }

        private void HighlightPowerButton()
        {
            int defaultWidth = 150;
            int defaultHeight = 100;
            int highlightedWidth = 200;
            int highlightedHeight = 75;

            foreach (Button btn in buttons)
            {
                btn.Size = new Size(defaultWidth, defaultHeight);
                btn.BackColor = Color.Transparent;
            }

            powerButton.Size = new Size(highlightedWidth, highlightedHeight);
            powerButton.BackColor = Color.LightGray;
        }

        private void ExecuteAction()
        {
            if (isPowerButtonSelected)
            {
                PowerButton_Click(this, EventArgs.Empty);
            }
            else if (selectedIndex >= 0 && selectedIndex < buttons.Length)
            {
                LaunchApp(selectedIndex);
            }
        }

        private void LaunchApp(int index)
        {
            if (index >= 0 && index < exePaths.Length)
            {
                try
                {
                    string path = exePaths[index];
                    string arguments = appArguments[index];

                    if (path == "firefox")
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "C:\\Program Files\\Mozilla Firefox\\firefox.exe",
                            Arguments = arguments,
                            UseShellExecute = true
                        });
                    }
                    else if (path == "vlc")
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "C:\\Program Files\\VideoLAN\\VLC\\vlc.exe",
                            Arguments = arguments,
                            UseShellExecute = true
                        });
                    }
                    else if (path == "steam")
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "C:\\Program Files (x86)\\Steam\\Steam.exe",
                            Arguments = arguments,
                            UseShellExecute = true
                        });
                    }
                    else
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = path,
                            Arguments = arguments,
                            UseShellExecute = true
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error launching application: {ex.Message}");
                }
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                int index = (int)clickedButton.Tag;
                LaunchApp(index);
            }
        }

        private void PowerButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Controller turned off.");
        }

        private void SelectPowerButton()
        {
            if (!isPowerButtonSelected)
            {
                isPowerButtonSelected = true;
                HighlightPowerButton();
            }
        }

        private void SelectAppButtons()
        {
            if (isPowerButtonSelected)
            {
                isPowerButtonSelected = false;
                HighlightButton(selectedIndex);
            }
        }
    }
}
