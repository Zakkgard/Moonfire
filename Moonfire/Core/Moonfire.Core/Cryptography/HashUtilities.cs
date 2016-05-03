namespace Moonfire.Core.Cryptography
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Provides facilities for performing common-but-specific
    /// cryptographical operations
    /// </summary>
    public static class HashUtilities
    {
        /// <summary>
        /// Brokers various data types into their integral raw
        /// form for usage by other cryptographical functions
        /// </summary>
        public class HashDataBroker
        {
            /// <summary>
            /// Default constructor
            /// </summary>
            /// <param name="data">the data to broker</param>
            public HashDataBroker(byte[] data)
            {
                RawData = data;
            }

            internal byte[] RawData;

            internal int Length
            {
                get { return RawData.Length; }
            }

            /// <summary>
            /// Implicit operator for byte[]->HashDataBroker casts
            /// </summary>
            /// <param name="data">the data to broker</param>
            /// <returns>a HashDataBroker object representing the original data</returns>
            public static implicit operator HashDataBroker(byte[] data)
            {
                return new HashDataBroker(data);
            }

            /// <summary>
            /// Implicit operator for string->HashDataBroker casts
            /// </summary>
            /// <param name="str">the data to broker</param>
            /// <returns>a HashDataBroker object representing the original data</returns>
            public static implicit operator HashDataBroker(string str)
            {
                return new HashDataBroker(Encoding.UTF8.GetBytes(str));
            }

            /// <summary>
            /// Implicit operator for BigInteger->HashDataBroker casts
            /// </summary>
            /// <param name="integer">the data to broker</param>
            /// <returns>a HashDataBroker object representing the original data</returns>
            public static implicit operator HashDataBroker(BigInteger integer)
            {
                return new HashDataBroker(integer.GetBytes());
            }

            /// <summary>
            /// Implicit operator for uint->HashDataBroker casts
            /// </summary>
            /// <param name="integer">the data to broker</param>
            /// <returns>a HashDataBroker object representing the original data</returns>
            public static implicit operator HashDataBroker(uint integer)
            {
                return new HashDataBroker(new BigInteger(integer).GetBytes());
            }
        }

        /// <summary>
        /// Computes a hash from hash data brokers using the given
        /// hashing algorithm
        /// </summary>
        /// <param name="algorithm">the hashing algorithm to use</param>
        /// <param name="brokers">the data brokers to hash</param>
        /// <returns>the hash result of all the data brokers</returns>
        public static byte[] FinalizeHash(HashAlgorithm algorithm, params HashDataBroker[] brokers)
        {
            MemoryStream buffer = new MemoryStream();

            foreach (HashDataBroker broker in brokers)
            {
                buffer.Write(broker.RawData, 0, broker.Length);
            }

            buffer.Position = 0;

            return algorithm.ComputeHash(buffer);
        }

        /// <summary>
        /// Computes a hash from hash data brokers using the given 
        /// hash algorithm, and generates a BigInteger from it
        /// </summary>
        /// <param name="algorithm"></param>
        /// <param name="brokers"></param>
        /// <returns></returns>
        public static BigInteger HashToBigInteger(HashAlgorithm algorithm, params HashDataBroker[] brokers)
        {
            return new BigInteger(FinalizeHash(algorithm, brokers));
        }
    }
}
