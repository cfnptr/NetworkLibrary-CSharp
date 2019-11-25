
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
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace QuantumBranch.OpenNetworkLibrary
{
    /// <summary>
    /// Password hash container
    /// </summary>
    public class Passhash
    {
        /// <summary>
        /// Passhash byte array size
        /// </summary>
        public const int ByteSize = 64;
        /// <summary>
        /// Minimum password length
        /// </summary>
        public const int MinPasswordLength = 6;

        /// <summary>
        /// Passhash big integer value
        /// </summary>
        protected BigInteger value;

        /// <summary>
        /// Creates a new passhash container class instance
        /// </summary>
        public Passhash(BigInteger value)
        {
            var bytes = value.ToByteArray();

            if (!IsValidLength(bytes.Length))
                throw new ArgumentException("Invalid passhash length", nameof(value));

            this.value = value;
        }
        /// <summary>
        /// Creates a new passhash container class instance
        /// </summary>
        public Passhash(byte[] value)
        {
            if (!IsValidLength(value.Length))
                throw new ArgumentException("Invalid passhash length", nameof(value));

            this.value = new BigInteger(value);
        }
        /// <summary>
        /// Creates a new passhash container class instance
        /// </summary>
        public Passhash(byte[] array, int index)
        {
            var bytes = new byte[ByteSize];
            Buffer.BlockCopy(array, index, bytes, 0, ByteSize);
            value = new BigInteger(bytes);
        }
        /// <summary>
        /// Creates a new passhash container class instance
        /// </summary>
        public Passhash(string base64)
        {
            var bytes = Convert.FromBase64String(base64);

            if (!IsValidLength(bytes.Length))
                throw new ArgumentException("Invalid passhash length", nameof(base64));
            
            value = new BigInteger(bytes);
        }

        /// <summary>
        /// Returns true if the passhash is equal to the object
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            return value == (BigInteger)obj;
        }
        /// <summary>
        /// Returns passhash hash code 
        /// </summary>
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        /// <summary>
        /// Converts passhash value to the base 64 string value
        /// </summary>
        public string ToBase64()
        {
            var bytes = value.ToByteArray();
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Converts passhash value to the byte array
        /// </summary>
        public void ToBytes(byte[] array, int index)
        {
            var bytes = value.ToByteArray();
            Buffer.BlockCopy(bytes, 0, array, index, ByteSize);
        }
        /// <summary>
        /// Converts passhash value to the byte array
        /// </summary>
        public byte[] ToBytes()
        {
            return value.ToByteArray();
        }

        /// <summary>
        /// Returns true if the passhash has valid length
        /// </summary>
        public static bool IsValidLength(int length)
        {
            return length == ByteSize;
        }

        /// <summary>
        /// Returns true if the password has valid length
        /// </summary>
        public static bool IsPasswordValidLength(int length)
        {
            return length >= MinPasswordLength;
        }

        /// <summary>
        /// Creaetes a new passhash from the password string value
        /// </summary>
        public static Passhash PasswordToPasshash(string password, int iteration = ushort.MaxValue)
        {
            if (!IsPasswordValidLength(password.Length))
                throw new ArgumentException("Invalid password length", nameof(password));

            var hash = Encoding.UTF8.GetBytes(password);

            using(var encryptor = SHA512.Create())
            {
                for (int i = 0; i < iteration; i++)
                    hash = encryptor.ComputeHash(hash);
            }

            return new Passhash(hash);
        }

        public static bool operator ==(Passhash a, Passhash b) { return a.value == b.value; }
        public static bool operator !=(Passhash a, Passhash b) { return a.value != b.value; }
        public static bool operator ==(Passhash a, BigInteger b) { return a.value == b; }
        public static bool operator !=(Passhash a, BigInteger b) { return a.value != b; }
        public static bool operator ==(BigInteger a, Passhash b) { return a == b.value; }
        public static bool operator !=(BigInteger a, Passhash b) { return a != b.value; }

        public static implicit operator BigInteger(Passhash v) { return v.value; }
        public static implicit operator Passhash(BigInteger v) { return new Passhash(v); }
        public static implicit operator byte[](Passhash v) { return v.ToBytes(); }
        public static implicit operator Passhash(byte[] v) { return new Passhash(v); }
    }
}
