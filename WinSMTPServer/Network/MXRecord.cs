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


namespace WinSMTPServer.Network
	{
	/// <summary>
	/// Container for an MX record's data.
	/// </summary>
	public class MXRecord
		{
		public MXRecord(string DomainName,string ServerName,int Preference)
			{
			this.DomainName = DomainName;
			this.ServerName = ServerName;
			this.Preference = Preference;
			}

		/// <summary>
		/// Returns the domain name that this MX server responds for.
		/// </summary>
		public string DomainName
			{
			get;
			private set;
			}

		/// <summary>
		/// The MX server name as returned from the DNS.
		/// </summary>
		public string ServerName
			{
			get;
			private set;
			}

		/// <summary>
		/// The preference number for this MX
		/// </summary>
		public int Preference
			{
			get;
			private set;
			}
		}
	}
