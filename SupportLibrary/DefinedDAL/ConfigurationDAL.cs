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
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Data.Sql;
using System.Collections.Generic;
using SupportLibrary.Database;


namespace SupportLibrary.Database
	{
	public partial class ConfigurationDAL
		{
		public virtual void NewRecord(ConfigurationRecord record)
			{
			SqlConnection conn = new SqlConnection(_connectionString);
			conn.Open();
			if (conn != null)
				{
				try
					{
					string sql = "INSERT INTO [Configuration] " +
					"(ID,AllowLocalHost,AllowRemote,Port,ExpireAfterMinutes,RetryAfterMinutes,CleanupDays	) " +
					"VALUES (@ID,@AllowLocalHost,@AllowRemote,@Port,@ExpireAfterMinutes,@RetryAfterMinutes,@CleanupDays	)";
					SqlCommand command = new SqlCommand(sql,conn);

					SqlParameter param = new SqlParameter("@ID",SqlDbType.Int,0);
					param.Value = record.ID;
					command.Parameters.Add(param);

					param = new SqlParameter("@AllowLocalHost",SqlDbType.Bit,0);
					param.Value = record.AllowLocalHost;
					command.Parameters.Add(param);
					param = new SqlParameter("@AllowRemote",SqlDbType.Bit,0);
					param.Value = record.AllowRemote;
					command.Parameters.Add(param);
					param = new SqlParameter("@Port",SqlDbType.SmallInt,0);
					if (record.Port == DatabaseSupport.InvalidID)
						param.Value = DBNull.Value;
					else
						param.Value = record.Port;
					command.Parameters.Add(param);
					param = new SqlParameter("@ExpireAfterMinutes",SqlDbType.Int,0);
					if (record.ExpireAfterMinutes == DatabaseSupport.InvalidID)
						param.Value = DBNull.Value;
					else
						param.Value = record.ExpireAfterMinutes;
					command.Parameters.Add(param);
					param = new SqlParameter("@RetryAfterMinutes",SqlDbType.Int,0);
					if (record.RetryAfterMinutes == DatabaseSupport.InvalidID)
						param.Value = DBNull.Value;
					else
						param.Value = record.RetryAfterMinutes;
					command.Parameters.Add(param);
					param = new SqlParameter("@CleanupDays",SqlDbType.Int,0);
					if (record.CleanupDays == DatabaseSupport.InvalidID)
						param.Value = DBNull.Value;
					else
						param.Value = record.CleanupDays;
					command.Parameters.Add(param);
					command.ExecuteNonQuery();
					}
				finally
					{
					if (conn != null)
						conn.Close();
					}
				}
			}
		}
	}
