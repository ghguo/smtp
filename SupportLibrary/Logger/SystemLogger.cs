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
using System.Web;
using System.Diagnostics;

namespace SupportLibrary.Logger
	{
	/// <summary>
	/// Contains common logging support routines used within the system.  The
	/// event source must be registered an all host machines.  Use the 
	/// RegisterEventSource application as Administrator to register it.
	/// </summary>
	public class SystemLogger
		{
		static public readonly string EventSourceName = "WinSMTPServer";

		/// <summary>
		/// Logs a message to the error event log.
		/// </summary>
		/// <param name="Location">The location the message comes from in the code.</param>
		/// <param name="Message">The message to write out</param>
		static public void LogError(string Location, string Message)
			{
			if (Message != null && Message.Trim().Length > 0)
				{
				EventLog logger = new EventLog();
				logger.Source = EventSourceName;
				try
					{
					logger.WriteEntry(Location + ":: " + Message, EventLogEntryType.Error);
					}
				catch (Exception)
					{
					// do not let exceptions in logging propagate back up and cause a double-exception situation
					}
				}
			}


		/// <summary>
		/// Logs a message to the information event log.
		/// </summary>
		/// <param name="Location">The location the message comes from in the code.</param>
		/// <param name="Message">The message to write out</param>
		static public void LogInfo(string Location, string Message)
			{
			if (Message != null && Message.Trim().Length > 0)
				{
				EventLog logger = new EventLog();
				logger.Source = EventSourceName;
				try
					{
					logger.WriteEntry(Location + ":: " + Message, EventLogEntryType.Information);
					}
				catch (Exception)
					{
					// do not let exceptions in logging propagate back up and cause a double-exception situation
					}
				}
			}
		/// <summary>
		/// Logs a message to the information event log.
		/// </summary>
		/// <param name="IsSuccess">Indicates the item being audit-logged was successful if true or failed if false</param>
		/// <param name="Location">The location the message comes from in the code.</param>
		/// <param name="Message">The message to write out</param>
		static public void LogAudit(bool IsSuccess, string Location, string Message)
			{
			if (Message != null && Message.Trim().Length > 0)
				{
				EventLog logger = new EventLog();
				logger.Source = EventSourceName;
				try
					{
					logger.WriteEntry(Location + ":: " + Message, IsSuccess ? EventLogEntryType.SuccessAudit : EventLogEntryType.FailureAudit);
					}
				catch (Exception)
					{
					// do not let exceptions in logging propagate back up and cause a double-exception situation
					}
				}
			}
		}
	}
