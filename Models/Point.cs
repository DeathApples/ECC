using System;
using System.Numerics;

namespace ECDH.Models
{
    public class Point
    {
        public BigInteger x { get; set; }
        public BigInteger y { get; set; }

        public override string? ToString()
        {
            return $"({x}; {y})";
        }
    }
}
