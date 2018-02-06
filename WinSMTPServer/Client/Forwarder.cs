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
using System.Linq;
using System.Text;
using System.Threading;
using SupportLibrary.DataSource;
using SupportLibrary.Database;
using WinSMTPServer.Session;
using WinSMTPServer.SMTPClient;
using WinSMTPServer.Mail;

namespace WinSMTPServer.Client
	{
	/// <summary>
	/// The purpose of this class is to support the Forward
	/// part of the Store-and-Forward SMTP mechanism.  It 
	/// looks for messages to send and attempts to transmit 
	/// them.
	/// </summary>
	public class Forwarder
		{
		private const int ScanTimeoutSeconds = 30;

		private Thread _thread;

		public Forwarder()
			{

			_thread = new Thread(RunThread);
			_thread.IsBackground = true;
			_thread.Start();
			}


		/// <summary>
		/// Requests closing the socket and exiting the SMTP server.
		/// </summary>
		public void Close()
			{
			if (_thread != null)
				{
				_thread.Interrupt();
				_thread.Join(10000);
				_thread = null;
				}
			}
		
		
		/// <summary>
		/// This is the central thread routine that monitors and
		/// directs the system to forward out Emails to SMTP servers.
		/// </summary>
		private void RunThread()
			{
			try
				{
				while (true)
					{
					DateTime now = DateTime.UtcNow;

					// first to decide if we have and messages to autodelete....
					CleanupOldMail(now,Configuration.GetConfiguration().CleanupDays);

					// second, take any messages beyond expiration date out of play...
					ExpireMessages(now,Configuration.GetConfiguration().ExpireAfterMinutes);

					// now to determine if we have any candidates to kick off....
					LaunchPendingDeliveries(now,Configuration.GetConfiguration().RetryAfterMinutes,Configuration.GetConfiguration().ExpireAfterMinutes);

					Thread.Sleep(ScanTimeoutSeconds * 1000);
					}
				}
			catch (ThreadInterruptedException)
				{
				}
			catch (Exception ee)
				{
				SupportLibrary.Logger.SystemLogger.LogError("SMTPServer::RunThread()","Caught exception: " + ee);
				}
			}


		/// <summary>
		/// Looks for old mail and deletes it.
		/// </summary>
		/// <param name="UtcTime">Start time (now) in UTC</param>
		/// <param name="CleanupDays">The number of days to clean up.</param>
		private void CleanupOldMail(DateTime UtcTime,int CleanupDays)
			{
			DateTime limitDate = UtcTime.Subtract(new TimeSpan(CleanupDays,0,0,0));
			IList<HeaderRecord> oldHeaders = HeaderDataSource.ReadAllRecordsReceivedBefore(limitDate);
			foreach (HeaderRecord header in oldHeaders)
				{
				BodyRecord body = null;
				if (header.BodyID != null)
					body = BodyDataSource.ReadRecord((int)header.BodyID);

				if (header.DeliveredDateTime == null || header.ExpiredDateTime == null)
					SupportLibrary.Logger.SystemLogger.LogError("Forwarder.CleanupOldMail()","Deleting message ID " + header.ID + " which was not delivered.");

				IList<DeliveryAttemptRecord>  oldDeliveries = DeliveryAttemptDataSource.ReadAllRecordsByHeaderID(header.ID);
				foreach (DeliveryAttemptRecord delivery in oldDeliveries)
					{
					DeliveryAttemptDataSource.DeleteRecord(delivery.ID);
					}

				HeaderDataSource.DeleteRecord(header.ID);
				if (body != null)
					BodyDataSource.DeleteRecord(body.ID);
				}
			}


		/// <summary>
		/// Looks for old mail and expire it.
		/// </summary>
		/// <param name="UtcTime">Start time (now) in UTC</param>
		/// <param name="ExpireAfterMinutes">The number of days to clean up.</param>
		private void ExpireMessages(DateTime UtcTime,int ExpireAfterMinutes)
			{
			DateTime now = DateTime.UtcNow;

			DateTime limitDate = UtcTime.Subtract(new TimeSpan(0,ExpireAfterMinutes,0));
			IList<HeaderRecord> expireHeaders = HeaderDataSource.ReadAllRecordsReceivedBefore(limitDate);
			foreach (HeaderRecord header in expireHeaders)
				{
				if (header.DeliveredDateTime == null || header.ExpiredDateTime == null)
					SupportLibrary.Logger.SystemLogger.LogError("Forwarder.ExpireMessage()","Expiring message ID " + header.ID + " which was not delivered.");

				header.ExpiredDateTime = now;
				HeaderDataSource.UpdateRecord(header);
				}
			}


		/// <summary>
		/// Kicks off delivery threads to handle any messages that 
		/// need to be sent out.
		/// </summary>
		/// <param name="UtcTime">Start time (now) in UTC</param>
		/// <param name="RetryAfterMinutes">Retry timeout period.</param>
		/// <param name="ExpireAfterMinutes">Expiration timeout period.</param>
		private void LaunchPendingDeliveries(DateTime UtcTime,int RetryAfterMinutes,int ExpireAfterMinutes)
			{
			DateTime retryTime = UtcTime.Subtract(new TimeSpan(0,RetryAfterMinutes,0));
			DateTime expireTime = UtcTime.Subtract(new TimeSpan(0,ExpireAfterMinutes,0));

			IList<HeaderRecord> candidates = HeaderDataSource.ReadAllCandidateRecords(retryTime);
			foreach (HeaderRecord candidate in candidates)
				{
				//if (candidate.ReceivedDateTime <= expireTime)
				//    {
				//    SupportLibrary.Logger.SystemLogger.LogError("Forwarder.LaunchPendingDeliveries()","Expiring message ID " + candidate.ID + " which was not delivered.");

				//    candidate.ExpiredDateTime = UtcTime;
				//    HeaderDataSource.UpdateRecord(candidate);
				//    continue;
				//    }

				if (candidate.BodyID == null)
					{
					SupportLibrary.Logger.SystemLogger.LogError("Forwarder.LaunchPendingDeliveries()","Expiring message ID " + candidate.ID + " which was has no body record.");

					candidate.ExpiredDateTime = UtcTime;
					HeaderDataSource.UpdateRecord(candidate);
					continue;
					}

				BodyRecord body = BodyDataSource.ReadRecord((int)candidate.BodyID);
				if (body == null)
					continue;

				SessionManager.AddOutboundSession(candidate,body);
				}
			}
		}
	}
