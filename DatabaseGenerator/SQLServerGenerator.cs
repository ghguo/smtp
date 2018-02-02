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
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.IO;


// This assumes the existence of the following classes in the destination project:
//
// 1. IRecord - an interface for all Record types that enforce a unique identity ID
// 2. Timestamp - a class containing a DateTime struct used by decoding methods
// 3. IDAL - an interface for the Data Access Layer



// This creates several partial classes:
//
// DALContainer - provides methods for accessing each individual database DAL and DataSource.
// *DAL - provides access to the underlying Database table
// *Record - provides a C# interface class to the columns in a row in the respective database table.


namespace DatabaseGenerator
	{
	class Column
		{
		public String name;
		public int xType;
		public int userType;
		public int precision;
		public bool isIdentity = false;
		public bool isNullable = false;
		public bool isForeignKey = false;
		public bool isUniqueConstraint = false;

		public Column(String Name,int XType,int UserType,int Precision,bool IsIdentity,bool IsNullable)
			{
			name = Name;
			xType = XType;
			userType = UserType;
			precision = Precision;
			isIdentity = IsIdentity;
			isNullable = IsNullable;
			}

		public string PropertyName
			{
			get
				{
				return "" + char.ToUpper(name[0]) + name.Substring(1);
				}
			}

		public bool IsIntegerType()
			{
			if (GetCSharpType().CompareTo("int") == 0 ||
				GetCSharpType().CompareTo("byte") == 0 ||
				GetCSharpType().CompareTo("short") == 0 ||
				GetCSharpType().CompareTo("long") == 0)
				return true;
			return false;
			}

		public string GetCSharpType()
			{
			switch (userType)
				{
				case 19:
				case 2:
				case 1:
					return "string";

				case 7:
					if (isNullable)
						return "int?";
					else
						return "int";

				case 8:
					if (isNullable)
						return "double?";
					else
						return "double";

				case 5:
					if (isNullable)
						return "byte?";
					else
						return "byte";

				case 6:
					if (isNullable)
						return "short?";
					else
						return "short";

				case 16:
					if (isNullable)
						return "bool?";
					else
						return "bool";

				case 12:
				case 80:
					if (isNullable)
						return "DateTime?";
					else
						return "DateTime";

				case 0:
					switch (xType)
						{
						case 127:
							if (isNullable)
								return "long?";
							else
								return "long";

						case 231:
						case 99:
						case 239:
							return "string";
						}
					break;
				}
			return "string";
			}

		public string GetSqlType()
			{
			switch (userType)
				{
				case 19:
					return "Text";

				case 2:
					return "VarChar";

				case 1:
					return "Char";

				case 7:
					return "Int";

				case 8:
					return "Float";

				case 5:
					return "TinyInt";

				case 6:
					return "SmallInt";

				case 16:
					return "Bit";

				case 12:
				case 80:
					return "DateTime";

				case 0:
					switch (xType)
						{
						case 127:
							return "BigInt";

						case 231:
							return "NVarChar";

						case 99:
							return "NText";

						case 239:
							return "NChar";
						}
					break;
				}
			return "Text";
			}

		public string GetDataType()
			{
			switch (userType)
				{
				case 19:
				case 2:
				case 1:
					return "String";

				case 7:
					return "Int32";

				case 8:
					return "Float";

				case 5:
					return "Byte";

				case 6:
					return "Int16";

				case 16:
					return "Boolean";

				case 12:
				case 80:
					return "DateTime";

				case 0:
					switch (xType)
						{
						case 127:
							return "Int64";

						case 231:
						case 99:
						case 239:
							return "String";
						}
					break;
				}
			return "String";
			}
		}


	/// <summary>
	/// Generator for a SQL server based IssueTracker database.
	/// </summary>
	public class SQLServerGenerator
		{
		private static string RecordPath = "GeneratedRecord\\";
		private static string DALPath = "GeneratedDAL\\";
		private static string DataSourcePath = "GeneratedDataSource\\";
		private static string DALSupportPath = "GeneratedDataSupport\\";


		/// <summary>
		/// Retrieves a list of tables from an open MSSQL database.
		/// </summary>
		/// <param name="Conn"></param>
		/// <returns></returns>
		static IList<string> GetTableList(SqlConnection Conn)
			{
			IList<string> list = new List<string>();
			try
				{
				SqlCommand command = new SqlCommand("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'",Conn);

				SqlDataReader rs = command.ExecuteReader();
				while (rs.Read())
					{
					String name = rs.GetString(rs.GetOrdinal("TABLE_NAME"));
					list.Add(name);
					}
				rs.Close();
				}
			catch (Exception ee)
				{
				Console.WriteLine("Error loading tables: " + ee.Message);
				throw ee;
				}
			return list;
			}




		/// <summary>
		/// Retrieves the fact that the column in the table is a primary key reference.
		/// </summary>
		/// <param name="Conn"></param>
		/// <param name="TableName"></param>
		/// <param name="ColumnName"></param>
		/// <returns></returns>
		static bool IsTableColumnPrimaryKey(SqlConnection Conn,string TableName,string ColumnName)
			{
			bool isPK = false;

			try
				{
				SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK, " +
					"INFORMATION_SCHEMA.KEY_COLUMN_USAGE C " +
					"WHERE PK.TABLE_NAME = '" + TableName + "' " +
					"AND CONSTRAINT_TYPE = 'PRIMARY KEY' " +
					"AND C.TABLE_NAME = PK.TABLE_NAME " +
					"AND C.CONSTRAINT_NAME = PK.CONSTRAINT_NAME " +
					"AND C.COLUMN_NAME = '" + ColumnName + "'",Conn);

				object o = command.ExecuteScalar();
				if (o != null)
					{
					int total = Convert.ToInt32(o.ToString());
					if (total > 0)
						isPK = true;
					}
				}
			catch (Exception ee)
				{
				Console.WriteLine("Error loading constraints for table: " + ee);
				throw ee;
				}

			return isPK;
			}

	
		/// <summary>
		/// Retrieves the fact that the column in the table is a foreign key reference.
		/// </summary>
		/// <param name="Conn"></param>
		/// <param name="TableName"></param>
		/// <param name="ColumnName"></param>
		/// <returns></returns>
		static bool IsTableColumnForeignKey(SqlConnection Conn,string TableName,string ColumnName)
			{
			bool isFK = false;

			try
				{
				SqlCommand command = new SqlCommand("SELECT KCU.TABLE_NAME AS TABLE_NAME, KCU.COLUMN_NAME AS COLUMN_NAME " +
					"FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC " +
					"LEFT JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU " +
					"  ON KCU.CONSTRAINT_CATALOG = RC.CONSTRAINT_CATALOG " +
					"  AND KCU.CONSTRAINT_SCHEMA = RC.CONSTRAINT_SCHEMA " +
					"  AND KCU.CONSTRAINT_NAME = RC.CONSTRAINT_NAME " +
					"WHERE KCU.TABLE_NAME='" + TableName + "' AND KCU.COLUMN_NAME='" + ColumnName + "'",Conn);

				SqlDataReader rs = command.ExecuteReader();
				if (rs.Read())
					isFK = true;

				rs.Close();
				}
			catch (Exception ee)
				{
				Console.WriteLine("Error loading constraints for table: " + ee);
				throw ee;
				}

			return isFK;
			}


		/// <summary>
		/// Retrieves the fact that the column in the table is a Unique reference.
		/// </summary>
		/// <param name="Conn">The open connection.</param>
		/// <param name="TableName">The table name.</param>
		/// <param name="ColumnName">The column name to check</param>
		/// <returns>True if the column in the specified table is a unique key.</returns>
		static bool IsTableColumnUniqueConstraint(SqlConnection Conn,string TableName,string ColumnName)
			{
			bool isUnique = false;
			string constraintName = string.Empty;

			try
				{
				SqlCommand command = new SqlCommand("SELECT KCU.TABLE_NAME AS TABLE_NAME, KCU.COLUMN_NAME AS COLUMN_NAME, TC.CONSTRAINT_NAME AS CONSTRAINT_NAME, TC.CONSTRAINT_TYPE AS CONSTRAINT_TYPE " +
					"FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC " +
					"LEFT JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU " + 
					"  ON KCU.CONSTRAINT_CATALOG = TC.CONSTRAINT_CATALOG " + 
					"  AND KCU.CONSTRAINT_SCHEMA = TC.CONSTRAINT_SCHEMA " + 
					"  AND KCU.CONSTRAINT_NAME = TC.CONSTRAINT_NAME " + 
					"WHERE TC.CONSTRAINT_TYPE LIKE '%UNIQUE%' " +
					"  AND KCU.TABLE_NAME='" + TableName + "' AND KCU.COLUMN_NAME='" + ColumnName + "'",Conn);

				SqlDataReader rs = command.ExecuteReader();
				if (rs.Read())
					{
					constraintName = rs.GetString(rs.GetOrdinal("CONSTRAINT_NAME"));
					isUnique = true;
					}

				rs.Close();

				if (isUnique)		// determine that this is not a multi-field unique key, in which case we will flip OFF the unique flag
					{
					command = new SqlCommand("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC " +
						"LEFT JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU " +
						"  ON KCU.CONSTRAINT_CATALOG = TC.CONSTRAINT_CATALOG " +
						"  AND KCU.CONSTRAINT_SCHEMA = TC.CONSTRAINT_SCHEMA " +
						"  AND KCU.CONSTRAINT_NAME = TC.CONSTRAINT_NAME " +
						"WHERE TC.CONSTRAINT_NAME='" + constraintName + "'",Conn);

					int referenceCount = (int)command.ExecuteScalar();
					if (referenceCount > 1)
						isUnique = false;
					}
				}
			catch (Exception ee)
				{
				Console.WriteLine("Error loading unique constraints for table: " + ee);
				throw ee;
				}

			return isUnique;
			}
	
		
		/// <summary>
		/// Retrieves the column definitions for the specific table.
		/// </summary>
		/// <param name="Conn"></param>
		/// <param name="TableName"></param>
		/// <returns></returns>
		static List<Column> DecodeTable(SqlConnection Conn,String TableName)
			{
			// To get type names: 
			// select cast( name as varchar(17) ) as name, usertype, xtype from systypes
			List<Column> list = new List<Column>();
			try
				{
				SqlCommand command = new SqlCommand("SELECT name,xtype,usertype,prec,status,isnullable FROM SYSCOLUMNS WHERE id=object_id('" + TableName + "')",Conn);
				//				SqlParameter param = new SqlParameter("@TableName",SqlDbType.VarChar,0);
				//				param.Value = TableName;
				//				command.Parameters.Add(param);

				SqlDataReader rs = command.ExecuteReader();
				while (rs.Read())
					{
					string name = rs.GetString(rs.GetOrdinal("name"));
					int xType = (int)rs.GetByte(rs.GetOrdinal("xtype"));
					int userType = (int)rs.GetInt16(rs.GetOrdinal("usertype"));
					int prec = 0;
					if (!rs.IsDBNull(rs.GetOrdinal("prec")))
						prec = (int)rs.GetInt16(rs.GetOrdinal("prec"));
					int status = (int)rs.GetByte(rs.GetOrdinal("status"));		// determines if identity column
					bool isNullable = rs.GetInt32(rs.GetOrdinal("isnullable")) == 0 ? false : true;
					list.Add(new Column(name,xType,userType,prec,status == 128,isNullable));
					}
				rs.Close();

				foreach (Column col in list)
					{
					if (SQLServerGenerator.IsTableColumnPrimaryKey(Conn,TableName,col.name))
						col.isIdentity = true;

					if (SQLServerGenerator.IsTableColumnForeignKey(Conn,TableName,col.name))
						col.isForeignKey = true;

					if (!col.isIdentity &&
						SQLServerGenerator.IsTableColumnUniqueConstraint(Conn,TableName,col.name))
						col.isUniqueConstraint = true;
					}
				}
			catch (Exception ee)
				{
				Console.WriteLine("Error loading columns for " + TableName + ": " + ee.Message);
				throw ee;
				}

			return list;
			}


		/// <summary>
		/// Reads a list of tables from the database.  For each table, it generates
		/// a partial class file with the same name as the table in the specified
		/// location.
		/// Login to db is Generator with pw 2g3n3r8
		/// </summary>
		/// <param name="Conn">A database connection</param>
		/// <param name="Path">The base path</param>
		static void Generate(SqlConnection Conn,String Path)
			{
			DateTime now = DateTime.Now;
			string filename = "";

			IList<String> tables = GetTableList(Conn);
			foreach (String table in tables)
				{
				if (string.Compare(table,"ITEM",true) == 0 ||
					string.Compare(table,"SECTION",true) == 0)
					continue;		// skip the menu item and section tables -- supported elsewhere


				IList<Column> columns = DecodeTable(Conn,table);

				if (columns.Count == 0)
					{
					Console.WriteLine("ERROR: No columns found in " + table);
					continue;
					}

				// we REQUIRE identity columns
				string identity = null;
				string identityType = null;
				string identityCSType = null;
				foreach (Column column in columns)
					{
					if (column.isIdentity)
						{
						identity = column.name;
						identityType = column.GetSqlType();
						identityCSType = column.GetCSharpType();
						break;
						}
					}

				if (identity == null)
					{
					Console.WriteLine("ERROR: No identity column found in " + table);
					continue;
					}

				// -- Generate the Record ----
				filename = table + ".cs";
				Console.WriteLine("Generating " + RecordPath + filename + "...");

				StreamWriter recordFile = new StreamWriter(Path + "\\" + RecordPath + filename,false);
				recordFile.WriteLine("// " + table + " record support file");
				recordFile.WriteLine("//");
				recordFile.WriteLine("// DO NOT MODIFY THIS FILE DIRECTLY.  IT WILL BE OVERWRITTEN.");
				recordFile.WriteLine("// Generated by SQLServerGenerator.cs");
				recordFile.WriteLine("// Copyright (c) 2010, Chris Laforet Software.  All rights reserved");
				recordFile.WriteLine("//");
				recordFile.WriteLine("// Generated at " + now.ToString("MM/dd/yyyy hh:mm"));
				recordFile.WriteLine();
				recordFile.WriteLine();
				recordFile.WriteLine("using System;");
				recordFile.WriteLine("using SupportLibrary.Database;");
				recordFile.WriteLine();
				recordFile.WriteLine();
				recordFile.WriteLine("namespace SupportLibrary.Database");
				recordFile.WriteLine("\t{");
				recordFile.WriteLine("\tpublic partial class " + table + "Record: IRecord");
				recordFile.WriteLine("\t\t{");
				recordFile.WriteLine("\t\tprivate " + identityCSType + " _" + identity + " = DatabaseSupport.InvalidID;");
				foreach (Column column in columns)
					{
					if (!string.Equals(column.name,identity,StringComparison.CurrentCultureIgnoreCase))
						recordFile.WriteLine("\t\tprivate " + column.GetCSharpType() + " _" + column.name + ";\t\t\t// " + table + "." + column.name);
					}
				recordFile.WriteLine();

				recordFile.WriteLine("\t\tpublic " + table + "Record()");
				recordFile.WriteLine("\t\t\t{");
				recordFile.WriteLine("\t\t\t}");
				recordFile.WriteLine();

				recordFile.WriteLine("\t\tpublic " + table + "Record(" + table + "Record Other)");
				recordFile.WriteLine("\t\t\t{");
				foreach (Column column in columns)
					recordFile.WriteLine("\t\t\tthis." + column.name + " = Other." + column.name + ";");
				recordFile.WriteLine("\t\t\t}");
				recordFile.WriteLine();

				recordFile.WriteLine("\t\tpublic " + identityCSType + " ID");
				recordFile.WriteLine("\t\t\t{");
				recordFile.WriteLine("\t\t\tset");
				recordFile.WriteLine("\t\t\t\t{");
				recordFile.WriteLine("\t\t\t\tthis._" + identity + " = value;");
				recordFile.WriteLine("\t\t\t\t}");

				recordFile.WriteLine("\t\t\tget");
				recordFile.WriteLine("\t\t\t\t{");
				recordFile.WriteLine("\t\t\t\treturn _" + identity + ";");
				recordFile.WriteLine("\t\t\t\t}");
				recordFile.WriteLine("\t\t\t}");

				if (String.Compare(identity,"ID",true) != 0)
					{
					recordFile.WriteLine();
					recordFile.WriteLine("\t\tpublic " + identityCSType + " " + identity + "\t\t\t// alias for the ID field");
					recordFile.WriteLine("\t\t\t{");
					recordFile.WriteLine("\t\t\tset");
					recordFile.WriteLine("\t\t\t\t{");
					recordFile.WriteLine("\t\t\t\tthis._" + identity + " = value;");
					recordFile.WriteLine("\t\t\t\t}");

					recordFile.WriteLine("\t\t\tget");
					recordFile.WriteLine("\t\t\t\t{");
					recordFile.WriteLine("\t\t\t\treturn _" + identity + ";");
					recordFile.WriteLine("\t\t\t\t}");
					recordFile.WriteLine("\t\t\t}");
					}

				foreach (Column column in columns)
					{
					if (!string.Equals(column.name,identity,StringComparison.CurrentCultureIgnoreCase))
						{
						recordFile.WriteLine();

						string type = column.GetCSharpType();

						recordFile.WriteLine("\t\tpublic " + type + " " + column.PropertyName);
						recordFile.WriteLine("\t\t\t{");
						recordFile.WriteLine("\t\t\tset");
						recordFile.WriteLine("\t\t\t\t{");
						recordFile.WriteLine("\t\t\t\tthis._" + column.name + " = value;");
						recordFile.WriteLine("\t\t\t\t}");

						recordFile.WriteLine("\t\t\tget");
						recordFile.WriteLine("\t\t\t\t{");
						if (type.CompareTo("string") == 0)
							recordFile.WriteLine("\t\t\t\treturn (_" + column.name + " == null ? \"\" : _" + column.name + ");");
						else
							recordFile.WriteLine("\t\t\t\treturn _" + column.name + ";");
						recordFile.WriteLine("\t\t\t\t}");

						recordFile.WriteLine("\t\t\t}");
						}
					}

				recordFile.WriteLine("\t\t}");
				recordFile.WriteLine("\t}");
				recordFile.Close();

				// -- Generate the DAL ----
				filename = table + "DAL.cs";
				Console.WriteLine("Generating " + DALPath + filename + "...");

				StreamWriter handlerFile = new StreamWriter(Path + "\\" + DALPath + filename,false);
				handlerFile.WriteLine("// " + table + " Data Access Layer");
				handlerFile.WriteLine("//");
				handlerFile.WriteLine("// DO NOT MODIFY THIS FILE DIRECTLY.  IT WILL BE OVERWRITTEN.");
				handlerFile.WriteLine("// Generated by SQLServerGenerator.cs");
				handlerFile.WriteLine("// Copyright (c) 2010, Chris Laforet Software.  All rights reserved");
				handlerFile.WriteLine("//");
				handlerFile.WriteLine("// Generated at " + now.ToString("MM/dd/yyyy hh:mm"));
				handlerFile.WriteLine();
				handlerFile.WriteLine();
				handlerFile.WriteLine("using System;");
				handlerFile.WriteLine("using System.ComponentModel;");
				handlerFile.WriteLine("using System.Data;");
				handlerFile.WriteLine("using System.Data.SqlClient;");
				handlerFile.WriteLine("using System.Data.SqlTypes;");
				handlerFile.WriteLine("using System.Data.Sql;");
				handlerFile.WriteLine("using System.Collections.Generic;");
				handlerFile.WriteLine("using SupportLibrary.Database;");
				handlerFile.WriteLine();
				handlerFile.WriteLine();
				handlerFile.WriteLine("namespace SupportLibrary.Database");
				handlerFile.WriteLine("\t{");
				handlerFile.WriteLine("\tpublic partial class " + table + "DAL: IDAL<" + table + "Record>");
				handlerFile.WriteLine("\t\t{");
				handlerFile.WriteLine("\t\tprivate readonly string _connectionString;");
				handlerFile.WriteLine("\t\tpublic readonly string TableName = \"" + table + "\";");
				handlerFile.WriteLine();
				handlerFile.WriteLine("\t\tpublic " + table + "DAL(string ConnectionString)");
				handlerFile.WriteLine("\t\t\t{");
				handlerFile.WriteLine("\t\t\t_connectionString = ConnectionString;");
				handlerFile.WriteLine("\t\t\t}");
				handlerFile.WriteLine();

				handlerFile.WriteLine("\t\tstatic public " + table + "Record ReapRecord(SqlDataReader Reader)");
				handlerFile.WriteLine("\t\t\t{");
				foreach (Column column in columns)
					{
					string typeName = column.GetCSharpType();
					if (typeName.CompareTo("DateTime") == 0)
						handlerFile.WriteLine("\t\t\t" + typeName + " " + column.PropertyName + " = Reader.GetDateTime(Reader.GetOrdinal(\"" + column.name + "\"));");
					else if (typeName.CompareTo("DateTime?") == 0)
						{
						//						handlerFile.WriteLine("\t\t\tChrisLaforetSoftwareLibrary.Timestamp " + column.PropertyName + "Stamp = ChrisLaforetSoftwareLibrary.Database.getDateTimeWithoutNull(Reader,\"" + column.name + "\");");
						handlerFile.WriteLine("\t\t\t" + typeName + " " + column.PropertyName + " = null;");
						handlerFile.WriteLine("\t\t\tif (!Reader.IsDBNull(Reader.GetOrdinal(\"" + column.name + "\")))");
						handlerFile.WriteLine("\t\t\t\t" + column.PropertyName + " = Reader.GetDateTime(Reader.GetOrdinal(\"" + column.name + "\"));");
						}
					else if (column.isNullable)
						{
						handlerFile.WriteLine("\t\t\t" + typeName + " " + column.PropertyName + " = null;");
						handlerFile.WriteLine("\t\t\tif (!Reader.IsDBNull(Reader.GetOrdinal(\"" + column.name + "\")))");
						handlerFile.WriteLine("\t\t\t\t" + column.PropertyName + " = Reader.Get" + column.GetDataType() + "(Reader.GetOrdinal(\"" + column.name + "\"));");
						}
					else
						handlerFile.WriteLine("\t\t\t" + typeName + " " + column.PropertyName + " = Reader.Get" + column.GetDataType() + "(Reader.GetOrdinal(\"" + column.name + "\"));");
					}
				handlerFile.WriteLine("\t\t\t" + table + "Record record = new " + table + "Record();");
				handlerFile.WriteLine();
				foreach (Column column in columns)
					handlerFile.WriteLine("\t\t\trecord." + column.PropertyName + " = " + column.PropertyName + ";");
				handlerFile.WriteLine("\t\t\treturn record;");
				handlerFile.WriteLine("\t\t\t}");
				handlerFile.WriteLine();

				handlerFile.WriteLine("\t\tpublic virtual IList<" + table + "Record> ReadAllRecords()");
				handlerFile.WriteLine("\t\t\t{");
				handlerFile.WriteLine("\t\t\tIList<" + table + "Record> list = new List<" + table + "Record>(64);");
				handlerFile.WriteLine("\t\t\tSqlConnection conn = new SqlConnection(_connectionString);");
				handlerFile.WriteLine("\t\t\tconn.Open();");
				handlerFile.WriteLine("\t\t\tif (conn != null)");
				handlerFile.WriteLine("\t\t\t\t{");
				handlerFile.WriteLine("\t\t\t\ttry");
				handlerFile.WriteLine("\t\t\t\t\t{");
				handlerFile.WriteLine("\t\t\t\t\tSqlCommand command = new SqlCommand(\"SELECT * FROM [" + table + "]\",conn);");
				handlerFile.WriteLine("\t\t\t\t\tSqlDataReader rs = command.ExecuteReader();");
				handlerFile.WriteLine("\t\t\t\t\twhile (rs.Read())");
				handlerFile.WriteLine("\t\t\t\t\t\tlist.Add(ReapRecord(rs));");
				handlerFile.WriteLine("\t\t\t\t\trs.Close();");
				handlerFile.WriteLine("\t\t\t\t\t}");
				handlerFile.WriteLine("\t\t\t\tfinally");
				handlerFile.WriteLine("\t\t\t\t\t{");
				handlerFile.WriteLine("\t\t\t\t\tif (conn != null)");
				handlerFile.WriteLine("\t\t\t\t\t\tconn.Close();");
				handlerFile.WriteLine("\t\t\t\t\t}");
				handlerFile.WriteLine("\t\t\t\t}");
				handlerFile.WriteLine("\t\t\treturn list;");
				handlerFile.WriteLine("\t\t\t}");

				handlerFile.WriteLine();
				handlerFile.WriteLine("\t\tpublic virtual " + table + "Record ReadRecord(" + identityCSType + " RecordID)");
				handlerFile.WriteLine("\t\t\t{");
				handlerFile.WriteLine("\t\t\t" + table + "Record record = null;");
				handlerFile.WriteLine("\t\t\tSqlConnection conn = new SqlConnection(_connectionString);");
				handlerFile.WriteLine("\t\t\tconn.Open();");
				handlerFile.WriteLine("\t\t\tif (conn != null)");
				handlerFile.WriteLine("\t\t\t\t{");
				handlerFile.WriteLine("\t\t\t\ttry");
				handlerFile.WriteLine("\t\t\t\t\t{");
				handlerFile.WriteLine("\t\t\t\t\tSqlCommand command = new SqlCommand(\"SELECT * FROM [" + table + "] WHERE " + identity + "=@" + identity + "\",conn);");
				handlerFile.WriteLine("\t\t\t\t\tSqlParameter param = new SqlParameter(\"@" + identity + "\",SqlDbType." + identityType + ",0);");
				handlerFile.WriteLine("\t\t\t\t\tparam.Value = RecordID;");
				handlerFile.WriteLine("\t\t\t\t\tcommand.Parameters.Add(param);");
				handlerFile.WriteLine("\t\t\t\t\tSqlDataReader rs = command.ExecuteReader();");
				handlerFile.WriteLine("\t\t\t\t\tif (rs.Read())");
				handlerFile.WriteLine("\t\t\t\t\t\trecord = ReapRecord(rs);");
				handlerFile.WriteLine("\t\t\t\t\trs.Close();");
				handlerFile.WriteLine("\t\t\t\t\t}");
				handlerFile.WriteLine("\t\t\t\tfinally");
				handlerFile.WriteLine("\t\t\t\t\t{");
				handlerFile.WriteLine("\t\t\t\t\tif (conn != null)");
				handlerFile.WriteLine("\t\t\t\t\t\tconn.Close();");
				handlerFile.WriteLine("\t\t\t\t\t}");
				handlerFile.WriteLine("\t\t\t\t}");
				handlerFile.WriteLine("\t\t\treturn record;");
				handlerFile.WriteLine("\t\t\t}");

				// read all records by fieldname
				foreach (Column column in columns)
					{
					if (column.name.CompareTo(identity) != 0)
						{
						string typeName = column.GetCSharpType();
						string sqlTypeName = column.GetSqlType();

						if (column.isUniqueConstraint)
							{
							handlerFile.WriteLine();
							handlerFile.WriteLine("\t\tpublic virtual " + table + "Record ReadRecordBy" + column.name + "(" + typeName + " " + column.name + ")");
							handlerFile.WriteLine("\t\t\t{");
							handlerFile.WriteLine("\t\t\t" + table + "Record record = null;");
							handlerFile.WriteLine("\t\t\tSqlConnection conn = new SqlConnection(_connectionString);");
							handlerFile.WriteLine("\t\t\tconn.Open();");
							handlerFile.WriteLine("\t\t\tif (conn != null)");
							handlerFile.WriteLine("\t\t\t\t{");
							handlerFile.WriteLine("\t\t\t\ttry");
							handlerFile.WriteLine("\t\t\t\t\t{");
							handlerFile.WriteLine("\t\t\t\t\tSqlCommand command = new SqlCommand(\"SELECT * FROM [" + table + "] WHERE " + column.name + "=@" + column.name + "\",conn);");
							handlerFile.WriteLine("\t\t\t\t\tSqlParameter param = new SqlParameter(\"@" + column.name + "\",SqlDbType." + sqlTypeName + ",0);");
							handlerFile.WriteLine("\t\t\t\t\tparam.Value = " + column.name + ";");
							handlerFile.WriteLine("\t\t\t\t\tcommand.Parameters.Add(param);");
							handlerFile.WriteLine("\t\t\t\t\tSqlDataReader rs = command.ExecuteReader();");
							handlerFile.WriteLine("\t\t\t\t\tif (rs.Read())");
							handlerFile.WriteLine("\t\t\t\t\t\trecord = ReapRecord(rs);");
							handlerFile.WriteLine("\t\t\t\t\trs.Close();");
							handlerFile.WriteLine("\t\t\t\t\t}");
							handlerFile.WriteLine("\t\t\t\tfinally");
							handlerFile.WriteLine("\t\t\t\t\t{");
							handlerFile.WriteLine("\t\t\t\t\tif (conn != null)");
							handlerFile.WriteLine("\t\t\t\t\t\tconn.Close();");
							handlerFile.WriteLine("\t\t\t\t\t}");
							handlerFile.WriteLine("\t\t\t\t}");
							handlerFile.WriteLine("\t\t\treturn record;");
							handlerFile.WriteLine("\t\t\t}");
							}
						else if (column.isForeignKey)		// foreign keys (nullable or not)
							{
							if (typeName.EndsWith("?"))
								typeName = typeName.Substring(0,typeName.Length - 1);		// we don't accept null...

							handlerFile.WriteLine();
							handlerFile.WriteLine("\t\tpublic virtual IList<" + table + "Record> ReadAllRecordsBy" + column.name + "(" + typeName + " " + column.name + ")");
							handlerFile.WriteLine("\t\t\t{");

							handlerFile.WriteLine("\t\t\tIList<" + table + "Record> list = new List<" + table + "Record>(64);");
							handlerFile.WriteLine("\t\t\tSqlConnection conn = new SqlConnection(_connectionString);");
							handlerFile.WriteLine("\t\t\tconn.Open();");
							handlerFile.WriteLine("\t\t\tif (conn != null)");
							handlerFile.WriteLine("\t\t\t\t{");
							handlerFile.WriteLine("\t\t\t\ttry");
							handlerFile.WriteLine("\t\t\t\t\t{");
							handlerFile.WriteLine("\t\t\t\t\tSqlCommand command = new SqlCommand(\"SELECT * FROM [" + table + "] WHERE " + column.name + "=@" + column.name + "\",conn);");
							handlerFile.WriteLine("\t\t\t\t\tSqlParameter param = new SqlParameter(\"@" + column.name + "\",SqlDbType." + sqlTypeName + ",0);");
							handlerFile.WriteLine("\t\t\t\t\tparam.Value = " + column.name + ";");
							handlerFile.WriteLine("\t\t\t\t\tcommand.Parameters.Add(param);");
							handlerFile.WriteLine("\t\t\t\t\tSqlDataReader rs = command.ExecuteReader();");
							handlerFile.WriteLine("\t\t\t\t\twhile (rs.Read())");
							handlerFile.WriteLine("\t\t\t\t\t\tlist.Add(ReapRecord(rs));");
							handlerFile.WriteLine("\t\t\t\t\trs.Close();");
							handlerFile.WriteLine("\t\t\t\t\t}");
							handlerFile.WriteLine("\t\t\t\tfinally");
							handlerFile.WriteLine("\t\t\t\t\t{");
							handlerFile.WriteLine("\t\t\t\t\tif (conn != null)");
							handlerFile.WriteLine("\t\t\t\t\t\tconn.Close();");
							handlerFile.WriteLine("\t\t\t\t\t}");
							handlerFile.WriteLine("\t\t\t\t}");
							handlerFile.WriteLine("\t\t\treturn list;");
							handlerFile.WriteLine("\t\t\t}");
							}
						}
					}

				handlerFile.WriteLine();
				handlerFile.WriteLine("\t\tpublic virtual void CreateRecord(" + table + "Record record)");
				handlerFile.WriteLine("\t\t\t{");
				handlerFile.WriteLine("\t\t\tSqlConnection conn = new SqlConnection(_connectionString);");
				handlerFile.WriteLine("\t\t\tconn.Open();");
				handlerFile.WriteLine("\t\t\tif (conn != null)");
				handlerFile.WriteLine("\t\t\t\t{");
				handlerFile.WriteLine("\t\t\t\ttry");
				handlerFile.WriteLine("\t\t\t\t\t{");
				handlerFile.WriteLine("\t\t\t\t\tstring sql = \"INSERT INTO [" + table + "] \" + ");
				handlerFile.Write("\t\t\t\t\t\"(");
				bool gotOne = false;
				foreach (Column column in columns)
					{
					if (!string.Equals(column.name,identity,StringComparison.CurrentCultureIgnoreCase))
						{
						handlerFile.Write((gotOne ? "," : "") + column.name);
						gotOne = true;
						}
					}
				handlerFile.WriteLine("\t) \" +");
				handlerFile.Write("\t\t\t\t\t\"VALUES (");
				gotOne = false;
				foreach (Column column in columns)
					{
					if (!string.Equals(column.name,identity,StringComparison.CurrentCultureIgnoreCase))
						{
						handlerFile.Write((gotOne ? "," : "") + "@" + column.name);
						gotOne = true;
						}
					}
				handlerFile.WriteLine("\t)\";");
				handlerFile.WriteLine("\t\t\t\t\tSqlCommand command = new SqlCommand(sql,conn);");

				gotOne = false;
				foreach (Column column in columns)
					{
					if (!string.Equals(column.name,identity,StringComparison.CurrentCultureIgnoreCase))
						{
						handlerFile.WriteLine("\t\t\t\t\t" + (!gotOne ? "SqlParameter " : "") + "param = new SqlParameter(\"@" + column.name + "\",SqlDbType." + column.GetSqlType() + ",0);");
						gotOne = true;
						if (column.IsIntegerType() && !column.isIdentity)
							{
							handlerFile.WriteLine("\t\t\t\t\tif (record." + column.PropertyName + " == DatabaseSupport.InvalidID)");
							handlerFile.WriteLine("\t\t\t\t\t\tparam.Value = DBNull.Value;");
							handlerFile.WriteLine("\t\t\t\t\telse");
							handlerFile.WriteLine("\t\t\t\t\t\tparam.Value = record." + column.PropertyName + ";");
							}
						else if (column.isNullable && !column.isIdentity)
							{
							handlerFile.WriteLine("\t\t\t\t\tif (record." + column.PropertyName + " == null)");
							handlerFile.WriteLine("\t\t\t\t\t\tparam.Value = DBNull.Value;");
							handlerFile.WriteLine("\t\t\t\t\telse");
							handlerFile.WriteLine("\t\t\t\t\t\tparam.Value = record." + column.PropertyName + ";");
							}
						else
							handlerFile.WriteLine("\t\t\t\t\tparam.Value = record." + column.PropertyName + ";");

						handlerFile.WriteLine("\t\t\t\t\tcommand.Parameters.Add(param);");
						}
					}
				handlerFile.WriteLine("\t\t\t\t\tcommand.ExecuteNonQuery();");
				handlerFile.WriteLine("\t\t\t\t\trecord.ID = DatabaseSupport.GetIdentity(conn);");
				handlerFile.WriteLine("\t\t\t\t\t}");
				handlerFile.WriteLine("\t\t\t\tfinally");
				handlerFile.WriteLine("\t\t\t\t\t{");
				handlerFile.WriteLine("\t\t\t\t\tif (conn != null)");
				handlerFile.WriteLine("\t\t\t\t\t\tconn.Close();");
				handlerFile.WriteLine("\t\t\t\t\t}");
				handlerFile.WriteLine("\t\t\t\t}");
				handlerFile.WriteLine("\t\t\t}");
				handlerFile.WriteLine();

				handlerFile.Write("\t\tpublic virtual " + identityCSType + " CreateRecord(");
				gotOne = false;
				foreach (Column column in columns)
					{
					if (!string.Equals(column.name,identity,StringComparison.CurrentCultureIgnoreCase))
						{
						handlerFile.Write((gotOne ? "," : "") + column.GetCSharpType() + " " + column.name);
						gotOne = true;
						}
					}
				handlerFile.WriteLine(")");
				handlerFile.WriteLine("\t\t\t{");
				handlerFile.WriteLine("\t\t\t// populate the record with items");
				handlerFile.WriteLine("\t\t\t" + table + "Record record = new " + table + "Record();");
				foreach (Column column in columns)
					if (!string.Equals(column.name,identity,StringComparison.CurrentCultureIgnoreCase))
						handlerFile.WriteLine("\t\t\trecord." + column.PropertyName + " = " + column.name + ";");
				handlerFile.WriteLine();
				handlerFile.WriteLine("\t\t\t// write the record out");
				handlerFile.WriteLine("\t\t\tCreateRecord(record);");
				handlerFile.WriteLine("\t\t\treturn record." + identity + ";");
				handlerFile.WriteLine("\t\t\t}");
				handlerFile.WriteLine();

				handlerFile.WriteLine("\t\tpublic virtual void UpdateRecord(" + table + "Record record)");
				handlerFile.WriteLine("\t\t\t{");
				handlerFile.WriteLine("\t\t\tSqlConnection conn = new SqlConnection(_connectionString);");
				handlerFile.WriteLine("\t\t\tconn.Open();");
				handlerFile.WriteLine("\t\t\tif (conn != null)");
				handlerFile.WriteLine("\t\t\t\t{");
				handlerFile.WriteLine("\t\t\t\ttry");
				handlerFile.WriteLine("\t\t\t\t\t{");
				handlerFile.WriteLine("\t\t\t\t\tstring sql = \"UPDATE [" + table + "] \" +");
				handlerFile.Write("\t\t\t\t\t\"SET ");
				gotOne = false;
				foreach (Column column in columns)
					{
					if (!string.Equals(column.name,identity,StringComparison.CurrentCultureIgnoreCase))
						{
						handlerFile.Write((gotOne ? "," : "") + column.name + "=@" + column.name);
						gotOne = true;
						}
					}
				handlerFile.WriteLine(" WHERE " + identity + "=@" + identity + "\";");

				handlerFile.WriteLine("\t\t\t\t\tSqlCommand command = new SqlCommand(sql,conn);");

				gotOne = false;
				foreach (Column column in columns)
					{
					if (!string.Equals(column.name,identity,StringComparison.CurrentCultureIgnoreCase))
						{
						handlerFile.WriteLine("\t\t\t\t\t" + (!gotOne ? "SqlParameter " : "") + "param = new SqlParameter(\"@" + column.name + "\",SqlDbType." + column.GetSqlType() + ",0);");
						gotOne = true;
						if (column.IsIntegerType() && !column.isIdentity)
							{
							handlerFile.WriteLine("\t\t\t\t\tif (record." + column.PropertyName + " == DatabaseSupport.InvalidID)");
							handlerFile.WriteLine("\t\t\t\t\t\tparam.Value = DBNull.Value;");
							handlerFile.WriteLine("\t\t\t\t\telse");
							handlerFile.WriteLine("\t\t\t\t\t\tparam.Value = record." + column.PropertyName + ";");
							}
						else if (column.isNullable && !column.isIdentity)
							{
							handlerFile.WriteLine("\t\t\t\t\tif (record." + column.PropertyName + " == null)");
							handlerFile.WriteLine("\t\t\t\t\t\tparam.Value = DBNull.Value;");
							handlerFile.WriteLine("\t\t\t\t\telse");
							handlerFile.WriteLine("\t\t\t\t\t\tparam.Value = record." + column.PropertyName + ";");
							}
						else
							handlerFile.WriteLine("\t\t\t\t\tparam.Value = record." + column.PropertyName + ";");

						//handlerFile.WriteLine("\t\t\t\t\tparam.Value = record." + column.PropertyName + ";");
						handlerFile.WriteLine("\t\t\t\t\tcommand.Parameters.Add(param);");
						}
					}

				handlerFile.WriteLine("\t\t\t\t\t" + (!gotOne ? "SqlParameter " : "") + "param = new SqlParameter(\"@" + identity + "\",SqlDbType." + identityType + ",0);");
				handlerFile.WriteLine("\t\t\t\t\tparam.Value = record.ID;");
				handlerFile.WriteLine("\t\t\t\t\tcommand.Parameters.Add(param);");

				handlerFile.WriteLine("\t\t\t\t\tcommand.ExecuteNonQuery();");
				handlerFile.WriteLine("\t\t\t\t\t}");
				handlerFile.WriteLine("\t\t\t\tfinally");
				handlerFile.WriteLine("\t\t\t\t\t{");
				handlerFile.WriteLine("\t\t\t\t\tif (conn != null)");
				handlerFile.WriteLine("\t\t\t\t\t\tconn.Close();");
				handlerFile.WriteLine("\t\t\t\t\t}");
				handlerFile.WriteLine("\t\t\t\t}");
				handlerFile.WriteLine("\t\t\t}");
				handlerFile.WriteLine();

				handlerFile.Write("\t\tpublic virtual void UpdateRecord(");
				handlerFile.Write(identityCSType + " " + identity);
				foreach (Column column in columns)
					{
					if (!string.Equals(column.name,identity,StringComparison.CurrentCultureIgnoreCase))
						handlerFile.Write((gotOne ? "," : "") + column.GetCSharpType() + " " + column.name);
					}
				handlerFile.WriteLine(")");
				handlerFile.WriteLine("\t\t\t{");
				handlerFile.WriteLine("\t\t\t// populate the record with items");
				handlerFile.WriteLine("\t\t\t" + table + "Record record = new " + table + "Record();");
				handlerFile.WriteLine("\t\t\trecord.ID = " + identity + ";");

				foreach (Column column in columns)
					if (!string.Equals(column.name,identity,StringComparison.CurrentCultureIgnoreCase))
						handlerFile.WriteLine("\t\t\trecord." + column.PropertyName + " = " + column.name + ";");

				handlerFile.WriteLine();
				handlerFile.WriteLine("\t\t\t// write the record out");
				handlerFile.WriteLine("\t\t\tUpdateRecord(record);");
				handlerFile.WriteLine("\t\t\t}");
				handlerFile.WriteLine();
				handlerFile.WriteLine("\t\tpublic virtual void DeleteRecord(" + identityCSType + " RecordID)");
				handlerFile.WriteLine("\t\t\t{");
				handlerFile.WriteLine("\t\t\tSqlConnection conn = new SqlConnection(_connectionString);");
				handlerFile.WriteLine("\t\t\tconn.Open();");
				handlerFile.WriteLine("\t\t\tif (conn != null)");
				handlerFile.WriteLine("\t\t\t\t{");
				handlerFile.WriteLine("\t\t\t\ttry");
				handlerFile.WriteLine("\t\t\t\t\t{");
				handlerFile.WriteLine("\t\t\t\t\tstring sql = \"DELETE FROM [" + table + "] WHERE " + identity + "=@" + identity + "\";");
				handlerFile.WriteLine("\t\t\t\t\tSqlCommand command = new SqlCommand(sql,conn);");
				handlerFile.WriteLine("\t\t\t\t\tSqlParameter param = new SqlParameter(\"@" + identity + "\",SqlDbType." + identityType + ",0);");
				handlerFile.WriteLine("\t\t\t\t\tparam.Value = RecordID;");
				handlerFile.WriteLine("\t\t\t\t\tcommand.Parameters.Add(param);");
				handlerFile.WriteLine("\t\t\t\t\tcommand.ExecuteNonQuery();");
				handlerFile.WriteLine("\t\t\t\t\t}");
				handlerFile.WriteLine("\t\t\t\tfinally");
				handlerFile.WriteLine("\t\t\t\t\t{");
				handlerFile.WriteLine("\t\t\t\t\tif (conn != null)");
				handlerFile.WriteLine("\t\t\t\t\t\tconn.Close();");
				handlerFile.WriteLine("\t\t\t\t\t}");
				handlerFile.WriteLine("\t\t\t\t}");
				handlerFile.WriteLine("\t\t\t}");
				handlerFile.WriteLine();

				handlerFile.WriteLine("\t\tpublic virtual void DeleteRecord(" + table + "Record record)");
				handlerFile.WriteLine("\t\t\t{");
				handlerFile.WriteLine("\t\t\tDeleteRecord(record.ID);");
				handlerFile.WriteLine("\t\t\t}");

				handlerFile.WriteLine("\t\t}");
				handlerFile.WriteLine("\t}");
				handlerFile.Close();

				// -- Adds data support ------
				filename = table + "DataSource.cs";
				Console.WriteLine("Generating " + DataSourcePath + filename + "...");

				StreamWriter datasourceFile = new StreamWriter(Path + "\\" + DataSourcePath + filename,false);
				datasourceFile.WriteLine("// " + table + "DataSource Web Interface support file");
				datasourceFile.WriteLine("//");
				datasourceFile.WriteLine("// DO NOT MODIFY THIS FILE DIRECTLY.  IT WILL BE OVERWRITTEN.");
				datasourceFile.WriteLine("// Generated by SQLServerGenerator.cs");
				datasourceFile.WriteLine("// Copyright (c) 2010, Chris Laforet Software.  All rights reserved");
				datasourceFile.WriteLine("//");
				datasourceFile.WriteLine("// Generated at " + now.ToString("MM/dd/yyyy hh:mm"));
				datasourceFile.WriteLine();
				datasourceFile.WriteLine();
				datasourceFile.WriteLine("using System;");
				datasourceFile.WriteLine("using System.ComponentModel;");
				datasourceFile.WriteLine("using System.Collections.Generic;");
				datasourceFile.WriteLine("using System.Web.UI.WebControls;");
				datasourceFile.WriteLine("using SupportLibrary.Database;");
				datasourceFile.WriteLine();
				datasourceFile.WriteLine("namespace SupportLibrary.DataSource");
				datasourceFile.WriteLine("\t{");
				datasourceFile.WriteLine("\tpublic partial class " + table + "DataSource : ObjectDataSource");
				datasourceFile.WriteLine("\t\t{");
				datasourceFile.WriteLine("\t\tpublic static IList<" + table + "Record> ReadAllRecords()");
				datasourceFile.WriteLine("\t\t\t{");
				datasourceFile.WriteLine("\t\t\tDALContainer dc = DALContainer.GetDALContainer();");
				datasourceFile.WriteLine("\t\t\t" + table + "DAL dal = dc.Get" + table + "DAL();");
				datasourceFile.WriteLine("\t\t\treturn dal.ReadAllRecords();");
				datasourceFile.WriteLine("\t\t\t}");
				datasourceFile.WriteLine();
				datasourceFile.WriteLine("\t\tpublic static " + table + "Record ReadRecord(int RecordID)");
				datasourceFile.WriteLine("\t\t\t{");
				datasourceFile.WriteLine("\t\t\tDALContainer dc = DALContainer.GetDALContainer();");
				datasourceFile.WriteLine("\t\t\t" + table + "DAL dal = dc.Get" + table + "DAL();");
				datasourceFile.WriteLine("\t\t\treturn dal.ReadRecord(RecordID);");
				datasourceFile.WriteLine("\t\t\t}");
				datasourceFile.WriteLine();

				// read all records by fieldname
				foreach (Column column in columns)
					{
					if (column.name.CompareTo(identity) != 0)
						{
						string typeName = column.GetCSharpType();

						if (column.isUniqueConstraint)
							{
							datasourceFile.WriteLine("\t\tpublic static " + table + "Record ReadRecordBy" + column.name + "(" + typeName + " " + column.name + ")");
							datasourceFile.WriteLine("\t\t\t{");
							datasourceFile.WriteLine("\t\t\tDALContainer dc = DALContainer.GetDALContainer();");
							datasourceFile.WriteLine("\t\t\t" + table + "DAL dal = dc.Get" + table + "DAL();");
							datasourceFile.WriteLine("\t\t\treturn dal.ReadRecordBy" + column.name + "(" + column.name + ");");
							datasourceFile.WriteLine("\t\t\t}");
							datasourceFile.WriteLine();
							}
						else if (column.isForeignKey)		// foreign keys (nullable or not)
							{
							if (typeName.EndsWith("?"))
								typeName = typeName.Substring(0,typeName.Length - 1);		// we don't accept null...

							datasourceFile.WriteLine("\t\tpublic static IList<" + table + "Record> ReadAllRecordsBy" + column.name + "(" + typeName + " " + column.name + ")");
							datasourceFile.WriteLine("\t\t\t{");
							datasourceFile.WriteLine("\t\t\tDALContainer dc = DALContainer.GetDALContainer();");
							datasourceFile.WriteLine("\t\t\t" + table + "DAL dal = dc.Get" + table + "DAL();");
							datasourceFile.WriteLine("\t\t\treturn dal.ReadAllRecordsBy" + column.name + "(" + column.name + ");");
							datasourceFile.WriteLine("\t\t\t}");
							datasourceFile.WriteLine();
							}
						}
					}

				datasourceFile.WriteLine("\t\tpublic static void CreateRecord(" + table + "Record Record)");
				datasourceFile.WriteLine("\t\t\t{");
				datasourceFile.WriteLine("\t\t\tDALContainer dc = DALContainer.GetDALContainer();");
				datasourceFile.WriteLine("\t\t\t" + table + "DAL dal = dc.Get" + table + "DAL();");
				datasourceFile.WriteLine("\t\t\tdal.CreateRecord(Record);");
				datasourceFile.WriteLine("\t\t\t}");
				datasourceFile.WriteLine();
				datasourceFile.WriteLine("\t\tpublic static void UpdateRecord(" + table + "Record Record)");
				datasourceFile.WriteLine("\t\t\t{");
				datasourceFile.WriteLine("\t\t\tDALContainer dc = DALContainer.GetDALContainer();");
				datasourceFile.WriteLine("\t\t\t" + table + "DAL dal = dc.Get" + table + "DAL();");
				datasourceFile.WriteLine("\t\t\tdal.UpdateRecord(Record);");
				datasourceFile.WriteLine("\t\t\t}");
				datasourceFile.WriteLine();
				datasourceFile.WriteLine("\t\tpublic static void DeleteRecord(int RecordID)");
				datasourceFile.WriteLine("\t\t\t{");
				datasourceFile.WriteLine("\t\t\tDALContainer dc = DALContainer.GetDALContainer();");
				datasourceFile.WriteLine("\t\t\t" + table + "DAL dal = dc.Get" + table + "DAL();");
				datasourceFile.WriteLine("\t\t\tdal.DeleteRecord(RecordID);");
				datasourceFile.WriteLine("\t\t\t}");
				datasourceFile.WriteLine("\t\t}");
				datasourceFile.WriteLine("\t}");
				datasourceFile.Close();
				}

			// -- Adds data support ------
			filename = "DALContainer.cs";
			Console.WriteLine("Generating " + DALSupportPath + filename + "...");

			StreamWriter interfaceFile = new StreamWriter(Path + "\\" + DALSupportPath + filename,false);
			interfaceFile.WriteLine("// DALContainer support file");
			interfaceFile.WriteLine("//");
			interfaceFile.WriteLine("// DO NOT MODIFY THIS FILE DIRECTLY.  IT WILL BE OVERWRITTEN.");
			interfaceFile.WriteLine("// Generated by SQLServerGenerator.cs");
			interfaceFile.WriteLine("// Copyright (c) 2010, Chris Laforet Software.  All rights reserved");
			interfaceFile.WriteLine("//");
			interfaceFile.WriteLine("// Generated at " + now.ToString("MM/dd/yyyy hh:mm"));
			interfaceFile.WriteLine();
			interfaceFile.WriteLine();
			interfaceFile.WriteLine("using System;");
			interfaceFile.WriteLine("using System.Data;");
			interfaceFile.WriteLine("using System.Data.SqlClient;");
			interfaceFile.WriteLine("using System.Data.SqlTypes;");
			interfaceFile.WriteLine("using System.Data.Sql;");
			interfaceFile.WriteLine("using System.Collections.Generic;");
			interfaceFile.WriteLine("using SupportLibrary.Database;");
			interfaceFile.WriteLine();
			interfaceFile.WriteLine();
			interfaceFile.WriteLine("namespace SupportLibrary.Database");
			interfaceFile.WriteLine("\t{");

			interfaceFile.WriteLine("\tpublic partial class DALContainer");
			interfaceFile.WriteLine("\t\t{");
			interfaceFile.WriteLine("\t\tprivate static DALContainer TheContainer = null;");
			interfaceFile.WriteLine("\t\tprivate Dictionary<string,object> dals = new Dictionary<string,object>();");
			interfaceFile.WriteLine();

			interfaceFile.WriteLine("\t\tprivate void OpenDALs()");
			interfaceFile.WriteLine("\t\t\t{");
			interfaceFile.WriteLine("\t\t\tstring connectionString = DatabaseSupport.GetConnectionString();");
			interfaceFile.WriteLine();
			Boolean firstTime = true;
			foreach (String table in tables)
				{
				if (string.Compare(table,"ITEM",true) == 0 ||
					string.Compare(table,"SECTION",true) == 0)
					continue;		// skip the menu item and section tables -- supported elsewhere

				IList<Column> columns = DecodeTable(Conn,table);
				bool isIdentity = false;

				foreach (Column column in columns)
					{
					if (column.isIdentity)
						{
						isIdentity = true;
						break;
						}
					}
				if (!isIdentity)
					continue;

				if (firstTime)
					{
					interfaceFile.Write("\t\t\tobject ");
					firstTime = false;
					}
				else
					{
					interfaceFile.WriteLine();
					interfaceFile.Write("\t\t\t");
					}

				interfaceFile.WriteLine("dal = new " + table + "DAL(connectionString);");
				interfaceFile.WriteLine("\t\t\tdals.Add(\"" + table + "\",dal);");
				}
			interfaceFile.WriteLine("\t\t\t}");
			interfaceFile.WriteLine();

			interfaceFile.WriteLine("\t\tprivate DALContainer()");
			interfaceFile.WriteLine("\t\t\t{");
			interfaceFile.WriteLine("\t\t\tOpenDALs();");
			interfaceFile.WriteLine("\t\t\tOpenDefinedDALs();");
			interfaceFile.WriteLine("\t\t\t}");
			interfaceFile.WriteLine();

			interfaceFile.WriteLine("\t\t///<summary>");
			interfaceFile.WriteLine("\t\t///Provides a singleton DALContainer object.  Call this to retrieve the object or to create it on first invocation.");
			interfaceFile.WriteLine("\t\t///</summary>");
			interfaceFile.WriteLine("\t\tstatic public DALContainer GetDALContainer()");
			interfaceFile.WriteLine("\t\t\t{");
			interfaceFile.WriteLine("\t\t\tif (DALContainer.TheContainer == null)");
			interfaceFile.WriteLine("\t\t\t\tDALContainer.TheContainer = new DALContainer();");
			interfaceFile.WriteLine("\t\t\treturn DALContainer.TheContainer;");
			interfaceFile.WriteLine("\t\t\t}");
			interfaceFile.WriteLine();

			interfaceFile.WriteLine("\t\tpublic object GetDAL(string table)");
			interfaceFile.WriteLine("\t\t\t{");
			interfaceFile.WriteLine("\t\t\tif (dals.ContainsKey(table))");
			interfaceFile.WriteLine("\t\t\t\treturn dals[table];");
			interfaceFile.WriteLine("\t\t\treturn null;");
			interfaceFile.WriteLine("\t\t\t}");
			interfaceFile.WriteLine();

			interfaceFile.WriteLine("\t\tpublic void RegisterDAL(string TableName,object DALHandler)");
			interfaceFile.WriteLine("\t\t\t{");
			interfaceFile.WriteLine("\t\t\tif (dals.ContainsKey(TableName))");
			interfaceFile.WriteLine("\t\t\t\tdals.Remove(TableName);");
			interfaceFile.WriteLine("\t\t\tdals[TableName] = DALHandler;");
			interfaceFile.WriteLine("\t\t\t}");
			interfaceFile.WriteLine();

			foreach (String table in tables)
				{
				if (string.Compare(table,"ITEM",true) == 0 ||
					string.Compare(table,"SECTION",true) == 0)
					continue;		// skip the menu item and section tables -- supported elsewhere

				IList<Column> columns = DecodeTable(Conn,table);
				bool isIdentity = false;

				foreach (Column column in columns)
					{
					if (column.isIdentity)
						{
						isIdentity = true;
						break;
						}
					}
				if (isIdentity)
					{
					interfaceFile.WriteLine("\t\tpublic " + table + "DAL Get" + table + "DAL()");
					interfaceFile.WriteLine("\t\t\t{");
					interfaceFile.WriteLine("\t\t\treturn GetDAL(\"" + table + "\") as " + table + "DAL;");
					interfaceFile.WriteLine("\t\t\t}");
					interfaceFile.WriteLine();
					}
				}

			interfaceFile.WriteLine("\t\t}");
			interfaceFile.WriteLine("\t}");
			interfaceFile.Close();
			}


		static void Main(string[] args)
			{
			Console.WriteLine("SQLServerGenerator: Generates Database Access Layer Classes from Database");
			Console.WriteLine("Copyright (c) 2006-10, Chris Laforet Software");
			Console.WriteLine();

			Console.Write("Which server/IP is the SQL server on? ");
			String server = Console.ReadLine().Trim();
			if (server.Length == 0)
				{
				Console.WriteLine("No server specified...");
				return;
				}

			Console.Write("Which database to fetch? ");
			String dbName = Console.ReadLine().Trim();
			if (dbName.Length == 0)
				{
				Console.WriteLine("No database name specified...");
				return;
				}

			Console.Write("What login to use for database? ");
			String login = Console.ReadLine().Trim();

			String password = "";
			if (login.Length > 0)
				{
				Console.Write("What password to use for database? ");
				password = Console.ReadLine().Trim();
				}

			try
				{
				Console.WriteLine("Attempting to connect to " + server + ":" + dbName);
				//SqlDatabase db = new SqlDatabase(server,dbName,login,password);
				SqlConnection conn = new SqlConnection("Server=" + server + ";Database=" + dbName + ";User ID=" + login + ";Password=" + password + ";");
				conn.Open();

				Console.Write("Where is the base project directory to place the code? ");
				String path = Console.ReadLine().Trim();
				if (path.Length > 0)
					{
					Generate(conn,path);
					conn.Close();
					}
				else
					{
					Console.WriteLine("No path provided...");
					conn.Close();
					return;
					}

				Console.WriteLine("Success...");
				}
			catch (Exception ee)
				{
				Console.WriteLine("Error: " + ee);
				}
			}
		}
	}

