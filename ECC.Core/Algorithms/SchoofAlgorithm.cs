using ECC.Core.Models;
using System.Numerics;

namespace ECC.Core.Algorithms
{
    internal class SchoofAlgorithm
    {
        internal static BigInteger GetPointsCount()
        {
            Polynomial.ClearCache();

            var p = EllipticCurve.Prime;
            var E = Polynomial.EllipticCurve;
            var sqrt16p = CeilSqrt(16 * p);

            BigInteger M = 1, l = 2;
            List<FiniteFieldNumber> remaindersOfT = [];

            while (M < sqrt16p)
            {
                if (l == 2)
                {
                    var gcd = Polynomial.GreatestCommonDivisor(E, new((1, p, 0), (-1, 1, 0)));

                    if (gcd.Degree > 0)
                        remaindersOfT.Add(new(0, l));

                    else
                        remaindersOfT.Add(new(1, l));
                }

                else
                {
                    var p_l = new FiniteFieldNumber(p, l);
                    var f_l = Polynomial.F(l);

                    var x = Polynomial.X;
                    var y = Polynomial.Y;

                    var xp = Polynomial.ModPow(x, p, f_l);
                    var yp = Polynomial.ModPow(y, p % 2, f_l) * Polynomial.ModPow(E, p / 2, f_l) % f_l;

                    var xp2 = Polynomial.ModPow(x, p * p, f_l);
                    var yp2 = Polynomial.ModPow(y, p * p % 2, f_l) * Polynomial.ModPow(E, p * p / 2, f_l) % f_l;

                    var psi_p_l = Polynomial.ModReplaceSquareY(Polynomial.ModPsi(p_l.Value, f_l), f_l);
                    var psi_p_l__1 = Polynomial.ModReplaceSquareY(Polynomial.ModPsi(p_l.Value - 1, f_l), f_l);
                    var psi_p_l_1 = Polynomial.ModReplaceSquareY(Polynomial.ModPsi(p_l.Value + 1, f_l), f_l);
                    var psi2_p_l = Polynomial.ModReplaceSquareY(Polynomial.ModPow(psi_p_l, 2, f_l), f_l);

                    var gcd_x_p_l = Polynomial.GreatestCommonDivisor(Polynomial.ModReplaceSquareY(psi2_p_l * (xp2 - x) + psi_p_l__1 * psi_p_l_1, f_l), f_l);

                    if (gcd_x_p_l.Degree > 0)
                    {
                        if (LegendreSymbol.Compute(p_l.Value, l) == -1)
                            remaindersOfT.Add(new(0, l));

                        else
                        {
                            var omega = TonelliShanksAlgorithm.GetSquareRoot(p_l.Value, l);

                            if (omega > l / 2)
                                omega = l - omega;

                            var psi_omega = Polynomial.ModReplaceSquareY(Polynomial.ModPsi(omega, f_l), f_l);
                            var psi_omega__1 = Polynomial.ModReplaceSquareY(Polynomial.ModPsi(omega - 1, f_l), f_l);
                            var psi_omega_1 = Polynomial.ModReplaceSquareY(Polynomial.ModPsi(omega + 1, f_l), f_l);
                            var psi2_omega = Polynomial.ModReplaceSquareY(Polynomial.ModPow(psi_omega, 2, f_l), f_l);

                            var gcd_x_omega = Polynomial.GreatestCommonDivisor(Polynomial.ModReplaceSquareY(psi2_omega * (xp - x) + psi_omega__1 * psi_omega_1, f_l), f_l);

                            if (gcd_x_omega.Degree == 0)
                                remaindersOfT.Add(new(0, l));

                            else
                            {
                                var psi_2_omega = Polynomial.ModReplaceSquareY(Polynomial.ModPsi(2 * omega, f_l), f_l);
                                var psi4_omega = Polynomial.ModReplaceSquareY(Polynomial.ModPow(psi_omega, 4, f_l), f_l);

                                var gcd_y_omega = Polynomial.GreatestCommonDivisor(Polynomial.ModReplaceSquareY((2 * yp * psi4_omega - psi_2_omega) / y, f_l), f_l);

                                if (gcd_y_omega.Degree == 0)
                                    remaindersOfT.Add(new(-2 * omega, l));

                                else
                                    remaindersOfT.Add(new(2 * omega, l));
                            }
                        }
                    }

                    else
                    {
                        var x3 = xp2;
                        var y3 = yp2;

                        Polynomial x4, y4;

                        if (p_l.Value == 1)
                        {
                            x4 = x;
                            y4 = y;
                        }

                        else if (p_l.Prime - p_l.Value == 1)
                        {
                            x4 = x;
                            y4 = -y;
                        }

                        else
                        {
                            var (min_p_l_sign, min_p_l) = p_l.Value > l / 2 ? (-1, l - p_l.Value) : (1, p_l.Value);

                            var psi_min_p_l = Polynomial.ModReplaceSquareY(Polynomial.ModPsi(min_p_l, f_l), f_l);
                            var psi_min_p_l__1 = Polynomial.ModReplaceSquareY(Polynomial.ModPsi(min_p_l - 1, f_l), f_l);
                            var psi_min_p_l_1 = Polynomial.ModReplaceSquareY(Polynomial.ModPsi(min_p_l + 1, f_l), f_l);
                            var psi2_min_p_l = Polynomial.ModReplaceSquareY(Polynomial.ModPow(psi_min_p_l, 2, f_l), f_l);

                            var numeratorOfX4 = Polynomial.ModReplaceSquareY(psi_min_p_l__1 * psi_min_p_l_1, f_l);
                            var denominatorOfX4 = Polynomial.ModReplaceSquareY(psi2_min_p_l, f_l);
                            x4 = (x - numeratorOfX4 * denominatorOfX4.Inverse(f_l)) % f_l;

                            var psi_2_min_p_l = Polynomial.ModReplaceSquareY(Polynomial.ModPsi(2 * min_p_l, f_l), f_l);
                            var psi4_min_p_l = Polynomial.ModReplaceSquareY(Polynomial.ModPow(psi_min_p_l, 4, f_l), f_l);

                            var numeratorOfY4 = Polynomial.ModReplaceSquareY(psi_2_min_p_l, f_l);
                            var denominatorOfY4 = Polynomial.ModReplaceSquareY(2 * psi4_min_p_l, f_l);
                            y4 = min_p_l_sign * numeratorOfY4 * denominatorOfY4.Inverse(f_l) % f_l;
                        }

                        var numeratorOfScope = (y3 - y4) % f_l;
                        var denominatorOfScope = (x3 - x4) % f_l;
                        var scope = numeratorOfScope * denominatorOfScope.Inverse(f_l) % f_l;

                        var x5 = Polynomial.ModReplaceSquareY(Polynomial.ModPow(scope, 2, f_l) - x3 - x4, f_l);
                        var y5 = Polynomial.ModReplaceSquareY(scope * (x3 - x5) - y3, f_l);

                        if (yp == y5)
                            remaindersOfT.Add(new(1, l));

                        else if (yp == -y5)
                            remaindersOfT.Add(new(-1, l));

                        else
                        {
                            for (BigInteger tau = 2; tau <= (l - 1) / 2; tau++)
                            {
                                var psi_tau = Polynomial.ModReplaceSquareY(Polynomial.ModPsi(tau, f_l), f_l);
                                var psi_tau__1 = Polynomial.ModReplaceSquareY(Polynomial.ModPsi(tau - 1, f_l), f_l);
                                var psi_tau_1 = Polynomial.ModReplaceSquareY(Polynomial.ModPsi(tau + 1, f_l), f_l);
                                var psi2_tau = Polynomial.ModReplaceSquareY(Polynomial.ModPow(psi_tau, 2, f_l), f_l);

                                var numeratorOfTauX = Polynomial.ModReplaceSquareY(psi_tau__1 * psi_tau_1, f_l);
                                var denominatorOfTauX = Polynomial.ModReplaceSquareY(psi2_tau, f_l);
                                var tauX = (x - numeratorOfTauX * denominatorOfTauX.Inverse(f_l)) % f_l;

                                var tauXp = Polynomial.ModReplaceXY(tauX, xp, yp, f_l);

                                if (x5 == tauXp)
                                {
                                    var psi_2_tau = Polynomial.ModReplaceSquareY(Polynomial.ModPsi(2 * tau, f_l), f_l);
                                    var psi4_tau = Polynomial.ModReplaceSquareY(Polynomial.ModPow(psi_tau, 4, f_l), f_l);

                                    var numeratorOfTauY = Polynomial.ModReplaceSquareY(psi_2_tau, f_l);
                                    var denominatorOfTauY = Polynomial.ModReplaceSquareY(2 * psi4_tau, f_l);
                                    var tauY = numeratorOfTauY * denominatorOfTauY.Inverse(f_l) % f_l;

                                    var tauYp = Polynomial.ModReplaceXY(tauY, xp, yp, f_l);

                                    if (y5 == tauYp)
                                        remaindersOfT.Add(new(tau, l));

                                    else
                                        remaindersOfT.Add(new(-tau, l));

                                    break;
                                }
                            }
                        }
                    }
                }

                M *= l;
                l = MillerRabinPrimalityTest.NextPrimeNumber(l);
            }

            var number = ChineseRemainderTheorem.GetNumber(remaindersOfT);
            BigInteger t;

            if (number.Value < CeilSqrt(4 * p))
                t = number.Value;
            else
                t = number.Value - number.Prime;

            return p + 1 - t;
        }

        private static BigInteger CeilSqrt(BigInteger n)
        {
            BigInteger low = 1, high = n;
            BigInteger result = n;

            while (high >= low)
            {
                BigInteger middle = high - (high - low) / 2;

                if (middle * middle >= n)
                {
                    result = middle;
                    high = middle - 1;
                }

                else
                    low = middle + 1;
            }

            return result;
        }
    }
}
