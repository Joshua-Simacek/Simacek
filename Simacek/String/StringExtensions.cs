
namespace Simacek.String
{
    public static partial class StringExtensions
    {
        public static string FormatAsPhoneNumber(this string phoneNumber)
        {
            var length = phoneNumber.Length;
            switch (length)
            {
                case 10:
                    return FormatTenDigitPhoneNumber(phoneNumber);
                default:
                    return null;
            }
        }

        private static string FormatTenDigitPhoneNumber(string number)
        {
            var areaCode = number.Substring(0, 3);
            var exchange = number.Substring(3, 3);
            var suffix = number.Substring(6, 4);

            return $"({areaCode}) {exchange}-{suffix}";
        }
    }
}
