
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
        static void DisplayFundamentals(string symbol, DateTime time)
        {
            var instrument_detailed = rh.DownloadFundamentals(symbol).Result;

            Console.WriteLine("Stats: (as of {0:hh:mm tt} GMT)", time);

            try
            {
                Console.WriteLine("Today's Range:     {0}-{1}", instrument_detailed.Low.Value.ToString("F2"), instrument_detailed.High.Value.ToString("F2"));
            }
            catch
            {
                Console.WriteLine("Today's Range:     Unavailable");
            }
            if (instrument_detailed.Low52Weeks != null && instrument_detailed.High52Weeks != null)
                Console.WriteLine("52wk Range:        {0}-{1}", instrument_detailed.Low52Weeks.Value.ToString("F2"), instrument_detailed.High52Weeks.Value.ToString("F2"));
            else
                Console.WriteLine("52wk Range:        Unavailable");
            Console.WriteLine("Intraday Volume:   {0}", Display_ConvertLargeNumber(instrument_detailed.Volume));
            Console.WriteLine("Average Vol (12m): {0}", Display_ConvertLargeNumber(instrument_detailed.AverageVolume));
            Console.WriteLine("Market Cap:        {0}", Display_ConvertLargeNumber(instrument_detailed.MarketCap));
            Console.WriteLine("P/E Ratio:         {0}", Display_ConvertLargeNumber(instrument_detailed.PERatio));
            Console.WriteLine("Dividend Yield:    {0}",
                instrument_detailed.DividendYield > 0 && instrument_detailed.DividendYield != null ? instrument_detailed.DividendYield.Value.ToString("F2") + "%" : "N/A");
            Console.WriteLine();
            if (instrument_detailed.Description != "")
            {
                Console.WriteLine(instrument_detailed.Description);
                Console.WriteLine();
            }
        }
    }
}