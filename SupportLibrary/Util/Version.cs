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


namespace SupportLibrary.Util
	{
	/// <summary>
	/// Container for versioning information.  Versions
	/// are shared by all components.
	/// </summary>
	public class Version
		{
		public const int MajorVersion = 0;
		public const int MinorVersion = 90;
		public const int PatchVersion = 1;

		static public string VersionString
			{
			get
				{
				return MajorVersion.ToString("D2") + "." + MinorVersion.ToString("D2") + "." + PatchVersion.ToString("D2");
				}
			}
		}
	}
