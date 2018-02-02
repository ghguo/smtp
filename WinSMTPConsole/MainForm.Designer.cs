namespace WinSMTPConsole
	{
	partial class MainForm
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
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.configureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.activityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showTodaysActivityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showLastWeeksActivityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.selectActivityToShowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.activityToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(510, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configureToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// configureToolStripMenuItem
			// 
			this.configureToolStripMenuItem.Name = "configureToolStripMenuItem";
			this.configureToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.configureToolStripMenuItem.Text = "&Configure...";
			this.configureToolStripMenuItem.Click += new System.EventHandler(this.configureToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// activityToolStripMenuItem
			// 
			this.activityToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showTodaysActivityToolStripMenuItem,
            this.showLastWeeksActivityToolStripMenuItem,
            this.toolStripSeparator2,
            this.selectActivityToShowToolStripMenuItem});
			this.activityToolStripMenuItem.Name = "activityToolStripMenuItem";
			this.activityToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
			this.activityToolStripMenuItem.Text = "&Activity";
			// 
			// showTodaysActivityToolStripMenuItem
			// 
			this.showTodaysActivityToolStripMenuItem.Name = "showTodaysActivityToolStripMenuItem";
			this.showTodaysActivityToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
			this.showTodaysActivityToolStripMenuItem.Text = "Show today\'s activity...";
			this.showTodaysActivityToolStripMenuItem.Click += new System.EventHandler(this.showTodaysActivityToolStripMenuItem_Click);
			// 
			// showLastWeeksActivityToolStripMenuItem
			// 
			this.showLastWeeksActivityToolStripMenuItem.Name = "showLastWeeksActivityToolStripMenuItem";
			this.showLastWeeksActivityToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
			this.showLastWeeksActivityToolStripMenuItem.Text = "Show last week\'s activity...";
			this.showLastWeeksActivityToolStripMenuItem.Click += new System.EventHandler(this.showLastWeeksActivityToolStripMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(209, 6);
			// 
			// selectActivityToShowToolStripMenuItem
			// 
			this.selectActivityToShowToolStripMenuItem.Name = "selectActivityToShowToolStripMenuItem";
			this.selectActivityToShowToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
			this.selectActivityToShowToolStripMenuItem.Text = "Select activity to show...";
			this.selectActivityToShowToolStripMenuItem.Click += new System.EventHandler(this.selectActivityToShowToolStripMenuItem_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(510, 262);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "MainForm";
			this.Text = "WinSTMPConsole :: Configure and view SMTP activity";
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

			}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem configureToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem activityToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem showTodaysActivityToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem showLastWeeksActivityToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem selectActivityToShowToolStripMenuItem;
		}
	}

