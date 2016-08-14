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

// Things I have yet to finish.
 
namespace rh_cli
{
    partial class Program
    {
        static void GetHelp()
        {
            Console.WriteLine(@"Usage: rh command
If 'command' is not any listed below it is assumed to be a quote

Commands:
<quote>         Bring up a real-time quote, where you can buy and sell shares.
                Append a ? after it to get more info on the stock in question.
<quote,...>     Allows you to monitor multiple stocks in real time, up  to  a
                maximum of 20. You cannot buy and sell in this window.
orders <symbol> Bring up your recent orders for a particular symbol.
account         Brings up some of your basic account information.
positions       Bring up a list of your open positions, with  the  stock  you
                most recently bought for the first time appearing on top.
dividends       Returns dividends from each source, and total amount gained. 
                Ticker symbols marked with a star means you are within  their
                record date but not yet paid.
help            Brings up this text.
interactive     Bring up an interactive console combining real-time data; you 
                can enter app specific commands here.
query <query>   Gives you the top 10 results based on your search  query.  If 
                your query has spaces enclose your search term in quotes.
deposit         Allows you to send transfer money into RH. You will need  the 
                last four digits of the account number. You will be  able  to
                confirm before initating a transfer.
withdraw        Allows you to transfer money from RH into your bank.

");
            
        }

        // Various warnings that I can think of, they may or may not be used in the app 

        void WarningHighVolatility()
        {
            Console.WriteLine("WARNING: This purchase will exceed your daytrading "+
                "buying power. If you are flagged as a pattern day trader before the end "+
                "of the trading day you will be given a day trading margin call for "+
                "the difference.\n\nDo you want to continue?");
        }

        void WarningBuyingUsingDTBP()
        {
            Console.WriteLine("WARNING: You are attempting to buy past your normal buying "+
                "power. As a pattern day trader, you must close these positions before the " +
                "end of the trading day. \n\nDo you want to continue?");
        }

        void WarningNotEnoughCash()
        {
            Console.WriteLine("WARNING: You do not have enough cash but have a " +
                "pending deposit that will cover the cost. If your deposit reverses " +
                "or if you sell this stock before your transfer completes you may " +
                "incur a freeriding violation.\n\nDo you want to continue?");
        }
        void WarningGoodFaith()
        {
            Console.WriteLine("WARNING: You are attempting to buy this stock with "+
                "unsettled funds. Selling this position before funds are settled may "+
                "incur a good faith violation.\n\nDo you want to continue?");
        }

        void WarningBuyLowMaintainence()
        {
            Console.WriteLine("WARNING: Placing this order will bring you very close "+
                "to maintainence margin requirements. This order may be cancelled if your "+
                "equity goes below maintainence requirements.");
        }

        void PromptRedistributeLimit()
        {
            Console.WriteLine("You have open limit sell orders that can prevent you " +
                "from making your trade. Do you want rh-cli to cancel your limit orders "+
                "for this stock?");
        }

    }
}