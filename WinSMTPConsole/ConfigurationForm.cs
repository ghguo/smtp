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
using SupportLibrary.Database;
using SupportLibrary.DataSource;
using System.Text.RegularExpressions;

namespace WinSMTPConsole
	{
	public partial class ConfigurationForm : Form
		{
		private bool _isNewConfiguration = false;
		private ConfigurationRecord _configuration;
		private List<InternalAllowedRecord> _allowed = new List<InternalAllowedRecord>();
		private IList<InternalAllowedRecord> _deleted = new List<InternalAllowedRecord>();

		public ConfigurationForm()
			{
			InitializeComponent();

			gvAllowed.AutoGenerateColumns = false;

			try
				{
				_configuration = ConfigurationDataSource.ReadRecord(ConfigurationRecord.MasterConfigurationID);
				if (_configuration == null)
					{
					_isNewConfiguration = true;
					_configuration = new ConfigurationRecord();
					_configuration.ID = ConfigurationRecord.MasterConfigurationID;
					_configuration.Port = 25;
					_configuration.AllowLocalHost = true;
					_configuration.RetryAfterMinutes = 15;
					_configuration.ExpireAfterMinutes = 7200;		// 5 days
					_configuration.CleanupDays = 15;
					}

				bool wasRemoved = false;
				foreach (AllowedRecord allowed in AllowedDataSource.ReadAllRecords())
					{
					try
						{
						_allowed.Add(new InternalAllowedRecord(allowed, ConvertTextToIP(allowed.IP), ConvertTextToIP(allowed.Subnet)));
						}
					catch (FormatException)
						{
						AllowedDataSource.DeleteRecord(allowed.ID);		// delete the invalid record!
						wasRemoved = true;
						}
					}

				SortAllowedList();

				// prepare screen....
				gvAllowed.DataSource = _allowed;
				txtPort.Text = _configuration.Port.ToString();
				chkAllowRemote.Checked = _configuration.AllowRemote;
				chkLocalHost.Checked = _configuration.AllowLocalHost;
				txtExpire.Text = _configuration.ExpireAfterMinutes.ToString();
				txtRetry.Text = _configuration.RetryAfterMinutes.ToString();
				txtCleanup.Text = _configuration.CleanupDays.ToString();

				if (wasRemoved)
					MessageBox.Show("One (or more) allowed IP entries were invalid and have been removed from the database.",
						"Invalid format in allowed IP",
						MessageBoxButtons.OK,
						MessageBoxIcon.Exclamation);
				}
			catch (Exception ee)
				{
				MessageBox.Show("An error occurred while attempting to open the database to load the configuration data. " +
					"The text of the actual exception is:\r\n\r\n" + ee.Message,
					"Error: Unable to load configuration",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);

				this.Close();
				}
			}


		/// <summary>
		/// Converts a text form of an IP into an actual 32-bit number.
		/// </summary>
		/// <param name="IPText">The text form of the IP.</param>
		/// <returns>The converted IP adddress.</returns>
		/// <exception cref="FormatException">If the text is invalidly formatted</exception>
		private uint ConvertTextToIP(string IPText)
			{
			if (IPText.Trim().Length == 0)
				throw new FormatException("Invalid IP address");

			string pattern = @"\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b";
            Regex check = new Regex(pattern);
			if (!check.IsMatch(IPText,0))
				throw new FormatException("Not a dotted quad format.");

			uint ip = 0;
			string [] parts = IPText.Split(".".ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
			foreach (string part in parts)
				{
				uint octet = Convert.ToUInt32(part);
				ip = (ip << 8) | octet;
				}
			return ip;
			}


		/// <summary>
		/// Sorts the current allowed list in ascending IP order.
		/// </summary>
		private void SortAllowedList()
			{
			_allowed.Sort((a,b) =>
			{
				if (a.IPValue == b.IPValue)
					{
					if (a.SubnetValue == b.SubnetValue)
						return 0;
					return (a.SubnetValue > b.SubnetValue) ? 1 : -1;
					}
				else if (a.IPValue > b.IPValue)
					return 1;
				return -1;
			});
			}

		/// <summary>
		/// Handles a request to delete an IP from the list of Allowed IPs
		/// </summary>
		private void btnDeleteIP_Click(object sender, EventArgs e)
			{
			if (gvAllowed.SelectedRows.Count == 0)
				return;

			InternalAllowedRecord record = gvAllowed.SelectedRows[0].DataBoundItem as InternalAllowedRecord;
			if (record == null)
				return;

			if (MessageBox.Show("Are you CERTAIN you want to delete the IP " + record.IP + "/" + record.Subnet + "?",
				"Confirm delete of IP",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question) == DialogResult.No)
				return;

			if (record.ID != DatabaseSupport.InvalidID)
				_deleted.Add(record);
			_allowed.Remove(record);

			gvAllowed.DataSource = null;
			gvAllowed.DataSource = _allowed;
			}


		/// <summary>
		/// Handles a request to clear all IPs from the list of Allowed IPs
		/// </summary>
		private void btnClearIP_Click(object sender, EventArgs e)
			{
			if (MessageBox.Show("Are you CERTAIN you want to remove all the currently configured IPs?",
				"Confirm removing IPs",
				MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.No)
				return;

			_allowed.Clear();
			gvAllowed.DataSource = null;
			gvAllowed.DataSource = _allowed;
			}


		/// <summary>
		/// Handles a request to add a new IP/subnet to the list of Allowed IPs
		/// </summary>
		private void btnAddIP_Click(object sender, EventArgs e)
			{
			// force validators to run...
			newIP.Focus();
			newSubnet.Focus();
			btnAddIP.Focus();


			if (errorProvider1.GetError(newIP) != string.Empty ||
				errorProvider1.GetError(newSubnet) != string.Empty)
				return;

			UInt32 ip = ConvertTextToIP(newIP.Text);
			UInt32 subnet = ConvertTextToIP(newSubnet.Text);
			foreach (InternalAllowedRecord allowed in _allowed)
				{
				if (allowed.IPValue == ip && allowed.SubnetValue == subnet)
					return;
				}

			InternalAllowedRecord record = new InternalAllowedRecord(newIP.Text,newSubnet.Text,ip,subnet);

			_allowed.Add(record);
			SortAllowedList();

			newIP.Text = string.Empty;
			newSubnet.Text = string.Empty;
			newIP.Focus();

			gvAllowed.DataSource = null;
			gvAllowed.DataSource = _allowed;
			}


		/// <summary>
		/// Requests saving the configuration settings and exiting
		/// </summary>
		private void btnOK_Click(object sender, EventArgs e)
			{
			if (errorProvider1.GetError(txtCleanup) != string.Empty ||
				errorProvider1.GetError(txtExpire) != string.Empty ||
				errorProvider1.GetError(txtPort) != string.Empty ||
				errorProvider1.GetError(txtRetry) != string.Empty)
				return;

			if (MessageBox.Show("Are you certain you wish to save these changes and then exit?", "Confirm saving changes",
				MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
				if (chkAllowRemote.Checked == false && chkLocalHost.Checked == false)
					{
					if (MessageBox.Show("Warning! Are you sure you want to save this configuration which does not allow local nor remote connections?",
						"Confirm strange configuration",
						MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.No)
						return;
					}

				_configuration.Port = Convert.ToInt16(txtPort.Text);
				_configuration.RetryAfterMinutes = Convert.ToInt32(txtRetry.Text);
				_configuration.ExpireAfterMinutes = Convert.ToInt32(txtExpire.Text);
				_configuration.CleanupDays = Convert.ToInt32(txtCleanup.Text);
				_configuration.AllowLocalHost = chkLocalHost.Checked;
				_configuration.AllowRemote = chkAllowRemote.Checked;

				try
					{
					if (_isNewConfiguration)
						DALContainer.GetDALContainer().GetConfigurationDAL().NewRecord(_configuration);
					else
						DALContainer.GetDALContainer().GetConfigurationDAL().UpdateRecord(_configuration);
					}
				catch (Exception ee)
					{
					MessageBox.Show("An error occurred while attempting to save the configuration data.  The text of the error is:\r\n\r\n" + ee.Message,
						"Error while saving",
						MessageBoxButtons.OK,
						MessageBoxIcon.Exclamation);
					return;
					}

				int saveErrors = 0;
				foreach (InternalAllowedRecord deleted in _deleted)
					{
					try
						{
						DALContainer.GetDALContainer().GetAllowedDAL().DeleteRecord(deleted.ID);
						}
					catch (Exception)
						{
						++saveErrors;
						}
					}

				foreach (InternalAllowedRecord record in _allowed)
					{
					try
						{
						if (record.ID == DatabaseSupport.InvalidID)
							DALContainer.GetDALContainer().GetAllowedDAL().CreateRecord(record);
						}
					catch (Exception)
						{
						++saveErrors;
						}
					}

				if (saveErrors > 0)
					MessageBox.Show("There were errors deleting/adding IP records in your allowed list.  Please check them by selecting the configuration option again.",
						"Errors while saving IPs",
						MessageBoxButtons.OK,
						MessageBoxIcon.Exclamation);

				this.Close();
				}
			}

		/// <summary>
		/// Requests exiting immediately.
		/// </summary>
		private void btnCancel_Click(object sender, EventArgs e)
			{
			if (MessageBox.Show("Are you certain you wish to exit.  All changes to this configuration will be lost.", "Confirm aborting changes",
				MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				this.Close();
			}


		private void txtPort_Validating(object sender,CancelEventArgs e)
			{
			bool isSuccess = false;
			try
				{
				int port = Convert.ToInt32(txtPort.Text.Trim());
				if (port > 0 && port < 0x10000)
					isSuccess = true;
				}
			catch (Exception)
				{
				}

			if (isSuccess)
				errorProvider1.SetError(txtPort,"");
			else
				errorProvider1.SetError(txtPort,"A port must be a positive value 1-65535");
			}


		private void txtRetry_Validating(object sender,CancelEventArgs e)
			{
			bool isSuccess = false;
			try
				{
				int retry = Convert.ToInt32(txtRetry.Text.Trim());
				if (retry > 0 && retry < 1440)
					isSuccess = true;
				}
			catch (Exception)
				{
				}

			if (isSuccess)
				errorProvider1.SetError(txtRetry,"");
			else
				errorProvider1.SetError(txtRetry,"A retry period can be 1-1440 minutes");
			}


		private void txtExpire_Validating(object sender,CancelEventArgs e)
			{
			bool isSuccess = false;
			try
				{
				int expire = Convert.ToInt32(txtExpire.Text.Trim());
				if (expire > 0 && expire <= 20160)			// 2 weeks
					isSuccess = true;
				}
			catch (Exception)
				{
				}

			if (isSuccess)
				errorProvider1.SetError(txtExpire,"");
			else
				errorProvider1.SetError(txtExpire,"Message can be expired as undeliverable 10 minutes or longer");
			}


		private void txtCleanup_Validating(object sender,CancelEventArgs e)
			{
			bool isSuccess = false;
			try
				{
				int cleanup = Convert.ToInt32(txtCleanup.Text.Trim());
				if (cleanup > 0 && cleanup <= 90)
					isSuccess = true;
				}
			catch (Exception)
				{
				}

			if (isSuccess)
				errorProvider1.SetError(txtCleanup,"");
			else
				errorProvider1.SetError(txtCleanup,"Messages can be cleaned up 1-90 days");
			}


		/// <summary>
		/// Checks IP and subnet for validity before saving.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnAddIP_Validating(object sender,CancelEventArgs e)
			{

			}


		/// <summary>
		/// Checks the IP field for validity...
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void newIP_Validating(object sender,CancelEventArgs e)
			{
			bool isValid = false;

			try
				{
				ConvertTextToIP(newIP.Text);
				isValid = true;
				}
			catch (Exception)
				{
				}

			if (isValid)
				errorProvider1.SetError(newIP,"");
			else
				errorProvider1.SetError(newIP,"An IP must be x.x.x.x where x can be 0-255.");
			}


		/// <summary>
		/// Handles setting the subnet if empty.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void newIP_Leave(object sender,EventArgs e)
			{
			if (newIP.Text.Trim().Length > 0 && newSubnet.Text.Trim().Length == 0)
				newSubnet.Text = "255.255.255.255";
			}

	
		/// <summary>
		/// Checks the subnet field for validity...
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void newSubnet_Validating(object sender,CancelEventArgs e)
			{
			bool isValid = false;

			try
				{
				ConvertTextToIP(newSubnet.Text);
				isValid = true;
				}
			catch (Exception)
				{
				}

			if (isValid)
				errorProvider1.SetError(newIP,"");
			else
				errorProvider1.SetError(newIP,"A subnet must be x.x.x.x where x can be 0-255.");
			}
		}


	/// <summary>
	/// A simple internal class holder for 
	/// IP/subnet
	/// </summary>
	class InternalAllowedRecord : AllowedRecord
		{
		public InternalAllowedRecord(int ID,string IP,string Subnet,UInt32 IPValue,UInt32 SubnetValue)
			{
			this.ID = ID;
			this.IP = IP;
			this.Subnet = Subnet;
			this.IPValue = IPValue;
			this.SubnetValue = SubnetValue;
			}

		public InternalAllowedRecord(string IP, string Subnet, UInt32 IPValue,UInt32 SubnetValue)
			: this(DatabaseSupport.InvalidID,IP,Subnet,IPValue,SubnetValue)
			{
			}

		public InternalAllowedRecord(AllowedRecord Record,UInt32 IPValue,UInt32 SubnetValue)
			: this(Record.ID, Record.IP, Record.Subnet, IPValue,SubnetValue)
			{
			}

		public UInt32 IPValue
			{
			set;
			get;
			}

		public UInt32 SubnetValue
			{
			set;
			get;
			}

		public AllowedRecord AllowedRecord
			{
			get
				{
				return (AllowedRecord)this;
				}
			}
		}
	}
