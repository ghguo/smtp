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
	/// Waits for HELO/EHLO from the other end.  The 220 welcome
	/// message must already have been transmitted.
	/// </summary>
	public class SMTPStartProtocol : SMTPProtocol
		{
		private bool _gotHello = false;

		/// <summary>
		/// Starts a new session from scratch.
		/// </summary>
		public SMTPStartProtocol()
			: base(new SMTPData())
			{
			}


		/// <summary>
		/// Starts a new mail session from a reset or end of mail message.
		/// </summary>
		/// <param name="OriginalData"></param>
		public SMTPStartProtocol(SMTPData OriginalData)
			: this()
			{
			// Preserve keys.
			_data.IsExtendedHello = OriginalData.IsExtendedHello;
			_data.HelloName = OriginalData.HelloName;

			_gotHello = true;
			}


		#region SMTPProtocol Members

		/// <summary>
		/// Looks for the HELO/EHLO protocol.
		/// </summary>
		public override SMTPProtocol ProcessLine(string Line,LineWriter WriteLine)
			{
			if (Line.Length >= 4)
				{
				string command = Line.Substring(0,4);
				if (string.Compare(command,"HELO",true) == 0)
					{
					WriteLine("250 Hello to you also");
					if (Line.Length > 4)
						_data.HelloName = Line.Substring(4).Trim();
					return new SMTPFromProtocol(_data);
					}
				else if (string.Compare(command,"EHLO",true) == 0)
					{
					WriteLine("250-Hello (ehlo) to you also");
					WriteLine("250-SIZE " + Configuration.GetConfiguration().MaxMessageSizeBytes);
					WriteLine("250 8BITMIME");
					_data.IsExtendedHello = true;
					if (Line.Length > 4)
						_data.HelloName = Line.Substring(4).Trim();
					return new SMTPFromProtocol(_data);
					}
				else if (string.Compare(command,"NOOP",true) == 0)
					{
					WriteLine("250 Ok");
					return this;
					}
				else if (string.Compare(command,"RSET",true) == 0)
					{
					WriteLine("250 Ok, state has been reset");
					return this;
					}
				else if (string.Compare(command,"MAIL",true) == 0)
					{
					if (_gotHello)
						{
						SMTPFromProtocol from = new SMTPFromProtocol(_data);
						return from.ProcessLine(Line,WriteLine);		// delegate to from handler -- already got hello!
						}
					else
						{
						WriteLine("501 It is polite to say HELO/EHLO first");
						return this;
						}
					}
				else if (string.Compare(command,"QUIT",true) == 0)
					{
					WriteLine("221 Goodbye");
					return this;
					}
				}
			WriteLine("501 Command unknown");
			return this;
			}

		#endregion
		}
	}
