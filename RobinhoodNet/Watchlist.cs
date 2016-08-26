using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicallyMe.RobinhoodNet
{
    class Watchlist
    {
        Url<Instrument> Instrument { get; set; }
        DateTime CreatedAt { get; set; }
        Url<Watchlist> Url;

        public Watchlist() { }

        internal Watchlist(Newtonsoft.Json.Linq.JToken json) : this()
        {
            Instrument = new Url<Instrument>((string)json["instrument"]);
            CreatedAt = (DateTime)json["created_at"];
            Url = new Url<Watchlist> ((string)json["url"]);
        }
    }
}
