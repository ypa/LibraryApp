using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stripe;

namespace LibraryApp.Data.ViewModels
{
    public class StripeDashboardVM
    {
        public Balance Balance { get; set; }

        public List<BalanceTransaction> Transactions { get; set; }

        public List<Customer> Customers { get; set; }

        public List<Charge> Charges { get; set; }

        public List<Dispute> Disputes { get; set; }

        public List<Refund> Refunds { get; set; }

        public List<Product> Products { get; set; }
    }
}
