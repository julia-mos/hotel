using System;
using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class BookingEntity
    {
            [Required]
            public int Id { get; set; }

            [Required]
            [DataType(DataType.DateTime)]

            public DateTime DateFrom { get; set; }

            [Required]
            [DataType(DataType.DateTime)]
            public DateTime DateTo { get; set; }

            [Range(0, 100)]
            [Required]
            public int NoOfPeople { get; set; }

            [Required]
            [Range(0, 99999999999.99)]
            public decimal Price { get; set; }

            [Required]
            public RoomEntity Room { get; set; }

            [Required]
            public UserEntity User { get; set; }
    }
}
