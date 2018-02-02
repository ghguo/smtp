namespace WinSMTPConsole
	{
	partial class ActivityViewForm
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
			this.gvResults = new System.Windows.Forms.DataGridView();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnFind = new System.Windows.Forms.Button();
			this.chkExpired = new System.Windows.Forms.CheckBox();
			this.chkDelivered = new System.Windows.Forms.CheckBox();
			this.chkReceived = new System.Windows.Forms.CheckBox();
			this.chkSessions = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.dtpDateBefore = new System.Windows.Forms.DateTimePicker();
			this.dtpAfterDate = new System.Windows.Forms.DateTimePicker();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.btnClose = new System.Windows.Forms.Button();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.StartTimeString = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.EndTimeString = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.DirectionString = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ReceivedDateString = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Sender = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Recipient = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.AttemptDateString = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SuccessString = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.FailureReason = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.DeliveredDateString = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ExpiredDateString = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this.gvResults)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// gvResults
			// 
			this.gvResults.AllowUserToAddRows = false;
			this.gvResults.AllowUserToDeleteRows = false;
			this.gvResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.gvResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.gvResults.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.StartTimeString,
            this.EndTimeString,
            this.DirectionString,
            this.ReceivedDateString,
            this.Sender,
            this.Recipient,
            this.AttemptDateString,
            this.SuccessString,
            this.FailureReason,
            this.DeliveredDateString,
            this.ExpiredDateString});
			this.gvResults.Location = new System.Drawing.Point(12,118);
			this.gvResults.Name = "gvResults";
			this.gvResults.ReadOnly = true;
			this.gvResults.RowHeadersVisible = false;
			this.gvResults.Size = new System.Drawing.Size(659,193);
			this.gvResults.TabIndex = 0;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.btnFind);
			this.groupBox1.Controls.Add(this.chkExpired);
			this.groupBox1.Controls.Add(this.chkDelivered);
			this.groupBox1.Controls.Add(this.chkReceived);
			this.groupBox1.Controls.Add(this.chkSessions);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.dtpDateBefore);
			this.groupBox1.Controls.Add(this.dtpAfterDate);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(12,12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(659,100);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Filter activity by";
			// 
			// btnFind
			// 
			this.btnFind.Location = new System.Drawing.Point(578,15);
			this.btnFind.Name = "btnFind";
			this.btnFind.Size = new System.Drawing.Size(75,23);
			this.btnFind.TabIndex = 10;
			this.btnFind.Text = "&Find...";
			this.btnFind.UseVisualStyleBackColor = true;
			this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
			// 
			// chkExpired
			// 
			this.chkExpired.AutoSize = true;
			this.chkExpired.Location = new System.Drawing.Point(226,77);
			this.chkExpired.Name = "chkExpired";
			this.chkExpired.Size = new System.Drawing.Size(111,17);
			this.chkExpired.TabIndex = 9;
			this.chkExpired.Text = "Expired messages";
			this.chkExpired.UseVisualStyleBackColor = true;
			// 
			// chkDelivered
			// 
			this.chkDelivered.AutoSize = true;
			this.chkDelivered.Location = new System.Drawing.Point(85,77);
			this.chkDelivered.Name = "chkDelivered";
			this.chkDelivered.Size = new System.Drawing.Size(121,17);
			this.chkDelivered.TabIndex = 8;
			this.chkDelivered.Text = "Delivered messages";
			this.chkDelivered.UseVisualStyleBackColor = true;
			// 
			// chkReceived
			// 
			this.chkReceived.AutoSize = true;
			this.chkReceived.Checked = true;
			this.chkReceived.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkReceived.Location = new System.Drawing.Point(226,54);
			this.chkReceived.Name = "chkReceived";
			this.chkReceived.Size = new System.Drawing.Size(122,17);
			this.chkReceived.TabIndex = 7;
			this.chkReceived.Text = "Received messages";
			this.chkReceived.UseVisualStyleBackColor = true;
			// 
			// chkSessions
			// 
			this.chkSessions.AutoSize = true;
			this.chkSessions.Checked = true;
			this.chkSessions.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkSessions.Location = new System.Drawing.Point(85,54);
			this.chkSessions.Name = "chkSessions";
			this.chkSessions.Size = new System.Drawing.Size(68,17);
			this.chkSessions.TabIndex = 6;
			this.chkSessions.Text = "Sessions";
			this.chkSessions.UseVisualStyleBackColor = true;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(17,55);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(59,13);
			this.label3.TabIndex = 5;
			this.label3.Text = "Search for:";
			// 
			// dtpDateBefore
			// 
			this.dtpDateBefore.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.dtpDateBefore.Location = new System.Drawing.Point(267,19);
			this.dtpDateBefore.Name = "dtpDateBefore";
			this.dtpDateBefore.Size = new System.Drawing.Size(108,20);
			this.dtpDateBefore.TabIndex = 4;
			// 
			// dtpAfterDate
			// 
			this.dtpAfterDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.dtpAfterDate.Location = new System.Drawing.Point(85,19);
			this.dtpAfterDate.Name = "dtpAfterDate";
			this.dtpAfterDate.Size = new System.Drawing.Size(110,20);
			this.dtpAfterDate.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(211,25);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(50,13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Through:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(46,25);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(33,13);
			this.label1.TabIndex = 0;
			this.label1.Text = "From:";
			// 
			// btnClose
			// 
			this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnClose.Location = new System.Drawing.Point(12,317);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(75,23);
			this.btnClose.TabIndex = 2;
			this.btnClose.Text = "Close";
			this.btnClose.UseVisualStyleBackColor = true;
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// timer1
			// 
			this.timer1.Interval = 1000;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// StartTimeString
			// 
			this.StartTimeString.DataPropertyName = "StartTimeString";
			this.StartTimeString.HeaderText = "Conn start";
			this.StartTimeString.MaxInputLength = 20;
			this.StartTimeString.Name = "StartTimeString";
			this.StartTimeString.ReadOnly = true;
			// 
			// EndTimeString
			// 
			this.EndTimeString.DataPropertyName = "EndTimeString";
			this.EndTimeString.HeaderText = "Conn end";
			this.EndTimeString.MaxInputLength = 20;
			this.EndTimeString.Name = "EndTimeString";
			this.EndTimeString.ReadOnly = true;
			// 
			// DirectionString
			// 
			this.DirectionString.DataPropertyName = "DirectionString";
			this.DirectionString.HeaderText = "Dir";
			this.DirectionString.MaxInputLength = 3;
			this.DirectionString.Name = "DirectionString";
			this.DirectionString.ReadOnly = true;
			// 
			// ReceivedDateString
			// 
			this.ReceivedDateString.DataPropertyName = "ReceivedDateString";
			this.ReceivedDateString.HeaderText = "Received";
			this.ReceivedDateString.MaxInputLength = 20;
			this.ReceivedDateString.Name = "ReceivedDateString";
			this.ReceivedDateString.ReadOnly = true;
			// 
			// Sender
			// 
			this.Sender.DataPropertyName = "Sender";
			this.Sender.HeaderText = "Sender";
			this.Sender.MaxInputLength = 255;
			this.Sender.Name = "Sender";
			this.Sender.ReadOnly = true;
			// 
			// Recipient
			// 
			this.Recipient.DataPropertyName = "Recipient";
			this.Recipient.HeaderText = "Recipient";
			this.Recipient.MaxInputLength = 255;
			this.Recipient.Name = "Recipient";
			this.Recipient.ReadOnly = true;
			// 
			// AttemptDateString
			// 
			this.AttemptDateString.DataPropertyName = "AttemptDateString";
			this.AttemptDateString.HeaderText = "Attempt";
			this.AttemptDateString.Name = "AttemptDateString";
			this.AttemptDateString.ReadOnly = true;
			// 
			// SuccessString
			// 
			this.SuccessString.DataPropertyName = "SuccessString";
			this.SuccessString.HeaderText = "Status";
			this.SuccessString.MaxInputLength = 10;
			this.SuccessString.Name = "SuccessString";
			this.SuccessString.ReadOnly = true;
			// 
			// FailureReason
			// 
			this.FailureReason.DataPropertyName = "FailureReason";
			this.FailureReason.HeaderText = "Fail reason";
			this.FailureReason.MaxInputLength = 255;
			this.FailureReason.Name = "FailureReason";
			this.FailureReason.ReadOnly = true;
			// 
			// DeliveredDateString
			// 
			this.DeliveredDateString.DataPropertyName = "DeliveredDateString";
			this.DeliveredDateString.HeaderText = "Delivered";
			this.DeliveredDateString.MaxInputLength = 20;
			this.DeliveredDateString.Name = "DeliveredDateString";
			this.DeliveredDateString.ReadOnly = true;
			// 
			// ExpiredDateString
			// 
			this.ExpiredDateString.DataPropertyName = "ExpiredDateString";
			this.ExpiredDateString.HeaderText = "Expired";
			this.ExpiredDateString.MaxInputLength = 20;
			this.ExpiredDateString.Name = "ExpiredDateString";
			this.ExpiredDateString.ReadOnly = true;
			// 
			// ActivityViewForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F,13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnClose;
			this.ClientSize = new System.Drawing.Size(684,352);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.gvResults);
			this.MinimumSize = new System.Drawing.Size(700,390);
			this.Name = "ActivityViewForm";
			this.Text = "WinSMTPConsole :: View activity";
			((System.ComponentModel.ISupportInitialize)(this.gvResults)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

			}

		#endregion

		private System.Windows.Forms.DataGridView gvResults;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.DateTimePicker dtpAfterDate;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.DateTimePicker dtpDateBefore;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btnFind;
		private System.Windows.Forms.CheckBox chkExpired;
		private System.Windows.Forms.CheckBox chkDelivered;
		private System.Windows.Forms.CheckBox chkReceived;
		private System.Windows.Forms.CheckBox chkSessions;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.DataGridViewTextBoxColumn StartTimeString;
		private System.Windows.Forms.DataGridViewTextBoxColumn EndTimeString;
		private System.Windows.Forms.DataGridViewTextBoxColumn DirectionString;
		private System.Windows.Forms.DataGridViewTextBoxColumn ReceivedDateString;
		private System.Windows.Forms.DataGridViewTextBoxColumn Sender;
		private System.Windows.Forms.DataGridViewTextBoxColumn Recipient;
		private System.Windows.Forms.DataGridViewTextBoxColumn AttemptDateString;
		private System.Windows.Forms.DataGridViewTextBoxColumn SuccessString;
		private System.Windows.Forms.DataGridViewTextBoxColumn FailureReason;
		private System.Windows.Forms.DataGridViewTextBoxColumn DeliveredDateString;
		private System.Windows.Forms.DataGridViewTextBoxColumn ExpiredDateString;
		}
	}