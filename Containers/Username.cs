
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
using System.Text;

namespace QuantumBranch.OpenNetworkLibrary
{
    /// <summary>
    /// Alphanumeric lowercase username container (with "_")
    /// </summary>
    public class Username
    {
        /// <summary>
        /// Minimum username length
        /// </summary>
        public const int MinLength = 4;
        /// <summary>
        /// Maximum username length
        /// </summary>
        public const int MaxLength = 16;
        /// <summary>
        /// Username value size in bytes
        /// </summary>
        public const int ByteSize = 16;

        /// <summary>
        /// Username string value
        /// </summary>
        protected readonly string value;

        /// <summary>
        /// Creates a new username container class instance
        /// </summary>
        public Username(string value)
        {
            if (!IsValidLength(value.Length))
                throw new ArgumentException("Invalid username length", nameof(value));
            if (!IsValidLetters(value))
                throw new ArgumentException("Invalid username letters", nameof(value));

            this.value = value;
        }
        /// <summary>
        /// Creates a new username container class instance
        /// </summary>
        public Username(byte[] array, int index)
        {
            var usernameLength = 0;

            for (int i = index, length = index + ByteSize; i < length; i++)
            {
                if (array[i] == byte.MinValue)
                {
                    usernameLength = i - index;
                    break;
                }    
            }

            if (!IsValidLength(usernameLength))
                throw new ArgumentException("Invalid username length", nameof(array));

            var username = Encoding.ASCII.GetString(array, index, usernameLength);

            if (!IsValidLetters(username))
                throw new ArgumentException("Invalid username letters", nameof(array));

            value = username;
        }

        /// <summary>
        /// Returns true if the username is equal to the object
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            return value == (Username)obj;
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
            return value;
        }

        /// <summary>
        /// Converts username value to the byte array
        /// </summary>
        public void ToBytes(byte[] array, int index)
        {
            var usernameLength = value.Length;
            Encoding.ASCII.GetBytes(value, 0, usernameLength, array, index);
            array[index + usernameLength] = byte.MinValue;
        }
        /// <summary>
        /// Converts username value to the byte array
        /// </summary>
        public byte[] ToBytes()
        {
            var array = new byte[ByteSize];
            ToBytes(array, 0);
            return array;
        }

        /// <summary>
        /// Returns true if the username has valid length
        /// </summary>
        public static bool IsValidLength(int length)
        {
            return length >= MinLength && length <= MaxLength;
        }

        /// <summary>
        /// Returns true if the username has valid letters
        /// </summary>
        public static bool IsValidLetters(string username)
        {
            for (int i = 0; i < username.Length; i++)
            {
                var letter = username[i];

                if (!((letter > 47 && letter < 58) || (letter > 96 && letter < 123) || letter == 95))
                    return false;
            }

            return true;
        }

        public static bool operator ==(Username a, Username b) { return a.value == b.value; }
        public static bool operator !=(Username a, Username b) { return a.value != b.value; }
        public static bool operator ==(Username a, string b) { return a.value == b; }
        public static bool operator !=(Username a, string b) { return a.value != b; }
        public static bool operator ==(string a, Username b) { return a == b.value; }
        public static bool operator !=(string a, Username b) { return a != b.value; }

        public static implicit operator string(Username v) { return v.value; }
        public static implicit operator Username(string v) { return new Username(v); }
    }
}
