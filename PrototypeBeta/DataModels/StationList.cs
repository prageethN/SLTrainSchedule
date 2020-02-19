using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace PrototypeBeta.DataModels
{
    public class DetailedStationList
    {
        public string stationCode { get; set; }
        public int stationID { get; set; }
        public string stationName { get; set; }
        public int lineOne { get; set; }
        public int lineTwo { get; set; }
        public int lineThree { get; set; }
        public int stationOrder { get; set; }
        public string contactNo { get; set; }
        
    }

    public class DetailedStationListExt 
    {
        public string stationName { get; set; }
        public string expanderTag { get; set; }
        public GeoCoordinateInfo stationGeoCoordinate { get; set; }
        public double fromDistance { get; set; }
        public string fromDistanceDisplay { get; set; }
        public int lineOne { get; set; }
        public bool isExpanded { get; set;} 
    }

    public class DetailedStationListExt2 : DetailedStationList {

        public GeoCoordinateInfo stationGeoCoordinate { get; set; }
        public double fromDistance { get; set; }
    
    }

    public class StationList
    {
        public string stationCode { get; set; }
        public int stationID { get; set; }
        public string stationName { get; set; }
    }

    public class STATIONRESULTS
    {
        public List<StationList> stationList { get; set; }
    }

    public class RootStationObject
    {
        public bool SUCCESS { get; set; }
        public string MESSAGE { get; set; }
        public int NOFRESULTS { get; set; }
        public STATIONRESULTS RESULTS { get; set; }
        public string STATUSCODE { get; set; }
    }

    public class GeoCoordinateInfo : GeoCoordinate
    {
        public string locationName { get; set; }

        public GeoCoordinateInfo(string locName,double locLatitude,double locLongitude) {
            locationName = locName;
            Latitude = locLatitude;
            Longitude = locLongitude;
        }
    }
}
