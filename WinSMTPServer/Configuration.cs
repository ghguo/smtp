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
using SupportLibrary.Database;
using SupportLibrary.DataSource;

namespace WinSMTPServer
	{
	public class Configuration
		{
		private const int MaxMessageSize = 10000000;

		private static Configuration _configuration;

		private ConfigurationRecord _configurationRecord;
		private string _hostName;

		private Configuration()
			{
			_configurationRecord = ConfigurationDataSource.ReadRecord(ConfigurationRecord.MasterConfigurationID);
			if (_configurationRecord == null)
				throw new InvalidOperationException("Missing configuration record in database (must be ID " + ConfigurationRecord.MasterConfigurationID + ")");

			_hostName = System.Net.Dns.GetHostName();
			}


		/// <summary>
		/// Returns the singleton configuration object.
		/// </summary>
		/// <returns>The configuration object</returns>
		public static Configuration GetConfiguration()
			{
			if (_configuration == null)
				_configuration = new Configuration();
			return _configuration;
			}


		/// <summary>
		/// Derives the maximum size message that is accepted.
		/// </summary>
		public int MaxMessageSizeBytes
			{
			get
				{
				return MaxMessageSize;
				}
			}


		/// <summary>
		/// Derives the name of this host machine.
		/// </summary>
		public string HostName
			{
			get
				{
				return _hostName;
				}
			}

		
		/// <summary>
		/// Derives if localhost is permitted to connect to this server.
		/// </summary>
		public bool AllowLocalHost
			{
			get
				{
				return _configurationRecord.AllowLocalHost;
				}
			}


		/// <summary>
		/// Derives if remote connections are allowed to this server.
		/// </summary>
		public bool AllowRemote
			{
			get
				{
				return _configurationRecord.AllowRemote;
				}
			}


		/// <summary>
		/// Determines the port to listen on for SMTP.
		/// </summary>
		public short Port
			{
			get
				{
				return _configurationRecord.Port;
				}
			}


		/// <summary>
		/// Derives the timeout before expiring messages that cannot be sent.
		/// </summary>
		public int ExpireAfterMinutes
			{
			get
				{
				return _configurationRecord.ExpireAfterMinutes;
				}
			}


		/// <summary>
		/// Derives the timeout between retries to send messages.
		/// </summary>
		public int RetryAfterMinutes
			{
			get
				{
				return _configurationRecord.RetryAfterMinutes;
				}
			}


		/// <summary>
		/// Derives the number of days before which messages are removed.
		/// </summary>
		public int CleanupDays
			{
			get
				{
				return _configurationRecord.CleanupDays;
				}
			}
		}
	}
