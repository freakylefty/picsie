namespace Picsie
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.FocusChecker = new System.Windows.Forms.Timer(this.components);
            this.VideoPanel = new System.Windows.Forms.Panel();
            this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // FocusChecker
            // 
            this.FocusChecker.Tick += new System.EventHandler(this.FocusChecker_Tick);
            // 
            // VideoPanel
            // 
            this.VideoPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.VideoPanel.Location = new System.Drawing.Point(0, 0);
            this.VideoPanel.Name = "VideoPanel";
            this.VideoPanel.Size = new System.Drawing.Size(284, 262);
            this.VideoPanel.TabIndex = 0;
            this.VideoPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Image_MouseMove);
            this.VideoPanel.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.Image_MouseDoubleClick);
            this.VideoPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Image_MouseDown);
            this.VideoPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Image_MouseUp);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.VideoPanel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Picsie";
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Image_MouseUp);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.Image_MouseDoubleClick);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Image_MouseDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Form1_KeyPress);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Image_MouseMove);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer FocusChecker;
        private System.Windows.Forms.Panel VideoPanel;
        private System.Windows.Forms.ToolTip ToolTip;


    }
}

