namespace Hub_Joystick
{
    partial class core
    {
        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.topMenu = new Hub_Joystick.Components.TopMenu(this);
            this.appMenu = new Hub_Joystick.Components.AppMenu(this);

            // 
            // topMenu
            // 
            this.topMenu.Dock = System.Windows.Forms.DockStyle.Top;
            this.topMenu.Height = 100;
            this.topMenu.Location = new System.Drawing.Point(0, 0);
            this.topMenu.Name = "topMenu";
            this.topMenu.Size = new System.Drawing.Size(800, 100);
            this.topMenu.TabIndex = 0;

            // 
            // appMenu
            // 
            this.appMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.appMenu.Location = new System.Drawing.Point(0, this.topMenu.Bottom);
            this.appMenu.Name = "appMenu";
            this.appMenu.Size = new System.Drawing.Size(800, 350);
            this.appMenu.TabIndex = 1;

            // 
            // core
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.appMenu);
            this.Controls.Add(this.topMenu);
            this.Name = "core";
            this.Text = "Hub Joystick";
            this.ResumeLayout(false);
        }
    }
}
