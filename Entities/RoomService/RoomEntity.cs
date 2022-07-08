using System.Collections.Generic;

namespace Entities
{
    public class RoomEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int NoOfPeople { get; set; }
        public decimal PriceForNight { get; set; }
        public bool Deleted { get; set; } = false;
        public List<MediaEntity> Photos { get; set; }
    }
}
