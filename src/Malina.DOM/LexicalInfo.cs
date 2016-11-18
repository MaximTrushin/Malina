﻿#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;

namespace Malina.DOM
{
    public class LexicalInfo : SourceLocation, IEquatable<LexicalInfo>, IComparable<LexicalInfo>
    {
        public static readonly LexicalInfo Empty = new LexicalInfo(null, -1, -1, -1);

        private readonly string _filename;

        private string _fullPath;

        public LexicalInfo(string filename, int line, int column, int index)
            : base(line, column, index)
        {
            _filename = filename;
        }

        public LexicalInfo(string filename) : this(filename, -1, -1, -1)
        {
        }

        public LexicalInfo(LexicalInfo other) : this(other.FileName, other.Line, other.Column, other.Index)
        {
        }

        override public bool IsValid
        {
            get { return null != _filename && base.IsValid; }
        }

        public string FileName
        {
            get { return _filename; }
        }

        public string FullPath
        {
            get
            {
                if (null != _fullPath) return _fullPath;
                _fullPath = SafeGetFullPath(_filename);
                return _fullPath;
            }
        }

        override public string ToString()
        {
            return _filename + base.ToString();
        }

        private static string SafeGetFullPath(string fname)
        {
            try
            {
                return System.IO.Path.GetFullPath(fname);
            }
            catch (Exception)
            {
            }
            return fname;
        }

        public int CompareTo(LexicalInfo other)
        {
            int result = string.Compare(_filename, other._filename);
            if (result != 0) return result;

            return base.CompareTo(other); 
        }

        public bool Equals(LexicalInfo other)
        {
            return CompareTo(other) == 0;
        }
    }
}
