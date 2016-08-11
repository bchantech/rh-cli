
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

            Console.WriteLine("Today's Range: {0}-{1}", instrument_detailed.Low, instrument_detailed.High);
            if (instrument_detailed.Low52Weeks != null && instrument_detailed.High52Weeks != null)
                Console.WriteLine("52wk Range: {0}-{1}", instrument_detailed.Low52Weeks, instrument_detailed.High52Weeks);
            else
                Console.WriteLine("52wk Range: Unavailable");
            Console.WriteLine("Intraday Volume: {0}", instrument_detailed.Volume);
            Console.WriteLine("Average Volume: {0}", instrument_detailed.AverageVolume);
            Console.WriteLine("Market Cap: {0}", instrument_detailed.MarketCap);
            Console.WriteLine("P/E Ratio: {0}", instrument_detailed.PERatio.ToString() ?? "---");
            Console.WriteLine("Dividend Yield: {0}", instrument_detailed.DividendYield);
            Console.WriteLine();
            Console.WriteLine(instrument_detailed.Description);
            Console.WriteLine();
        }
    }
}