// ConfigurationDataSource Web Interface support file
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
	public partial class ConfigurationDataSource : ObjectDataSource
		{
		public static IList<ConfigurationRecord> ReadAllRecords()
			{
			DALContainer dc = DALContainer.GetDALContainer();
			ConfigurationDAL dal = dc.GetConfigurationDAL();
			return dal.ReadAllRecords();
			}

		public static ConfigurationRecord ReadRecord(int RecordID)
			{
			DALContainer dc = DALContainer.GetDALContainer();
			ConfigurationDAL dal = dc.GetConfigurationDAL();
			return dal.ReadRecord(RecordID);
			}

		public static void CreateRecord(ConfigurationRecord Record)
			{
			DALContainer dc = DALContainer.GetDALContainer();
			ConfigurationDAL dal = dc.GetConfigurationDAL();
			dal.CreateRecord(Record);
			}

		public static void UpdateRecord(ConfigurationRecord Record)
			{
			DALContainer dc = DALContainer.GetDALContainer();
			ConfigurationDAL dal = dc.GetConfigurationDAL();
			dal.UpdateRecord(Record);
			}

		public static void DeleteRecord(int RecordID)
			{
			DALContainer dc = DALContainer.GetDALContainer();
			ConfigurationDAL dal = dc.GetConfigurationDAL();
			dal.DeleteRecord(RecordID);
			}
		}
	}
