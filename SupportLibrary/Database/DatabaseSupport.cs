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
using System.Configuration;

namespace SupportLibrary.Database
	{
	public class DatabaseSupport
		{
		public const int InvalidID = -1;

		/// <summary>
		/// Retrieves the connection string for the database.
		/// </summary>
		/// <returns>The defined string for the database from SMTPDB entry in config file.</returns>
		public static string GetConnectionString()
			{
			return ConfigurationManager.ConnectionStrings["SMTPDB"].ConnectionString;
			}


		/// <summary>
		/// Retrieves the identity value for the last added record on
		/// the specified connection.
		/// </summary>
		/// <param name="Conn">The connection used to INSERT</param>
		/// <returns>The newly assigned Identity value.</returns>
		public static int GetIdentity(SqlConnection Conn)
			{
			SqlCommand cmd = new SqlCommand("SELECT @@Identity AS IDENT", Conn);
			SqlDataReader reader = cmd.ExecuteReader();
			if (reader.Read())
				return Convert.ToInt32(reader[0].ToString());
			return -1;
			}
		}
	}
