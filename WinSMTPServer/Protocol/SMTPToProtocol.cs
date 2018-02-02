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
	/// Handles looking for the RCPT TO part of the protocol.
	/// </summary>
	public class SMTPToProtocol : SMTPProtocol
		{
		internal SMTPToProtocol(SMTPData Data)
			: base(Data)
			{
			}

		#region SMTPProtocol Members

		/// <summary>
		/// Looks for the RCPT TO line.
		/// </summary>
		public override SMTPProtocol ProcessLine(string Line,LineWriter WriteLine)
			{
			if (Line.Length >= 4)
				{
				string command = Line.Substring(0,4);
				if (string.Compare(command,"RCPT",true) == 0)
					{
					string[] parts = Line.Split(" \t".ToCharArray());
					if (parts.Length < 2)
						{
						WriteLine("500 Unknown command");
						return this;
						}
					//else if (string.Compare(parts[1],"TO:",true) != 0)
					else if (!parts[1].Contains("TO:"))
					{
						WriteLine("500 Unknown command");
						return this;
						}

					if (parts.Length < 2)
						{
						WriteLine("501 Incorrect address format");
						return this;
						}
					string recipient = SMTPUtil.ParseEmailAddress(Line.Substring(Line.IndexOf(":") + 1).Trim());
					if (recipient != null)
						{
						_data.AddRecipient(recipient);
						WriteLine("250 Recipient of " + recipient + " has been accepted");
						}
					else
						WriteLine("501 Incorrect address format");
					return this;
					}
				else if (string.Compare(command,"DATA",true) == 0)
					{
					if (_data.Recipients.Count == 0)
						{
						WriteLine("503 No recipients provided");
						return this;
						}
					else
						{
						WriteLine("354 Enter text for message and end with single . on a line by itself");
						return new SMTPDataProtocol(_data);
						}
					}
				else if (string.Compare(command,"MAIL",true) == 0)
					{
					WriteLine("500 MAIL command has already been provided.");
					return this;
					}
				else if (string.Compare(command,"NOOP",true) == 0)
					{
					WriteLine("250 Ok");
					return this;
					}
				else if (string.Compare(command,"NOOP",true) == 0)
					{
					WriteLine("250 Ok");
					return this;
					}
				else if (string.Compare(command,"RSET",true) == 0)
					{
					WriteLine("250 Ok, state has been reset");
					return new SMTPStartProtocol(_data);
					}
				else if (string.Compare(command,"QUIT",true) == 0)
					{
					WriteLine("221 Goodbye");
					return this;
					}
				}
			WriteLine("500 Command unknown");
			return this;
			}

		#endregion
		}
	}
