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
	/// Handles reading the internals for a DATA block.
	/// </summary>
	public class SMTPDataProtocol : SMTPProtocol
		{
		private bool _inHeaders = true;
		private StringBuilder _headers = new StringBuilder(2048);
		private StringBuilder _body = new StringBuilder(4096);

		internal SMTPDataProtocol(SMTPData Data)
			: base(Data)
			{
			}


		#region SMTPProtocol Members

		/// <summary>
		/// Loads the contents of a DATA body.  It provides no response except
		/// at the end IF the message size was too large.  A valid response is
		/// provided through the SMTPMessageDoneProtocol which is the output of 
		/// this process.
		/// </summary>
		public override SMTPProtocol ProcessLine(string Line,LineWriter WriteLine)
			{
			int size = _headers.Length + _body.Length;
			bool tooBig = size > Configuration.GetConfiguration().MaxMessageSizeBytes;

			if (Line.Length == 0 && _inHeaders)
				_inHeaders = false;
			else if (Line.Length == 1 && Line[0] == '.')		// got the end of DATA dot
				{
				if (tooBig)
					{
					WriteLine("552 Message has been rejected because it is too big (>" + 
						Configuration.GetConfiguration().MaxMessageSizeBytes + " bytes)");
					return new SMTPStartProtocol(_data);
					}

// TODO: how to determine a data of Hello\r\n\r\nHow are you? is only a body???
				if (_inHeaders)
					{
					_data.Headers = string.Empty;
					_data.Body = _headers.ToString();
					}
				else
					{
					_data.Headers = _headers.ToString();
					_data.Body = _body.ToString();
					}
				return new SMTPMessageDoneProtocol(_data);
				}

			if (!tooBig)
				{
				if (_inHeaders)
					{
					_headers.Append(Line);
					_headers.Append("\n");			// we separate by only line-feeds in our database!
					}
				else
					{
					_body.Append(Line);
					_body.Append("\n");			// we separate by only line-feeds in our database!
					}
				}

			return this;
			}

		#endregion
		}
	}
