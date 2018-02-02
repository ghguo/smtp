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

namespace SupportLibrary.Util
	{
	/// <summary>
	/// Creates the concept of a byte string builder similar to StringBuilder.
	/// </summary>
	public class ByteBuilder
		{
		private byte[] _buffer;
		private int _count;


		/// <summary>
		/// Creates a byte string with the initial buffering set to the InitialLength.
		/// This object uses a binary doubling algorithm to handle growth.
		/// </summary>
		/// <param name="InitialLength">The preferred starting buffer size.</param>
		public ByteBuilder(int InitialLength)
			{
			if (InitialLength < 16)
				InitialLength = 16;

			_buffer = new byte[InitialLength];
			}



		/// <summary>
		/// Creates an empty byte string with a default buffer size.
		/// </summary>
		public ByteBuilder()
			: this(0)
			{
			}



		/// <summary>
		/// Copy constructor -- creates a deep copy of the byte string object.
		/// </summary>
		/// <param name="Other"></param>
		public ByteBuilder(ByteBuilder Other)
			: this(Other.Length)
			{
			for (int count = 0; count < Other.Length; count++)
				_buffer[count] = Other._buffer[count];
			}


		/// <summary>
		/// Retrieves the length of this byte string.
		/// </summary>
		public int Length
			{
			get
				{
				return _count;
				}
			}


		/// <summary>
		/// Retrieves the byte at the specified offset in the byte string.
		/// </summary>
		/// <param name="Index">The 0-based offset to retrieve.</param>
		/// <returns>A byte value.</returns>
		public byte this[int Index]
			{
			get
				{
				if (Index >= _count)
					throw new IndexOutOfRangeException();
				return _buffer[Index];
				}
			}


		/// <summary>
		/// Adds the byte to the current string.
		/// </summary>
		/// <param name="b">The byte to append to this buffer.</param>
		public void Append(byte b)
			{
			if (_count >= _buffer.Length)
				{
				byte[] newBuffer = new byte[_buffer.Length * 2];

				for (int count = 0; count < _count; count++)
					newBuffer[count] = _buffer[count];
				_buffer = newBuffer;
				}
			_buffer[_count++] = b;
			}


		/// <summary>
		/// Adds the array of bytes to the buffer.
		/// </summary>
		/// <param name="Buffer">An array of bytes to append to this buffer.</param>
		public void Append(byte[] Buffer)
			{
			for (int count = 0; count < Buffer.Length; count++)
				Append(Buffer[count]);
			}



		/// <summary>
		/// Adds the contents of another byte string to this buffer.
		/// </summary>
		/// <param name="Other">The byte string to append to this one.</param>
		public void Append(ByteBuilder Other)
			{
			for (int count = 0; count < Other._count; count++)
				Append(Other._buffer[count]);
			}


		/// <summary>
		/// Retrieves a byte array representation of the contents of this buffer.
		/// </summary>
		/// <returns>A byte array of the contents of this buffer.</returns>
		public byte[] ToByteArray()
			{
			byte[] array = new byte[_count];
			for (int count = 0; count < array.Length; count++)
				array[count] = _buffer[count];
			return array;
			}


		/// <summary>
		/// Converts into a string assuming ASCII.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
			{
			char [] chars = System.Text.Encoding.ASCII.GetChars(_buffer,0,_count);
			return new string(chars);
			}
		}	
	}
