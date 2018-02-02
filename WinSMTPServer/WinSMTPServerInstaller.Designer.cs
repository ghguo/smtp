namespace WinSMTPServer
	{
	partial class WinSMTPServerInstaller
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

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
			{
			this.WindowsSMTPServerServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
			this.WindowsSMTPServerServiceInstaller = new System.ServiceProcess.ServiceInstaller();
			// 
			// WindowsSMTPServerServiceProcessInstaller
			// 
			this.WindowsSMTPServerServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
			this.WindowsSMTPServerServiceProcessInstaller.Password = null;
			this.WindowsSMTPServerServiceProcessInstaller.Username = null;
			// 
			// WindowsSMTPServerServiceInstaller
			// 
			this.WindowsSMTPServerServiceInstaller.Description = "The Windows SMTP Service";
			this.WindowsSMTPServerServiceInstaller.DisplayName = "Windows SMTP Service";
			this.WindowsSMTPServerServiceInstaller.ServiceName = "WinSMTPService";
			this.WindowsSMTPServerServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
			// 
			// WinSMTPServerInstaller
			// 
			this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.WindowsSMTPServerServiceProcessInstaller,
            this.WindowsSMTPServerServiceInstaller});

			}

		#endregion

		private System.ServiceProcess.ServiceProcessInstaller WindowsSMTPServerServiceProcessInstaller;
		private System.ServiceProcess.ServiceInstaller WindowsSMTPServerServiceInstaller;
		}
	}