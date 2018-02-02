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
using WinSMTPConsole.Activity;

namespace WinSMTPConsole
	{
	public partial class ActivityViewForm : Form
		{
		delegate List<ActivityRecord> SearchActivityRecordsDelegate(DateTime? StartUTC,DateTime? EndUTC,
			bool SearchSessionDate,bool SearchReceivedDate,bool SearchDeliveredDate,
			bool SearchExpiredDate);
		private SearchActivityRecordsDelegate _delegate = new SearchActivityRecordsDelegate(ActivitySearch.FindRecords);
		private IAsyncResult _asyncResult;

		public ActivityViewForm()
			{
			InitializeComponent();

			gvResults.AutoGenerateColumns = false;

			DateTime now = DateTime.Now;
			DateTime start = new DateTime(now.Year,now.Month,now.Day);
			dtpAfterDate.Value = start;

			dtpDateBefore.Value = start.AddDays(1);
			}


		public ActivityViewForm(DateTime DateToSearch) : this()
			{
			DateTime start = new DateTime(DateToSearch.Year,DateToSearch.Month,DateToSearch.Day);
			dtpAfterDate.Value = start;

			DateTime end = start.AddDays(1);
			dtpDateBefore.Value = end;

			StartSearch();
			}


		public ActivityViewForm(DateTime StartSearchDate,DateTime EndSearchDate) : this()
			{
			DateTime start = new DateTime(StartSearchDate.Year,StartSearchDate.Month,StartSearchDate.Day);
			dtpAfterDate.Value = start;

			DateTime end = new DateTime(EndSearchDate.Year,EndSearchDate.Month,EndSearchDate.Day);
			dtpDateBefore.Value = end;

			StartSearch();
			}


		/// <summary>
		/// Requests a find operation for log entries.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnFind_Click(object sender,EventArgs e)
			{
			StartSearch();
			}


		/// <summary>
		/// Requests a close operation for the window.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnClose_Click(object sender,EventArgs e)
			{
			this.Close();
			}


		/// <summary>
		/// Kicks off a search for matching items using an 
		/// asynchronous delegate thread.
		/// </summary>
		private void StartSearch()
			{
			DateTime startUTC = dtpAfterDate.Value.ToUniversalTime();
			DateTime endUTC = dtpDateBefore.Value.ToUniversalTime();

			if (startUTC > endUTC)
				{
				DateTime temp = startUTC;
				startUTC = endUTC;
				endUTC = temp;
				}

			btnClose.Enabled = false;
			btnFind.Enabled = false;

			_asyncResult = _delegate.BeginInvoke(startUTC,endUTC,chkSessions.Checked,chkReceived.Checked,chkDelivered.Checked,
				chkExpired.Checked,null,null);

			timer1.Enabled = true;
			}


		/// <summary>
		/// Handles checking search is finished or not....
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void timer1_Tick(object sender,EventArgs e)
			{
			if (_asyncResult.IsCompleted)
				{
				timer1.Enabled = false;

				List<ActivityRecord> records = _delegate.EndInvoke(_asyncResult);

				gvResults.DataSource = records;
				btnClose.Enabled = true;
				btnFind.Enabled = true;
				}
			}
		}
	}
