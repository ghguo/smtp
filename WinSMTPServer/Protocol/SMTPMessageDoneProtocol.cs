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
	/// Handles a message when it is finished being received.  The
	/// client code should look for this data type then snag the 
	/// data object and save it.  The response will not be sent until
	/// the data has been saved.  Note that this is a TERMINAL
	/// condition and cannot process lines of text.
	/// </summary>
	public class SMTPMessageDoneProtocol : SMTPProtocol
		{
		internal SMTPMessageDoneProtocol(SMTPData Data)
			: base(Data)
			{
			}


		/// <summary>
		/// Called to dispatch a success message for the 
		/// saving of the data.
		/// </summary>
		/// <param name="WriteLine">The routine that sends a line of text to the client.</param>
		/// <returns>A protocol handler to await the next message.</returns>
		public SMTPProtocol HandleSuccess(LineWriter WriteLine)
			{
			WriteLine("250 Message queued for delivery");
			return new SMTPStartProtocol(_data);		// back to waiting for next command
			}


		/// <summary>
		/// Called to dispatch a failure message for the 
		/// saving of the data.
		/// </summary>
		/// <param name="WriteLine">The routine that sends a line of text to the client.</param>
		/// <returns>A protocol handler to await the next message.</returns>
		public SMTPProtocol HandleFailure(LineWriter WriteLine)
			{
			WriteLine("452 Internal error detected and message will not be delivered");
			return new SMTPStartProtocol(_data);		// back to waiting for next command
			}


		#region SMTPProtocol Members

		/// <summary>
		/// Loads the contents of a DATA body
		/// </summary>
		public override SMTPProtocol ProcessLine(string Line,LineWriter WriteLine)
			{
			throw new InvalidOperationException("SMTPMessageDoneProtocol cannot accept commands.  It is a terminal state.");
			}

		#endregion
		}
	}
