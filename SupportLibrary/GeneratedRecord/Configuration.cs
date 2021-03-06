// Configuration record support file
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
	public partial class ConfigurationRecord: IRecord
		{
		private int _ID = DatabaseSupport.InvalidID;
		private bool _AllowLocalHost;			// Configuration.AllowLocalHost
		private bool _AllowRemote;			// Configuration.AllowRemote
		private short _Port;			// Configuration.Port
		private int _ExpireAfterMinutes;			// Configuration.ExpireAfterMinutes
		private int _RetryAfterMinutes;			// Configuration.RetryAfterMinutes
		private int _CleanupDays;			// Configuration.CleanupDays

		public ConfigurationRecord()
			{
			}

		public ConfigurationRecord(ConfigurationRecord Other)
			{
			this.ID = Other.ID;
			this.AllowLocalHost = Other.AllowLocalHost;
			this.AllowRemote = Other.AllowRemote;
			this.Port = Other.Port;
			this.ExpireAfterMinutes = Other.ExpireAfterMinutes;
			this.RetryAfterMinutes = Other.RetryAfterMinutes;
			this.CleanupDays = Other.CleanupDays;
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

		public bool AllowLocalHost
			{
			set
				{
				this._AllowLocalHost = value;
				}
			get
				{
				return _AllowLocalHost;
				}
			}

		public bool AllowRemote
			{
			set
				{
				this._AllowRemote = value;
				}
			get
				{
				return _AllowRemote;
				}
			}

		public short Port
			{
			set
				{
				this._Port = value;
				}
			get
				{
				return _Port;
				}
			}

		public int ExpireAfterMinutes
			{
			set
				{
				this._ExpireAfterMinutes = value;
				}
			get
				{
				return _ExpireAfterMinutes;
				}
			}

		public int RetryAfterMinutes
			{
			set
				{
				this._RetryAfterMinutes = value;
				}
			get
				{
				return _RetryAfterMinutes;
				}
			}

		public int CleanupDays
			{
			set
				{
				this._CleanupDays = value;
				}
			get
				{
				return _CleanupDays;
				}
			}
		}
	}
