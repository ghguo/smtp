// ConnectionDataSource Web Interface support file
//
// DO NOT MODIFY THIS FILE DIRECTLY.  IT WILL BE OVERWRITTEN.
// Generated by SQLServerGenerator.cs
// Copyright (c) 2010, Chris Laforet Software.  All rights reserved
//
// Generated at 04/24/2010 03:03


using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using SupportLibrary.Database;

namespace SupportLibrary.DataSource
	{
	public partial class ConnectionDataSource : ObjectDataSource
		{
		public static IList<ConnectionRecord> ReadAllRecords()
			{
			DALContainer dc = DALContainer.GetDALContainer();
			ConnectionDAL dal = dc.GetConnectionDAL();
			return dal.ReadAllRecords();
			}

		public static ConnectionRecord ReadRecord(int RecordID)
			{
			DALContainer dc = DALContainer.GetDALContainer();
			ConnectionDAL dal = dc.GetConnectionDAL();
			return dal.ReadRecord(RecordID);
			}

		public static void CreateRecord(ConnectionRecord Record)
			{
			DALContainer dc = DALContainer.GetDALContainer();
			ConnectionDAL dal = dc.GetConnectionDAL();
			dal.CreateRecord(Record);
			}

		public static void UpdateRecord(ConnectionRecord Record)
			{
			DALContainer dc = DALContainer.GetDALContainer();
			ConnectionDAL dal = dc.GetConnectionDAL();
			dal.UpdateRecord(Record);
			}

		public static void DeleteRecord(int RecordID)
			{
			DALContainer dc = DALContainer.GetDALContainer();
			ConnectionDAL dal = dc.GetConnectionDAL();
			dal.DeleteRecord(RecordID);
			}
		}
	}
