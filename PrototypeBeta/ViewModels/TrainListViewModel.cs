using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrototypeBeta.DataModels;

namespace PrototypeBeta.ViewModels
{
    public class TrainListViewModel
    {
        public string depatureTime { get; set; }
        public string arrivalTime { get; set; }
        public string duration { get; set; }
        public string trainType { get; set; }
        public string depZone { get; set; }
        public string arriveZone { get; set; }
        public DateTime depTime { get; set; }
        public int trainTypeFlag { get; set; }
        public ScheduleDetailViewModel scheduleDetail { get; set; }
    }
}
