using PrototypeBeta.SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrototypeBeta.ViewModels
{
    class FavoriteScheduleViewModel
    {
        public int RecordId { get; set; }
        public string favoriteName { get; set; }
        public int startStationId { get; set; }
        public int endStationId { get; set; }
        public string startStationName { get; set; }
        public string endStationName { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public string dayOfWeek{ get; set; }
        public string startTimeStr { get; set; }
        public string endTimeStr { get; set; }
        public Favorite_Schedule_Table fav_sched_obj { get; set; }
    }
}
