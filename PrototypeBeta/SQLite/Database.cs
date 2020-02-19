using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace PrototypeBeta.SQLite
{
         /// <summary>
        /// RecentSearchTable class representing the RecentSearchTable table.
        /// </summary>
        public sealed class Recent_Search_Table
        {
            [PrimaryKey, AutoIncrement]
            public int RecordId { get; set; }
            public int  startStationId { get; set; }
            public int endStationId { get; set; }
            public string startStationName { get; set; }
            public string endStationName { get; set; }
            public DateTime startTime { get; set; }
            public DateTime endTime { get; set; }
        }

        /// <summary>
        /// FavoriteScheduleTable class representing the FavoriteScheduleTable table.
        /// </summary>
        public sealed class Favorite_Schedule_Table
        {
            [PrimaryKey, AutoIncrement]
            public int RecordId { get; set; }
            public string favoriteName {get; set;}
            public int startStationId { get; set; }
            public int endStationId { get; set; }
            public string startStationName { get; set; }
            public string endStationName { get; set; }
            public DateTime startTime { get; set; }
            public DateTime endTime { get; set; }
        }
    
}
