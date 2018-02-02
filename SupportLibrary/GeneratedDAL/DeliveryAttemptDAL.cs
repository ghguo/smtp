// DeliveryAttempt Data Access Layer
//
// DO NOT MODIFY THIS FILE DIRECTLY.  IT WILL BE OVERWRITTEN.
// Generated by SQLServerGenerator.cs
// Copyright (c) 2010, Chris Laforet Software.  All rights reserved
//
// Generated at 04/24/2010 03:03


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
	public partial class DeliveryAttemptDAL: IDAL<DeliveryAttemptRecord>
		{
		private readonly string _connectionString;
		public readonly string TableName = "DeliveryAttempt";

		public DeliveryAttemptDAL(string ConnectionString)
			{
			_connectionString = ConnectionString;
			}

		static public DeliveryAttemptRecord ReapRecord(SqlDataReader Reader)
			{
			int ID = Reader.GetInt32(Reader.GetOrdinal("ID"));
			int HeaderID = Reader.GetInt32(Reader.GetOrdinal("HeaderID"));
			DateTime AttemptDateTime = Reader.GetDateTime(Reader.GetOrdinal("AttemptDateTime"));
			int? ConnectionID = null;
			if (!Reader.IsDBNull(Reader.GetOrdinal("ConnectionID")))
				ConnectionID = Reader.GetInt32(Reader.GetOrdinal("ConnectionID"));
			string MXAddress = Reader.GetString(Reader.GetOrdinal("MXAddress"));
			bool IsSuccess = Reader.GetBoolean(Reader.GetOrdinal("IsSuccess"));
			string FailureReason = null;
			if (!Reader.IsDBNull(Reader.GetOrdinal("FailureReason")))
				FailureReason = Reader.GetString(Reader.GetOrdinal("FailureReason"));
			DeliveryAttemptRecord record = new DeliveryAttemptRecord();

			record.ID = ID;
			record.HeaderID = HeaderID;
			record.AttemptDateTime = AttemptDateTime;
			record.ConnectionID = ConnectionID;
			record.MXAddress = MXAddress;
			record.IsSuccess = IsSuccess;
			record.FailureReason = FailureReason;
			return record;
			}

		public virtual IList<DeliveryAttemptRecord> ReadAllRecords()
			{
			IList<DeliveryAttemptRecord> list = new List<DeliveryAttemptRecord>(64);
			SqlConnection conn = new SqlConnection(_connectionString);
			conn.Open();
			if (conn != null)
				{
				try
					{
					SqlCommand command = new SqlCommand("SELECT * FROM [DeliveryAttempt]",conn);
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

		public virtual DeliveryAttemptRecord ReadRecord(int RecordID)
			{
			DeliveryAttemptRecord record = null;
			SqlConnection conn = new SqlConnection(_connectionString);
			conn.Open();
			if (conn != null)
				{
				try
					{
					SqlCommand command = new SqlCommand("SELECT * FROM [DeliveryAttempt] WHERE ID=@ID",conn);
					SqlParameter param = new SqlParameter("@ID",SqlDbType.Int,0);
					param.Value = RecordID;
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

		public virtual IList<DeliveryAttemptRecord> ReadAllRecordsByHeaderID(int HeaderID)
			{
			IList<DeliveryAttemptRecord> list = new List<DeliveryAttemptRecord>(64);
			SqlConnection conn = new SqlConnection(_connectionString);
			conn.Open();
			if (conn != null)
				{
				try
					{
					SqlCommand command = new SqlCommand("SELECT * FROM [DeliveryAttempt] WHERE HeaderID=@HeaderID",conn);
					SqlParameter param = new SqlParameter("@HeaderID",SqlDbType.Int,0);
					param.Value = HeaderID;
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

		public virtual IList<DeliveryAttemptRecord> ReadAllRecordsByConnectionID(int ConnectionID)
			{
			IList<DeliveryAttemptRecord> list = new List<DeliveryAttemptRecord>(64);
			SqlConnection conn = new SqlConnection(_connectionString);
			conn.Open();
			if (conn != null)
				{
				try
					{
					SqlCommand command = new SqlCommand("SELECT * FROM [DeliveryAttempt] WHERE ConnectionID=@ConnectionID",conn);
					SqlParameter param = new SqlParameter("@ConnectionID",SqlDbType.Int,0);
					param.Value = ConnectionID;
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

		public virtual void CreateRecord(DeliveryAttemptRecord record)
			{
			SqlConnection conn = new SqlConnection(_connectionString);
			conn.Open();
			if (conn != null)
				{
				try
					{
					string sql = "INSERT INTO [DeliveryAttempt] " + 
					"(HeaderID,AttemptDateTime,ConnectionID,MXAddress,IsSuccess,FailureReason	) " +
					"VALUES (@HeaderID,@AttemptDateTime,@ConnectionID,@MXAddress,@IsSuccess,@FailureReason	)";
					SqlCommand command = new SqlCommand(sql,conn);
					SqlParameter param = new SqlParameter("@HeaderID",SqlDbType.Int,0);
					if (record.HeaderID == DatabaseSupport.InvalidID)
						param.Value = DBNull.Value;
					else
						param.Value = record.HeaderID;
					command.Parameters.Add(param);
					param = new SqlParameter("@AttemptDateTime",SqlDbType.DateTime,0);
					param.Value = record.AttemptDateTime;
					command.Parameters.Add(param);
					param = new SqlParameter("@ConnectionID",SqlDbType.Int,0);
					if (record.ConnectionID == null)
						param.Value = DBNull.Value;
					else
						param.Value = record.ConnectionID;
					command.Parameters.Add(param);
					param = new SqlParameter("@MXAddress",SqlDbType.VarChar,0);
					param.Value = record.MXAddress;
					command.Parameters.Add(param);
					param = new SqlParameter("@IsSuccess",SqlDbType.Bit,0);
					param.Value = record.IsSuccess;
					command.Parameters.Add(param);
					param = new SqlParameter("@FailureReason",SqlDbType.VarChar,0);
					if (record.FailureReason == null)
						param.Value = DBNull.Value;
					else
						param.Value = record.FailureReason;
					command.Parameters.Add(param);
					command.ExecuteNonQuery();
					record.ID = DatabaseSupport.GetIdentity(conn);
					}
				finally
					{
					if (conn != null)
						conn.Close();
					}
				}
			}

		public virtual int CreateRecord(int HeaderID,DateTime AttemptDateTime,int? ConnectionID,string MXAddress,bool IsSuccess,string FailureReason)
			{
			// populate the record with items
			DeliveryAttemptRecord record = new DeliveryAttemptRecord();
			record.HeaderID = HeaderID;
			record.AttemptDateTime = AttemptDateTime;
			record.ConnectionID = ConnectionID;
			record.MXAddress = MXAddress;
			record.IsSuccess = IsSuccess;
			record.FailureReason = FailureReason;

			// write the record out
			CreateRecord(record);
			return record.ID;
			}

		public virtual void UpdateRecord(DeliveryAttemptRecord record)
			{
			SqlConnection conn = new SqlConnection(_connectionString);
			conn.Open();
			if (conn != null)
				{
				try
					{
					string sql = "UPDATE [DeliveryAttempt] " +
					"SET HeaderID=@HeaderID,AttemptDateTime=@AttemptDateTime,ConnectionID=@ConnectionID,MXAddress=@MXAddress,IsSuccess=@IsSuccess,FailureReason=@FailureReason WHERE ID=@ID";
					SqlCommand command = new SqlCommand(sql,conn);
					SqlParameter param = new SqlParameter("@HeaderID",SqlDbType.Int,0);
					if (record.HeaderID == DatabaseSupport.InvalidID)
						param.Value = DBNull.Value;
					else
						param.Value = record.HeaderID;
					command.Parameters.Add(param);
					param = new SqlParameter("@AttemptDateTime",SqlDbType.DateTime,0);
					param.Value = record.AttemptDateTime;
					command.Parameters.Add(param);
					param = new SqlParameter("@ConnectionID",SqlDbType.Int,0);
					if (record.ConnectionID == null)
						param.Value = DBNull.Value;
					else
						param.Value = record.ConnectionID;
					command.Parameters.Add(param);
					param = new SqlParameter("@MXAddress",SqlDbType.VarChar,0);
					param.Value = record.MXAddress;
					command.Parameters.Add(param);
					param = new SqlParameter("@IsSuccess",SqlDbType.Bit,0);
					param.Value = record.IsSuccess;
					command.Parameters.Add(param);
					param = new SqlParameter("@FailureReason",SqlDbType.VarChar,0);
					if (record.FailureReason == null)
						param.Value = DBNull.Value;
					else
						param.Value = record.FailureReason;
					command.Parameters.Add(param);
					param = new SqlParameter("@ID",SqlDbType.Int,0);
					param.Value = record.ID;
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

		public virtual void UpdateRecord(int ID,int HeaderID,DateTime AttemptDateTime,int? ConnectionID,string MXAddress,bool IsSuccess,string FailureReason)
			{
			// populate the record with items
			DeliveryAttemptRecord record = new DeliveryAttemptRecord();
			record.ID = ID;
			record.HeaderID = HeaderID;
			record.AttemptDateTime = AttemptDateTime;
			record.ConnectionID = ConnectionID;
			record.MXAddress = MXAddress;
			record.IsSuccess = IsSuccess;
			record.FailureReason = FailureReason;

			// write the record out
			UpdateRecord(record);
			}

		public virtual void DeleteRecord(int RecordID)
			{
			SqlConnection conn = new SqlConnection(_connectionString);
			conn.Open();
			if (conn != null)
				{
				try
					{
					string sql = "DELETE FROM [DeliveryAttempt] WHERE ID=@ID";
					SqlCommand command = new SqlCommand(sql,conn);
					SqlParameter param = new SqlParameter("@ID",SqlDbType.Int,0);
					param.Value = RecordID;
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

		public virtual void DeleteRecord(DeliveryAttemptRecord record)
			{
			DeleteRecord(record.ID);
			}
		}
	}
