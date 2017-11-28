using System;
using System.Text;

/// <summary>
/// Class for nig numbers string representation.
/// </summary>
namespace Big
{
	public class BigString
	{
		public static readonly string[] names = new string[]
		{
			string.Empty,
			"K",
			"M",
			"B",
			"T",
			"aa",
			"bb",
			"cc",
			"dd",
			"ee",
			"ff",
			"gg",
			"hh",
			"ii",
			"jj",
			"kk",
			"ll",
			"mm",
			"nn",
			"oo",
			"pp",
			"qq",
			"rr",
			"ss",
			"tt",
			"uu",
			"vv",
			"ww",
			"xx",
			"yy",
			"zz"
		};

		private static readonly int[] multiplier_exponents = new int[]
		{
			1,
			10,
			100
		};

		private static readonly int maxExponent = BigString.names.Length * 3;

		public static string ToString(BigDouble d, int precision = 2)
		{
			if (d == BigDouble.ZERO)
			{
				return "0";
			}
			if (d.exponent < 0L)
			{
				return "0.0";
			}
			int num = (int)d.exponent % 3;
			long num2 = d.exponent - (long)num;
			string format = (d.exponent >= 3L) ? ("F" + (precision - num)) : string.Empty;
			string text = Math.Round(d.numerator * (double)BigString.multiplier_exponents[num], precision - num).ToString(format);
			if (num2 == 0L)
			{
				return text;
			}
			string str = (num2 >= (long)BigString.maxExponent) ? BigString.doubleLetterCurrency(num2) : BigString.names[(int)(checked((IntPtr)(num2 / 3L)))];
			return text + str;
		}

		public static string doubleLetterCurrency(long exp)
		{
			long num = (exp - (long)BigString.maxExponent) / 3L;
			StringBuilder stringBuilder = new StringBuilder();
			char c = 'a';
			while (true)
			{
				char value = (char)((long)c + num % 26L);
				stringBuilder.Insert(0, value);
				if (stringBuilder.Length >= 2 && num < 26L)
				{
					break;
				}
				num /= 26L;
				if (stringBuilder.Length >= 2)
				{
					num -= 1L;
				}
				c = 'A';
			}
			return stringBuilder.ToString();
		}
	}
}
