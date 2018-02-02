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
using System.Threading;
using System.Net;
using WinSMTPServer.Connection;
using SupportLibrary.Database;

namespace WinSMTPServer.Session
	{
	/// <summary>
	/// Handles all current sessions.
	/// </summary>
	public class SessionManager : IDisposable
		{
		private const int CleanupTimeoutSeconds = 10;
		private const int TimeoutMinutes = 5;		// if you cannot do your SMTP business in this time, you are getting booted!

		private static object _locker = new object();
		private static SessionManager _sessionManager;

		private Thread _thread;
		private Dictionary<int,ISession> _sessions = new Dictionary<int,ISession>();

		private SessionManager()
			{
			_thread = new Thread(RunThread);
			_thread.IsBackground = true;
			_thread.Start();
			}


		/// <summary>
		/// Preempts all sessions and closes them.
		/// </summary>
		public void Close()
			{
			if (_thread != null)
				{
				_thread.Interrupt();
				_thread.Join(10000);
				_thread = null;
				}

			lock (_sessions)
				{
				foreach (ISession session in _sessions.Values)
					{
					if (!session.IsConnectionTerminated)
						session.TerminateConnection();
					}

				_sessions.Clear();
				}
			}


		/// <summary>
		/// Internal thread that pops off periodically and cleans
		/// up old sessions, expires sessions that have been around
		/// too long.
		/// </summary>
		private void RunThread()
			{
			while (true)
				{
				Thread.Sleep(CleanupTimeoutSeconds * 1000);

				try
					{
					// clean out old sessions
					lock (_sessions)
						{
						DateTime timeoutStartTime = DateTime.UtcNow.Subtract(new TimeSpan(0, TimeoutMinutes, 0));
						IList<int> toDelete = new List<int>();
						foreach (ISession session in _sessions.Values)
							{
							if (session.IsConnectionTerminated)
								toDelete.Add(session.ConnectionID);
							else if (timeoutStartTime.CompareTo(session.ConnectionStartTime) >= 0)
								session.TerminateConnection();
							}

						foreach (int sessionID in toDelete)
							_sessions.Remove(sessionID);
						}
					}
				catch (ThreadInterruptedException)
					{
					break;
					}
				catch (Exception ee)
					{
					SupportLibrary.Logger.SystemLogger.LogError("SessionManager::RunThread()","Caught exception: " + ee);
					}
				}
			}


		/// <summary>
		/// Adds a session to the list of managed sessions.
		/// </summary>
		/// <param name="Session">The session to manage.</param>
		public void AddSession(ISession Session)
			{
			lock (_sessions)
				{
				_sessions.Add(Session.ConnectionID, Session);
				}
			}

		
		/// <summary>
		/// Internal method used to retrieve the singleton
		/// session manager.
		/// </summary>
		/// <returns></returns>
		private static SessionManager GetSessionManager()
			{
			lock (_locker)
				{
				if (_sessionManager == null)
					_sessionManager = new SessionManager();
				return _sessionManager;
				}
			}


		/// <summary>
		/// Adds a session manager to tend the incoming localhost SMTP connection.
		/// </summary>
		/// <param name="RemoteSocket">The connected socket from remote.</param>
		public static void AddInboundSession(Socket RemoteSocket)
			{
			SessionManager manager = GetSessionManager();
			manager.AddSession(new InboundSession(RemoteSocket));
			}


		/// <summary>
		/// Adds a session manager to tend the incoming remote SMTP connection.
		/// The connection source is checked to see if it is in the allowed 
		/// list and is closed if not.
		/// </summary>
		/// <param name="RemoteSocket">The connected socket from remote.</param>
		/// <param name="RemoteAddress">The remote address of the connected socket.</param>
		public static void AddInboundSession(Socket RemoteSocket,IPEndPoint RemoteAddress)
			{
			SessionManager manager = GetSessionManager();
			manager.AddSession(new InboundSession(RemoteSocket,RemoteAddress));
			}


		/// <summary>
		/// Adds a new session to handle a transmission attempt for an SMTP message
		/// going out to a remote server.
		/// </summary>
		/// <param name="Header">The header of the message to transmit.</param>
		/// <param name="Body">The body of the message to transmit.</param>
		public static void AddOutboundSession(HeaderRecord Header,BodyRecord Body)
			{
			SessionManager manager = GetSessionManager();
			manager.AddSession(new OutboundSession(Header,Body));
			}


		#region IDisposable Members

		public void Dispose()
			{
			Close();
			}

		#endregion
		}
	}
