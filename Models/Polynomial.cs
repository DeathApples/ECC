using ECDH.Services;
using System.Numerics;

namespace ECDH.Models
{
    public class Polynomial
    {
        public BigInteger[] Coefficients { get; private set; }
        public static BigInteger P { get; set; }

        public int Degree => Coefficients.Length - 1;

        private static BigInteger NormilizeInField(BigInteger a) => MathService.NormilizeInField(a, P);
        private static BigInteger ModularInverse(BigInteger a) => ExtendedEuclideanAlgorithm.ModularInverse(a, P);

        public Polynomial()
        {
            Coefficients = [];
        }

        public Polynomial(int degree)
        {
            Coefficients = new BigInteger[degree + 1];
        }

        public Polynomial(params BigInteger[] numbers)
        {
            Coefficients = new BigInteger[numbers.Length];
            Array.Copy(numbers, Coefficients, numbers.Length);

            NormilizeCoefficients();
        }

        public Polynomial(Polynomial polynomial)
        {
            Coefficients = new BigInteger[polynomial.Degree + 1];
            Array.Copy(polynomial.Coefficients, Coefficients, polynomial.Degree + 1);
        }

        public BigInteger this[int index]
        {
            get => Coefficients[index];
            set => Coefficients[index] = value;
        }

        public static Polynomial operator +(Polynomial left, Polynomial right)
        {
            Polynomial result = new (Math.Max(left.Degree, right.Degree));

            for (int i = 0; i <= result.Degree; i++)
            {
                if (i <= left.Degree && i <= right.Degree)
                    result[i] = NormilizeInField(left[i] + right[i]);

                else if (i <= left.Degree)
                    result[i] = left[i];

                else if (i <= right.Degree)
                    result[i] = right[i];
            }

            result.NormilizeDegree();
            return result;
        }

        public static Polynomial operator -(Polynomial left, Polynomial right)
        {
            Polynomial result = new(Math.Max(left.Degree, right.Degree));

            for (int i = 0; i <= result.Degree; i++)
            {
                if (i <= left.Degree && i <= right.Degree)
                    result[i] = NormilizeInField(left[i] - right[i]);

                else if (i <= left.Degree)
                    result[i] = left[i];

                else if (i <= right.Degree)
                    result[i] = P - right[i];
            }

            result.NormilizeDegree();
            return result;
        }

        public static Polynomial operator *(Polynomial left, Polynomial right)
        {
            Polynomial result = new(left.Degree + right.Degree);

            for (int i = 0; i <= left.Degree; i++)
            {
                for (int k = 0; k <= right.Degree; k++)
                {
                    result[i + k] = NormilizeInField(result[i + k] + left[i] * right[k]);
                }
            }

            result.NormilizeDegree();
            return result;
        }

        public static Polynomial operator /(Polynomial left, Polynomial right)
        {
            var newDegree = left.Degree - right.Degree;
            Polynomial result = new(newDegree);
            Polynomial temp = new(left);

            for (int i = newDegree; i >= 0; i--)
            {
                result[i] = NormilizeInField(temp[right.Degree + i] * ModularInverse(right[right.Degree]));

                for (int k = right.Degree + i - 1; k >= i; k--)
                {
                    temp[k] = NormilizeInField(temp[k] - result[i] * right[k - i]);
                }
            }

            result.NormilizeDegree();
            return result;
        }

        public void NormilizeDegree()
        {
            while (Coefficients.Length > 1)
            {
                if (Coefficients[^1] == 0)
                {
                    BigInteger[] temp = new BigInteger[Coefficients.Length - 1];
                    Array.Copy(Coefficients, temp, Coefficients.Length - 1);
                    Coefficients = temp;
                }
                else
                {
                    break;
                }
            }
        }

        public void NormilizeCoefficients()
        {
            for (int i = 0; i < Coefficients.Length; i++)
            {
                Coefficients[i] = NormilizeInField(Coefficients[i]);
            }
        }
    }
}
