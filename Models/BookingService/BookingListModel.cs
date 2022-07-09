using System;
using System.Collections.Generic;
using Entities;

namespace Models
{
    public class BookingListModel

    {
        public List<BookingEntity> bookings { get; set; }
        public string userId { get; set; }
    }
}