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
	/// General purpose utility functions for 
	/// SMTP operations.
	/// </summary>
	public class SMTPUtil
		{
		/// <summary>
		/// This utility method parses out an address from a
		/// line of text or from angle brackets within the line
		/// of text.
		/// </summary>
		/// <param name="Text">The text blob to extract Email address from.</param>
		/// <returns>An Email address or null if not found.</returns>
		static public string ParseEmailAddress(string Text)
			{
			// determine if we have angle-brackets
			int openOffset = Text.IndexOf('<');
			if (openOffset >= 0)
				{
				int closeOffset = Text.IndexOf('>',openOffset);
				if (closeOffset >= 0)
					{
					int len = closeOffset - (openOffset + 1);
					return Text.Substring(openOffset + 1,len);
					}
				}

			// no angle brackets?  We need to derive everything left and right of an @ sign
			int atOffset = Text.IndexOf('@');
			if (atOffset < 0)
				return null;
			else if (atOffset == 0 || (atOffset + 1) == Text.Length)
				return null;

			// look right till space....

			int rightOffset = atOffset + 1;
			while (rightOffset < Text.Length)
				{
				if (Text[rightOffset] == ' ' || Text[rightOffset] == '\t')
					break;
				++rightOffset;
				}

			// look left till space...
			int leftOffset = atOffset - 1;
			bool inQuote = false;
			while (leftOffset >= 0)
				{
				if (Text[leftOffset] == '"')
					inQuote = !inQuote;

				if (!inQuote &&
					(Text[leftOffset] == ' ' || Text[leftOffset] == '\t'))
					break;

				--leftOffset;
				}

			if (inQuote)		// invalid name
				return null;

			++leftOffset;	// put left offset on first character

			return Text.Substring(leftOffset,rightOffset - leftOffset);
			}
		}
	}
