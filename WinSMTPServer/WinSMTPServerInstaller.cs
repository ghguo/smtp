using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace WinSMTPServer
	{
	[RunInstaller(true)]
	public partial class WinSMTPServerInstaller : Installer
		{
		public WinSMTPServerInstaller()
			{
			InitializeComponent();
			}
		}
	}
