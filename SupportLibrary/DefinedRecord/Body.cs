﻿//The contents of this file are subject to the Mozilla Public License
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
using SupportLibrary.Database;
using System.Text;


namespace SupportLibrary.Database
	{
	public partial class BodyRecord
		{
		/// <summary>
		/// Retrieves the full text (headers + blank line + body).
		/// Note, the end of line is a single LF (0xa).
		/// </summary>
		public string FullText
			{
			get
				{
				if (HeaderText.Length == 0)
					return BodyText;
				else if (BodyText.Length == 0)
					return HeaderText;

				StringBuilder sb = new StringBuilder(HeaderText.Length + BodyText.Length + 2);
				sb.Append(HeaderText);
				if (HeaderText.Length > 0 && !HeaderText.EndsWith("\r\n\r\n"))
					sb.Append("\r\n");
				sb.Append(BodyText);

				return sb.ToString();
				}
			}
		}
	}
