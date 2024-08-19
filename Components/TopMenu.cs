using SharpDX.XInput;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace Hub_Joystick.Components
{
    public class TopMenu : UserControl
    {
        private core core;
        private Button powerButton;
        private Button turnOffNotebookButton;
        private Button[] buttons;
        private int selectedIndex = 0;

        public TopMenu(core form)
        {
            core = form;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            powerButton = new Button
            {
                Size = new Size(200, 75),
                Location = new Point(10, 10),
                Text = "Turn Off Controller",
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat
            };
            powerButton.FlatAppearance.BorderSize = 0; 
            powerButton.Click += (sender, e) => MessageBox.Show("controle desligado");

            turnOffNotebookButton = new Button
            {
                Size = new Size(200, 75),
                Location = new Point(powerButton.Right + 10, 10),
                Text = "Turn Off Notebook",
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat
            };
            turnOffNotebookButton.FlatAppearance.BorderSize = 0;
            turnOffNotebookButton.Click += (sender, e) => MessageBox.Show("fechar todas as abas e desligar notebook");

            this.Controls.Add(powerButton);
            this.Controls.Add(turnOffNotebookButton);

            buttons = new[] { powerButton, turnOffNotebookButton };

            this.Size = new Size(400, 100);
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
                button.BackColor = Color.LightGray;
                button.Size = new Size(100, 100);
            }

            if (buttons.Length > 0 && selectedIndex >= 0 && selectedIndex < buttons.Length)
            {
                var selectedButton = buttons[selectedIndex];
                selectedButton.BackColor = Color.DarkGray;
                selectedButton.Size = new Size(120, 120);
            }
        }


        public void DeselectAllButtons()
        {
            foreach (var button in buttons)
            {
                button.BackColor = Color.LightGray;
                button.Size = new Size(100, 100);
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
            int buttonWidth = this.Width / 4;
            int buttonHeight = 75;
            int spacing = 10;

            powerButton.Size = new Size(buttonWidth, buttonHeight);
            powerButton.Location = new Point(10, 10);

            turnOffNotebookButton.Size = new Size(buttonWidth, buttonHeight);
            turnOffNotebookButton.Location = new Point(powerButton.Right + 10, 10);

            this.Width = this.Parent.ClientSize.Width;
        }
    }
}
