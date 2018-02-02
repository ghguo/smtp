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
using System.ComponentModel;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using SupportLibrary.Database;

namespace SupportLibrary.DataSource
	{
	public partial class ConfigurationDataSource
		{
		public static void NewRecord(ConfigurationRecord Record)
			{
			DALContainer dc = DALContainer.GetDALContainer();
			ConfigurationDAL dal = dc.GetConfigurationDAL();
			dal.NewRecord(Record);
			}
		}
	}
