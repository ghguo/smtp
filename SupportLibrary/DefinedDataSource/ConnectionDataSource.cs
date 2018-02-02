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
using SupportLibrary.Database;

namespace SupportLibrary.DataSource
	{
	public partial class ConnectionDataSource
		{
		/// <summary>
		/// Loads up all connection records that are incomplete.
		/// </summary>
		/// <returns>A list of zero or more incomplete records.</returns>
		public static IList<ConnectionRecord> ReadAllUnclosedRecords()
			{
			DALContainer dc = DALContainer.GetDALContainer();
			ConnectionDAL dal = dc.GetConnectionDAL();
			return dal.ReadAllUnclosedRecords();
			}

		/// <summary>
		/// Reads all records with start dates between the specified
		/// range of dates.  Either of the dates can be null.  If 
		/// both are null, this is equivalent to ReadAllRecords().
		/// </summary>
		/// <param name="StartUTC">The start date or UTC or null</param>
		/// <param name="EndUTC">The end date or UTC or null</param>
		/// <returns>A list containing zero or more records.</returns>
		public static IList<ConnectionRecord> ReadRecordsByStartDate(DateTime? StartUTC,DateTime? EndUTC)
			{
			DALContainer dc = DALContainer.GetDALContainer();
			ConnectionDAL dal = dc.GetConnectionDAL();
			if (StartUTC == null && EndUTC == null)
				return dal.ReadAllRecords();
			return dal.ReadRecordsByStartDate(StartUTC,EndUTC);
			}
		}
	}
