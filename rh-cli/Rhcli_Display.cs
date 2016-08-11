
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
    }
}