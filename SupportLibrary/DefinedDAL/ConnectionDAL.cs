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
	public partial class ConnectionDAL
		{
		/// <summary>
		/// Loads up all connection records that are incomplete.
		/// </summary>
		/// <returns>A list of zero or more records.</returns>
		public virtual IList<ConnectionRecord> ReadAllUnclosedRecords()
			{
			IList<ConnectionRecord> list = new List<ConnectionRecord>();
			SqlConnection conn = new SqlConnection(_connectionString);
			conn.Open();
			if (conn != null)
				{
				try
					{
					SqlCommand command = new SqlCommand("SELECT * FROM [Connection] WHERE EndTime IS NULL",conn);
					SqlDataReader rs = command.ExecuteReader();
					while (rs.Read())
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
		/// Reads all records with start dates between the specified
		/// range of dates.  Either of the dates can be null.  If 
		/// both are null, this is equivalent to ReadAllRecords().
		/// </summary>
		/// <param name="StartUTC">The start date or UTC or null</param>
		/// <param name="EndUTC">The end date or UTC or null</param>
		/// <returns>A list containing zero or more records.</returns>
		public virtual IList<ConnectionRecord> ReadRecordsByStartDate(DateTime? StartUTC,DateTime? EndUTC)
			{
			IList<ConnectionRecord> list = new List<ConnectionRecord>();
			SqlConnection conn = new SqlConnection(_connectionString);
			conn.Open();
			if (conn != null)
				{
				try
					{
					string sql = "SELECT * FROM [Connection]";

					List<SqlParameter> parameters = new List<SqlParameter>();

					if (StartUTC != null || EndUTC != null)
						sql = sql + " WHERE";

					if (StartUTC != null)
						{
						sql = sql + " StartTime>=@StartTime";
						
						SqlParameter param = new SqlParameter("@StartTime",SqlDbType.DateTime);
						param.Value = (DateTime)StartUTC;
						parameters.Add(param);
						}

					if (EndUTC != null)
						{
						if (StartUTC != null)
							sql = sql + " AND ";
						sql = sql + " EndTime<=@EndTime";

						SqlParameter param = new SqlParameter("@EndTime",SqlDbType.DateTime);
						param.Value = (DateTime)EndUTC;
						parameters.Add(param);
						}

					SqlCommand command = new SqlCommand(sql,conn);
					foreach (SqlParameter param in parameters)
						command.Parameters.Add(param);

					SqlDataReader rs = command.ExecuteReader();
					while (rs.Read())
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
