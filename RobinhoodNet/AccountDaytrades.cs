using System;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace BasicallyMe.RobinhoodNet
{
    public class AccountDaytrades
    {
        public DateTime Date { get; set; }
        public Url<Instrument> Instrument { get; set; }
        public DateTime ExpiryDate { get; set; }
        public IList<string> Orders { get; set; }

        public AccountDaytrades()
        {
            this.Orders = new List<string>();
        }

        internal AccountDaytrades(Newtonsoft.Json.Linq.JToken json) : this()
        {
            this.Date = (DateTime)json["created_at"];
            this.Instrument = new Url<Instrument>((string)json["instrument"]);
            this.ExpiryDate = (DateTime)json["created_at"];

            foreach (var e in (Newtonsoft.Json.Linq.JArray)json["orders"])
            {
                this.Orders.Add((string)e);
            }
        }
    }
}
