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
using System.Text;
using SupportLibrary.Database;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using SupportLibrary.DataSource;
using System.IO;
using SupportLibrary.Util;
using WinSMTPServer.Protocol;
using WinSMTPServer.Session;
using WinSMTPServer.Mail;

namespace WinSMTPServer.Connection
	{
	/// <summary>
	/// An incoming SMTP session from a remote client is handled here.
	/// </summary>
	public class InboundSession : ISession, IDisposable
		{
		private bool _isTerminated = false;
		private DateTime _startTime = DateTime.UtcNow;

		private ConnectionRecord _connection;
		private BufferedStream _stream;
		private Thread _thread;
		private int _messageCount = 1;

		private SMTPProtocol _state;

		/// <summary>
		/// Creates a new localhost session from a client and logs it to the database.  
		/// Starts a thread to control the session.
		/// </summary>
		/// <param name="RemoteSocket">The socket connection to the client.</param>
		public InboundSession(Socket RemoteSocket)
			{
			CreateSession(RemoteSocket);
			_thread.Start();
			}


		/// <summary>
		/// Creates a new remote session from a client and logs it to the database.  
		/// Starts a thread to control the session.
		/// </summary>
		/// <param name="RemoteSocket">The socket connection to the client.</param>
		/// <param name="RemoteAddress">The remote client end address.</param>
		public InboundSession(Socket RemoteSocket,IPEndPoint RemoteAddress) 
			{
			CreateSession(RemoteSocket);

			// check the remote is acceptable after kicking off the session
// TODO: add centralized checker for allowed IPs			

			_thread.Start();
			}


		/// <summary>
		/// Internal shared "constructor" code.
		/// </summary>
		/// <param name="RemoteSocket">The socket connection to the client.</param>
		private void CreateSession(Socket RemoteSocket)
			{
			_stream = new BufferedStream(new NetworkStream(RemoteSocket,true));

			_connection = new ConnectionRecord();
			_connection.StartTime = DateTime.UtcNow;
			_connection.Remote = RemoteSocket.RemoteEndPoint.ToString();
			_connection.IsInbound = true;

			ConnectionDataSource.CreateRecord(_connection);

			_thread = new Thread(RunThread);
			_thread.IsBackground = true;
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

			CloseSession();
			}

		/// <summary>
		/// Internal method to close socket and record end time.
		/// It marks the session as terminated.
		/// </summary>
		private void CloseSession()
			{
			if (_stream != null)
				{
				_stream.Close();
				_stream = null;
				}

			if (_connection.EndTime == null)
				{
				_connection.EndTime = DateTime.UtcNow;
				
				try
					{
					ConnectionDataSource.UpdateRecord(_connection);
					}
				catch (Exception)
					{
					}
				}

			_isTerminated = true;
			}


		/// <summary>
		/// Writes a line of data to the other side and flushes it.
		/// </summary>
		/// <param name="Data">The data to write.</param>
		private void Write(string Data)
			{
			byte[] bytes = System.Text.Encoding.ASCII.GetBytes(Data);
			SendBytes(bytes);
			_stream.Flush();
			}


		/// <summary>
		/// Writes a line of data and appends a crlf to the other side and flushes it.
		/// </summary>
		/// <param name="Data">The data to write.</param>
		private void WriteLine(string Data)
			{
			byte [] bytes = System.Text.Encoding.ASCII.GetBytes(Data);
			SendBytes(bytes);
			SendBytes(new byte[] { 0xd, 0xa});
			_stream.Flush();
			}


		/// <summary>
		/// Transmits the specified bytes of data to the other side.
		/// </summary>
		/// <param name="ToSend"></param>
		private void SendBytes(byte[] ToSend)
			{
			_stream.Write(ToSend,0,ToSend.Length);
			}
		
		
		/// <summary>
		/// Reads the next line of data from the other end and blocks
		/// until a line has been read.
		/// </summary>
		/// <returns>A string containing the contents of the string MINUS cr and lf.</returns>
		private String ReadLine()
			{
			ByteBuilder bb = new ByteBuilder(128);

			while (true)
				{
				int b = _stream.ReadByte();
				if (b < 0)
					break;

				if (b == 0xa)
					break;
				else if (b != 0xd)
					bb.Append((byte)b);
				}
			return bb.ToString();
			}


		/// <summary>
		/// Attempts to save the message data to the database and steps back when done.
		/// </summary>
		/// <param name="Data">The SMTP data to save.</param>
		/// <returns>true if successfully saved or false if failure.</returns>
		private bool SaveMessageData(SMTPData Data)
			{
			DateTime now = DateTime.UtcNow;

			BodyRecord bodyRecord = new BodyRecord();
			bodyRecord.MessageIdentifier = MessageSupport.GetNextMessageIdentifier(_connection.ID,now,_messageCount++);

			StringBuilder header = new StringBuilder(Data.Headers.Length + 256);
			header.Append("Received: by ");
			header.Append(Configuration.GetConfiguration().HostName);
			header.Append(" (WinSMTPServer " + SupportLibrary.Util.Version.VersionString + ") ");
			header.Append(" with " + (Data.IsExtendedHello ? "ESMTP" : "SMTP") + " id ");
			header.Append(bodyRecord.MessageIdentifier);
			header.Append(";\r\n");
			header.Append("        ");
			header.Append(now.ToUniversalTime().ToString("r"));
			header.Append("\r\n");
			header.Append(Data.Headers);

			bodyRecord.HeaderText = header.ToString();
			bodyRecord.BodyText = Data.Body;

			List<int> headerIDs = new List<int>();

			try
				{
				BodyDataSource.CreateRecord(bodyRecord);

				foreach (string recipient in Data.Recipients)
					{
					HeaderRecord headerRecord = new HeaderRecord();
					headerRecord.BodyID = bodyRecord.ID;
					headerRecord.ReceivedDateTime = now;
					headerRecord.Sender = Data.Sender;
					headerRecord.Recipient = recipient;
					headerRecord.RecvConnectionID = _connection.ID;

					HeaderDataSource.CreateRecord(headerRecord);
					headerIDs.Add(headerRecord.ID);
					}
				}
			catch (Exception ee)
				{
				SupportLibrary.Logger.SystemLogger.LogError("InboundSession::SaveMessageData()","Caught exception writing message: " + ee);

				// roll back the body and headers...
				try
					{
					if (bodyRecord.ID != DatabaseSupport.InvalidID)
						BodyDataSource.DeleteRecord(bodyRecord.ID);

					foreach (int headerID in headerIDs)
						HeaderDataSource.DeleteRecord(headerID);
					}
				catch (Exception)
					{
					}
				return false;
				}

			return true;
			}

		
		/// <summary>
		/// Handles the session with the remote client.
		/// </summary>
		private void RunThread()
			{
			try
				{
				_state = new SMTPStartProtocol();
				WriteLine("220 SMTP server is ready.  Please state your business.");

				while (true)
					{
					string line = ReadLine();

					try
						{
						_state = _state.ProcessLine(line,WriteLine);

						if (_state == null)		// ended with a quit
							break;
						else if (_state is SMTPMessageDoneProtocol)
							{
							SMTPMessageDoneProtocol doneState = (SMTPMessageDoneProtocol)_state;
							if (SaveMessageData(doneState.Data))
								_state = doneState.HandleSuccess(WriteLine);
							else
								_state = doneState.HandleFailure(WriteLine);
							}
						}
					catch (ThreadInterruptedException)
						{
						break;
						}
					}
				}
			catch (Exception ee)
				{
				SupportLibrary.Logger.SystemLogger.LogError("InboundSession::RunThread()","Caught exception: " + ee);
				}

			CloseSession();
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
				return _startTime;
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
