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
using System.Net.Sockets;
using System.Net;
using System.IO;
using SupportLibrary.Util;
using WinSMTPServer.Network;
using System.Threading;

namespace WinSMTPServer.SMTPClient
	{
	/// <summary>
	/// An SMTP client that can take preassembled messages
	/// and transfer them.  The .NET SmtpClient does not work
	/// in this way.
	/// </summary>
	public class SMTPClient
		{
		private string _host = string.Empty;
		private int _port = 25;
		private int _timeoutMSec = 90 * 1000;

		private BufferedStream _stream = null;


		/// <summary>
		/// Creates a client with the specified server name as the host
		/// and the default port.
		/// </summary>
		/// <param name="MXServerName">The mail exchange host name or IP string.</param>
		public SMTPClient(string MXServerName)
			{
			_host = MXServerName;
			}


		/// <summary>
		/// Creates a client with the specified server name as the host
		/// and the specified port.
		/// </summary>
		/// <param name="MXServerName">The mail exchange host name or IP string.</param>
		/// <param name="Port">The port to connect to.</param>
		public SMTPClient(string MXServerName,int Port) : this(MXServerName)
			{
			_port = Port;
			}


		/// <summary>
		/// Creates a client with the default port but the host must be
		/// set before use.
		/// </summary>
		public SMTPClient() : this(string.Empty)
			{
			}


		/// <summary>
		/// Gets and sets the MX (mail exchanger) host address.
		/// </summary>
		public string Host
			{
			set
				{
				_host = value;
				}
			get
				{
				return _host;
				}
			}


		/// <summary>
		/// Gets and sets the port.
		/// </summary>
		public int Port
			{
			set
				{
				_port = value;
				}
			get
				{
				return _port;
				}
			}


		/// <summary>
		/// Gets and sets the timeout for the SMTP operations
		/// in milliseconds.  The default is 90 seconds.
		/// </summary>
		public int Timeout
			{
			set
				{
				_timeoutMSec = value;
				}
			get
				{
				return _timeoutMSec;
				}
			}


		/// <summary>
		/// Transmits a message to the MX.
		/// </summary>
		/// <param name="Sender">The sender address for the Email sender.</param>
		/// <param name="Recipient">The recipient address for the Email sender.</param>
		/// <param name="HeadersAndBody">The message data as an intact whole.</param>
		/// <returns>One of the SMTPClientResult codes which indicate the outcome of the operation.</returns>
		public SMTPClientResult Send(string Sender,string Recipient,string HeadersAndBody)
			{
			try
				{
				IPAddress[] addresses = Dns.GetHostAddresses(_host);
				if (addresses.Length == 0)
					throw new ArgumentException("Cannot resolve the address " + _host);

				int which = 0;
				if (addresses.Length > 1)		// randomly pick one of several addresses
					{
					Random random = new Random();
					which = random.Next(addresses.Length);
					}

				IPEndPoint endPoint = new IPEndPoint(addresses[which],_port);
				Socket socket = new Socket(endPoint.AddressFamily,SocketType.Stream,ProtocolType.Tcp);
				socket.NoDelay = true;		// turn of nagle
				socket.Connect(endPoint.Address,endPoint.Port);
				socket.ReceiveTimeout = _timeoutMSec;

				_stream = new BufferedStream(new NetworkStream(socket,true));

				string[] lines = HeadersAndBody.Split("\n".ToCharArray(),StringSplitOptions.None);

				return SendMessage(Sender,Recipient,lines);
				}
			catch (Exception ee)
				{
				SupportLibrary.Logger.SystemLogger.LogError("SMTPClient.Send()","Caught exception: " + ee);
				return SMTPClientResult.ExceptionCaught;
				}
			finally
				{
				if (_stream != null)
					{
					_stream.Close();
					_stream = null;
					}
				}
			}


		/// <summary>
		/// Writes a line of data to the other side and flushes it.
		/// </summary>
		/// <param name="Data">The data to write.</param>
		private void Write(string Data)
			{
			byte[] bytes = System.Text.Encoding.ASCII.GetBytes(Data);
			SendBytes(bytes);
			_stream.Flush();
			}


		/// <summary>
		/// Writes a line of data and appends a crlf to the other side and flushes it.
		/// </summary>
		/// <param name="Data">The data to write.</param>
		private void WriteLine(string Data)
			{
			byte[] bytes = System.Text.Encoding.ASCII.GetBytes(Data);
			SendBytes(bytes);
			SendBytes(new byte[] { 0xd,0xa });
			_stream.Flush();
			}


		/// <summary>
		/// Transmits the specified bytes of data to the other side.
		/// </summary>
		/// <param name="ToSend"></param>
		private void SendBytes(byte[] ToSend)
			{
			_stream.Write(ToSend,0,ToSend.Length);
			}


		/// <summary>
		/// Reads the next line of data from the other end and blocks
		/// until a line has been read.
		/// </summary>
		/// <returns>A string containing the contents of the string MINUS cr and lf.</returns>
		private String ReadLine()
			{
			ByteBuilder bb = new ByteBuilder(128);

			while (true)
				{
				int b = _stream.ReadByte();
				if (b < 0)
					break;

				if (b == 0xa)
					break;
				else if (b != 0xd)
					bb.Append((byte)b);
				}
			return bb.ToString();
			}


		/// <summary>
		/// Takes a response from the server and deciphers the code.
		/// </summary>
		/// <param name="ResponseLine">The line from the server.</param>
		/// <returns>A response code or null if cannot be parsed.</returns>
		private ResponseCode ParseResponse(string ResponseLine)
			{
			if (ResponseLine.Length >= 3)
				{
				string part = ResponseLine.Substring(0,3);
				return new ResponseCode(Convert.ToInt32(part));
				}
			return null;
			}

	
		/// <summary>
		/// Reads the response from the server.  A response consists of a code+text.
		/// The response can span multiple lines.  This is signalled in the fact
		/// that a 3-digit code is followed by a dash then text.  The final (or only) line
		/// of response is signalled by a 3-digit code followed by a space then text.
		/// </summary>
		/// <returns>The response code or null if nothing received.</returns>
		/// <exception cref="InvalidResponseException">If the response fails standard SMTP response tests.</exception>
		private ResponseCode ReadServerResponse()
			{
			ResponseCode responseCode = null;

			while (true)
				{
				string serverResponse = ReadLine();
				if (serverResponse.Length < 4)
					throw new InvalidResponseException();

				for (int count = 0; count < 3; count++)
					{
					if (!Char.IsDigit(serverResponse[count]))
						throw new InvalidResponseException();
					}

				if (serverResponse[3] != ' ' && serverResponse[3] != '-')
					throw new InvalidResponseException();

				bool isFinal = serverResponse[3] == ' ';

				ResponseCode code = ParseResponse(serverResponse);
				if (responseCode == null)
					responseCode = code;
				else if (responseCode.Code != code.Code)
					throw new InvalidResponseException();

				if (isFinal)
					break;
				}

			return responseCode;
			}


		/// <summary>
		/// Internal method, attempts to transmit the message to the server.
		/// </summary>
		/// <param name="Sender">The sender address for the Email sender.</param>
		/// <param name="Recipient">The recipient address for the Email sender.</param>
		/// <param name="HeadersAndBody">The message data as lines of text.</param>
		/// <returns>One of the SMTPClientResult enumerations which describe the outcome.</returns>
		private SMTPClientResult SendMessage(string Sender,string Recipient,string [] HeadersAndBody)
			{
			try
				{
				// wait for 220 signon
				ResponseCode response = ReadServerResponse();
				if (response.Code != 220)
					return SMTPClientResult.ErrorOnServer;

				WriteLine("EHLO " + Configuration.GetConfiguration().HostName);
				response = ReadServerResponse();
				if (!response.IsPositiveCompletionReply)
					return SMTPClientResult.ErrorOnServer;

				// send MAIL FROM and wait for confirmation
				WriteLine("MAIL FROM: <" + Sender + ">");
				response = ReadServerResponse();
				if (!response.IsPositiveCompletionReply)
					return SMTPClientResult.InvalidSender;

				// send RCPT TO and wait for confirmation
				WriteLine("RCPT TO: <" + Recipient + ">");
				response = ReadServerResponse();
				if (!response.IsPositiveCompletionReply)
					return SMTPClientResult.InvalidRecipient;

				// send DATA and wait for confirmation
				WriteLine("DATA");
				response = ReadServerResponse();
				if (!response.IsPositiveIntermediateReply)
					return SMTPClientResult.ErrorOnServer;

				// send message
				foreach (string line in HeadersAndBody)
					WriteLine(line);
				WriteLine(".");

				// wait for confirmation
				response = ReadServerResponse();
				if (!response.IsPositiveCompletionReply)
					return SMTPClientResult.ErrorOnServer;
				return SMTPClientResult.SuccessfulSend;
				}
catch (Exception ee)
	{
	throw ee;
	}
			finally
				{
				WriteLine("QUIT");
				Thread.Sleep(1000);
				}
			}

		}


	/// <summary>
	/// Container for an RFC-821 response.
	/// </summary>
	class ResponseCode
		{
		private int _code;

		public ResponseCode(int Code)
			{
			this.Code = Code;
			}


		public int Code
			{
			private set
				{
				_code = value;
				}
			get
				{
				return _code;
				}
			}

		/// <summary>
		/// 1xx codes
		/// </summary>
		public bool IsPositivePreliminaryReply
			{
			get
				{
				return _code >= 100 && _code < 200;
				}
			}

		/// <summary>
		/// 2xx codes
		/// </summary>
		public bool IsPositiveCompletionReply
			{
			get
				{
				return _code >= 200 && _code < 300;
				}
			}

		/// <summary>
		/// 3xx codes
		/// </summary>
		public bool IsPositiveIntermediateReply
			{
			get
				{
				return _code >= 300 && _code < 400;
				}
			}

		/// <summary>
		/// 4xx codes
		/// </summary>
		public bool IsTransientNegativeReply
			{
			get
				{
				return _code >= 400 && _code < 500;
				}
			}
		
		/// <summary>
		/// 5xx codes
		/// </summary>
		public bool IsPermanentNegativeReply
			{
			get
				{
				return _code >= 500 && _code < 600;
				}
			}

		public override bool Equals(object obj)
			{
			return (ResponseCode)obj == this;
			}
		}


	/// <summary>
	/// Signals an issue receiving a response from the SMTP server.
	/// </summary>
	class InvalidResponseException : Exception
		{
		public InvalidResponseException()
			: base()
			{
			}

		public InvalidResponseException(string Message)
			: base(Message)
			{
			}
		}


	/// <summary>
	/// Response codes from an SMTP client session
	/// </summary>
	public enum SMTPClientResult
		{
		SuccessfulSend = 0,
		FailedToConnect,
		ErrorOnServer,
		ExceptionCaught,
		InvalidSender,
		InvalidRecipient,
		Expired
		}
	}
