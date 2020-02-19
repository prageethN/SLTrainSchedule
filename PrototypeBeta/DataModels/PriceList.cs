using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrototypeBeta.DataModels
{
    public class PriceList
    {
        public string className { get; set; }
        public double distanceKM { get; set; }
        public string priceLKR { get; set; }
    }

    public class PRICERESULTS
    {
        public List<PriceList> priceList { get; set; }
    }

    public class RootPriceObject
    {
        public bool SUCCESS { get; set; }
        public string MESSAGE { get; set; }
        public int NOFRESULTS { get; set; }
        public PRICERESULTS RESULTS { get; set; }
        public string STATUSCODE { get; set; }
    }
}
