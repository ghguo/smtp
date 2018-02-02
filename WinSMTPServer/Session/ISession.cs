using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinSMTPServer.Session
	{
	/// <summary>
	/// Defines a session interface for enabling the
	/// SessionManager to control it.
	/// </summary>
	public interface ISession
		{
		/// <summary>
		/// Derives the unique ID for this connection session.
		/// </summary>
		int ConnectionID
			{
			get;
			}

		/// <summary>
		/// Derives the time this session was started in UTC.
		/// </summary>
		DateTime ConnectionStartTime
			{
			get;
			}

		/// <summary>
		/// Determines if the session is still active or is completed.
		/// </summary>
		bool IsConnectionTerminated
			{
			get;
			}

		/// <summary>
		/// Forces a session to shutdown immediately with prejudice.
		/// </summary>
		void TerminateConnection();
		}
	}
