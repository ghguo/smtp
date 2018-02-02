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
using System.ServiceProcess;
using System.Text;

namespace WinSMTPServer
	{
	static class Launcher
		{
		static private bool RunningFromCommandLine = false;		// permits debugging if we use the -cl command line variable

		/// <summary>
		/// Retrieves the startup conditions.  If started as a service this 
		/// will be false.  If started from the command line, this will be true.
		/// </summary>
		static public bool IsRunningFromCommandLine
			{
			get
				{
				return RunningFromCommandLine;
				}
			}

	
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main(string [] args)
			{
			//#if (DEBUG)
			//WinSMTPService service = new WinSMTPService();
			//service.Start(new string[] { });
			//#else
			//// See the following for creating and installing a Windows Service:
			//// http://msdn.microsoft.com/en-us/library/zt39148a(VS.80).aspx

			//// More than one user Service may run within the same process. To add
			//// another service to this process, change the following line to
			//// create a second service object. For example,
			////
			////   ServicesToRun = new ServiceBase[] {new Service1(), new MySecondUserService()};
			////

			ServiceBase[] ServicesToRun = new ServiceBase[]
				{
				new WinSMTPService()
				};

			ServiceBase.Run(ServicesToRun);
			//#endif
		}
		}
	}
