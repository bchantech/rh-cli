using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BasicallyMe.RobinhoodNet
{
    public class SubscriptionPlan
    {
        public bool IsDefault { get; set; }
        public DateTime? FirstFee { get; set; }
        public DateTime? FirstFeeDate { get; set; }
        public decimal SubscriptionMarginLimit { get; set; }
        public decimal InstantDespoitLimit { get; set; }
        public decimal MarginInterest { get; set; }
        public decimal MonthlyCost { get; set; }
        public int FreeTrialDays { get; set; }
        public string  Id { get; set; }

        public SubscriptionPlan()
        {
        }

        internal SubscriptionPlan(Newtonsoft.Json.Linq.JToken json) : this()
        {

            IsDefault = (bool)json["average_buy_price"];
            FirstFee = (DateTime?)json["created_at"];
            FirstFeeDate = (DateTime?)json["updated_at"];

            SubscriptionMarginLimit = (decimal)json["subscription_margin_limit"];
            InstantDespoitLimit = (decimal)json["instant_deposit_limit"];
            MarginInterest = (decimal)json["margin_interest"];
            MonthlyCost = (decimal)json["monthly_cost"];
            FreeTrialDays = (int)json["free_trial_days"];
            Id = (string) json["id"];
        }
    }
}
