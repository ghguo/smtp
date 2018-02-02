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
using System.Net.Sockets;
using System.Net;
using System.Threading;
using WinSMTPServer.Session;

namespace WinSMTPServer.Server
	{
	/// <summary>
	/// The listener for SMTP connections itself.  When 
	/// connections occur, they are handed off to the SessionManager.
	/// The SMTPServer spawns its own management thread.
	/// </summary>
	public class SMTPServer : IDisposable
		{
		private TcpListener _listener;
		private bool _allowLocalHost;
		private bool _allowRemote;
		private Thread _thread;

		/// <summary>
		/// Kicks off the SMTPServer with the specific parameters.
		/// </summary>
		/// <param name="Port">The port to listen on.</param>
		/// <param name="AllowLocalHost">True if localhost connections are permitted.</param>
		/// <param name="AllowRemote">True if remote connections are permitted (checked against Allowed table).</param>
		/// <exception cref="InvalidOperationException">If an invalid configuration is provided.</exception>
		/// <exception cref="SocketException">If an error occurs attempting to bind to the specified port.</exception>
		public SMTPServer(int Port, bool AllowLocalHost, bool AllowRemote)
			{
			if (!AllowLocalHost && !AllowRemote)
				throw new InvalidOperationException("Both localhost and remote connections are prohibited.");

			_allowLocalHost = AllowLocalHost;
			_allowRemote = AllowRemote;

			if (AllowLocalHost && !AllowRemote)
				_listener = new TcpListener(Dns.GetHostAddresses("127.0.0.1")[0], Port);
			else
				_listener = new TcpListener(Port);

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

			if (_listener != null)
				{
				_listener.Stop();
				_listener = null;
				}
			}


		/// <summary>
		/// This is the central thread routine that monitors the
		/// TcpListener and drives the connection-driven session pump.
		/// </summary>
		private void RunThread()
			{
			try
				{
				_listener.Start(5);
				
				while (true)
					{
					try
						{
						Socket socket = _listener.AcceptSocket();
						if (socket != null)
							{
							SupportLibrary.Logger.SystemLogger.LogInfo("SMTPServer::RunThread()","Received connection from " + socket.RemoteEndPoint.ToString());
							IPEndPoint endPoint = socket.RemoteEndPoint as IPEndPoint;
							if (endPoint == null)
								{
								SupportLibrary.Logger.SystemLogger.LogError("SMTPServer::RunThread()","Dropping non-IP connection from " + socket.RemoteEndPoint.ToString());
								socket.Close();
								}
							else if (endPoint.Address.ToString() == "127.0.0.1")
								{
								if (!_allowLocalHost)
									{
									SupportLibrary.Logger.SystemLogger.LogError("SMTPServer::RunThread()","Dropping localhost (prohibited) connection");
									socket.Close();
									}
								else
									SessionManager.AddInboundSession(socket);
								}
							else if (_allowRemote)
								{
								socket.NoDelay = false;		// turn off Nagle
								SessionManager.AddInboundSession(socket,endPoint);
								}
							else
								{
								SupportLibrary.Logger.SystemLogger.LogError("SMTPServer::RunThread()","Dropping remote (prohibited) connection from " + endPoint.ToString());
								socket.Close();
								}
							}
						}
					catch (ThreadInterruptedException)
						{
						break;
						}
					}

				_listener.Stop();
				_listener = null;
				}
			catch (Exception ee)
				{
				SupportLibrary.Logger.SystemLogger.LogError("SMTPServer::RunThread()","Caught exception: " + ee);
				}
			}


		#region IDisposable Members

		public void Dispose()
			{
			Close();
			}

		#endregion
		}
	}
