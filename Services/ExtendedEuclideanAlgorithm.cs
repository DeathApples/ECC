using System.Numerics;

namespace ECDH.Services
{
    public static class ExtendedEuclideanAlgorithm
    {
        public static (BigInteger gcd, BigInteger s, BigInteger t) Compute(BigInteger n, BigInteger m)
        {
            BigInteger s = 0, s0 = 1;
            BigInteger t = 1, t0 = 0;
            BigInteger r = m, r0 = n;
            BigInteger q;

            while (r != 0)
            {
                q = r0 / r;
                (r0, r) = (r, r0 - q * r);
                (s0, s) = (s, s0 - q * s);
                (t0, t) = (t, t0 - q * t);
            }

            return (r0, s0, t0);
        }

        public static BigInteger ModularInverse(BigInteger n, BigInteger p)
        {
            BigInteger gcd, s;
            (gcd, s, _) = Compute(n, p);

            if (gcd != 1)
                throw new ArithmeticException("Argument “n” must be greater than zero and Argument “p” must be a prime number");

            return s < 0 ? s + p : s;
        }
    }
}
