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
	public partial class DeliveryAttemptDAL
		{
		/// <summary>
		/// Loads the most recent delivery attempt for the
		/// specified message header ID.
		/// </summary>
		/// <param name="HeaderID">The message header ID to load.</param>
		/// <returns>A null if no record is found or the most recent delivery attempt record.</returns>
		public virtual DeliveryAttemptRecord ReadMostRecentByHeaderID(int HeaderID)
			{
			DeliveryAttemptRecord record = null;
			SqlConnection conn = new SqlConnection(_connectionString);
			conn.Open();
			if (conn != null)
				{
				try
					{
					SqlCommand command = new SqlCommand("SELECT TOP 1 * FROM [DeliveryAttempt] " +
						"WHERE HeaderID=@HeaderID " +
						"ORDER BY AttemptDateTime DESC",conn);
					SqlParameter param = new SqlParameter("@HeaderID",SqlDbType.Int,0);
					param.Value = HeaderID;
					command.Parameters.Add(param);
					SqlDataReader rs = command.ExecuteReader();
					if (rs.Read())
						record = ReapRecord(rs);
					rs.Close();
					}
				finally
					{
					if (conn != null)
						conn.Close();
					}
				}
			return record;
			}
		}
	}
