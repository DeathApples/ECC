using System.Numerics;

namespace ECC.Core.Models
{
    internal class Polynomial
    {
        #region Поля
        private static readonly Dictionary<(BigInteger, Polynomial), Polynomial> cacheModPsi = [];

        private static readonly Dictionary<BigInteger, Polynomial> cacheF = [];
        #endregion

        #region Свойства
        private static BigInteger A => Models.EllipticCurve.A;

        private static BigInteger B => Models.EllipticCurve.B;

        private static BigInteger Prime => Models.EllipticCurve.Prime;

        internal BigInteger Degree => Monomials.Count > 0 ? Monomials.Keys.Select(key => key.xPower + key.yPower).Max() : 0;

        internal Dictionary<(BigInteger xPower, BigInteger yPower), FiniteFieldNumber> MonomialsWithMaxDegree => Monomials.Where(monomial => monomial.Key.xPower + monomial.Key.yPower == Degree).ToDictionary();

        internal Dictionary<(BigInteger xPower, BigInteger yPower), FiniteFieldNumber> Monomials { get; } = [];

        internal static Polynomial X => new((1, 1, 0));

        internal static Polynomial Y => new((1, 0, 1));

        internal static Polynomial EllipticCurve => new((1, 3, 0), (A, 1, 0), (B, 0, 0));
        #endregion

        #region Конструкторы
        internal Polynomial() { }

        internal Polynomial(BigInteger a)
        {
            if (a % Prime != 0)
                Monomials[(0, 0)] = new(a, Prime);
        }

        internal Polynomial(FiniteFieldNumber a)
        {
            if (a.Prime != Prime)
                throw new ArgumentException("Primes must match");

            if (a.Value != 0)
                Monomials[(0, 0)] = a;
        }

        internal Polynomial(Dictionary<(BigInteger xPower, BigInteger yPower), FiniteFieldNumber> monomials)
        {
            foreach (var monomial in monomials)
                if (monomial.Value.Value != 0)
                    Monomials[monomial.Key] = monomial.Value;
        }

        internal Polynomial(params (BigInteger coefficient, BigInteger xPower, BigInteger yPower)[] monomials)
        {
            foreach (var (coefficient, xPower, yPower) in monomials)
                if (coefficient != 0)
                    Monomials[(xPower, yPower)] = new(coefficient, Prime);
        }

        internal Polynomial(params (FiniteFieldNumber coefficient, BigInteger xPower, BigInteger yPower)[] monomials)
        {
            foreach (var (coefficient, xPower, yPower) in monomials)
            {
                if (coefficient.Prime != Prime)
                    throw new ArgumentException("Primes must match");

                if (coefficient.Value != 0)
                    Monomials[(xPower, yPower)] = coefficient;
            }
        }
        #endregion

        #region Индексаторы
        internal FiniteFieldNumber this[(BigInteger xPower, BigInteger yPower) index]
        {
            get => Monomials.TryGetValue(index, out FiniteFieldNumber? value) ? value : new(0, Prime);

            set
            {
                if (value.Value == 0)
                    Monomials.Remove(index);
                else
                    Monomials[index] = value;
            }
        }
        #endregion

        #region Методы
        private static (Polynomial quotient, Polynomial remainder) Divide(Polynomial P, Polynomial Q)
        {
            var quotient = new Polynomial();
            var remainder = P.Copy();

            bool hasFound = true;

            while (hasFound && remainder.Monomials.Count > 0 && remainder.Degree >= Q.Degree)
            {
                hasFound = false;

                foreach (var monomialP in remainder.MonomialsWithMaxDegree)
                {
                    var (xPowerP, yPowerP) = monomialP.Key;

                    foreach (var monomialQ in Q.MonomialsWithMaxDegree)
                    {
                        var (xPowerQ, yPowerQ) = monomialQ.Key;

                        var x = xPowerP - xPowerQ;
                        var y = yPowerP - yPowerQ;

                        if (x >= 0 && y >= 0)
                        {
                            var coefficient = monomialP.Value / monomialQ.Value;
                            quotient[(x, y)] += coefficient;
                            remainder -= Q * new Polynomial((coefficient, x, y));

                            if (x == 0 && y == 0)
                                return (quotient, remainder);

                            hasFound = true;
                            break;
                        }
                    }

                    if (hasFound)
                        break;
                }
            }

            return (quotient, remainder);
        }

        private static Polynomial ExtendedEuclideanAlgoritm(Polynomial P, Polynomial Q, out Polynomial S, out Polynomial T)
        {
            (S, var S0, T, var T0) = (new(1), new Polynomial(), new(), new Polynomial(1));
            var P0 = P.Copy();
            var Q0 = Q.Copy();

            while (Q0.Monomials.Count > 0 && (Q0.Degree > 0 || Q0[(0, 0)].Value > 0))
            {
                var (q, r) = Divide(P0, Q0);
                (P0, Q0) = (Q0, r);
                (S, S0) = (S0, S - q * S0);
                (T, T0) = (T0, T - q * T0);
            }

            return P0;
        }

        internal static void ClearCache()
        {
            cacheModPsi.Clear();
            cacheF.Clear();
        }

        internal Polynomial Copy() => new(Monomials);

        internal Polynomial Inverse(Polynomial M)
        {
            var gcd = ExtendedEuclideanAlgoritm(this, M, out Polynomial P0, out _);

            if (gcd.Degree > 0)
                throw new InvalidOperationException("Element not invertible");

            return P0 / gcd;
        }

        internal static Polynomial GreatestCommonDivisor(Polynomial P, Polynomial Q) => ExtendedEuclideanAlgoritm(P, Q, out _, out _);

        internal static Polynomial ModReplaceSquareY(Polynomial P, Polynomial M)
        {
            var result = new Polynomial();

            foreach (var monomial in P.Monomials)
            {
                var (xPower, yPower) = monomial.Key;
                result = (result + ModPow(EllipticCurve, yPower / 2, M) * new Polynomial((monomial.Value, xPower, yPower % 2)) % M) % M;
            }

            return result;
        }

        internal static Polynomial ModReplaceXY(Polynomial P, Polynomial Xp, Polynomial Yp, Polynomial M)
        {
            var result = new Polynomial();

            foreach (var monomial in P.Monomials)
            {
                var (xPower, yPower) = monomial.Key;
                result = (result + monomial.Value * ModPow(Xp, xPower, M) * ModPow(Yp, yPower, M)) % M;
            }

            return result;
        }

        internal static Polynomial Pow(Polynomial P, BigInteger exponent)
        {
            if (exponent < 0)
                throw new ArgumentException("Exponent must be greater than 0");

            var result = new Polynomial(1);
            var addend = P.Copy();

            while (exponent > 0)
            {
                if ((exponent & 1) == 1)
                    result *= addend;

                addend *= addend;
                exponent >>= 1;
            }

            return result;
        }

        internal static Polynomial ModPow(Polynomial P, BigInteger exponent, Polynomial M)
        {
            if (exponent < 0)
                throw new ArgumentException("Exponent must be greater than 0");

            var result = new Polynomial(1);
            var addend = P.Copy();

            while (exponent > 0)
            {
                if ((exponent & 1) == 1)
                    result = result * addend % M;

                addend = addend * addend % M;
                exponent >>= 1;
            }

            return result;
        }

        internal static Polynomial ModPsi(BigInteger n, Polynomial M)
        {
            if (cacheModPsi.TryGetValue((n, M), out Polynomial? value))
                return value;

            if (n == 0 || n == 1)
                cacheModPsi[(n, M)] = new(n);

            else if (n == 2)
                cacheModPsi[(n, M)] = new((2, 0, 1));

            else if (n == 3)
                cacheModPsi[(n, M)] = new Polynomial((3, 4, 0), (6 * A, 2, 0), (12 * B, 1, 0), (-A * A, 0, 0)) % M;

            else if (n == 4)
                cacheModPsi[(n, M)] = 4 * Y * (new Polynomial((1, 6, 0), (5 * A, 4, 0), (20 * B, 3, 0), (-5 * A * A, 2, 0), (-4 * A * B, 1, 0), (-8 * B * B - A * A * A, 0, 0)) % M);

            else if (n % 2 == 1)
            {
                var m = (n - 1) / 2;
                cacheModPsi[(n, M)] = ModReplaceSquareY(ModPsi(m + 2, M) * ModPow(ModPsi(m, M), 3, M) - ModPsi(m - 1, M) * ModPow(ModPsi(m + 1, M), 3, M), M);
            }

            else
            {
                var m = n / 2;
                cacheModPsi[(n, M)] = ModReplaceSquareY((ModPsi(m + 2, M) * ModPow(ModPsi(m - 1, M), 2, M) - ModPsi(m - 2, M) * ModPow(ModPsi(m + 1, M), 2, M)) * ModPsi(m, M) / ModPsi(2, M), M);
            }

            return cacheModPsi[(n, M)];
        }

        internal static Polynomial F(BigInteger n)
        {
            if (cacheF.TryGetValue(n, out Polynomial? value))
                return value;

            if (n == 0 || n == 1)
                cacheF[n] = new(n);

            else if (n == 2)
                cacheF[n] = new(1);

            else if (n == 3)
                cacheF[n] = new((3, 4, 0), (6 * A, 2, 0), (12 * B, 1, 0), (-A * A, 0, 0));

            else if (n == 4)
                cacheF[n] = new((2, 6, 0), (10 * A, 4, 0), (40 * B, 3, 0), (-10 * A * A, 2, 0), (-8 * A * B, 1, 0), (-16 * B * B - 2 * A * A * A, 0, 0));

            else if (n % 2 == 1)
            {
                var m = (n - 1) / 2;

                if (m % 2 == 1)
                    cacheF[n] = F(m + 2) * Pow(F(m), 3) - 16 * Pow(EllipticCurve, 2) * F(m - 1) * Pow(F(m + 1), 3);

                else
                    cacheF[n] = 16 * Pow(EllipticCurve, 2) * F(m + 2) * Pow(F(m), 3) - F(m - 1) * Pow(F(m + 1), 3);
            }

            else
            {
                var m = n / 2;
                cacheF[n] = (F(m + 2) * Pow(F(m - 1), 2) - F(m - 2) * Pow(F(m + 1), 2)) * F(m);
            }

            return cacheF[n];
        }
        #endregion

        #region Перегрузки операторов
        public static Polynomial operator +(Polynomial P, Polynomial Q)
        {
            var result = P.Copy();

            foreach (var monomial in Q.Monomials)
                result[monomial.Key] += monomial.Value;

            return result;
        }

        public static Polynomial operator +(BigInteger a, Polynomial P)
        {
            var result = P.Copy();

            result[(0, 0)] += a;

            return result;
        }

        public static Polynomial operator +(Polynomial P, BigInteger a) => a + P;

        public static Polynomial operator +(FiniteFieldNumber a, Polynomial P) => a.Value + P;

        public static Polynomial operator +(Polynomial P, FiniteFieldNumber a) => a.Value + P;

        public static Polynomial operator -(Polynomial P, Polynomial Q) => P + -Q;

        public static Polynomial operator -(BigInteger a, Polynomial P) => a + -P;

        public static Polynomial operator -(Polynomial P, BigInteger a) => -a + P;

        public static Polynomial operator -(FiniteFieldNumber a, Polynomial P) => a.Value - P;

        public static Polynomial operator -(Polynomial P, FiniteFieldNumber a) => -a.Value + P;

        public static Polynomial operator -(Polynomial P)
        {
            var result = new Polynomial();

            foreach (var monomial in P.Monomials)
                result[monomial.Key] = -monomial.Value;

            return result;
        }

        public static Polynomial operator *(Polynomial P, Polynomial Q)
        {
            var result = new Polynomial();

            foreach (var monomialP in P.Monomials)
            {
                var (xPowerP, yPowerP) = monomialP.Key;

                foreach (var monomialQ in Q.Monomials)
                {
                    var (xPowerQ, yPowerQ) = monomialQ.Key;

                    result[(xPowerP + xPowerQ, yPowerP + yPowerQ)] += monomialP.Value * monomialQ.Value;
                }
            }

            return result;
        }

        public static Polynomial operator *(BigInteger n, Polynomial P)
        {
            var result = new Polynomial();

            foreach (var monomial in P.Monomials)
                result[monomial.Key] += n * monomial.Value;

            return result;
        }

        public static Polynomial operator *(Polynomial P, BigInteger n) => n * P;

        public static Polynomial operator *(FiniteFieldNumber n, Polynomial P) => n.Value * P;

        public static Polynomial operator *(Polynomial P, FiniteFieldNumber n) => n.Value * P;

        public static Polynomial operator /(Polynomial P, Polynomial Q) => Divide(P, Q).quotient;

        public static Polynomial operator %(Polynomial P, Polynomial Q) => Divide(P, Q).remainder;

        public static bool operator ==(Polynomial P, Polynomial Q) => P.Equals(Q);

        public static bool operator !=(Polynomial P, Polynomial Q) => !P.Equals(Q);
        #endregion

        #region Перегрузки методов
        public override bool Equals(object? obj)
        {
            ArgumentNullException.ThrowIfNull(obj, nameof(obj));

            if (obj is Polynomial P)
            {
                if (Monomials.Count != P.Monomials.Count) return false;
                return Monomials.All(kvp => P.Monomials.TryGetValue(kvp.Key, out var value) && value.Equals(kvp.Value));
            }
            else
                throw new ArgumentException("Argument must be DivisionPolynomial", nameof(obj));
        }

        public override int GetHashCode() => Monomials.GetHashCode();

        public override string? ToString()
        {
            if (Monomials.Count == 0)
                return "0";

            string polynomial = "";

            for (int i = 0; i < Monomials.Count; i++)
            {
                var monomial = Monomials.ElementAt(i);
                var (xPower, yPower) = monomial.Key;
                var value = monomial.Value.Value;
                var sign = value.Sign;

                polynomial += (i > 0 ? sign < 0 ? " - " : " + " : "") +
                    (sign * value > 1 || xPower == 0 && yPower == 0 ? i > 0 ? $"{sign * value}" : $"{value}" : "") +
                    (xPower > 0 ? xPower > 1 ? $"x^{xPower}" : "x" : "") +
                    (yPower > 0 ? yPower > 1 ? $"y^{yPower}" : "y" : "");
            }

            return polynomial;
        }
        #endregion
    }
}
