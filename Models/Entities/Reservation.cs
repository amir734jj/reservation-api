using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Models.Interfaces;

namespace Models.Entities
{
    public class Reservation : IEntity
    {
        [Key]
        public Guid Id { get; set; }
        
        public User Reserver { get; set; }

        public List<Schedule> Schedules { get; set; }
    }
}