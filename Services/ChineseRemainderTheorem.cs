using System.Numerics;

namespace ECDH.Services
{
    public static class ChineseRemainderTheorem
    {
        public static BigInteger Compute(List<BigInteger> listOfAs, List<BigInteger> listOfNs)
        {
            CheckRequirements(listOfAs, listOfNs);

            var prodN = BigInteger.One;
            foreach (var n in listOfNs)
            {
                prodN *= n;
            }

            var result = BigInteger.Zero;
            for (var i = 0; i < listOfNs.Count; i++)
            {
                var a_i = listOfAs[i];
                var n_i = listOfNs[i];
                var modulus_i = prodN / n_i;

                var bezout_modulus_i = ExtendedEuclideanAlgorithm.Compute(n_i, modulus_i).t;
                result += a_i * bezout_modulus_i * modulus_i;
            }

            result %= prodN;
            if (result < 0)
            {
                result += prodN;
            }

            return result;
        }

        private static void CheckRequirements(List<BigInteger> listOfAs, List<BigInteger> listOfNs)
        {
            if (listOfAs == null || listOfNs == null || listOfAs.Count != listOfNs.Count)
            {
                throw new ArgumentException("The parameters 'listOfAs' and 'listOfNs' must not be null and have to be of equal length!");
            }

            if (listOfNs.Any(x => x <= 1))
            {
                throw new ArgumentException($"The value {listOfNs.First(x => x <= 1)} for some n_i is smaller than or equal to 1.");
            }

            if (listOfAs.Any(x => x < 0))
            {
                throw new ArgumentException($"The value {listOfAs.First(x => x < 0)} for some a_i is smaller than 0.");
            }

            for (var i = 0; i < listOfNs.Count; i++)
            {
                for (var j = i + 1; j < listOfNs.Count; j++)
                {
                    BigInteger gcd;
                    if ((gcd = ExtendedEuclideanAlgorithm.Compute(listOfNs[i], listOfNs[j]).gcd) != BigInteger.One)
                    {
                        throw new ArgumentException($"The GCD of n_{i} = {listOfNs[i]} and n_{j} = {listOfNs[j]} equals {gcd} and thus these values aren't coprime.");
                    }
                }
            }
        }
    }
}
