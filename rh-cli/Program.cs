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
        protected static int orderRow;
        protected static int orderCol;
        protected static int quoteRow;
        protected static int quoteCol;
        protected static RobinhoodClient rh;
        protected static bool returnfundamentals = false;


        static void Main(string[] args)
        {
            rh = new RobinhoodClient();
            // Get usage if nothing entered
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: 'rh <symbol>' or type in 'rh help' on available commands");
                Environment.Exit(0);
            }
            else if (args.Length == 1 && (args[0] == "help" || args[0] == "?"))
            {
                GetHelp();
                Environment.Exit(0);
            }

            // Authenticate the user
            if (!Auth(rh))
            {
                Console.WriteLine("Unable to authenticate user.");
                Environment.Exit(1);
            }
            
            // Establish instrument

            Instrument instrument = new Instrument();
            
            Account account = rh.DownloadAllAccounts().Result.First();
            AccountPortfolio ap = rh.DownloadSinglePortfolio(account.AccountNumber).Result;
            Quote qqq = null;

            if (args.Length == 1)
            {
                if (args[0] == "positions")
                {
                    GetAllPositions(account);
                    Environment.Exit(0);
                }
                else if (args[0] == "account")
                {
                    User u = rh.DownloadUserInfo().Result;
                    Console.WriteLine("Current info for: {0} {1} ({2}):", u.FirstName, u.LastName, u.Username );
                    Console.WriteLine("Established: {0:yyyy MMM dd}", u.CreatedAt);
                    Console.Write("Account Type: ");
                    if (account.AccountType == "cash")
                    {
                        Console.WriteLine("Cash Account");
                    }
                    else
                    {
                        if (account.MarginBalance.MarginLimit > 0)
                        {
                            Console.WriteLine("Gold Account ({0} Extra Buying Power)", account.MarginBalance.MarginLimit.ToString("C0"));
                        }
                        else
                        {
                            Console.WriteLine("Instant Account");
                        }
                    
                    }
                    Console.WriteLine("Max Gold Extra Buying Power: {0}", ((account.Sma+account.UnclearedDeposits)*2-account.EffectiveCash).Value.ToString("C2"));
                    Console.WriteLine("Cash on Hand: {0}", account.Cash.ToString("C2"));
                    Console.WriteLine("Total value of all securities held: {0}", ap.MarketValue.ToString("C2"));
                    Console.Write("Account Equity: ");
                    Display_PriceChange(ap.Equity, ap.AdjustedEquityPreviousClose);
                    Console.Write("Maintainence Level: {0}", (ap.Equity - ap.ExcessMargin).ToString("C2"));
                    Console.WriteLine();
                    Environment.Exit(0);
                }
                else
                {
                    symbol = args[0];
                    if (symbol.Last() == '?')
                        returnfundamentals = true;
                    symbol = symbol.TrimEnd('?');
                    symbol = symbol.ToUpperInvariant();
                }
            }
            else
            {
                // Query multiple symbols.
                args = args.Select(s => s.ToUpperInvariant()).ToArray();
                if (args.Length > 50)
                {
                    Console.WriteLine("More than 50 stocks entered, will monitor only the first 50.");
                    Array.Resize(ref args, 50);
                }
                Console.WriteLine("Continuously monitoring entered stocks, CTRL+C to exit");
                Console.CursorVisible = false;
                quoteRow = Console.CursorTop;
                quoteCol = Console.CursorLeft;
                bool looped_quotes = false;
                while (true)
                {
                    try
                    {
                        var quotes = rh.DownloadQuote(args).Result;
                        looped_quotes = true;
                        foreach (Quote q in quotes)
                        {
                            if (q == null) continue;

                            Console.Write("{0,-6}", q.Symbol);
                            Display_PriceChange(q.LastTradePrice, q.PreviousClose, false);
                            Console.WriteLine(" {0:hh:mm:ss tt} GMT", q.UpdatedAt);
                        }

                        // Set delay based on how many stocks we are watching at once
                        if (args.Length < 10)
                            System.Threading.Thread.Sleep(1000);
                        else if (args.Length < 25)
                            System.Threading.Thread.Sleep(3000);
                        else
                            System.Threading.Thread.Sleep(5000);

                        Console.SetCursorPosition(quoteCol, quoteRow);
                    }
                    catch
                    {
                        if (!looped_quotes)
                        {
                            Console.WriteLine("None of the symbols entered were found.");
                            Environment.Exit(1);
                        }
                        else
                        {
                            // If the initial quote loaded, but connection lost or throttled when refreshing,
                            // delay by 15s instead until that ends.
                            System.Threading.Thread.Sleep(15000);
                        }
                    }
                }
            }

            try
            {
                // will return a list, but return the first result of a filter
                instrument = rh.FindInstrument(symbol).Result.First(i => i.Symbol == symbol);
            }
            catch
            {
                Console.WriteLine("Symbol '{0}' was not found.", symbol);
                Environment.Exit(1);
            }

            // check if there is actually an account to trade
            try
            {
                account = rh.DownloadAllAccounts().Result.First();
            }
            catch
            {
                Console.WriteLine("No trading accounts are associated with this login.");
                Environment.Exit(1);
            }

            Console.WriteLine(instrument.Name + " (" + instrument.Symbol + ")");
            qqq = rh.DownloadQuote(symbol).Result;

            if (returnfundamentals)
                DisplayFundamentals(symbol, qqq.UpdatedAt);
            
            // Check for stock split
            try
            {
                var split_check = rh.DownloadAllInstrumentSplits(instrument.InstrumentSplitsUrl.ToString()).First();
                TimeSpan t2 = qqq.UpdatedAt - split_check.Execution_Date;
                if (t2.Days <= 14)
                {
                    Console.WriteLine("NOTE: This stock underwent a {0} {1}stock split on {2:MMM d, yyyy}",
                        split_check.Divisor.ToString("N0") + " for " + split_check.Multiplier.ToString("N0"),
                        split_check.Divisor > split_check.Multiplier ? "reverse " : "",
                        split_check.Execution_Date);
                }
            }
            catch
            {
            }
            
            Console.Write("Last trade price: ");
            quoteRow = Console.CursorTop;
            quoteCol = Console.CursorLeft;

            Display_PriceChange(qqq.LastTradePrice, qqq.PreviousClose);

            Console.Write("Initial / Maintainence Margin Requirement: ");

            if (instrument.MarginInitialRatio > 0.5m)
                Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(instrument.MarginInitialRatio.ToString("P"));
            Console.ResetColor();
            Console.Write(" / ");

            if (instrument.MaintenanceRatio > 0.25m)
                Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(instrument.MaintenanceRatio.ToString("P"));
            Console.ResetColor();

            if (!instrument.IsTradeable)
            {
                Console.WriteLine("You cannot trade this stock on RH.");
            }
            else
            {
                if (qqq.TradingHalted)
                {
                    Console.WriteLine("This stock is currently halted; orders made will execute when it resumes.");
                }
            }

            // max the initial margin and maintainence margin on RH, should they be below regulation requirements
            instrument.MarginInitialRatio = Math.Max(instrument.MarginInitialRatio, 0.50m);
            instrument.MaintenanceRatio = Math.Max(instrument.MaintenanceRatio, 0.25m);
            if (instrument.MaintenanceRatio > instrument.MarginInitialRatio) instrument.MarginInitialRatio = instrument.MaintenanceRatio;
            
            int qty = 0;
            Console.WriteLine("You currently have:");
            Console.WriteLine(" - ${0,11} Cash Balance", account.Cash.ToString("N2"));

            if (account.AccountType == "cash")
            {
                Console.WriteLine(" - ${0,11} Unsettled Funds", account.UnsettledFunds.ToString("N2"));
            }
            else
            {
                Console.WriteLine(" - ${0,11} Margin Buying Power", (account.EffectiveCash + account.MarginBalance.MarginLimit).ToString("N2"));
            }

            if (account.CashHeldForOrders > 0)
                Console.WriteLine(" - ${0,11} Cash held in Buy orders ", account.CashHeldForOrders.ToString("N2"));

            // actually SMA
            //Console.WriteLine(" - ${0,11} Buying Power", account.BuyingPower.ToString("N2"));

            if (account.DTBP != null)
                Console.WriteLine(" - ${0,11} Intraday Buying Power {1}",
                    (account.DTBP.Value / instrument.DayTradeRatio).ToString("N2"),
                    instrument.DayTradeRatio >= 1.00m ? "(Day Trading Ineligible)" :
                    instrument.DayTradeRatio >= 0.75m ? "(3x Leveraged ETF)" :
                    instrument.DayTradeRatio >= 0.50m ? "(2x Leveraged ETF)" : "");

            // get max
            // restrict if equity is under 2000 
            // we assume that if your equity is under 2000, your excess margin is as well.
            if (account.MarginBalance.MarginLimit > 0)
            {
                if (ap.Equity < 2000)
                {
                    Console.WriteLine("You can only purchase up to {0} of this stock as you are below the $2,000 minimum to open a leveraged position.",
                        (Math.Min(0,(account.EffectiveCash)).ToString("C2")));
                }

                else if (ap.ExcessMargin < account.EffectiveCash + account.MarginBalance.MarginLimit)
                {
                    Console.WriteLine("You can only purchase up to {0} of this stock due to initial margin requirements.",
                        (ap.ExcessMargin / instrument.MarginInitialRatio).ToString("C2"));
                }
                else
                {
                    Console.WriteLine("You may utilize your full extra buying power to purchase this stock.");
                }
            }

            // if margin call

            if (ap.ExcessMargin < 0)
            {
                Console.WriteLine("Your account is undergoing a margin call for {0} and cannot open positions; you will need to liquidate {1} of this stock to cover this margin call.",
                (Math.Abs(ap.ExcessMargin).ToString("C2")),
                (Math.Abs(ap.ExcessMargin) / instrument.MaintenanceRatio).ToString("C2"));
            }

            // get close only
            if (account.OnlyPositionClosingTrades)
            {
                Console.WriteLine("Your account is restricted and can only close existing positions.");
            }
            if (account.IsDeactivated)
            {
                Console.WriteLine("Your account is deactivated and cannot buy or sell stocks.");
            }

            // get the current position
            Position pp = null;
            try
            {
                pp = rh.DownloadSinglePosition(account.AccountNumber, instrument.InstrumentId).Result;

                Console.WriteLine("You currently have:");
                if (pp.Quantity > 0)
                {
                    // todo: Currency to four places when under 1 dollar
                    Console.WriteLine(" - {0,12} shares available to sell (AVG: {1}, P/L {2})", 
                        pp.Quantity.ToString("N2"), 
                        pp.AverageBuyPrice.ToString("C"), 
                        ((qqq.LastTradePrice - pp.AverageBuyPrice) * pp.Quantity).ToString("F2"));
                }
                else if (pp.Quantity == 0)
                {
                    Console.WriteLine(" - No position in this stock");
                }
                else
                {
                    Console.WriteLine(" - {0,12} Shares sold short", Math.Abs(pp.Quantity).ToString("N2"));
                }


                Console.WriteLine(" - {0,12} Shares in outstanding sell orders", pp.SharesHeldForSells.ToString("N2"));
                Console.WriteLine(" - {0,12} Shares in outstanding buy orders", pp.SharesHeldForBuys.ToString("N2"));
            }
            catch
            {
                Console.WriteLine("You do not own any shares in this stock.");
            }

            Console.WriteLine("Max you can buy: " + account.GetBuyableShares(qqq.LastTradePrice * 1.05m));
            if (pp != null)
                Console.WriteLine("Max you can sell: " + Math.Max(pp.Quantity - pp.SharesHeldForSells, 0).ToString("F0"));

            var t = Task.Run(() => UpdateQuote());
            TimeInForce tif = TimeInForce.GoodForDay;
            bool stoploss = false;
            bool stoplimit = false;
            bool afterhours = false;


            while (true)
            {

                Console.Write("Quantity to buy (negative for sell) or set a order option: ");

                string q = Console.ReadLine();
                q = q.ToLowerInvariant();

                if (q.Equals("GFD", StringComparison.InvariantCultureIgnoreCase))
                {
                    Console.WriteLine("Placing a Good For Day order.");
                    tif = TimeInForce.GoodForDay;
                }
                else if (q.Equals("GTC", StringComparison.InvariantCultureIgnoreCase))
                {
                    Console.WriteLine("Placing a Good Til Canceled order.");
                    tif = TimeInForce.GoodTillCancel;
                }
                else if (q.Equals("STOP", StringComparison.InvariantCultureIgnoreCase))
                {
                    Console.WriteLine("Placing a {0} Stop Loss order.", tif == TimeInForce.GoodForDay ? "Good for Day" : "Good Til Canceled");

                    stoploss = true;
                    stoplimit = false;
                }
                else if (q.Equals("STOPL", StringComparison.InvariantCultureIgnoreCase))
                {
                    Console.WriteLine("Placing a {0} Stop Limit order.", tif == TimeInForce.GoodForDay ? "Good for Day" : "Good Til Canceled");
                    stoploss = false;
                    stoplimit = true;
                }
                else if (q.Equals("AH", StringComparison.InvariantCultureIgnoreCase))
                {
                    Console.WriteLine("Placing an after-hours expiry order (Please ensure your account level supports this order).");
                    afterhours = true;
                }

                if (Int32.TryParse(q, out qty))
                {
                    break;
                }
            }

            var newOrderSingle = new NewOrderSingle(instrument);

            decimal price = 0m;
            while (true && !(stoploss || stoplimit))
            {
                Console.Write("Limit price (0 applies a 5% collared Market order): ");
                string p = Console.ReadLine();
                if (Decimal.TryParse(p, out price))
                {
                    if (price < 0)
                    {
                        Console.WriteLine("Invalid price entered.");
                        continue;
                    }

                    // warn if way lower than limit price.
                    if (price / qqq.LastTradePrice < 0.6m)
                    {
                        Console.Write("NOTE: Your limit price is way below last trade price. Press C to confirm, anything else to re-enter. ");
                        var x = Console.ReadKey();
                        if (x.KeyChar == 'c' || x.KeyChar == 'C')
                        {
                            Console.WriteLine();
                            break;
                        }
                        Console.WriteLine();
                        continue;
                    }
                    break;
                }
            }
            
            while (stoploss || stoplimit)
            {
                Console.Write("Stop Price{0}: ", stoploss ? " (Converts to a 5% collared Market order when triggered)" : "" );
                string p = Console.ReadLine();
                if (Decimal.TryParse(p, out price))
                {
                    if (price <= 0)
                    {
                        Console.WriteLine("Invalid price entered.");
                        continue;
                    }

                    newOrderSingle.StopPrice = price;
                    newOrderSingle.Trigger = TriggerType.Stop;

                    // price is zero to trigger the next loop
                    if (stoploss) price = 0;

                    break;
                }
            }

            newOrderSingle.AccountUrl = account.AccountUrl;
            newOrderSingle.Quantity = Math.Abs(qty);
            newOrderSingle.Side = qty > 0 ? Side.Buy : Side.Sell;

            newOrderSingle.TimeInForce = tif;
            if (price == 0)
            {
                price = qqq.LastTradePrice;

                // restore the price back to its entered price if stop
                if (stoploss) price = newOrderSingle.StopPrice.Value;

                newOrderSingle.OrderType = OrderType.Market;
                if (newOrderSingle.Side == Side.Buy)
                {
                    newOrderSingle.Price = price * 1.05m;
                }
                else
                {
                    newOrderSingle.Price = price * 0.75m;
                }
            }
            else
            {
                newOrderSingle.OrderType = OrderType.Limit;
                newOrderSingle.Price = price;
            }

            // afterhours flag
            if (afterhours) newOrderSingle.ExtendedHours = true;
            string order_id = "";

            try
            {
                var order = rh.PlaceOrder(newOrderSingle).Result;
                Console.WriteLine("{0,-10}{1,-6} {2,5} x {3,-9}{4,-20}",
                                    order.Side,
                                    instrument.Symbol,
                                    order.Quantity,
                                    newOrderSingle.OrderType == OrderType.Limit ? order.Price.Value.ToString("F4") : "mkt",
                                    order.State);
                order_id = order.OrderId;
            }
            catch
            {
                Console.WriteLine("ERROR: There was a problem with your order; your order has not been submitted.");
                Environment.Exit(2);
            }
            
            System.Threading.Thread.Sleep(150);
            // update to see what happened
            var order_update = rh.DownloadSingleOrder(order_id).Result;
            while (order_update.State == "unconfirmed")
            {
                System.Threading.Thread.Sleep(100);
                order_update = rh.DownloadSingleOrder(order_id).Result;
            }

            Console.WriteLine("{0,-10}{1,-6} {2,5} x {3,-9}{4,-20}",
                                order_update.Side,
                                instrument.Symbol,
                                order_update.Quantity,
                                order_update.AveragePrice.HasValue ? order_update.AveragePrice.Value.ToString("F4") : order_update.Price.Value.ToString("F4"),
                                order_update.State);

            if (!String.IsNullOrEmpty(order_update.RejectReason))
            {
                Console.WriteLine("Reject Reason: " + order_update.RejectReason);
            }
            else if (order_update.CancelUrl != null)
            {
                // display SL trigger price
                if (stoploss || stoplimit)
                    Console.WriteLine("This order will trigger when the price hits {0}.", order_update.StopPrice.Value.ToString("C2"));

                Console.WriteLine("Press C to cancel this order, or anything else to quit");
                var x = Console.ReadKey();
                if (x.KeyChar == 'c' || x.KeyChar == 'C')
                {
                    rh.CancelOrder(order_update.CancelUrl).Wait();
                    Console.WriteLine("Cancelled");
                }
            }
            else
            {
                if (order_update.AveragePrice.HasValue)
                {
                    Console.WriteLine("Total Notional: " + order_update.TotalNotional.ToString("C"));
                }
                if (order_update.Fees > 0)
                {
                    Console.WriteLine("Total fees from this transaction: " + order_update.Fees.ToString("C"));
                }

                // check if the account was cash, and warn if reserved buy > cash)
                if (account.AccountType == "cash" && account.CashHeldForOrders > account.Cash)
                {
                    if (order_update.State == "filled" )
                        WarningCashViolation(true);
                    else
                    {
                        WarningCashViolation(false);
                    }
                }
                
            }

        }

        static Task UpdateQuote()
        {
            while (true)
            {
                // do not download realtime quote data if the buffer is full
                if (quoteRow >= Console.BufferHeight - 1)
                {

                    System.Threading.Thread.Sleep(60000);
                    continue;
                }

                // place action to get quote here.
                Quote q4 = rh.DownloadQuote(symbol).Result;

                // todo: Offset by -3 if not daylight savings time
                DateTime q4_time = q4.UpdatedAt.AddHours(-4);

                orderRow = Console.CursorTop;
                orderCol = Console.CursorLeft;

                Console.SetCursorPosition(quoteCol, quoteRow);
                
                // if extended hours update slower
                if (q4.LastExtendedHoursTradePrice != null)
                {
                    Display_PriceChange(q4.LastExtendedHoursTradePrice.Value, q4.PreviousClose, false);
                    Console.Write(" (After Hours)");
                    Console.SetCursorPosition(orderCol, orderRow);
                    System.Threading.Thread.Sleep(60000);
                }
                else
                {
                    Display_PriceChange(q4.LastTradePrice, q4.PreviousClose, false);
                    Console.Write(" {0:hh:mm:ss tt} EDT", q4_time);
                    Console.SetCursorPosition(orderCol, orderRow);
                    System.Threading.Thread.Sleep(1000);
                }
                
            }
        }
    }


}

