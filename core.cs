using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hub_Joystick.Components;
using SharpDX.XInput;
using System.Diagnostics;

namespace Hub_Joystick
{
    public partial class core : Form
    {
        private TopMenu topMenu;
        private AppMenu appMenu;
        private Controller controller;
        private bool isTopMenu = true;

        public core()
        {
            InitializeComponent();
            InitializeMenus();
            this.Resize += MainForm_Resize;
            this.controller = new Controller(UserIndex.One);
            StartControllerMonitoring();
        }

        private void InitializeMenus()
        {
            if (topMenu == null)
            {
                topMenu = new TopMenu(this);
                this.Controls.Add(topMenu);
            }
            if (appMenu == null)
            {
                appMenu = new AppMenu(this);
                this.Controls.Add(appMenu);
            }

            topMenu.Dock = DockStyle.Top;
            appMenu.Dock = DockStyle.Fill;

            topMenu.BringToFront();
            appMenu.BringToFront();

            topMenu.Show();
            appMenu.Show();
        }


        private async void StartControllerMonitoring()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    if (controller != null && controller.IsConnected)
                    {
                        var state = controller.GetState();

                        if (state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadLeft) ||
                            state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadRight) ||
                            state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadUp) ||
                            state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.X) ||
                            state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadDown))
                        {
                            Invoke((Action)(() => HandleControllerInput(state.Gamepad.Buttons)));
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

                    Task.Delay(100).Wait();
                }
            });
        }

        private void HandleControllerInput(GamepadButtonFlags buttons)
        {
            if (buttons.HasFlag(GamepadButtonFlags.DPadDown))
            {
                if (isTopMenu)
                {
                    topMenu.DeselectAllButtons(); 
                    isTopMenu = false;
                    appMenu.HighlightSelectedButton(); 
                }
            }
            else if (buttons.HasFlag(GamepadButtonFlags.DPadUp))
            {
                if (!isTopMenu)
                {
                    appMenu.DeselectAllButtons();
                    isTopMenu = true;
                    topMenu.HighlightSelectedButton(); 
                }
            }
            else if (buttons.HasFlag(GamepadButtonFlags.X))
            {
                CloseApplication();
            }
            else
            {
                if (isTopMenu)
                {
                    topMenu.MoveSelection(buttons);
                }
                else
                {
                    appMenu.MoveSelection(buttons);
                }
            }
        }



        private void ExecuteAction()
        {
            if (isTopMenu)
            {
                topMenu.ExecuteAction();
            }
            else
            {
                appMenu.ExecuteAction();
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            topMenu.PositionButtons(); 
            appMenu.PositionButtons();
        }


        private void SwitchToTopMenu()
        {
            isTopMenu = true;
            topMenu.BringToFront();
            topMenu.Show(); 
            appMenu.Hide(); 
        }

        private void SwitchToAppMenu()
        {
            isTopMenu = false;
            appMenu.BringToFront();
            appMenu.Show(); 
            topMenu.Hide(); 
        }

        private void CloseApplication()
        {
            Application.Exit();
        }
    }
}
