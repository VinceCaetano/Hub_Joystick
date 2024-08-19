using SharpDX.XInput;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
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

        public TopMenu(core form)
        {
            core = form;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            powerButton = new Button
            {
                Size = new Size(100, 75),
                Text = "Power",
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat
            };
            powerButton.FlatAppearance.BorderSize = 0;
            powerButton.Click += (sender, e) => MessageBox.Show("Power clicked");

            controllerButton = new Button
            {
                Size = new Size(100, 75),
                Text = "Controller",
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat
            };
            controllerButton.FlatAppearance.BorderSize = 0;
            controllerButton.Click += (sender, e) => MessageBox.Show("Controller clicked");

            batteryButton = new Button
            {
                Size = new Size(100, 75),
                Text = "Battery",
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat
            };
            batteryButton.FlatAppearance.BorderSize = 0;
            batteryButton.Click += (sender, e) => MessageBox.Show("Battery clicked");

            timeButton = new Button
            {
                Size = new Size(100, 75),
                Text = "Time",
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat
            };
            timeButton.FlatAppearance.BorderSize = 0;
            timeButton.Click += (sender, e) => MessageBox.Show("Time clicked");

            this.Controls.Add(powerButton);
            this.Controls.Add(controllerButton);
            this.Controls.Add(batteryButton);
            this.Controls.Add(timeButton);

            buttons = new[] { powerButton, controllerButton, batteryButton, timeButton };
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
            if (buttons != null)
            {
                foreach (var button in buttons)
                {
                    button.BackColor = Color.LightGray;
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
                    button.BackColor = Color.LightGray;
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

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (var brush = new LinearGradientBrush(this.ClientRectangle, Color.Blue, Color.DarkBlue, LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }
    }
}
