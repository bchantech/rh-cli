
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicallyMe.RobinhoodNet;

namespace rh_cli
{
    partial class Program
    {
        static void Display_PriceChange(decimal LastPrice, decimal PrevClose, bool newline = true)
        {
            Console.Write(LastPrice.ToString("C3"));

            decimal close_difference = (LastPrice - PrevClose) / PrevClose;

            if (close_difference >= 0) Console.ForegroundColor = ConsoleColor.Green;
            else Console.ForegroundColor = ConsoleColor.Red;


            Console.Write(" {0}{1}",
                close_difference >= 0 ? "+" : "",
                (LastPrice - PrevClose).ToString("F2"));
            Console.Write(" ({0})", close_difference.ToString("P"));
            Console.ResetColor();
            if (newline) Console.WriteLine();
        }

        // convert a large number to M, B, etc.
        static string Display_ConvertLargeNumber(decimal? number)
        {
            if (number == null)
                return "N/A";

            if (number > 1000000000)
            {
                number /= 1000000000;
                return Decimal.Round((decimal)number,2) + "B";
            }
            if (number > 1000000)
            {
                number /= 1000000;
                return Decimal.Round((decimal)number, 2) + "M";
            }
            if (number > 10000)
            {
                number /= 1000;
                return Decimal.Round((decimal)number, 2) + "K";
            }
            return number.ToString();
        }
    }
}