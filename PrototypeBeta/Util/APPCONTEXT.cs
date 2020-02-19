using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrototypeBeta.Util
{
    class APPCONTEXT
    {
        public static string globalConnectedTrain = "Connected";
        public static string globalTicketPrice = "Rs {0}";

        public static string sheduleListHeader = "{0} to {1}";
        public static string sheduleListTimePeriod = "from {0} {1} to {2} {3}";
        public static string sheduleListDuration = "{0} h";
        public static string scheduleListPromptTitle = "Favorite Name";
        public static string scheduleListPromptMessage = "Please name your favorote schedule";

        public static string sceduleDetailTimeHeader = "{0} at {1} {2}";
        public static string sceduleDetailTime = "{0} {1}";
        public static string sceduleDetailDistance= "{0} KM";

        public static DateTime defaultStartTime = DateTime.ParseExact("00:00:00", "HH:mm:ss",
                                        CultureInfo.InvariantCulture);

        public static DateTime defaultEndTime = DateTime.ParseExact("23:59:59", "HH:mm:ss",
                                        CultureInfo.InvariantCulture);

        public static int MAX_RECENT_SEARCH = 20;

        public static string recentSearchListPromptTitle = "Delete Recent Searches";
        public static string recentSearchListPromptMessage = "Are you sure you want clear recent search list?";

        public static string favoriteScheduleListPromptTitle = "Delete Favorite Schedules";
        public static string favoriteScheduleListPromptMessage = "Are you sure you want clear favorite schedule list?";

        public static string ticketPriceShareMessage = "Ticket price from {0} to {1}\n 1st Class - {2}\n 2nd Class - {3}\n 3rd Class - {4}";

        public static string scheduleDetailShareMessage = "Schedule {0} to {1}\n Dep : {2}\n Arr : {3}\n Dur : {4}\n Freq: {5}";
        public static string scheduleDetailReservationTitle = "Call Mobitel Ticketing service";
        public static string scheduleDetailReservationContactName = "Mobitel Ticketing service";
        public static string scheduleDetailReservationNo= "365";

        public static string stationListNumberNotAvailableMsg = "Not Available";
        public static string stationListStationName = "{0} Station";

        public static string nearByStationCountSummary = "{0} station(s) found within a {1} km radious";
    }
}
