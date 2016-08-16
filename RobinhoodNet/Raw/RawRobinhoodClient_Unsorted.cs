using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace BasicallyMe.RobinhoodNet.Raw
{
    public partial class RawRobinhoodClient
    {
        public Task<JToken>
        DownloadPortfolio(string account)
        {
            string url = PORTFOLIOS_URL + account + "/";
            return doGet(url);
        }

        public Task<JToken>
        DownloadDayTrades(string account)
        {
            string url = ACCOUNTS_URL + account + "/recent_day_trades/";
            return doGet(url);
        }

        public Task<JToken>
        DownloadUserInfo()
        {
            string url = USER_URL;
            return doGet(url);
        }
        
        public Task<JToken>
        DownloadSplits(string splitsUrl = null)
        {
            string url = splitsUrl;
            return doGet(url);
        }

    }
}
