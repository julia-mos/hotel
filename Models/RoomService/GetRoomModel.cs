using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Entities;
using Microsoft.AspNetCore.Http;

namespace Models
{
    public class GetRoomModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int NoOfPeople { get; set; }

        public decimal PriceForNight { get; set; }

        public int Count { get; set; }

        public List<string> Photos { get; set; }
    }
}
