using System.Security.Cryptography;
using System.Text;

namespace DriveAdviser.Helpers
{
    public static class NumberGen
    {
        public static string CreateRandomNumber(int size)
        {
            var chars = new char[62];
            chars =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            var data = new byte[size];
            using (var crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[32];
                crypto.GetNonZeroBytes(data);
            }
            var result = new StringBuilder(size);
            foreach (var b in data)
                result.Append(chars[b%chars.Length]);

            return result.ToString();
        }
    }
}