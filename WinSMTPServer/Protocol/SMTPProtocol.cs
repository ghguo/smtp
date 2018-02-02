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

namespace WinSMTPServer.Protocol
	{
	/// <summary>
	/// Defines the signature for a routine to
	/// write a line of text to the remote side.
	/// </summary>
	/// <param name="Line">The line of text to send without cr/lf -- these will be appended when sent.</param>
	public delegate void LineWriter(string Line);

	/// <summary>
	/// The base type for the state handlers (maybe even strategy handlers)
	/// for the SMTP server protocol.
	/// </summary>
	abstract public class SMTPProtocol
		{
		protected SMTPData _data;

		protected SMTPProtocol(SMTPData Data)
			{
			_data = Data;
			}

		/// <summary>
		/// Processes the next line of text from the remote client.
		/// </summary>
		/// <param name="Line">The line of text to process with cr/lf torn off.</param>
		/// <param name="WriteLine">The routine that sends a line of text to the client.</param>
		/// <returns>The new SMTPProtocol state to continue processing with.  A return of null
		/// signals the end of the session.</returns>
		public abstract SMTPProtocol ProcessLine(string Line,LineWriter WriteLine);

		/// <summary>
		/// Retrieves the current data for this SMTP session.
		/// </summary>
		public SMTPData Data
			{
			get
				{
				return _data;
				}
			}
		}
	}
