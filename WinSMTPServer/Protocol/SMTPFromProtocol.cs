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
	/// Handles looking for the MAIL FROM part of the protoco.
	/// </summary>
	public class SMTPFromProtocol : SMTPProtocol
		{
		internal SMTPFromProtocol(SMTPData Data)
			: base(Data)
			{
			}


		#region SMTPProtocol Members

		/// <summary>
		/// Looks for MAIL FROM line.
		/// </summary>
		public override SMTPProtocol ProcessLine(string Line,LineWriter WriteLine)
			{
			if (Line.Length >= 4)
				{
				string command = Line.Substring(0,4);
				if (string.Compare(command,"MAIL",true) == 0)
					{
					string[] parts = Line.Split(" \t".ToCharArray());
					if (parts.Length < 2)
						{
						WriteLine("500 Unknown command");
						return this;
						}
					else if (string.Compare(parts[1],"FROM:",true) != 0)
						{
						WriteLine("500 Unknown command");
						return this;
						}

					if (parts.Length < 3)
						{
						WriteLine("501 Incorrect address format");
						return this;
						}
					string sender = SMTPUtil.ParseEmailAddress(Line.Substring(Line.IndexOf(":") + 1).Trim());
					if (sender != null)
						{
						WriteLine("250 Sender of " + sender + " has been accepted");
						_data.Sender = sender;
						return new SMTPToProtocol(_data);
						}

					WriteLine("501 Incorrect address format");
					return this;
					}
				else if (string.Compare(command,"RCPT",true) == 0)
					{
					WriteLine("500 RCPT requires MAIL first");
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
