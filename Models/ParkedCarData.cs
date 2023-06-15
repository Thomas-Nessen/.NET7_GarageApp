using Microsoft.VisualBasic;

namespace GarageApp.Models
{
    public class ParkedCarData
    {
        public int ID { get; set; }
        public int Name { get; set; }
        public string? CarID { get; set; }
        public string? ParkingSpot { get; set; }
        public string? ParkInd { get; set; }
        public DateAndTime? StartParkTime { get; set; }

        public DateAndTime? EndParTime { get; set; }
    }
}