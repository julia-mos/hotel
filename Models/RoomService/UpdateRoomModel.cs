using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class UpdateRoomModel
    {
        public int Id { get; set; }

        [MinLength(5)]
        [MaxLength(100)]
        public string? Name { get; set; }

        [MinLength(5)]
        [MaxLength(1000)]
        public string? Description { get; set; }

        [Range(1, 100)]
        public int? NoOfPeople { get; set; }

        [Range(1, 10000)]
        public int? Count { get; set; }

        [Range(1, 199999.99)]
        public decimal? PriceForNight { get; set; }
    }
}
