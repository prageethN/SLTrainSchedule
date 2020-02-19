using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrototypeBeta.DataModels
{
  public class TRAINQUERY
    {
        public string startStaionName { get; set; }
        public int endStaion { get; set; }
        public string searchTimeEnd { get; set; }
        public string searchDate { get; set; }
        public string endStaionName { get; set; }
        public int startStaion { get; set; }
        public string searchTimeStart { get; set; }
    }

    public class ClassList
    {
        public int classID { get; set; }
        public string className { get; set; }
    }

    public class TrainsList
    {
        public string trainFrequncy { get; set; }
        public string finalStationName { get; set; }
        public List<ClassList> classList { get; set; }
        public int trainID { get; set; }
        public int trainNo { get; set; }
        public string arrivalTime { get; set; }
        public string endStationName { get; set; }
        public string arrivalTimeEndStation { get; set; }
        public string depatureTime { get; set; }
        public string trainType { get; set; }
        public string trainName { get; set; }
        public string startStationName { get; set; }
        public string arrivalTimeFinalStation { get; set; }
    }

    public class DirectTrains
    {
        public List<TrainsList> trainsList { get; set; }
    }

    public class RecordHeader
    {
        public string endCode { get; set; }
        public string startCode { get; set; }
        public string startName { get; set; }
        public int endID { get; set; }
        public string startArrivalTime { get; set; }
        public string endArrivalTime { get; set; }
        public string endName { get; set; }
        public string startDepartureTime { get; set; }
        public int startID { get; set; }
    }

    public class ClassList2
    {
        public int classID { get; set; }
        public string className { get; set; }
    }

    public class ConnectedTrain
    {
        public string trainFrequncy { get; set; }
        public string startTime { get; set; }
        public string selected { get; set; }
        public string startStation { get; set; }
        public List<ClassList2> classList { get; set; }
        public string endStation { get; set; }
        public int isTransit { get; set; }
        public string endTime { get; set; }
        public int trainNo { get; set; }
        public int recID { get; set; }
        public string trainName { get; set; }
    }

    public class TrainsList2
    {
        public List<RecordHeader> recordHeader { get; set; }
        public List<ConnectedTrain> connectedTrains { get; set; }
    }

    public class ConnectingTrains
    {
        public List<TrainsList2> trainsList { get; set; }
    }

    public class TRAINRESULTS
    {
        public DirectTrains directTrains { get; set; }
        public ConnectingTrains connectingTrains { get; set; }
    }

    public class RootTrainObject
    {
        public bool SUCCESS { get; set; }
        public string MESSAGE { get; set; }
        public TRAINQUERY QUERY { get; set; }
        public int NOFRESULTS { get; set; }
        public TRAINRESULTS RESULTS { get; set; }
        public string STATUSCODE { get; set; }
    }
}
