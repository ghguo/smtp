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
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using WinSMTPServer.Server;
using WinSMTPServer.Client;
using SupportLibrary.Database;
using SupportLibrary.DataSource;
using System.Threading;

namespace WinSMTPServer
	{
	public partial class WinSMTPService : ServiceBase
		{
		private SMTPServer _server;
		private Forwarder _forwarder;

		public WinSMTPService()
			{
			InitializeComponent();

			// Make sure there logger source exists...this code matches RegisterEventSource.
			try
				{
				if (!EventLog.SourceExists(SupportLibrary.Logger.SystemLogger.EventSourceName))
					EventLog.CreateEventSource(SupportLibrary.Logger.SystemLogger.EventSourceName,SupportLibrary.Logger.SystemLogger.EventSourceName + "Log");
				}
			catch (Exception)
				{
				}
			}


		/// <summary>
		/// To support command-line startup.
		/// </summary>
		/// <param name="args"></param>
		public void Start(string [] args)
			{
			OnStart(args);
			while (true)
				{
				Thread.Sleep(60000);
				}
			}


		protected override void OnStart(string[] args)
			{
			try
				{
				SupportLibrary.Logger.SystemLogger.LogInfo("WinSMTPServer::OnStart()","Requested a start of the service.");

				// load up configuration and kick off server.....
				Configuration cfg = Configuration.GetConfiguration();

				CloseOpenSessions();

				SupportLibrary.Logger.SystemLogger.LogInfo("WinSMTPServer::OnStart()","Starting SMTP server.");
				_server = new SMTPServer(cfg.Port,cfg.AllowLocalHost,cfg.AllowRemote);


				SupportLibrary.Logger.SystemLogger.LogInfo("WinSMTPServer::OnStart()","Starting SMTP forwarder.");
				_forwarder = new Forwarder();
				}
			catch (Exception ee)
				{
				SupportLibrary.Logger.SystemLogger.LogError("WinSMTPService::OnStart()","Caught an exception: " + ee);
				}
			}

		protected override void OnStop()
			{
			if (_server != null)
				{
				SupportLibrary.Logger.SystemLogger.LogInfo("WinSMTPServer::OnStop()","Stopping SMTP server.");
				_server.Close();
				_server = null;
				}

			if (_forwarder != null)
				{
				SupportLibrary.Logger.SystemLogger.LogInfo("WinSMTPServer::OnStop()","Stopping SMTP forwarder.");
				_forwarder.Close();
				_forwarder = null;
				}
			}


		/// <summary>
		// Close out any open sessions....they were somehow abandoned in a previous run.
		/// </summary>
		private void CloseOpenSessions()
			{
			DateTime now = DateTime.UtcNow;
			IList<ConnectionRecord> records = ConnectionDataSource.ReadAllUnclosedRecords();
			foreach (ConnectionRecord record in records)
				{
				record.EndTime = now;
				ConnectionDataSource.UpdateRecord(record);
				}
			}
		}
	}
