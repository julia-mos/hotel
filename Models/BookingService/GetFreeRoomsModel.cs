using System;
namespace Models
{
    public class GetFreeRoomsModel
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int NumberOfPeople { get; set; }
    }
}
