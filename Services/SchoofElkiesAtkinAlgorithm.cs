using ECDH.Models;
using System.Numerics;

namespace ECDH.Services
{
    public static class SchoofElkiesAtkinAlgorithm
    {
        //private readonly static List<BigInteger> listOfAtkinAs = [];
        //private readonly static List<BigInteger> listOfAtkinNs = [];
        //private readonly static List<BigInteger> listOfElkiesAs = [];
        //private readonly static List<BigInteger> listOfElkiesNs = [];

        //public static BigInteger Compute()
        //{
        //    BigInteger M = 1, l = 2;

        //    listOfAtkinAs.Clear(); listOfAtkinNs.Clear();
        //    listOfElkiesAs.Clear(); listOfElkiesNs.Clear();

        //    BigInteger j = EllipticCurve.JInvariant;
        //    BigInteger sqrt16p = MathService.CeilSqrt(16 * EllipticCurve.P);

        //    while (M < sqrt16p)
        //    {
        //        // Выполнить проверку, является ли число l простым Элкиса или Аткина

        //        M *= l;
        //        l = MillerRabinPrimalityTest.NextPrimeNumber(l);
        //    }

        //    BigInteger t = RecoverTraceOfFrobeniusEndomorphism();
        //    return EllipticCurve.P + 1 - t;
        //}

        //private static (BigInteger, BigInteger) BabyStepGiantStepAlgorithm(BigInteger tE, BigInteger mE, BigInteger t1, BigInteger m1, BigInteger t2, BigInteger m2)
        //{
        //    Dictionary<Point, BigInteger> steps = [];

        //    var p = EllipticCurve.GeneratePoint();
        //    Point q;

        //    BigInteger r, k = 0, m1half = m1 / 2;
        //    BigInteger r1 = 0, r2 = 0;

        //    do
        //    {
        //        r = (t1 + m1 * k - tE) / (mE * m2);
        //        q = (EllipticCurve.P + 1 - tE) * p - (r * m1 * mE) * p;
        //        steps.Add(q, r);
        //        k++;

        //    } while (BigInteger.Abs(r) <= m1half);

        //    k = 0;

        //    do
        //    {
        //        r = (t2 + m2 * k - tE) / (mE * m1);
        //        q = (r * m2 * mE) * p;
        //        k++;

        //        if (steps.ContainsKey(q))
        //        {
        //            r1 = steps.GetValueOrDefault(q);
        //            r2 = r;
        //            break;
        //        }

        //    } while (BigInteger.Abs(r) <= m2);

        //    return (r1, r2);
        //}

        //private static BigInteger RecoverTraceOfFrobeniusEndomorphism()
        //{
        //    BigInteger tE = ChineseRemainderTheorem.Compute(listOfElkiesAs, listOfElkiesNs);
        //    BigInteger mE = listOfElkiesNs.Aggregate(1, (BigInteger acc, BigInteger val) => acc * val);

        //    List<BigInteger> listOfAtkinFirstHalfAs = [.. listOfAtkinAs.Where((val, index) => index % 2 == 0)];
        //    List<BigInteger> listOfAtkinFirstHalfNs = [.. listOfAtkinNs.Where((val, index) => index % 2 == 0)];
        //    List<BigInteger> listOfAtkinSecondHalfAs = [.. listOfAtkinAs.Where((val, index) => index % 2 == 1)];
        //    List<BigInteger> listOfAtkinSecondHalfNs = [.. listOfAtkinNs.Where((val, index) => index % 2 == 1)];

        //    BigInteger t1 = ChineseRemainderTheorem.Compute(listOfAtkinFirstHalfAs, listOfAtkinFirstHalfNs);
        //    BigInteger m1 = listOfAtkinFirstHalfNs.Aggregate(1, (BigInteger acc, BigInteger val) => acc * val);

        //    BigInteger t2 = ChineseRemainderTheorem.Compute(listOfAtkinSecondHalfAs, listOfAtkinSecondHalfNs);
        //    BigInteger m2 = listOfAtkinSecondHalfNs.Aggregate(1, (BigInteger acc, BigInteger val) => acc * val);

        //    (BigInteger r1, BigInteger r2) = BabyStepGiantStepAlgorithm(tE, mE, t1, m1, t2, m2);
        //    return tE + mE * (m1 * r2 + m2 * r1);
        //}
    }
}
