# rh-cli

**rh-cli** is a simple Windows command line interface for trading stocks on Robinhood. It utilizes a more comprehensive version of [RobinhoodNet](https://github.com/itsff/RobinhoodNet).

Neither this project nor the creator is affiliated with Robinhood Financial LLC.

## Features
- Buy and sell stocks
- View current P/L for stock, amount of shares in active buy/sell orders
- Monitor stocks in realtime 
- More technicial stock details (such as Initial Margin required)
- (More accurate) buying power calculations for Robinhood Gold accounts
- Indicator if stock was recently split
- Support for placing extended hours orders

## Usage

`rh <symbol>` to open information on a particular stock with that symbol and to fill out trade details. You may enter more than one symbol to monitor multiple stocks at one time.

You have several options when entering a single stock:

- Enter an integer to buy that many shares (enter a negative number to sell shares), followed by price you are willing to buy/sell
- Enter `gtc` to have the order persist indefinitely. Enter `gfd` to revert
- Enter `stop` for entering a stop loss order, `stopl` for a stop loss limit order
- Enter `ah` to allow the order to fill during extended hours (9AM-3PM ET)

For best results add the directory with the program to the PATH variable in order to trade anywhere you open a command prompt.

### Other Commands

`rh account` - return some information about your account.

`rh positions` - return all your holdings on RH and a bunch of other useful data.

More commands will be added in the future.
