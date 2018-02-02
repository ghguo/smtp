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
using DnDns.Query;
using DnDns.Records;
using DnDns.Enums;
using System.Net.Sockets;

namespace WinSMTPServer.Network
	{
	/// <summary>
	/// Aggregates any specialty Nameserver lookups.
	/// </summary>
	public class DNSOperations
		{
		/// <summary>
		/// Queries the specified name server for any MX records
		/// that can handle the specified domain.  The domain can
		/// be a tertiary or quaternary domain and the nameserver
		/// will be checked for each level until MX records or found. 
		/// In other words, alpha.beta.gamma.com will be checked for
		/// MX for alpha.beta.gamma.com then if none are found, then
		/// beta.gamma.com will be checked, then if none are not found,
		/// gamma.com will be checked.
		/// </summary>
		/// <param name="NSName">The name or IP of the name server.</param>
		/// <param name="DomainToQuery">The domain for which to fetch MX records.</param>
		/// <returns>An array of zero or more MX records.</returns>
		/// <exception cref="Exception">If an error occurs while querying.</exception>
		public static MXRecord[] DeriveMailExchangers(string NSName,string DomainToQuery)
			{
			List<MXRecord> mxRecords = new List<MXRecord>();

			// divide up the domain name into its parts to allow us to step down for quaternary and tertiary domain names.
			string[] labels = DomainToQuery.Split(".".ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
			for (int labelStart = 0; mxRecords.Count == 0 && labelStart <= (labels.Length - 2); labelStart++)
				{
				StringBuilder domain = new StringBuilder(128);
				for (int count = labelStart; count < labels.Length; count++)
					{
					if (domain.Length > 0)
						domain.Append('.');
					domain.Append(labels[count]);
					}

				DnsQueryResponse response = null;
				DnsQueryRequest request = new DnsQueryRequest();
				try
					{
					response = request.Resolve(NSName,domain.ToString(),NsType.MX,NsClass.INET,ProtocolType.Udp, null);
					}
				catch (Exception ex)
					{
					throw ex;
					}

				if (response == null || response.Answers.Length == 0)
					continue;

				// parse out the MX data for each answer
				foreach (IDnsRecord dns in response.Answers)
					{
					string mx = null;
					int preference = 1;

					//Parse the answer:
					//MX Preference: 10, Mail Exchanger: mail.laforet.name.
					string[] sections = dns.Answer.Split(",".ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
					foreach (string section in sections)
						{
						string[] parts = section.Split(":".ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
						if (parts.Length != 2)
							continue;

						switch (parts[0].Trim().ToUpper())
							{
							case "MAIL EXCHANGER":
								mx = parts[1].Trim();
								if (mx.EndsWith("."))		// remove trailing dot
									mx = mx.Substring(0,mx.Length - 1);
								break;

							case "MX PREFERENCE":
								try
									{
									preference = Convert.ToInt32(parts[1].Trim());
									}
								catch (Exception)
									{
									}
								break;
							}
						}

					if (mx != null)
						mxRecords.Add(new MXRecord(domain.ToString(),mx,preference));
					}
				}

			// finally sort in descending preference....
			mxRecords.Sort((a,b) => a.Preference - b.Preference);
			return mxRecords.ToArray();
			}
		}
	}
