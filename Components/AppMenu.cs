using System;
using System.Drawing;
using System.Windows.Forms;
using SharpDX.XInput;
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

            this.BackColor = Color.LightGray;

            string[] imageFiles = { "game_bar.png", "steam.png", "netflix.png", "primevideo.png" };
            int buttonWidth = 100;
            int buttonHeight = 100;
            int spacing = 10;

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i] = new Button
                {
                    Size = new Size(buttonWidth, buttonHeight),
                    Location = new Point(spacing + (buttonWidth + spacing) * i, 10),
                    BackgroundImage = Image.FromFile(System.IO.Path.Combine(Application.StartupPath, "assets", imageFiles[i])),
                    BackgroundImageLayout = ImageLayout.Stretch,
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = { BorderSize = 0 }
                };
               
                this.Controls.Add(buttons[i]);
            }

            this.Size = new Size(400, 150);
            HighlightSelectedButton(); 
        }

        public void MoveSelection(GamepadButtonFlags buttons)
        {
            const int totalButtons = 4;

            Debug.WriteLine($"Ação btn: {buttons}");

            if (buttons.HasFlag(GamepadButtonFlags.DPadLeft))
            {
                Debug.WriteLine("DPad L");
                selectedIndex = (selectedIndex - 1 + totalButtons) % totalButtons;
                HighlightSelectedButton();
            }
            else if (buttons.HasFlag(GamepadButtonFlags.DPadRight))
            {
                Debug.WriteLine("DPad R");
                selectedIndex = (selectedIndex + 1) % totalButtons;
                HighlightSelectedButton();
            }
        }

        private void HighlightSelectedButton()
        {
            int buttonWidth = 100;
            int buttonHeight = 100;
            int selectedButtonWidth = 120;
            int selectedButtonHeight = 120;
            int spacing = 10;

            foreach (var button in buttons)
            {
                button.BackColor = Color.LightGray; 
                button.Size = new Size(buttonWidth, buttonHeight); 
                button.FlatAppearance.BorderSize = 0; 
            }

            if (buttons.Length > 0)
            {
                buttons[selectedIndex].BackColor = Color.DarkGray;
                buttons[selectedIndex].Size = new Size(selectedButtonWidth, selectedButtonHeight);

                
                int newX = (this.Width / buttons.Length) * selectedIndex + spacing;
                buttons[selectedIndex].Location = new Point(newX + (buttonWidth - selectedButtonWidth) / 2, 10);
            }
        }

        public void ExecuteAction()
        {
            if (buttons.Length > 0)
            {
                buttons[selectedIndex].PerformClick();
            }
        }

        public void PositionButtons()
        {
            int buttonWidth = this.Width / buttons.Length;
            int buttonHeight = 100;
            int spacing = 10;

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].Size = new Size(buttonWidth - spacing, buttonHeight);
                buttons[i].Location = new Point((buttonWidth * i) + spacing, 10);
            }

            this.Width = this.Parent.ClientSize.Width;
            this.Height = this.Parent.ClientSize.Height - this.Top;
        }
    }
}
