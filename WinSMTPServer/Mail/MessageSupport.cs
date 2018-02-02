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
using SupportLibrary.Database;
using System.Text;
using WinSMTPServer.SMTPClient;
using SupportLibrary.DataSource;


namespace WinSMTPServer.Mail
	{
	/// <summary>
	/// Container for utility functions used in mail
	/// handling.
	/// </summary>
	public class MessageSupport
		{
		/// <summary>
		/// Retrieves a unique identifier for this message.
		/// </summary>
		/// <param name="ConnectionID">The ID of the connection attached to the message.</param>
		/// <param name="UtcTime">The time of the message receipt.</param>
		/// <param name="SessionMessageCount">The message number (count) within the session.</param>
		/// <returns>The defined unique identifier.</returns>
		static public string GetNextMessageIdentifier(int ConnectionID,DateTime UtcTime,int SessionMessageCount)
			{
			int milliseconds = (int)(UtcTime.Ticks / 10000L);		// convert from nanoseconds to milliseconds
			return ConnectionID.ToString("x6") + "-" + milliseconds.ToString("x8") + "-" + SessionMessageCount.ToString("x4");
			}


		/// <summary>
		/// Attempts to creates a Postmaster message back to the 
		/// sender indicating the message is undeliverable.
		/// </summary>
		/// <param name="Header">The header of the message that is undeliverable.</param>
		/// <param name="Reason">The reason code for its undeliverability.</param>
		/// <param name="SMTPID">The SMTP ID of the message that cannot be delivered.</param>
		static public void QueueUndeliverableNotification(HeaderRecord Header,
			SMTPClientResult Reason,string SMTPID)
			{
			if (Header.Sender.Trim().Length == 0)		// already a postmaster message....
				return;

			int bodyID = -1;
			try
				{
				ConnectionRecord connection = new ConnectionRecord();
				connection.Remote = "localhost";
				connection.StartTime = DateTime.UtcNow;
				connection.EndTime = DateTime.UtcNow;
				connection.IsInbound = true;
				ConnectionDataSource.CreateRecord(connection);

				BodyRecord body = new BodyRecord();
				body.MessageIdentifier = MessageSupport.GetNextMessageIdentifier(connection.ID,connection.StartTime,1);
				body.HeaderText = "Return-Path: <>\r\n" +
					"From: Postmaster <>\r\n" +
					"To: <" + Header.Sender + ">\r\n" +
					"Subject: Undeliverable mail - returning to sender\r\n" +
					"Date: " + connection.StartTime.ToUniversalTime().ToString("r") + "\r\n" +
					"Message-ID: " + body.MessageIdentifier;

				string reason = "Unknown";
				switch (Reason)
					{
					case SMTPClientResult.ErrorOnServer:
						reason = "Error on remote server";
						break;
					case SMTPClientResult.ExceptionCaught:
						reason = "An exceptional condition was caught internally";
						break;
					case SMTPClientResult.FailedToConnect:
						reason = "Unable to connect to remote server";
						break;
					case SMTPClientResult.Expired:
						reason = "Expired attempting to send the message";
						break;
					case SMTPClientResult.InvalidRecipient:
						reason = "Remote server indicates the recipient is invalid";
						break;
					case SMTPClientResult.InvalidSender:
						reason = "Remote server indicates the sender is invalid or unacceptable";
						break;
					}

				StringBuilder text = new StringBuilder(2048);
				text.Append("I am sorry to inform you that your message with our SMTP ID of ");
				text.Append(SMTPID);
				text.Append(" cannot be delivered to the destination of ");
				text.Append(Header.Recipient);
				text.Append(". The message was received into our system at ");
				text.Append(Header.ReceivedDateTime.ToString("MM/dd/yyyy HH:mm:ss zzz"));
				text.Append("\r\n\r\n");
				text.Append("The reason for the failure is: ");
				text.Append(reason);
				text.Append("\r\n\r\nIf this is a rejection from the remote server or a server error, please check with their postmaster for more details.");
				text.Append("\r\n\r\n-The Postmaster at ");
				text.Append(Network.Services.GetLocalServerName());
				text.Append("-\r\n\r\n");
//				text.Append("Message Headers for failed message are:\r\n");
				body.BodyText = text.ToString();
				BodyDataSource.CreateRecord(body);
				bodyID = body.ID;

				HeaderRecord header = new HeaderRecord();
				header.Sender = string.Empty;
				header.Recipient = Header.Sender;
				header.ReceivedDateTime = connection.StartTime;
				header.RecvConnectionID = connection.ID;
				header.BodyID = body.ID;
				HeaderDataSource.CreateRecord(header);
				}
			catch (Exception ee)
				{
				SupportLibrary.Logger.SystemLogger.LogError("MessageSupport::QueueUndeliverableNotification()","Caught exception: " + ee);
				if (bodyID >= 0)
					{
					try
						{
						// remove the orphaned body....
						DALContainer.GetDALContainer().GetBodyDAL().DeleteRecord(bodyID);
						}
					catch (Exception)
						{
						}
					}
				}
			}
		}
	}
