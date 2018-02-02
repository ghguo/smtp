//The contents of this file are subject to the Mozilla Public License
//Version 1.1 (the "License"); you may not use this file except in
//compliance with the License. You may obtain a copy of the License at
//http://www.mozilla.org/MPL/
//
//Software distributed under the License is distributed on an "AS IS"
//basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
//License for the specific language governing rights and limitations
//under the License.
//
//The Original Code is based on Strainer, a Spam filtering engine, created by and 
//Copyright (C) 2004, Chris Laforet Software, Inc.
//
//The Initial Developer of the Original Code is Chris Laforet from Chris Laforet Software.  
//Portions created by Chris Laforet Software are Copyright (C) 2010.  All Rights Reserved.
//
//Contributor(s): Chris Laforet Software.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SupportLibrary.DataSource;
using SupportLibrary.Database;

namespace WinSMTPConsole
	{
	public partial class MainForm : Form
		{
		public MainForm()
			{
			InitializeComponent();

			try
				{
				ConfigurationRecord record = ConfigurationDataSource.ReadRecord(ConfigurationRecord.MasterConfigurationID);
				if (record == null)
					{
					MessageBox.Show("There is no configuration record currently set in the database for the WinSMTPServer.  Please go to File->Configure to set this up.",
						"Notice: No configuration exists yet",
						MessageBoxButtons.OK,
						MessageBoxIcon.Information);
					}
				}
			catch (Exception ee)
				{
				MessageBox.Show("An error occurred while attempting to open the database. " + 
					"Either the database server is not started OR " +
					"the App.Config for this application and for the WinSMTPServer service needs to have " +
					"a connectionstring added called SMTPDB.  The text of the actual exception is:\r\n\r\n" +
					ee.Message,
					"Error: Unable to load configuration",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);

				this.Visible = false;
				Application.Exit();
				}
			}


		/// <summary>
		/// Handles a request to configure the SMTP server.
		/// </summary>
		private void configureToolStripMenuItem_Click(object sender,EventArgs e)
			{
			(new ConfigurationForm()).ShowDialog();
			}


		/// <summary>
		/// Handles the exit request
		/// </summary>
		private void exitToolStripMenuItem_Click(object sender,EventArgs e)
			{
			Application.Exit();
			}

		
		/// <summary>
		/// Requests showing the activity window for today only.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void showTodaysActivityToolStripMenuItem_Click(object sender, EventArgs e)
			{
			ActivityViewForm activity = new ActivityViewForm(DateTime.Now);
			activity.ShowDialog();
			}


		/// <summary>
		/// Requests showing the activity window for the past week only.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void showLastWeeksActivityToolStripMenuItem_Click(object sender, EventArgs e)
			{
			DateTime now = DateTime.Now;
			ActivityViewForm activity = new ActivityViewForm(now.Subtract(new TimeSpan(7,0,0,0)),now);
			activity.ShowDialog();
			}


		/// <summary>
		/// Requests showing activity for the period specified by the user
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void selectActivityToShowToolStripMenuItem_Click(object sender, EventArgs e)
			{
			ActivityViewForm activity = new ActivityViewForm();
			activity.ShowDialog();
			}
		}
	}
