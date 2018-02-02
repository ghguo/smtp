namespace WinSMTPConsole
	{
	partial class ConfigurationForm
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnClearIP = new System.Windows.Forms.Button();
			this.btnDeleteIP = new System.Windows.Forms.Button();
			this.gvAllowed = new System.Windows.Forms.DataGridView();
			this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.IP = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Subnet = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.newSubnet = new System.Windows.Forms.TextBox();
			this.newIP = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.btnAddIP = new System.Windows.Forms.Button();
			this.label8 = new System.Windows.Forms.Label();
			this.chkAllowRemote = new System.Windows.Forms.CheckBox();
			this.txtPort = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.chkLocalHost = new System.Windows.Forms.CheckBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label6 = new System.Windows.Forms.Label();
			this.txtCleanup = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.txtRetry = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.txtExpire = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gvAllowed)).BeginInit();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.btnClearIP);
			this.groupBox1.Controls.Add(this.btnDeleteIP);
			this.groupBox1.Controls.Add(this.gvAllowed);
			this.groupBox1.Controls.Add(this.newSubnet);
			this.groupBox1.Controls.Add(this.newIP);
			this.groupBox1.Controls.Add(this.label9);
			this.groupBox1.Controls.Add(this.btnAddIP);
			this.groupBox1.Controls.Add(this.label8);
			this.groupBox1.Controls.Add(this.chkAllowRemote);
			this.groupBox1.Controls.Add(this.txtPort);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.chkLocalHost);
			this.groupBox1.Location = new System.Drawing.Point(12,12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(328,294);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Permitted connections";
			// 
			// btnClearIP
			// 
			this.btnClearIP.Location = new System.Drawing.Point(237,194);
			this.btnClearIP.Name = "btnClearIP";
			this.btnClearIP.Size = new System.Drawing.Size(75,23);
			this.btnClearIP.TabIndex = 6;
			this.btnClearIP.Text = "Clear all";
			this.btnClearIP.UseVisualStyleBackColor = true;
			this.btnClearIP.Click += new System.EventHandler(this.btnClearIP_Click);
			// 
			// btnDeleteIP
			// 
			this.btnDeleteIP.Location = new System.Drawing.Point(237,100);
			this.btnDeleteIP.Name = "btnDeleteIP";
			this.btnDeleteIP.Size = new System.Drawing.Size(75,23);
			this.btnDeleteIP.TabIndex = 5;
			this.btnDeleteIP.Text = "Delete IP";
			this.btnDeleteIP.UseVisualStyleBackColor = true;
			this.btnDeleteIP.Click += new System.EventHandler(this.btnDeleteIP_Click);
			// 
			// gvAllowed
			// 
			this.gvAllowed.AllowUserToAddRows = false;
			this.gvAllowed.AllowUserToDeleteRows = false;
			this.gvAllowed.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.gvAllowed.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ID,
            this.IP,
            this.Subnet});
			this.gvAllowed.Location = new System.Drawing.Point(9,100);
			this.gvAllowed.Name = "gvAllowed";
			this.gvAllowed.ReadOnly = true;
			this.gvAllowed.RowHeadersVisible = false;
			this.gvAllowed.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.gvAllowed.Size = new System.Drawing.Size(222,117);
			this.gvAllowed.TabIndex = 4;
			// 
			// ID
			// 
			this.ID.DataPropertyName = "ID";
			this.ID.HeaderText = "ID";
			this.ID.Name = "ID";
			this.ID.ReadOnly = true;
			this.ID.Visible = false;
			// 
			// IP
			// 
			this.IP.DataPropertyName = "IP";
			this.IP.HeaderText = "Allowed IP";
			this.IP.Name = "IP";
			this.IP.ReadOnly = true;
			// 
			// Subnet
			// 
			this.Subnet.DataPropertyName = "Subnet";
			this.Subnet.HeaderText = "Subnet";
			this.Subnet.Name = "Subnet";
			this.Subnet.ReadOnly = true;
			// 
			// newSubnet
			// 
			this.newSubnet.Location = new System.Drawing.Point(88,255);
			this.newSubnet.MaxLength = 15;
			this.newSubnet.Name = "newSubnet";
			this.newSubnet.Size = new System.Drawing.Size(100,20);
			this.newSubnet.TabIndex = 10;
			this.newSubnet.Validating += new System.ComponentModel.CancelEventHandler(this.newSubnet_Validating);
			// 
			// newIP
			// 
			this.newIP.Location = new System.Drawing.Point(88,229);
			this.newIP.MaxLength = 15;
			this.newIP.Name = "newIP";
			this.newIP.Size = new System.Drawing.Size(100,20);
			this.newIP.TabIndex = 8;
			this.newIP.Leave += new System.EventHandler(this.newIP_Leave);
			this.newIP.Validating += new System.ComponentModel.CancelEventHandler(this.newIP_Validating);
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(15,255);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(67,13);
			this.label9.TabIndex = 9;
			this.label9.Text = "New subnet:";
			// 
			// btnAddIP
			// 
			this.btnAddIP.Location = new System.Drawing.Point(206,227);
			this.btnAddIP.Name = "btnAddIP";
			this.btnAddIP.Size = new System.Drawing.Size(75,23);
			this.btnAddIP.TabIndex = 11;
			this.btnAddIP.Text = "Add &IP";
			this.btnAddIP.UseVisualStyleBackColor = true;
			this.btnAddIP.Click += new System.EventHandler(this.btnAddIP_Click);
			this.btnAddIP.Validating += new System.ComponentModel.CancelEventHandler(this.btnAddIP_Validating);
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(37,232);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(45,13);
			this.label8.TabIndex = 7;
			this.label8.Text = "New IP:";
			// 
			// chkAllowRemote
			// 
			this.chkAllowRemote.AutoSize = true;
			this.chkAllowRemote.Location = new System.Drawing.Point(9,68);
			this.chkAllowRemote.Name = "chkAllowRemote";
			this.chkAllowRemote.Size = new System.Drawing.Size(222,17);
			this.chkAllowRemote.TabIndex = 3;
			this.chkAllowRemote.Text = "Allow connections from remote computers";
			this.chkAllowRemote.UseVisualStyleBackColor = true;
			// 
			// txtPort
			// 
			this.txtPort.Location = new System.Drawing.Point(86,21);
			this.txtPort.MaxLength = 5;
			this.txtPort.Name = "txtPort";
			this.txtPort.Size = new System.Drawing.Size(60,20);
			this.txtPort.TabIndex = 1;
			this.txtPort.Validating += new System.ComponentModel.CancelEventHandler(this.txtPort_Validating);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(6,24);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(74,13);
			this.label7.TabIndex = 0;
			this.label7.Text = "Listen on port:";
			// 
			// chkLocalHost
			// 
			this.chkLocalHost.AutoSize = true;
			this.chkLocalHost.Location = new System.Drawing.Point(9,47);
			this.chkLocalHost.Name = "chkLocalHost";
			this.chkLocalHost.Size = new System.Drawing.Size(183,17);
			this.chkLocalHost.TabIndex = 2;
			this.chkLocalHost.Text = "Allow connections from local host";
			this.chkLocalHost.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.txtCleanup);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.txtRetry);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.txtExpire);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Location = new System.Drawing.Point(346,12);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(288,123);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Cleanup and expiration";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(220,76);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(29,13);
			this.label6.TabIndex = 8;
			this.label6.Text = "days";
			// 
			// txtCleanup
			// 
			this.txtCleanup.Location = new System.Drawing.Point(138,73);
			this.txtCleanup.MaxLength = 3;
			this.txtCleanup.Name = "txtCleanup";
			this.txtCleanup.Size = new System.Drawing.Size(63,20);
			this.txtCleanup.TabIndex = 7;
			this.txtCleanup.Validating += new System.ComponentModel.CancelEventHandler(this.txtCleanup_Validating);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(6,76);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(126,13);
			this.label5.TabIndex = 6;
			this.label5.Text = "Clean up messages after:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(219,24);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(43,13);
			this.label4.TabIndex = 2;
			this.label4.Text = "minutes";
			// 
			// txtRetry
			// 
			this.txtRetry.Location = new System.Drawing.Point(138,21);
			this.txtRetry.MaxLength = 4;
			this.txtRetry.Name = "txtRetry";
			this.txtRetry.Size = new System.Drawing.Size(63,20);
			this.txtRetry.TabIndex = 1;
			this.txtRetry.Validating += new System.ComponentModel.CancelEventHandler(this.txtRetry_Validating);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(18,24);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(114,13);
			this.label3.TabIndex = 0;
			this.label3.Text = "Retry messages every:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(218,50);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(43,13);
			this.label2.TabIndex = 5;
			this.label2.Text = "minutes";
			// 
			// txtExpire
			// 
			this.txtExpire.Location = new System.Drawing.Point(138,47);
			this.txtExpire.MaxLength = 5;
			this.txtExpire.Name = "txtExpire";
			this.txtExpire.Size = new System.Drawing.Size(63,20);
			this.txtExpire.TabIndex = 4;
			this.txtExpire.Validating += new System.ComponentModel.CancelEventHandler(this.txtExpire_Validating);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(19,50);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(113,13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Expire messages after:";
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(640,12);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75,23);
			this.btnOK.TabIndex = 2;
			this.btnOK.Text = "Ok";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.CausesValidation = false;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(640,41);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75,23);
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// errorProvider1
			// 
			this.errorProvider1.ContainerControl = this;
			// 
			// ConfigurationForm
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F,13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(728,322);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Name = "ConfigurationForm";
			this.Text = "WinSMTPConsole :: Configure settings";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.gvAllowed)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
			this.ResumeLayout(false);

			}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.CheckBox chkLocalHost;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox txtCleanup;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txtRetry;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtExpire;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtPort;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Button btnAddIP;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.CheckBox chkAllowRemote;
		private System.Windows.Forms.DataGridView gvAllowed;
		private System.Windows.Forms.TextBox newSubnet;
		private System.Windows.Forms.TextBox newIP;
		private System.Windows.Forms.Button btnDeleteIP;
		private System.Windows.Forms.DataGridViewTextBoxColumn ID;
		private System.Windows.Forms.DataGridViewTextBoxColumn IP;
		private System.Windows.Forms.DataGridViewTextBoxColumn Subnet;
		private System.Windows.Forms.Button btnClearIP;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.ErrorProvider errorProvider1;
		}
	}