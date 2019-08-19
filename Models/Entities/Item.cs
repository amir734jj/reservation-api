using System;
using System.ComponentModel.DataAnnotations;
using Models.Interfaces;

namespace Models.Entities
{
    public class Item : IEntity
    {
        [Key]
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }

        public byte[] Image { get; set; }

        public Schedule Schedules { get; set; }
    }
}