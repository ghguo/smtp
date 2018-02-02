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
	/// Provides a common interface for Data Access Layer objects.
	/// </summary>
	public interface IDAL<T>
		{
		/// <summary>
		/// Reads the record specified by the ID number
		/// </summary>
		/// <param name="RecordID">The ID of the record to retrieve</param>
		/// <returns>An object if located or null if not found</returns>
		T ReadRecord(int RecordID);

		/// <summary>
		/// Retrieves a list of all records in the table.
		/// </summary>
		/// <returns></returns>
		IList<T> ReadAllRecords();

		/// <summary>
		/// Attempts to create a new record in the table.
		/// </summary>
		/// <param name="Record">The record to append to the table.  Upon return the ID will be set to the ID of the physical record.</param>
		void CreateRecord(T Record);

		/// <summary>
		/// Attempts to update the record in the table.  The ID in the record is used as the key to the
		/// physical record to change.
		/// </summary>
		/// <param name="Record"></param>
		void UpdateRecord(T Record);

		/// <summary>
		/// Attempts to delete the record specified by the ID number.
		/// <summary>
		/// <param name="RecordID">The ID of the record to delete</param>
		void DeleteRecord(int RecordID);

		/// <summary>
		/// Attempts to delete the record passed to it.
		/// <summary>
		/// <param name="Record">The record to delete (must have a valid ID number in the record)</param>
		void DeleteRecord(T Record);
		}
	}
