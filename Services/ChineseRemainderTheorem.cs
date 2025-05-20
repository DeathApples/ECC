using ECDH.Models;
using System.Numerics;

namespace ECDH.Services
{
    public static class ChineseRemainderTheorem
    {
        public static FiniteFieldNumber GetNumber(List<FiniteFieldNumber> remainders)
        {
            CheckRequirements(remainders);

            var M = BigInteger.One;

            foreach (var remainder in remainders)
                M *= remainder.Prime;

            FiniteFieldNumber result = new(0, M);

            for (var i = 0; i < remainders.Count; i++)
            {
                BigInteger prime = remainders[i].Prime;
                var Mi = M / prime;
                var MiInverse = new FiniteFieldNumber(Mi, prime).Inverse().Value;

                result += remainders[i].Value * Mi * MiInverse;
            }

            return result;
        }

        private static void CheckRequirements(List<FiniteFieldNumber> remainders)
        {
            if (remainders == null)
                throw new ArgumentException("The parameter 'remainders' must not be null!");

            if (remainders.Any(x => x.Prime <= 1))
                throw new ArgumentException($"The value {remainders.First(x => x.Prime <= 1)} for some n_i is smaller than or equal to 1.");

            if (remainders.Any(x => x.Value < 0))
                throw new ArgumentException($"The value {remainders.First(x => x.Value < 0)} for some a_i is smaller than 0.");

            for (var i = 0; i < remainders.Count; i++)
            {
                for (var j = i + 1; j < remainders.Count; j++)
                {
                    BigInteger gcd;

                    if ((gcd = BigInteger.GreatestCommonDivisor(remainders[i].Prime, remainders[j].Prime)) != 1)
                        throw new ArgumentException($"The GCD of n_{i} = {remainders[i].Prime} and n_{j} = {remainders[j].Prime} equals {gcd} and thus these values aren't coprime.");
                }
            }
        }
    }
}
