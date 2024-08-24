using SharpDX.XInput;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Hub_Joystick.Components
{
    public class TopMenu : UserControl
    {
        private core core;
        private Button powerButton;
        private Button controllerButton;
        private Button batteryButton;
        private Button timeButton;
        private Button[] buttons;
        private int selectedIndex = 0;
        private System.Windows.Forms.Timer timeUpdateTimer; 

        private string powerIconPath = @"assets\icon_power.png";
        private string controllerIconPath = @"assets\icon_controller.png";
        private string batteryIconPath = @"assets\icon_battery.png";
        

        public TopMenu(core form)
        {
            core = form;
            InitializeComponents();
            StartTimeUpdate();
        }

        private void StartTimeUpdate()
        {
            timeUpdateTimer = new System.Windows.Forms.Timer
            {
                Interval = 60000 
            };
            timeUpdateTimer.Tick += (sender, e) => UpdateTime();
            timeUpdateTimer.Start();
            UpdateTime(); 
        }

        private void UpdateTime()
        {
            if (timeButton != null)
            {
                timeButton.Text = DateTime.Now.ToString("HH:mm");
            }
        }

        private void InitializeComponents()
        {
            powerButton = new Button
            {
                Size = new Size(100, 75),
                Location = new Point(10, 10),
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                Image = ResizeImage(LoadImageFromFile(powerIconPath), new Size(50, 50)),
                ImageAlign = ContentAlignment.MiddleCenter,
                TextImageRelation = TextImageRelation.ImageAboveText,
                Text = string.Empty, 
                FlatAppearance = { BorderSize = 0 }
            };
            powerButton.Click += PowerButton_Click;

            controllerButton = new Button
            {
                Size = new Size(100, 75),
                Location = new Point(this.Width - 300, 10), 
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                Image = ResizeImage(LoadImageFromFile(controllerIconPath), new Size(50, 50)),
                ImageAlign = ContentAlignment.MiddleCenter,
                TextImageRelation = TextImageRelation.ImageAboveText,
                Text = string.Empty, 
                FlatAppearance = { BorderSize = 0 }
            };
            controllerButton.Click += (sender, e) => MessageBox.Show("Desligar controle");

            batteryButton = new Button
            {
                Size = new Size(100, 75),
                Location = new Point(this.Width - 200, 10), 
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                Image = ResizeImage(LoadImageFromFile(batteryIconPath), new Size(50, 50)),
                ImageAlign = ContentAlignment.MiddleCenter,
                TextImageRelation = TextImageRelation.ImageAboveText,
                Text = string.Empty, 
                FlatAppearance = { BorderSize = 0 }
            };
            batteryButton.Click += (sender, e) => MessageBox.Show("Nível da bateria");

            timeButton = new Button
            {
                Size = new Size(100, 75),
                Location = new Point(this.Width - 100, 10),
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = DateTime.Now.ToString("HH:mm"),
                Font = new Font("Arial", 12, FontStyle.Bold),
                FlatAppearance = { BorderSize = 0 }
            };

            this.Controls.Add(powerButton);
            this.Controls.Add(controllerButton);
            this.Controls.Add(batteryButton);
            this.Controls.Add(timeButton);

            buttons = new Button[] { powerButton, controllerButton, batteryButton, timeButton };

            this.BackColor = Color.Transparent;

            PositionButtons();
        }

        private Image ResizeImage(Image img, Size size)
        {
            return (Image)(new Bitmap(img, size));
        }

        private void PowerButton_Click(object sender, EventArgs e)
        {
            CloseAllApplications();

            ShutDownPC();
        }
        private void CloseAllApplications()
        {
            try
            {
                Process.Start("cmd.exe", "/c taskkill /f /fi \"STATUS eq RUNNING\"");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error closing applications: {ex.Message}");
            }
        }
        private void ShutDownPC()
        {
            try
            {
                Process.Start("cmd.exe", "/c shutdown /s /t 0");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error shutting down PC: {ex.Message}");
            }
        }

        private Image LoadImageFromFile(string path)
        {
            if (File.Exists(path))
            {
                return Image.FromFile(path);
            }
            else
            {
                return null;
            }
        }

        public void MoveSelection(GamepadButtonFlags buttons)
        {
            const int totalButtons = 4;

            if (buttons.HasFlag(GamepadButtonFlags.DPadLeft))
            {
                selectedIndex = (selectedIndex - 1 + totalButtons) % totalButtons;
                HighlightSelectedButton();
            }
            else if (buttons.HasFlag(GamepadButtonFlags.DPadRight))
            {
                selectedIndex = (selectedIndex + 1) % totalButtons;
                HighlightSelectedButton();
            }
            else if (buttons.HasFlag(GamepadButtonFlags.DPadUp))
            {
            }
            else if (buttons.HasFlag(GamepadButtonFlags.DPadDown))
            {
            }
        }

        public void HighlightSelectedButton()
        {
            if (buttons != null)
            {
                foreach (var button in buttons)
                {
                    button.BackColor = Color.Transparent;
                    button.Size = new Size(100, 75);
                }

                if (selectedIndex >= 0 && selectedIndex < buttons.Length)
                {
                    var selectedButton = buttons[selectedIndex];
                    selectedButton.BackColor = Color.DarkGray;
                    selectedButton.Size = new Size(120, 90);
                }
            }
        }

        public void DeselectAllButtons()
        {
            if (buttons != null)
            {
                foreach (var button in buttons)
                {
                    button.BackColor = Color.Transparent;
                    button.Size = new Size(100, 75);
                }
            }
        }

        public void ExecuteAction()
        {
            if (buttons != null && buttons.Length > 0)
            {
                buttons[selectedIndex].PerformClick();
            }
        }

        public void PositionButtons()
        {
            int buttonWidth = 100;
            int buttonHeight = 75;
            int spacing = 10;

            powerButton.Location = new Point(10, 10);
            controllerButton.Location = new Point(powerButton.Right + spacing, 10);
            batteryButton.Location = new Point(this.Width - 200, 10);
            timeButton.Location = new Point(this.Width - 100, 10);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            PositionButtons();
            this.Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (timeUpdateTimer != null))
            {
                timeUpdateTimer.Stop();
                timeUpdateTimer.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

