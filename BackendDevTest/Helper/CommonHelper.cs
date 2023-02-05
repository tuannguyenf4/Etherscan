using System.Configuration;

namespace BackendDevTest.Helper
{
    public static class CommonHelper
    {
        public static string EtherscanBaseAddress = ConfigurationManager.AppSettings["EtherScanAPI"];
        public static string APIKey = ConfigurationManager.AppSettings["APIKey"];
        public static int BlockFrom = Convert.ToInt32(ConfigurationManager.AppSettings["BlockFrom"]);
        public static int BlockTo = Convert.ToInt32(ConfigurationManager.AppSettings["BlockTo"]);
        public static string ConnectionString = ConfigurationManager.AppSettings["Connection"];

        public static string ConvertNumberToHEX(string number)
        {
            if (!string.IsNullOrEmpty(number) && number.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                number = number.Substring(2);
                int value = int.Parse(number, System.Globalization.NumberStyles.HexNumber);
                return value.ToString();
            }
            return number;
        }

        public static string ConvertHEXStringToDecimal(string number)
        {
            if (!string.IsNullOrEmpty(number) && number.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                number = number.Substring(2);
                long value = Convert.ToInt64(number, 16);
                return value.ToString();
            }
            return number;
        }
       
        public static string RemoveHEXString(string number)
        {
            if (!string.IsNullOrEmpty(number) && number.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                number = number.Substring(2);
            }
            return number;
        }

        public static string FormatDateTimeToLongString()
        {
            return DateTime.Now.ToString("MM/dd/yyyy h:mm tt");
        }

        public static string FormatDateTimeToShortString()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }
    }
}
