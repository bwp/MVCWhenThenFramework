using System;
using System.Text;

namespace MVCWhenThenFramework
{
    public static class IpConverter
    {
        public static string ConvertLongToIP(long ipLong)
        {
            StringBuilder b = new StringBuilder();
            long tempLong, temp;

            tempLong = ipLong;
            temp = tempLong / (256 * 256 * 256);
            tempLong = tempLong - (temp * 256 * 256 * 256);
            b.Append(Convert.ToString(temp)).Append(".");
            temp = tempLong / (256 * 256);
            tempLong = tempLong - (temp * 256 * 256);
            b.Append(Convert.ToString(temp)).Append(".");
            temp = tempLong / 256;
            tempLong = tempLong - (temp * 256);
            b.Append(Convert.ToString(temp)).Append(".");
            temp = tempLong;
            tempLong = tempLong - temp;
            b.Append(Convert.ToString(temp));

            return b.ToString().ToLower();
        }

        public static long ConvertIPToLong(string ipAddress)
        {
            System.Net.IPAddress ip;

            if (System.Net.IPAddress.TryParse(ipAddress, out ip))
            {
                byte[] bytes = ip.GetAddressBytes();

                return (long)(((long)bytes[0] << 24) | (bytes[1] << 16) |
                    (bytes[2] << 8) | bytes[3]);
            }

            return 0;
        }
    }
}
