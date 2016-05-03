namespace Moonfire.Core.Cryptography
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.Text;

    public class SRPParameters
    {
        #region SRPVersion enum

        /// <summary>
        /// Algorithm version. Consult specification for differences.
        /// </summary>
        public enum SRPVersion
        {
            SRP6,
            SRP6a,
        }

        #endregion

        /// <summary>
        /// Maximum length of crypto keys in bytes.
        /// </summary>
        /// <remarks>You might get unlucky and have much shorter keys - this should be checked and keys recalculated.</remarks>
        public const uint KeyLength = 32;

        /// <summary>
        /// Random number generator for this instance.
        /// </summary>
        public static RandomNumberGenerator RandomGenerator = new RNGCryptoServiceProvider();

        /// <summary>
        /// Version of this instance.
        /// </summary>
        public SRPVersion AlgorithmVersion = SRPVersion.SRP6;

        public bool CaseSensitive = false;

        /// <summary>
        /// Hashing function for the instance.
        /// </summary>
        /// <remarks>MD5 or other SHA hashes are usable, though SHA1 is more standard for SRP.</remarks>
        [NonSerialized]
        public static readonly HashAlgorithm Hash = new SHA1Managed();

        /// <summary>
        /// 'g' in the spec. This number must be a generator in the finite field Modulus.
        /// </summary>
        private static readonly BigInteger s_generator = new BigInteger(7);
        private static readonly BigInteger s_modulus =
            new BigInteger("894B645E89E1535BBDAD5B8B290650530801B18EBFBF5E8FAB3C82872A3E9BB7", 16);
        private static readonly BigInteger s_multiplier = new BigInteger(3);

        /// <summary>
        /// All operations are mod this number. It should be a large prime.
        /// </summary>
        /// <remarks>Referred to as 'N' in the spec.</remarks>
        /// <remarks>Defaults to 128 bits.</remarks>
        public BigInteger Modulus
        {
            get { return s_modulus; }
        }

        public BigInteger Generator
        {
            get { return s_generator; }
        }

        public BigInteger Multiplier
        {
            get { return s_multiplier; }
        }

        public static SRPParameters Defaults
        {
            get { return new SRPParameters(); }
        }
    }
}
