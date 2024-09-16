using System;

namespace DriveAdviser.Core.Extensions
{
    public static class ValueExtensions
    {
        public static int ToHex(this long value)
        {
            var hexString = value.ToString("X");
            hexString = hexString.Substring(Math.Max(0, hexString.Length - 4));
            var correctedValue = int.Parse(hexString, System.Globalization.NumberStyles.HexNumber);
            return correctedValue;
        }

        public static int ToFahrenheit(this int celsius)
        {
            
            return (int)(9f / 5f * celsius) + 32;
        }
    }
}
