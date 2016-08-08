// The MIT License (MIT)
// 
// Copyright (c) 2016 Brendan Chan
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

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
        // grab positions
        static void GetAllPositions(Account a)
        {
            // get all positions
            IList<Position> p2;
            IList<string> ps2 = new List<string>();

            try
            {
                p2 = rh.DownloadPositions(a.PositionsUrl.ToString() + "?nonzero=true");
                foreach (Position p3 in p2)
                {
                    ps2.Add(p3.InstrumentUrl.ToString());
                }

                var symbol_list = rh.DownloadQuoteInstrument(ps2).Result;

                // I want to grab: symbol, quantity, average cost, total equity, unrealized p/l
                decimal equity_cur = 0;
                decimal total_pl = 0;

                Console.WriteLine("{0,-10}{1,-10}{2,12}{3,12}{4,12}{5,12}", "Symbol", "Quantity", "Equity", "Avg Buy", "Cur Price", "Current P/L");
                for (int i = 0; i < symbol_list.Count; i++)
                {
                    total_pl += (symbol_list[i].LastTradePrice - p2[i].AverageBuyPrice) * p2[i].Quantity;
                    equity_cur += (p2[i].Quantity * symbol_list[i].LastTradePrice);

                    Console.WriteLine("{0,-10}{1,10}{2,12}{3,12}{4,12}{5,12}",
                        symbol_list[i].Symbol,
                        p2[i].Quantity.ToString("F4"),
                        (p2[i].Quantity * symbol_list[i].LastTradePrice).ToString("F4"),
                        p2[i].AverageBuyPrice,
                        symbol_list[i].LastTradePrice,
                        ((symbol_list[i].LastTradePrice - p2[i].AverageBuyPrice) * p2[i].Quantity).ToString("F2"));

                }


                Console.WriteLine("{0,-10}{1,10}{2,12}{3,12}{4,12}{5,12}", "Total", " ", equity_cur.ToString("F2"), " ", " ", total_pl.ToString("F2"));

                Console.ReadKey();
            }
            catch
            {
                Console.WriteLine("You are currently not in any positions.");
            }
        }
    }
}
