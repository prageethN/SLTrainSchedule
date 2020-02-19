using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrototypeBeta.DataModels;
namespace PrototypeBeta.ViewModels
{
    public class ScheduleDetailViewModel
    {
        public int startStationId {get; set;}
        public int endStationId { get; set; }
        public string detailHeader { get; set; }
        public string departureTime { get; set; }
        public string startStationName { get; set; }
        public string arrivalTime { get; set; }
        public string endStationName { get; set; }
        public string arrivalTimeEndStation { get; set; }
        public string travelTime { get; set; }
        public string trainFrequncy { get; set; }
        public string finalStationName { get; set; }
        public string arrivalTimeFinalStation { get; set; }
        public string trainType { get; set; }
        public string trainName { get; set; }    
        public string classList { get; set; }
        public ConnetedTrainViewModel connectedTrain { get; set; }
    }
}
