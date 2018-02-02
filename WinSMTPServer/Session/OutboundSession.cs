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
using SupportLibrary.Database;
using SupportLibrary.DataSource;
using DnDns.Query;
using DnDns.Enums;
using System.Net.Sockets;
using DnDns.Records;
using WinSMTPServer.Network;
using System.Net;
using WinSMTPServer.SMTPClient;
using WinSMTPServer.Client;
using WinSMTPServer.Mail;

namespace WinSMTPServer.Session
	{
	/// <summary>
	/// Handles an outbound session to send messages out
	/// </summary>
	public class OutboundSession : ISession, IDisposable
		{
		private ConnectionRecord _connection;
		private DeliveryAttemptRecord _deliveryAttempt;
		private HeaderRecord _header;
		private BodyRecord _body;
		private string _domain;

		private Thread _thread;
		private bool _isTerminated = false;

		/// <summary>
		/// Creates a new transmit session for a message. Logs the connection to the
		/// database and starts a thread to control the session.
		/// </summary>
		/// <param name="Header">The header for the message to transmit.</param>
		/// <param name="Body">The body for the message to transmit.</param>
		public OutboundSession(HeaderRecord Header,BodyRecord Body)
			{
			_header = Header;
			_body = Body;
			_domain = Header.Recipient.Substring(Header.Recipient.IndexOf("@") + 1);

			_connection = new ConnectionRecord();
			_connection.Remote = _domain;
			_connection.StartTime = DateTime.UtcNow;
			_connection.IsInbound = false;
			ConnectionDataSource.CreateRecord(_connection);

			// preslug the delivery attempt information for writeout later...
			_deliveryAttempt = new DeliveryAttemptRecord();
			_deliveryAttempt.ConnectionID = _connection.ID;
			_deliveryAttempt.HeaderID = Header.ID;
			_deliveryAttempt.IsSuccess = false;
			_deliveryAttempt.AttemptDateTime = _connection.StartTime;
			DeliveryAttemptDataSource.CreateRecord(_deliveryAttempt);

			_thread = new Thread(RunThread);
			_thread.IsBackground = true;
			_thread.Start();
			}


		/// <summary>
		/// Requests closing the session.
		/// </summary>
		public void Close()
			{
			if (_thread != null)
				{
				_thread.Interrupt();
				_thread.Join(10000);
				_thread = null;
				}

			if (_deliveryAttempt.ID == DatabaseSupport.InvalidID)
				{
				_deliveryAttempt.IsSuccess = false;
				_deliveryAttempt.FailureReason = "Forced to cancel";
				DeliveryAttemptDataSource.CreateRecord(_deliveryAttempt);
				}

			_isTerminated = true;
			}

		
		/// <summary>
		/// Handles the session with the remote client.
		/// </summary>
		private void RunThread()
			{
			try
				{
				// snag a list of Nameserver (NS) addresses
				IPAddress[] nsAddresses = DNSFetcher.GetDnsServers();
				if (nsAddresses.Length == 0)
					{
					SupportLibrary.Logger.SystemLogger.LogError("OutboundSession::RunThread()","Nameserver addresses cannot be located.  Maybe network is currently disconnected.");

					_deliveryAttempt.IsSuccess = false;
					_deliveryAttempt.FailureReason = "Nameserver addresses cannot be located. Network may be disconnected.";
					return;
					}
				else
					{
					// get the MX for the domain -- note that the
					// DNS logic built on top of DnDns library from http://dndns.codeplex.com/
					// since Microsoft's resolver does not handle other types like MX!
					MXRecord[] mxList = null;
					foreach (IPAddress ns in nsAddresses)
						{
						mxList = DNSOperations.DeriveMailExchangers(ns.ToString(),_domain);
						if (mxList != null && mxList.Length > 0)
							break;
						}
					if (mxList == null || mxList.Length == 0)
						{
						// mark the failure reason and exit....
						_deliveryAttempt.IsSuccess = false;
						_deliveryAttempt.FailureReason = "Unable to locate any MX for domain " + _domain;
						return;
						}

					// now to attempt to deliver the message....
					bool isSuccess = false;
					foreach (MXRecord mx in mxList)
						{
						_connection.Remote = mx.ServerName;
						_deliveryAttempt.MXAddress = mx.ServerName;

						ConnectionDataSource.UpdateRecord(_connection);
						SupportLibrary.Logger.SystemLogger.LogInfo("OutboundSession::RunThread()","Attempt to deliver msg #" + _header.ID + " for " + _header.Recipient + " to MX of " + mx.ServerName);

						try
							{
							SMTPClient.SMTPClient client = new SMTPClient.SMTPClient(mx.ServerName);
							SMTPClientResult result = client.Send(_header.Sender,_header.Recipient,_body.FullText);
							if (result == SMTPClientResult.SuccessfulSend)
								{
								isSuccess = true;
								break;
								}
							else if (result == SMTPClientResult.InvalidSender)
								{
								isSuccess = false;
								_deliveryAttempt.FailureReason = "The MX server for " + _domain + " rejected mail from " + _header.Sender;
								MessageSupport.QueueUndeliverableNotification(_header,result,_body.MessageIdentifier);

								_header.SendConnectionID = _connection.ID;
								_header.DeliveredDateTime = DateTime.UtcNow;
								break;
								}
							else if (result == SMTPClientResult.InvalidRecipient)
								{
								isSuccess = false;
								_deliveryAttempt.FailureReason = "The MX server for " + _domain + " rejected mail to " + _header.Recipient;
								MessageSupport.QueueUndeliverableNotification(_header,result,_body.MessageIdentifier);

								_header.SendConnectionID = _connection.ID;
								_header.DeliveredDateTime = DateTime.UtcNow;
								break;
								}
							}
						catch (Exception ee)
							{
							SupportLibrary.Logger.SystemLogger.LogError("OutboundSession::RunThread()","Exception caught while attempting to deliver msg #" + _header.ID + " for " + _header.Recipient + " to MX of " + mx.ServerName + " for domain " + _domain + ": " + ee.Message);
							}
						}

					if (isSuccess)
						{
						_deliveryAttempt.IsSuccess = true;
						_deliveryAttempt.FailureReason = null;

						_header.SendConnectionID = _connection.ID;
						_header.DeliveredDateTime = DateTime.UtcNow;
						}
					else
						{
						_deliveryAttempt.IsSuccess = false;
						if (_deliveryAttempt.FailureReason == null)
							_deliveryAttempt.FailureReason = "Error connecting to any MX server for " + _domain;
						}
					}
				}
			catch (Exception ee)
				{
				SupportLibrary.Logger.SystemLogger.LogError("OutboundSession::RunThread()","Caught exception: " + ee);

				_deliveryAttempt.IsSuccess = false;
				_deliveryAttempt.FailureReason = "Exception caught: " + ee.GetType().ToString();
				}
			finally
				{
				DeliveryAttemptDataSource.UpdateRecord(_deliveryAttempt);
				HeaderDataSource.UpdateRecord(_header);
				}
			}


		#region ISession Members

		public int ConnectionID
			{
			get
				{
				return _connection.ID;
				}
			}

		public DateTime ConnectionStartTime
			{
			get
				{
				return _connection.StartTime;
				}
			}

		public bool IsConnectionTerminated
			{
			get
				{
				return _isTerminated;
				}
			}

		public void TerminateConnection()
			{
			Close();
			}

		#endregion



		#region IDisposable Members

		public void Dispose()
			{
			Close();
			}

		#endregion
		}
	}
