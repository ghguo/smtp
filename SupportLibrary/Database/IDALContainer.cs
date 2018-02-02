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
using System.Linq;
using System.Text;

namespace SupportLibrary.Database
	{
	/// <summary>
	/// Defines basic operations for a DALContainer used to
	/// centralize all data access layer components.
	/// </summary>
	public interface IDALContainer
		{
		/// <summary>
		/// Attempts to retrieve the DAL for the specific
		/// table name.
		/// </summary>
		/// <param name="TableName">The table name for this DAL.</param>
		/// <returns>A DAL if found or null otherwise</returns>
		object GetDAL(string TableName);

		/// <summary>
		/// Registers a DAL handler to the specific table name.
		/// </summary>
		/// <param name="TableName">The table name for this DAL.</param>
		/// <param name="DALHandler">The handler for this table.</param>
		void RegisterDAL(string TableName, object DALHandler);
		}
	}
