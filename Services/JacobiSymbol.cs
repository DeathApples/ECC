using System.Numerics;

namespace ECDH.Services
{
    public static class JacobiSymbol
    {
        public static BigInteger Compute(BigInteger a, BigInteger p)
        {
            BigInteger r, Jacobi = 1;

            if (p <= 0 || (p & 1) == 0)
                throw new ArgumentException("Argument “p” must be greater than zero and odd");

            if (a < 0)
            {
                if ((p & 3) == 3)
                    Jacobi = -Jacobi;

                a = -a;
            }

            while (a > 0)
            {
                while ((a & 1) == 0)
                {
                    a >>= 1;
                    r = p & 7;

                    if ((r == 3) || (r == 5))
                        Jacobi = -Jacobi;
                }

                if ((a & 3) == 3 && (p & 3) == 3)
                    Jacobi = -Jacobi;

                p %= a; r = a;
                a = p; p = r;
            }

            return p == 1 ? Jacobi : 0;
        }

        public static BigInteger FindNonSquareInFiniteField(BigInteger l)
        {
            BigInteger d = 2;

            while (Compute(d, l) != -1)
            {
                d++;
            }

            return d;
        }
    }
}
