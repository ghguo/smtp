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
using System.Text;
using System.Diagnostics;

namespace RegisterEventSource
	{
	public class RegisterEventSource
		{
		static void Main(string[] args)
			{
			Console.WriteLine("RegisterEventSource: This should be run by an Administrator.");
			Console.WriteLine("Checking if " + SupportLibrary.Logger.SystemLogger.EventSourceName + " is registered.");
			try
				{
				// Create the source, if it does not already exist.
				if (!EventLog.SourceExists(SupportLibrary.Logger.SystemLogger.EventSourceName))
					{
					Console.WriteLine("Creating non-existent event source.");

					EventLog.CreateEventSource(SupportLibrary.Logger.SystemLogger.EventSourceName,SupportLibrary.Logger.SystemLogger.EventSourceName + "Log");
					Console.WriteLine("Successfully created new event source...");

					SupportLibrary.Logger.SystemLogger.LogInfo("RegisterEventSource","Created event source for logging from " + SupportLibrary.Logger.SystemLogger.EventSourceName);
					}
				else
					Console.WriteLine("Event source already exists");
				}
			catch (Exception)
				{
				Console.WriteLine("An error occurred while creating new event source. Are you running as Administrator?");
				}
			}
		}
	}

