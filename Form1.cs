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
        private Button powerButton; // Button for turning off the controller
        private int selectedIndex = 0; // Track the currently selected button
        private bool isPowerButtonSelected = false; // Track if the power button is selected

        private string[] exePaths = new string[]
        {
            @"C:\Program Files\WindowsApps\Microsoft.XboxGamingOverlay_7.124.5142.0_x64__8wekyb3d8bbwe\GameBar.exe",
            @"C:\Program Files (x86)\Steam\Steam.exe -bigpicture",
            "firefox", // Placeholder for Firefox
            "firefox"  // Placeholder for Firefox
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
            LoadImagesAsync();
            this.Resize += Form1_Resize;
            this.controller = new Controller(UserIndex.One); // Initialize Xbox controller
        }

        private async void LoadImagesAsync()
        {
            string[] imageUrls = new string[]
            {
                "https://observatoriodegames.uol.com.br/wp-content/uploads/2023/10/logo-xbox-1024x768.png",
                "https://img.itch.zone/aW1nLzE0ODYzNTM3LnBuZw==/original/HpW3cn.png",
                "https://i0.wp.com/assets.b9.com.br/wp-content/uploads/2016/06/netflix-logo-thumb.jpg",
                "https://rollingstone.com.br/media/_versions/logo_prime_video_foto_reproducao_widelg.jpg"
            };

            buttons = new Button[imageUrls.Length];

            for (int i = 0; i < imageUrls.Length; i++)
            {
                Button btn = new Button();
                btn.Size = new Size(150, 100); // Initial rectangular button size
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.BackgroundImageLayout = ImageLayout.Stretch;

                try
                {
                    var imageData = await httpClient.GetByteArrayAsync(imageUrls[i]);
                    using (var ms = new System.IO.MemoryStream(imageData))
                    {
                        btn.BackgroundImage = Image.FromStream(ms);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading image from {imageUrls[i]}: {ex.Message}");
                }

                this.Controls.Add(btn);
                buttons[i] = btn;
                btn.Tag = i; // Store index in Tag property
                btn.Click += Button_Click;
            }

            // Add the power button
            powerButton = new Button
            {
                Size = new Size(200, 75),
                Location = new Point(10, 10),
                Text = "Turn Off Controller",
                BackColor = Color.LightGray
            };
            powerButton.Click += PowerButton_Click;
            this.Controls.Add(powerButton);

            PositionButtons();
            HighlightButton(selectedIndex); // Highlight the first button initially
            StartControllerMonitoring();
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

            // Ensure the power button is positioned correctly
            powerButton.Location = new Point(10, 10);
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
                    var state = controller.GetState();

                    // Handle D-pad navigation
                    if (state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadLeft))
                    {
                        Invoke((Action)(() => MoveSelection(-1))); // Move selection left
                    }
                    if (state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadRight))
                    {
                        Invoke((Action)(() => MoveSelection(1))); // Move selection right
                    }
                    if (state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadUp))
                    {
                        Invoke((Action)(SelectPowerButton)); // Move selection to the power button
                    }
                    if (state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadDown))
                    {
                        Invoke((Action)(SelectAppButtons)); // Move selection back to app buttons
                    }
                    if (state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.A))
                    {
                        Invoke((Action)ExecuteAction); // Execute the selected action
                    }

                    // Small delay to reduce CPU usage
                    Task.Delay(100).Wait();
                }
            });
        }

        private void MoveSelection(int direction)
        {
            if (isPowerButtonSelected)
            {
                if (direction == -1) // Only move down from the power button
                {
                    selectedIndex = (selectedIndex + direction + buttons.Length) % buttons.Length;
                    isPowerButtonSelected = false;
                    HighlightButton(selectedIndex);
                }
            }
            else
            {
                // Move selection left or right, looping around
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
                int highlightedWidth = 170; // Larger size for highlighted button
                int highlightedHeight = 120;

                foreach (Button btn in buttons)
                {
                    btn.Size = new Size(defaultWidth, defaultHeight); // Reset button size
                    btn.BackColor = Color.Transparent; // Reset button color
                }
                buttons[index].Size = new Size(highlightedWidth, highlightedHeight); // Larger size for highlighted button
                buttons[index].BackColor = Color.LightGray; // Highlight selected button
            }
        }

        private void HighlightPowerButton()
        {
            int defaultWidth = 150;
            int defaultHeight = 100;
            int highlightedWidth = 200; // Larger size for highlighted power button
            int highlightedHeight = 75;

            foreach (Button btn in buttons)
            {
                btn.Size = new Size(defaultWidth, defaultHeight); // Reset button size
                btn.BackColor = Color.Transparent; // Reset button color
            }

            powerButton.Size = new Size(highlightedWidth, highlightedHeight); // Larger size for highlighted power button
            powerButton.BackColor = Color.LightGray; // Highlight power button
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

                    if (path == "firefox") // Placeholder for Firefox
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "C:\\Program Files\\Mozilla Firefox\\firefox.exe",
                            Arguments = arguments,
                            UseShellExecute = true
                        });
                    }
                    else if (path == "vlc") // Placeholder for VLC
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "C:\\Program Files\\VideoLAN\\VLC\\vlc.exe",
                            Arguments = arguments,
                            UseShellExecute = true
                        });
                    }
                    else if (path == "steam") // Steam with Big Picture Mode
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
                        // For general cases where path includes the executable
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
            // Implement actual controller shutdown logic here
            MessageBox.Show("Controller turned off."); // Placeholder action
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
