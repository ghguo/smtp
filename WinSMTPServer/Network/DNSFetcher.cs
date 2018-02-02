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

// The following code is based on a post by Merkaba81 on CodeProject
// in a thread.  The title is "How to Get DNS Server Addresses with 
// Managed Code" and is attached to the article "C# .NET DNS
// Query Component" by Rob Philpott.
//
// This answers a critical issue which DnDns did not, nor did
// Microsoft with their LAME DNS support.  Sometimes they just
// let Sun run all over them with Java, and this is one of those
// cases.  The question is HOW to derive the DNS addresses that
// we currently use.  Unfortunately, DnDns examples all use 
// defined NS addresses for the search.
//
// For Microsoft information on the use of this nic code:
// http://msdn.microsoft.com/en-us/library/system.net.networkinformation.ipv4interfaceproperties.aspx
//
// Adapted into this module by Christopher Laforet, with thanks
// to the brilliant minds that get around Microsoft's nonsense and
// pointed many to a workaround!
//

using System.Collections;
using System.Net;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using System.Net.Sockets;

namespace WinSMTPServer.Network
	{
	public class DNSFetcher
		{
		/// <summary>
		/// Attempts to retrieve a list of DNS servers that are
		/// attached to our network cards.
		/// </summary>
		/// <returns></returns>
		public static IPAddress[] GetDnsServers()
			{
			Dictionary<string,IPAddress> lookup = new Dictionary<string,IPAddress>();
//			List<IPAddress> DnsArray = new List<IPAddress>();
			NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

			foreach (NetworkInterface adapter in nics)
				{
				if (!adapter.Supports(NetworkInterfaceComponent.IPv4))
					continue;

				if (adapter.OperationalStatus != OperationalStatus.Up && adapter.Speed <= 0)
					continue;

				if (adapter.NetworkInterfaceType == NetworkInterfaceType.Tunnel ||
					adapter.NetworkInterfaceType == NetworkInterfaceType.Loopback)
					continue;

				foreach (IPAddress ns in adapter.GetIPProperties().DnsAddresses)
					{
					if (ns.GetAddressBytes().Length == 4 &&		// we only want IPv4 addresses
						!lookup.ContainsKey(ns.ToString()))
						lookup.Add(ns.ToString(),ns);
//						DnsArray.Add(DnsAddress);
					}
				}
			IPAddress[] nsList = new IPAddress[lookup.Values.Count];
			int count = 0;
			foreach (IPAddress ns in lookup.Values)
				nsList[count++] = ns;
			return nsList;
//			return DnsArray.ToArray();
			}
		}
	}
