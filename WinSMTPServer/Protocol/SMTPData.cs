using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinSMTPServer.Protocol
	{
	/// <summary>
	/// Container for data accumulated from an SMTP
	/// MAIL session.
	/// </summary>
	public class SMTPData
		{
		private bool _isExtended;
		private string _helloName;
		private string _sender;
		private IList<string> _recipients = new List<string>();
		private string _headers = string.Empty;
		private string _body = string.Empty;

		/// <summary>
		/// True if the client started with EHLO and can handle
		/// Extended SMTP.
		/// </summary>
		public bool IsExtendedHello
			{
			set
				{
				_isExtended = value;
				}
			get
				{
				return _isExtended;
				}
			}


		/// <summary>
		/// The name provided with the HELO/EHLO statement.
		/// </summary>
		public string HelloName
			{
			set
				{
				_helloName = value != null ? value.Trim() : value;
				}
			get
				{
				return _helloName == null ? string.Empty : _helloName;
				}
			}


		/// <summary>
		/// The sender's address.
		/// </summary>
		public string Sender
			{
			set
				{
				_sender = value != null ? value.Trim() : value;
				}
			get
				{
				return _sender == null ? string.Empty : _sender;
				}
			}


		/// <summary>
		/// Adds a recipient address to the list of addresses.
		/// </summary>
		/// <param name="Address">The address to add</param>
		public void AddRecipient(string Address)
			{
			if (Address != null && Address.Trim().Length > 0)
				_recipients.Add(Address.Trim());
			}


		/// <summary>
		/// Retrieves the list of recipients.
		/// </summary>
		public IList<string> Recipients
			{
			get
				{
				return _recipients;
				}
			}


		/// <summary>
		/// Retrieves the headers portion of the data.
		/// </summary>
		public string Headers
			{
			set
				{
				_headers = value;
				}
			get
				{
				return _headers;
				}
			}


		/// <summary>
		/// Returns the body portion of the data.
		/// </summary>
		public string Body
			{
			set
				{
				_body = value;
				}
			get
				{
				return _body;
				}
			}

		/// <summary>
		/// Retrieves the full data portion of the session (composite of headers and body).
		/// </summary>
		public string Data
			{
			get
				{
				if (_headers.Length > 0)
					{
					StringBuilder sb = new StringBuilder(_headers.Length + _body.Length + 2);
					sb.Append(_headers);
					sb.Append("\r\n");
					sb.Append(_body);
					return sb.ToString();
					}
				return _body;
				}
			}
		}
	}
