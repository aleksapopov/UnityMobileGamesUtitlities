using System;
using System.Linq;

/// <summary>
/// Class for big numbers operations. Reeeealy big.
/// </summary>
namespace Big
{
	[Serializable]
	public class BigDouble : IComparable<BigDouble>
	{
		public const double SQRT_10 = 3.16227766016838;

		public static readonly BigDouble ZERO = new BigDouble(0.0, -9223372036854775808L);

		public readonly double numerator;

		public readonly long exponent;

		public BigDouble(double numerator)
		{
			BigDouble bigDouble = BigDouble.create(numerator, 0L);
			this.numerator = bigDouble.numerator;
			this.exponent = bigDouble.exponent;
		}

		protected BigDouble(double numerator, long exponent)
		{
			this.numerator = numerator;
			this.exponent = exponent;
		}

		public static BigDouble create(double numerator, long exponent = 0L)
		{
			if (numerator == 0.0)
			{
				return BigDouble.ZERO;
			}
			if ((numerator >= 1.0 && numerator < 10.0) || (numerator <= -1.0 && numerator > -10.0))
			{
				return new BigDouble(numerator, exponent);
			}
			return BigDouble.normalize(numerator, exponent);
		}

		private static BigDouble normalize(double numerator, long exponent)
		{
			while (numerator < 1.0 && numerator > -1.0)
			{
				numerator *= 10.0;
				exponent -= 1L;
			}
			while (numerator >= 10.0 || numerator <= -10.0)
			{
				numerator *= 0.1;
				exponent += 1L;
			}
			return new BigDouble(numerator, exponent);
		}

		public BigDouble Floor(int precision)
		{
			if (this.Equals(BigDouble.ZERO))
			{
				return BigDouble.ZERO;
			}
			double num;
			if (this.exponent < (long)precision)
			{
				num = Math.Pow(10.0, (double)this.exponent);
			}
			else
			{
				num = Math.Pow(10.0, (double)precision);
			}
			double num2 = Math.Floor(this.numerator * num) * 1.0 / num;
			return new BigDouble(num2, this.exponent);
		}

		public override string ToString()
		{
			return this.ToString(31);
		}

		public string ToString(int startingExponent)
		{
			if (this == BigDouble.ZERO)
			{
				return "0";
			}
			if (this.exponent < (long)startingExponent && this.exponent > (long)(-(long)startingExponent))
			{
				return string.Empty + this.numerator * Math.Pow(10.0, (double)this.exponent);
			}
			string text = (this.exponent < 0L) ? string.Empty : "+";
			return string.Concat(new object[]
			{
				this.numerator,
				"E",
				text,
				this.exponent
			});
		}

		public static BigDouble Parse(string scientific)
		{
			int num = scientific.IndexOf("E");
			int length = (num <= 0) ? scientific.Count<char>() : num;
			double num2 = double.Parse(scientific.Substring(0, length));
			long num3 = (num <= 0) ? 0L : long.Parse(scientific.Substring(num + 1));
			return BigDouble.create(num2, num3);
		}

		public BigDouble Round(int precision = 0)
		{
			double num = Math.Round(this.numerator, precision, MidpointRounding.AwayFromZero);
			return BigDouble.create(num, this.exponent);
		}

		public int ToInt()
		{
			return Convert.ToInt32(this.ToFloat());
		}

		public long ToLong()
		{
			return Convert.ToInt64(this.ToFloat());
		}

		public float ToFloat()
		{
			return (float)(this.numerator * Math.Pow(10.0, (double)this.exponent));
		}

		public int CompareTo(BigDouble another)
		{
			if (this > another)
			{
				return 1;
			}
			if (this == another)
			{
				return 0;
			}
			return -1;
		}

		public override bool Equals(object obj)
		{
			return this.numerator == ((BigDouble)obj).numerator && this.exponent == ((BigDouble)obj).exponent;
		}

		public override int GetHashCode()
		{
			return 0;
		}

		public BigDouble RoundSmall(int expToRound = 200)
		{
			if (this.exponent <= (long)expToRound)
			{
				return BigDouble.create(Math.Round(this.numerator, (int)this.exponent, MidpointRounding.AwayFromZero), this.exponent);
			}
			return this;
		}

		private static BigDouble PlusLeftBigger(BigDouble bigger, BigDouble smaller)
		{
			if (smaller == BigDouble.ZERO)
			{
				return bigger;
			}
			double num = smaller.numerator * Math.Pow(10.0, (double)(smaller.exponent - bigger.exponent));
			return BigDouble.create(bigger.numerator + num, bigger.exponent);
		}

		public BigDouble Pow(double exp)
		{
			return BigDouble.Pow(this, exp);
		}

		public static BigDouble Pow(BigDouble num, double exp)
		{
			double num2 = (double)num.exponent * exp % 1.0;
			double d = Math.Pow(num.numerator, exp) * Math.Pow(10.0, num2);
			if (double.IsInfinity(d))
			{
				BigDouble bigDouble = BigDouble.Pow(num, exp / 2.0);
				return bigDouble * bigDouble;
			}
			long num3 = (long)(exp * (double)num.exponent - num2);
			return BigDouble.create(d, num3);
		}

		public BigDouble Sqrt()
		{
			return BigDouble.Sqrt(this);
		}

		public static BigDouble Sqrt(BigDouble d)
		{
			bool flag = d.exponent % 2L == 0L;
			double num = (!flag) ? 3.16227766016838 : 1.0;
			int num2 = (flag || d.exponent >= 0L) ? 0 : -1;
			long num3 = d.exponent / 2L + (long)num2;
			double num4 = Math.Sqrt(d.numerator) * num;
			return BigDouble.create(num4, num3);
		}

		public static BigDouble Max(BigDouble a, BigDouble b)
		{
			return (!(a >= b)) ? b : a;
		}

		public static BigDouble Min(BigDouble a, BigDouble b)
		{
			return (!(a <= b)) ? b : a;
		}

		public static implicit operator BigDouble(long value)
		{
			return BigDouble.create((double)value, 0L);
		}

		public static implicit operator BigDouble(double value)
		{
			return BigDouble.create(value, 0L);
		}

		public static bool operator >(BigDouble left, BigDouble right)
		{
			return (left.exponent <= right.exponent) ? ((left.exponent != right.exponent) ? (right.numerator < 0.0) : (left.numerator > right.numerator)) : (left.numerator > 0.0);
		}

		public static bool operator <(BigDouble left, BigDouble right)
		{
			return right > left;
		}

		public static bool operator >=(BigDouble left, BigDouble right)
		{
			return (left.exponent <= right.exponent) ? ((left.exponent != right.exponent) ? (right.numerator < 0.0) : (left.numerator >= right.numerator)) : (left.numerator > 0.0);
		}

		public static bool operator <=(BigDouble left, BigDouble right)
		{
			return right >= left;
		}

		public static bool operator ==(BigDouble left, BigDouble right)
		{
			return left.numerator == right.numerator && left.exponent == right.exponent;
		}

		public static bool operator !=(BigDouble left, BigDouble right)
		{
			return left.numerator != right.numerator || left.exponent != right.exponent;
		}

		public static BigDouble operator *(BigDouble left, BigDouble right)
		{
			return BigDouble.create(left.numerator * right.numerator, left.exponent + right.exponent);
		}

		public static BigDouble operator /(BigDouble left, BigDouble right)
		{
			return BigDouble.create(left.numerator / right.numerator, left.exponent - right.exponent);
		}

		public static BigDouble operator +(BigDouble left, BigDouble right)
		{
			return (left.exponent < right.exponent) ? BigDouble.PlusLeftBigger(right, left) : BigDouble.PlusLeftBigger(left, right);
		}

		public static BigDouble operator -(BigDouble left, BigDouble right)
		{
			return (left.exponent < right.exponent) ? BigDouble.PlusLeftBigger(-right, left) : BigDouble.PlusLeftBigger(left, -right);
		}

		public static BigDouble operator -(BigDouble left)
		{
			return BigDouble.create(-left.numerator, left.exponent);
		}
	}
}
