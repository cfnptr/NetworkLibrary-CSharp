
// Copyright 2019 Nikita Fediuchin (QuantumBranch)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;

namespace QuantumBranch.OpenNetworkLibrary
{
    /// <summary>
    /// Token container class
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Token value size in bytes
        /// </summary>
        public const int ByteSize = 16;

        /// <summary>
        /// Token GUIS value
        /// </summary>
        protected readonly Guid value;

        /// <summary>
        /// Creates a new token container class instance
        /// </summary>
        public Token(Guid value)
        {
            this.value = value;
        }
        /// <summary>
        /// Creates a new token container class instance
        /// </summary>
        public Token(byte[] array, int index)
        {
            var bytes = new byte[ByteSize];
            Buffer.BlockCopy(array, index, bytes, 0, ByteSize);
            value = new Guid(bytes);
        }

        /// <summary>
        /// Returns true if the username is equal to the object
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            return value == (Token)obj;
        }
        /// <summary>
        /// Returns username hash code 
        /// </summary>
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
        /// <summary>
        /// Returns username string value
        /// </summary>
        public override string ToString()
        {
            return value.ToString();
        }

        /// <summary>
        /// Converts token value to the byte array
        /// </summary>
        public void ToBytes(byte[] array, int index)
        {
            var bytes = value.ToByteArray();
            Buffer.BlockCopy(bytes, 0, array, index, ByteSize);
        }
        /// <summary>
        /// Converts token value to the byte array
        /// </summary>
        public byte[] ToBytes()
        {
            var array = new byte[ByteSize];
            ToBytes(array, 0);
            return array;
        }

        public static bool operator ==(Token a, Token b) { return a.value == b.value; }
        public static bool operator !=(Token a, Token b) { return a.value != b.value; }
        public static bool operator ==(Token a, Guid b) { return a.value == b; }
        public static bool operator !=(Token a, Guid b) { return a.value != b; }
        public static bool operator ==(Guid a, Token b) { return a == b.value; }
        public static bool operator !=(Guid a, Token b) { return a != b.value; }

        public static implicit operator Guid(Token v) { return v.value; }
        public static implicit operator Token(Guid v) { return new Token(v); }
    }
}
