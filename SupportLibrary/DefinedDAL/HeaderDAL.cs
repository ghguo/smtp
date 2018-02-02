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
using System.Data.SqlClient;
using System.Data;

namespace SupportLibrary.Database
	{
	public partial class HeaderDAL
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
		public virtual IList<HeaderRecord> ReadAllCandidateRecords(DateTime UtcRetryAttemptTime)
			{
			IList<HeaderRecord> list = new List<HeaderRecord>();
			SqlConnection conn = new SqlConnection(_connectionString);
			conn.Open();
			if (conn != null)
				{
				try
					{
					SqlCommand command = new SqlCommand("SELECT H.* FROM [Header] H " +
						"LEFT JOIN [Connection] C ON C.ID=H.RecvConnectionID " +
						"LEFT JOIN [DeliveryAttempt] DA on DA.HeaderID=H.ID " +
						"WHERE H.ExpiredDateTime IS NULL AND H.DeliveredDateTime IS NULL AND " +
						"C.EndTime IS NOT NULL AND " +
						"(DA.ID IS NULL OR DA.IsSuccess = 0)",conn);
					//SqlParameter param = new SqlParameter("@AttemptDateTime",SqlDbType.DateTime);
					//param.Value = UtcRetryAttemptTime;
					//command.Parameters.Add(param);

					Dictionary<int,HeaderRecord> lookup = new Dictionary<int,HeaderRecord>();

					SqlDataReader rs = command.ExecuteReader();
					while (rs.Read())
						{
						HeaderRecord record = ReapRecord(rs);
						if (!lookup.ContainsKey(record.ID))
							lookup.Add(record.ID,record);
						}
					rs.Close();

					// stage 2, check delivery attempt.....
					DeliveryAttemptDAL deliveryAttemptDAL = DALContainer.GetDALContainer().GetDeliveryAttemptDAL();
					foreach (HeaderRecord header in lookup.Values)
						{
						DeliveryAttemptRecord deliveryAttempt = deliveryAttemptDAL.ReadMostRecentByHeaderID(header.ID);
						if (deliveryAttempt == null || deliveryAttempt.AttemptDateTime <= UtcRetryAttemptTime)
							list.Add(header);
						}
					}
				finally
					{
					if (conn != null)
						conn.Close();
					}
				}
			return list;
			}


		/// <summary>
		/// Retrieves all headers created prior to the limit time.
		/// </summary>
		/// <param name="UtcLimitTime">The timestamp for which to find older records.</param>
		/// <returns>A list of zero or more records.</returns>
		public virtual IList<HeaderRecord> ReadAllRecordsReceivedBefore(DateTime UtcLimitTime)
			{
			IList<HeaderRecord> list = new List<HeaderRecord>();
			SqlConnection conn = new SqlConnection(_connectionString);
			conn.Open();
			if (conn != null)
				{
				try
					{
					SqlCommand command = new SqlCommand("SELECT * FROM [Header] WHERE ReceivedDateTime < @UTCLimitTime",conn);
					SqlParameter param = new SqlParameter("@UTCLimitTime",SqlDbType.DateTime);
					param.Value = UtcLimitTime;
					command.Parameters.Add(param);
					SqlDataReader rs = command.ExecuteReader();
					if (rs.Read())
						list.Add(ReapRecord(rs));
					rs.Close();
					}
				finally
					{
					if (conn != null)
						conn.Close();
					}
				}
			return list;
			}


		/// <summary>
		/// Reads all records with a receive date between the specified
		/// range of dates.  Either of the dates can be null.  If 
		/// both are null, this is equivalent to ReadAllRecords().
		/// </summary>
		/// <param name="StartUTC">The start date or UTC or null</param>
		/// <param name="EndUTC">The end date or UTC or null</param>
		/// <returns>A list containing zero or more records.</returns>
		public virtual IList<HeaderRecord> ReadRecordsByReceivedDate(DateTime? UtcStartTime,DateTime? UtcEndTime)
			{
			IList<HeaderRecord> list = new List<HeaderRecord>();
			SqlConnection conn = new SqlConnection(_connectionString);
			conn.Open();
			if (conn != null)
				{
				try
					{
					string sql = "SELECT * FROM [Header]";
					List<SqlParameter> parameters = new List<SqlParameter>();

					if (UtcStartTime != null || UtcEndTime != null)
						sql = sql + " WHERE";

					if (UtcStartTime != null)
						{
						sql = sql + " ReceivedDateTime>=@StartTime";
						
						SqlParameter param = new SqlParameter("@StartTime",SqlDbType.DateTime);
						param.Value = (DateTime)UtcStartTime;
						parameters.Add(param);
						}

					if (UtcEndTime != null)
						{
						if (UtcStartTime != null)
							sql = sql + " AND ";
						sql = sql + " ReceivedDateTime<=@EndTime";

						SqlParameter param = new SqlParameter("@EndTime",SqlDbType.DateTime);
						param.Value = (DateTime)UtcEndTime;
						parameters.Add(param);
						}

					SqlCommand command = new SqlCommand(sql,conn);
					foreach (SqlParameter param in parameters)
						command.Parameters.Add(param);

					SqlDataReader rs = command.ExecuteReader();
					if (rs.Read())
						list.Add(ReapRecord(rs));
					rs.Close();
					}
				finally
					{
					if (conn != null)
						conn.Close();
					}
				}
			return list;
			}


		/// <summary>
		/// Reads all records with a delivered date between the specified
		/// range of dates.  Either of the dates can be null.  If 
		/// both are null, this is equivalent to ReadAllRecords().
		/// </summary>
		/// <param name="StartUTC">The start date or UTC or null</param>
		/// <param name="EndUTC">The end date or UTC or null</param>
		/// <returns>A list containing zero or more records.</returns>
		public virtual IList<HeaderRecord> ReadRecordsByDeliveredDate(DateTime? UtcStartTime,DateTime? UtcEndTime)
			{
			IList<HeaderRecord> list = new List<HeaderRecord>();
			SqlConnection conn = new SqlConnection(_connectionString);
			conn.Open();
			if (conn != null)
				{
				try
					{
					string sql = "SELECT * FROM [Header] WHERE DeliveredDateTime IS NOT NULL";
					List<SqlParameter> parameters = new List<SqlParameter>();

					if (UtcStartTime != null)
						{
						sql = sql + " AND DeliveredDateTime>=@StartTime";

						SqlParameter param = new SqlParameter("@StartTime",SqlDbType.DateTime);
						param.Value = (DateTime)UtcStartTime;
						parameters.Add(param);
						}

					if (UtcEndTime != null)
						{
						if (UtcStartTime != null)
							sql = sql + " AND ";
						sql = sql + " AND DeliveredDateTime<=@EndTime";

						SqlParameter param = new SqlParameter("@EndTime",SqlDbType.DateTime);
						param.Value = (DateTime)UtcEndTime;
						parameters.Add(param);
						}

					SqlCommand command = new SqlCommand(sql,conn);
					foreach (SqlParameter param in parameters)
						command.Parameters.Add(param);

					SqlDataReader rs = command.ExecuteReader();
					if (rs.Read())
						list.Add(ReapRecord(rs));
					rs.Close();
					}
				finally
					{
					if (conn != null)
						conn.Close();
					}
				}
			return list;
			}


		/// <summary>
		/// Reads all records with an expired date between the specified
		/// range of dates.  Either of the dates can be null.  If 
		/// both are null, this is equivalent to ReadAllRecords().
		/// </summary>
		/// <param name="StartUTC">The start date or UTC or null</param>
		/// <param name="EndUTC">The end date or UTC or null</param>
		/// <returns>A list containing zero or more records.</returns>
		public virtual IList<HeaderRecord> ReadRecordsByExpiredDate(DateTime? UtcStartTime,DateTime? UtcEndTime)
			{
			IList<HeaderRecord> list = new List<HeaderRecord>();
			SqlConnection conn = new SqlConnection(_connectionString);
			conn.Open();
			if (conn != null)
				{
				try
					{
					string sql = "SELECT * FROM [Header] WHERE ExpiredDateTime IS NOT NULL";
					List<SqlParameter> parameters = new List<SqlParameter>();

					if (UtcStartTime != null)
						{
						sql = sql + " AND ExpiredDateTime>=@StartTime";

						SqlParameter param = new SqlParameter("@StartTime",SqlDbType.DateTime);
						param.Value = (DateTime)UtcStartTime;
						parameters.Add(param);
						}

					if (UtcEndTime != null)
						{
						if (UtcStartTime != null)
							sql = sql + " AND ";
						sql = sql + " AND ExpiredDateTime<=@EndTime";

						SqlParameter param = new SqlParameter("@EndTime",SqlDbType.DateTime);
						param.Value = (DateTime)UtcEndTime;
						parameters.Add(param);
						}

					SqlCommand command = new SqlCommand(sql,conn);
					foreach (SqlParameter param in parameters)
						command.Parameters.Add(param);

					SqlDataReader rs = command.ExecuteReader();
					if (rs.Read())
						list.Add(ReapRecord(rs));
					rs.Close();
					}
				finally
					{
					if (conn != null)
						conn.Close();
					}
				}
			return list;
			}
		}
	}
