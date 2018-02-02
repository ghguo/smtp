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
	public partial class HeaderDataSource
		{
		/// <summary>
		/// Retrieves all candidate records for transmission. A candidate
		/// is defined as a header on mail remains to be sent
		/// linked to a session that is ended,
		/// that has not been expired or delivered,
		/// with no delivery attempt or with a delivery attempt > retrytimeout
		/// </summary>
		/// <param name="UtcRetryAllowedTime">The time in UTC of the last 
		/// attempt or earlier that can be considered for candidacy</param>
		/// <returns>A list of zero or more records that are candidates to expire or transmit.</returns>
		public static IList<HeaderRecord> ReadAllCandidateRecords(DateTime UtcRetryAttemptTime)
			{
			DALContainer dc = DALContainer.GetDALContainer();
			HeaderDAL dal = dc.GetHeaderDAL();
			return dal.ReadAllCandidateRecords(UtcRetryAttemptTime);
			}


		/// <summary>
		/// Retrieves all headers created prior to the limit time.
		/// </summary>
		/// <param name="UtcLimitTime">The timestamp for which to find older records.</param>
		/// <returns>A list of zero or more records.</returns>
		public static IList<HeaderRecord> ReadAllRecordsReceivedBefore(DateTime UtcLimitTime)
			{
			DALContainer dc = DALContainer.GetDALContainer();
			HeaderDAL dal = dc.GetHeaderDAL();
			return dal.ReadAllRecordsReceivedBefore(UtcLimitTime);
			}


		/// <summary>
		/// Reads all records with a receive date between the specified
		/// range of dates.  Either of the dates can be null.  If 
		/// both are null, this is equivalent to ReadAllRecords().
		/// </summary>
		/// <param name="StartUTC">The start date or UTC or null</param>
		/// <param name="EndUTC">The end date or UTC or null</param>
		/// <returns>A list containing zero or more records.</returns>
		public static IList<HeaderRecord> ReadRecordsByReceivedDate(DateTime? UtcStartTime,DateTime? UtcEndTime)
			{
			DALContainer dc = DALContainer.GetDALContainer();
			HeaderDAL dal = dc.GetHeaderDAL();
			return dal.ReadRecordsByReceivedDate(UtcStartTime,UtcEndTime);
			}


		/// <summary>
		/// Reads all records with an expired date between the specified
		/// range of dates.  Either of the dates can be null.  If 
		/// both are null, this is equivalent to ReadAllRecords().
		/// </summary>
		/// <param name="StartUTC">The start date or UTC or null</param>
		/// <param name="EndUTC">The end date or UTC or null</param>
		/// <returns>A list containing zero or more records.</returns>
		public static IList<HeaderRecord> ReadRecordsByDeliveredDate(DateTime? UtcStartTime,DateTime? UtcEndTime)
			{
			DALContainer dc = DALContainer.GetDALContainer();
			HeaderDAL dal = dc.GetHeaderDAL();
			return dal.ReadRecordsByDeliveredDate(UtcStartTime,UtcEndTime);
			}


		/// <summary>
		/// Reads all records with a receive date between the specified
		/// range of dates.  Either of the dates can be null.  If 
		/// both are null, this is equivalent to ReadAllRecords().
		/// </summary>
		/// <param name="StartUTC">The start date or UTC or null</param>
		/// <param name="EndUTC">The end date or UTC or null</param>
		/// <returns>A list containing zero or more records.</returns>
		public static IList<HeaderRecord> ReadRecordsByExpiredDate(DateTime? UtcStartTime,DateTime? UtcEndTime)
			{
			DALContainer dc = DALContainer.GetDALContainer();
			HeaderDAL dal = dc.GetHeaderDAL();
			return dal.ReadRecordsByExpiredDate(UtcStartTime,UtcEndTime);
			}
		}
	}
