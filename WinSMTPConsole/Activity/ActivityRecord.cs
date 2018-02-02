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
using SupportLibrary.Database;

namespace WinSMTPConsole.Activity
	{
	/// <summary>
	/// Container class for a search for activity information.
	/// </summary>
	public class ActivityRecord
		{
		public ConnectionRecord Connection;
		public HeaderRecord Header;
		public DeliveryAttemptRecord DeliveryAttempt;

		public int ConnectionID
			{
			get
				{
				return Connection.ID;
				}
			}

		public string Remote
			{
			get
				{
				return Connection.Remote;
				}
			}

		public DateTime StartTimeUTC
			{
			get
				{
				return Connection.StartTime;
				}
			}

		public string StartTimeString
			{
			get
				{
				return StartTimeUTC.ToString("MM/dd/yy HH:mm:ss") + " UTC";
				}
			}

		public DateTime? EndTimeUTC
			{
			get
				{
				return Connection.EndTime;
				}
			}

		public string EndTimeString
			{
			get
				{
				DateTime? dt = EndTimeUTC;
				if (dt == null)
					return string.Empty;
				return ((DateTime)dt).ToString("MM/dd/yy HH:mm:ss") + " UTC";
				}
			}

		public bool IsInbound
			{
			get
				{
				return Connection.IsInbound;
				}
			}

		public string DirectionString
			{
			get
				{
				if (IsInbound)
					return "In";
				return "Out";
				}
			}

		public string Sender
			{
			get
				{
				if (Header == null)
					return string.Empty;
				return Header.Sender;
				}
			}

		public string Recipient
			{
			get
				{
				if (Header == null)
					return string.Empty;
				return Header.Recipient;
				}
			}

		public DateTime? ReceivedDateTimeUTC
			{
			get
				{
				if (Header == null)
					return null;
				return Header.ReceivedDateTime;
				}
			}

		public string ReceivedDateString
			{
			get
				{
				DateTime? dt = ReceivedDateTimeUTC;
				if (dt == null)
					return string.Empty;
				return ((DateTime)dt).ToString("MM/dd/yy HH:mm:ss") + " UTC";
				}
			}

		public DateTime? DeliveredDateTimeUTC
			{
			get
				{
				if (Header == null)
					return null;
				return Header.DeliveredDateTime;
				}
			}

		public string DeliveredDateString
			{
			get
				{
				DateTime? dt = DeliveredDateTimeUTC;
				if (dt == null)
					return string.Empty;
				return ((DateTime)dt).ToString("MM/dd/yy HH:mm:ss") + " UTC";
				}
			}

		public DateTime? ExpiredDateTimeUTC
			{
			get
				{
				if (Header == null)
					return null;
				return Header.ExpiredDateTime;
				}
			}

		public string ExpiredDateString
			{
			get
				{
				DateTime? dt = ExpiredDateTimeUTC;
				if (dt == null)
					return string.Empty;
				return ((DateTime)dt).ToString("MM/dd/yy HH:mm:ss") + " UTC";
				}
			}

		public string MXAddress
			{
			get
				{
				if (DeliveryAttempt == null)
					return string.Empty;
				return DeliveryAttempt.MXAddress;
				}
			}

		public bool? IsSuccess
			{
			get
				{
				if (DeliveryAttempt == null)
					return null;
				return DeliveryAttempt.IsSuccess;
				}
			}

		public string SuccessString
			{
			get
				{
				if (IsSuccess == null)
					return string.Empty;
				return ((bool)IsSuccess) ? "Success" : "Fail";
				}
			}

		public string FailureReason
			{
			get
				{
				if (DeliveryAttempt == null)
					return null;
				return DeliveryAttempt.FailureReason;
				}
			}
		}
	}
