using System;
namespace Models
{
    public class MakeBookingModel
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int NumberOfPeople { get; set; }
        public int RoomId { get; set; }
        public string UserId { get; set; }
    }
}
