using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrototypeBeta.Util
{
    class WebService
    {
        private static string wsRailwayStations = "http://m.icta.lk/services/railwayservicev2/station/getAll?lang=en";
        private static string wsTicketPrice = "http://m.icta.lk/services/railwayservicev2/ticket/getPrice?startStationID=SSID&endStationID=ESID&lang=en";
        private static string wsScheduleList = "http://m.icta.lk/services/railwayservicev2/train/searchTrain?startStationID={0}&endStationID={1}&searchDate={2}&startTime={3}&endTime={4}&lang=en";
        private static string wsRouteInformation = "http://dev.virtualearth.net/REST/V1/Routes?wp.0={0},{1}&wp.1={2},{3}&key=Aod0KBLxNUsTKgZmK0tNrzw9crEyGHfg38rQj3Ty01eXJp_eYu46PQv7X4i9ndlx";

        public static string RailwayStations
        {
            get { return wsRailwayStations; }
        }

        public static string TicketPrices { 
              get { return wsTicketPrice; }
        }
        public static string ScheduleList
        {
            get { return wsScheduleList; }
        }
        public static string RouteInformation
        {
            get { return wsRouteInformation; }
        }
    }
}
