using SharpDX.XInput;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace Hub_Joystick.Components
{
    public class AppMenu : UserControl
    {
        private core mainForm;
        private Button[] buttons;
        private int selectedIndex = 0;

        public AppMenu(core form)
        {
            mainForm = form;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            buttons = new Button[4];

            string[] imageFiles = { "game_bar.png", "steam.png", "netflix.png", "primevideo.png" };
            int buttonWidth = 100;
            int buttonHeight = 100;
            int spacing = 10;

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i] = new Button
                {
                    Size = new Size(buttonWidth, buttonHeight),
                    BackgroundImage = Image.FromFile(System.IO.Path.Combine(Application.StartupPath, "assets", imageFiles[i])),
                    BackgroundImageLayout = ImageLayout.Stretch,
                    FlatStyle = FlatStyle.Flat
                };
                buttons[i].FlatAppearance.BorderSize = 0;
                this.Controls.Add(buttons[i]);
            }

            this.Size = new Size(400, 150);
            this.BackColor = Color.Transparent; 
            PositionButtons();
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
        }

        public void HighlightSelectedButton()
        {
            foreach (var button in buttons)
            {
                button.BackColor = Color.Transparent;
                button.Size = new Size(100, 100);
            }

            if (buttons.Length > 0 && selectedIndex >= 0 && selectedIndex < buttons.Length)
            {
                var selectedButton = buttons[selectedIndex];
                selectedButton.BackColor = Color.FromArgb(255, 13, 90, 0); // Color for selected
                selectedButton.Size = new Size(120, 120);
            }
        }

        public void DeselectAllButtons()
        {
            foreach (var button in buttons)
            {
                button.BackColor = Color.Transparent;
                button.Size = new Size(100, 100);
            }
        }

        public void ExecuteAction()
        {
            if (buttons.Length > 0)
            {
                switch (selectedIndex)
                {
                    case 0:
                        Process.Start("C:\\Program Files\\WindowsApps\\Microsoft.XboxGamingOverlay_7.124.5142.0_x64__8wekyb3d8bbwe\\GameBar.exe");
                        break;
                    case 1:
                        try
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = "steam://open/bigpicture",
                                UseShellExecute = true
                            });
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"falha erro: {ex.Message}");
                        }
                        break;
                    case 2:
                        Process.Start(new ProcessStartInfo("https://www.netflix.com") { UseShellExecute = true });
                        break;
                    case 3:
                        Process.Start(new ProcessStartInfo("https://www.primevideo.com") { UseShellExecute = true });
                        break;
                }

                Application.Exit();
            }
        }

        public void PositionButtons()
        {
            int buttonWidth = 100;
            int buttonHeight = 100;
            int spacing = 10;

            
            int totalWidth = buttonWidth * buttons.Length + spacing * (buttons.Length - 1);
            int startX = (this.Width - totalWidth) / 2;

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].Size = new Size(buttonWidth, buttonHeight);
                buttons[i].Location = new Point(startX + i * (buttonWidth + spacing), this.Height / 2 - buttonHeight / 2);
            }
        }

    }
}
