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
using SupportLibrary.DataSource;
using SupportLibrary.Database;

namespace WinSMTPConsole.Activity
	{
	/// <summary>
	/// Provides search interfaces.
	/// </summary>
	public class ActivitySearch
		{
		/// <summary>
		/// Attempts to find records that match the criteria
		/// </summary>
		/// <param name="StartUTC">The start date or UTC or null</param>
		/// <param name="EndUTC">The end date or UTC or null</param>
		/// <param name="SearchSessionDate">True to search on session start date.</param>
		/// <param name="SearchReceivedDate">True to search on received date.</param>
		/// <param name="SearchDeliveredDate">True to search on delivered date.</param>
		/// <param name="SearchExpiredDate">True to search on expired date</param>
		/// <returns>A list of zero or more records</returns>
		static public List<ActivityRecord> FindRecords(DateTime? StartUTC,DateTime? EndUTC,
			bool SearchSessionDate,bool SearchReceivedDate,bool SearchDeliveredDate,
			bool SearchExpiredDate)
			{
			if (StartUTC == null || EndUTC == null)
				return new List<ActivityRecord>(0);

			Dictionary<int,IList<ActivityRecord>> matches = new Dictionary<int,IList<ActivityRecord>>();
			if (SearchSessionDate)
				FindConnectionRecords(matches,StartUTC,EndUTC);

			if (SearchReceivedDate)
				FindHeaderRecordsByReceived(matches,StartUTC,EndUTC);

			if (SearchDeliveredDate)
				FindHeaderRecordsByDelivered(matches,StartUTC,EndUTC);

			if (SearchExpiredDate)
				FindHeaderRecordsByExpired(matches,StartUTC,EndUTC);

			List<ActivityRecord> list = new List<ActivityRecord>();
			foreach (IList<ActivityRecord> activities in matches.Values)
				{
				foreach (ActivityRecord activity in activities)
					list.Add(activity);
				}

			list.Sort((a,b) => 
				{
				if (a.StartTimeUTC == b.StartTimeUTC)
					return 0;
				return a.StartTimeUTC > b.StartTimeUTC ? 1 : -1;
				});

			return list;
			}


		/// <summary>
		/// Internally used method which culls records from the Connection table.
		/// </summary>
		/// <param name="Matches">The dictionary to hold matches.</param>
		/// <param name="StartUTC">The start date or UTC or null</param>
		/// <param name="EndUTC">The end date or UTC or null</param>
		static private void FindConnectionRecords(Dictionary<int,IList<ActivityRecord>> Matches,
			DateTime? StartUTC,DateTime? EndUTC)
			{
			try
				{
				HeaderDAL headerDAL = DALContainer.GetDALContainer().GetHeaderDAL();
				DeliveryAttemptDAL deliveryAttemptDAL = DALContainer.GetDALContainer().GetDeliveryAttemptDAL();

				foreach (ConnectionRecord connection in ConnectionDataSource.ReadRecordsByStartDate(StartUTC,EndUTC))
					{
					if (Matches.ContainsKey(connection.ID))
						continue;

					List<ActivityRecord> list = new List<ActivityRecord>();

					// derive attempt to header mapping first....
					Dictionary<int,HeaderRecord> headerLookup = new Dictionary<int,HeaderRecord>();
					IList<DeliveryAttemptRecord> attempts = deliveryAttemptDAL.ReadAllRecordsByConnectionID(connection.ID);
					foreach (DeliveryAttemptRecord attempt in attempts)
						{
						HeaderRecord header = null;
						if (!headerLookup.ContainsKey(attempt.HeaderID))
							header = headerDAL.ReadRecord(attempt.HeaderID);
						else
							header = headerLookup[attempt.HeaderID];

						if (header != null)
							list.Add(new ActivityRecord() { Connection = connection,Header = header,DeliveryAttempt = attempt });
						}

					// derive headers that were not attempted yet...
					IList<HeaderRecord> headers = HeaderDataSource.ReadAllRecordsByRecvConnectionID(connection.ID);
					if (headers.Count == 0)
						headers = headerDAL.ReadAllRecordsBySendConnectionID(connection.ID);

					if (headers.Count > 0)
						{
						foreach (HeaderRecord header in headers)
							{
							if (!headerLookup.ContainsKey(header.ID))
								list.Add(new ActivityRecord() { Connection = connection,Header = header });
							}
						}
					else
						list.Add(new ActivityRecord() { Connection = connection });

					Matches.Add(connection.ID,list);
					}
				}
			catch (Exception ee)
				{
				SupportLibrary.Logger.SystemLogger.LogError("ActivitySearch::FindConnectionRecords","Exception caught: " + ee.Message);
				}
			}


		/// <summary>
		/// Internally used method which culls records from the Header table 
		/// based upon received date.
		/// </summary>
		/// <param name="Matches">The dictionary to hold matches.</param>
		/// <param name="StartUTC">The start date or UTC or null</param>
		/// <param name="EndUTC">The end date or UTC or null</param>
		static private void FindHeaderRecordsByReceived(Dictionary<int,IList<ActivityRecord>> Matches,
			DateTime? StartUTC,DateTime? EndUTC)
			{
			try
				{
				ConnectionDAL connectionDAL = DALContainer.GetDALContainer().GetConnectionDAL();
				DeliveryAttemptDAL deliveryAttemptDAL = DALContainer.GetDALContainer().GetDeliveryAttemptDAL();

				foreach (HeaderRecord header in HeaderDataSource.ReadRecordsByReceivedDate(StartUTC,EndUTC))
					{
					if (Matches.ContainsKey(header.RecvConnectionID))
						continue;

					List<ActivityRecord> list = new List<ActivityRecord>();

					ConnectionRecord connection = connectionDAL.ReadRecord(header.RecvConnectionID);
					IList<DeliveryAttemptRecord> attempts = DeliveryAttemptDataSource.ReadAllRecordsByHeaderID(header.ID);
					if (attempts.Count > 0)
						{
						foreach (DeliveryAttemptRecord attempt in attempts)
							list.Add(new ActivityRecord() { Connection = connection,Header = header,DeliveryAttempt = attempt });
						}
					else
						list.Add(new ActivityRecord() { Connection = connection,Header = header });

					Matches.Add(header.RecvConnectionID,list);
					}
				}
			catch (Exception ee)
				{
				SupportLibrary.Logger.SystemLogger.LogError("ActivitySearch::FindHeaderRecordsByReceived","Exception caught: " + ee.Message);
				}
			}


		/// <summary>
		/// Internally used method which culls records from the Header table 
		/// based upon delivered date.
		/// </summary>
		/// <param name="Matches">The dictionary to hold matches.</param>
		/// <param name="StartUTC">The start date or UTC or null</param>
		/// <param name="EndUTC">The end date or UTC or null</param>
		static private void FindHeaderRecordsByDelivered(Dictionary<int,IList<ActivityRecord>> Matches,
			DateTime? StartUTC,DateTime? EndUTC)
			{
			try
				{
				ConnectionDAL connectionDAL = DALContainer.GetDALContainer().GetConnectionDAL();
				DeliveryAttemptDAL deliveryAttemptDAL = DALContainer.GetDALContainer().GetDeliveryAttemptDAL();

				foreach (HeaderRecord header in HeaderDataSource.ReadRecordsByDeliveredDate(StartUTC,EndUTC))
					{
					if (header.SendConnectionID != null && Matches.ContainsKey((int)header.SendConnectionID))
						continue;

					List<ActivityRecord> list = new List<ActivityRecord>();

					ConnectionRecord connection = connectionDAL.ReadRecord((int)header.SendConnectionID);
					IList<DeliveryAttemptRecord> attempts = DeliveryAttemptDataSource.ReadAllRecordsByHeaderID(header.ID);
					if (attempts.Count > 0)
						{
						foreach (DeliveryAttemptRecord attempt in attempts)
							list.Add(new ActivityRecord() { Connection = connection,Header = header,DeliveryAttempt = attempt });
						}
					else
						list.Add(new ActivityRecord() { Connection = connection,Header = header });

					Matches.Add(header.RecvConnectionID,list);
					}
				}
			catch (Exception ee)
				{
				SupportLibrary.Logger.SystemLogger.LogError("ActivitySearch::FindHeaderRecordsByDelivered","Exception caught: " + ee.Message);
				}
			}


		/// <summary>
		/// Internally used method which culls records from the Header table 
		/// based upon expired date.
		/// </summary>
		/// <param name="Matches">The dictionary to hold matches.</param>
		/// <param name="StartUTC">The start date or UTC or null</param>
		/// <param name="EndUTC">The end date or UTC or null</param>
		static private void FindHeaderRecordsByExpired(Dictionary<int,IList<ActivityRecord>> Matches,
			DateTime? StartUTC,DateTime? EndUTC)
			{
			try
				{
				ConnectionDAL connectionDAL = DALContainer.GetDALContainer().GetConnectionDAL();
				DeliveryAttemptDAL deliveryAttemptDAL = DALContainer.GetDALContainer().GetDeliveryAttemptDAL();

				foreach (HeaderRecord header in HeaderDataSource.ReadRecordsByExpiredDate(StartUTC,EndUTC))
					{
					if (Matches.ContainsKey(header.RecvConnectionID))
						continue;

					List<ActivityRecord> list = new List<ActivityRecord>();

					ConnectionRecord connection = connectionDAL.ReadRecord(header.RecvConnectionID);
					IList<DeliveryAttemptRecord> attempts = DeliveryAttemptDataSource.ReadAllRecordsByHeaderID(header.ID);
					if (attempts.Count > 0)
						{
						foreach (DeliveryAttemptRecord attempt in attempts)
							list.Add(new ActivityRecord() { Connection = connection,Header = header,DeliveryAttempt = attempt });
						}
					else
						list.Add(new ActivityRecord() { Connection = connection,Header = header });

					Matches.Add(header.RecvConnectionID,list);
					}
				}
			catch (Exception ee)
				{
				SupportLibrary.Logger.SystemLogger.LogError("ActivitySearch::FindHeaderRecordsByReceived","Exception caught: " + ee.Message);
				}
			}
		}
	}
