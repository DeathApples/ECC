using ECDH.Models;
using System.Numerics;

namespace ECDH.Services
{
    public static class ExtendedEuclideanAlgorithm
    {
        public static (Polynomial gcd, Polynomial s, Polynomial t) Compute(Polynomial n, Polynomial m)
        {
            BigInteger prime = Polynomial.Prime;

            Polynomial s = new((0, new(0, prime))), s0 = new((0, new(1, prime)));
            Polynomial t = new((0, new(1, prime))), t0 = new((0, new(0, prime)));
            Polynomial r = new(m), r0 = new(n);
            Polynomial q;

            while (r.Degree > 0 || r[0].Value > 0)
            {
                q = r0 / r;
                (r0, r) = (r, r0 - q * r);
                (s0, s) = (s, s0 - q * s);
                (t0, t) = (t, t0 - q * t);
            }

            return (r0, s0, t0);
        }
    }
}
