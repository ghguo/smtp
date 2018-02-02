// Allowed record support file
//
// DO NOT MODIFY THIS FILE DIRECTLY.  IT WILL BE OVERWRITTEN.
// Generated by SQLServerGenerator.cs
// Copyright (c) 2010, Chris Laforet Software.  All rights reserved
//
// Generated at 04/24/2010 03:03


using System;
using SupportLibrary.Database;


namespace SupportLibrary.Database
	{
	public partial class AllowedRecord: IRecord
		{
		private int _ID = DatabaseSupport.InvalidID;
		private string _IP;			// Allowed.IP
		private string _Subnet;			// Allowed.Subnet

		public AllowedRecord()
			{
			}

		public AllowedRecord(AllowedRecord Other)
			{
			this.ID = Other.ID;
			this.IP = Other.IP;
			this.Subnet = Other.Subnet;
			}

		public int ID
			{
			set
				{
				this._ID = value;
				}
			get
				{
				return _ID;
				}
			}

		public string IP
			{
			set
				{
				this._IP = value;
				}
			get
				{
				return (_IP == null ? "" : _IP);
				}
			}

		public string Subnet
			{
			set
				{
				this._Subnet = value;
				}
			get
				{
				return (_Subnet == null ? "" : _Subnet);
				}
			}
		}
	}
