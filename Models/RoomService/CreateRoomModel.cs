﻿using System.ComponentModel.DataAnnotations;
using Entities;

namespace Models
{
    public class CreateRoomModel
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        [Range(1, 100)]
        public int NoOfPeople { get; set; }

        [Required]
        [Range(1, 199999.99)]
        public decimal PriceForNight { get; set; }
    }
}