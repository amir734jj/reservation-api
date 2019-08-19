using System;
using System.ComponentModel.DataAnnotations;
using Models.Enums;
using Models.Interfaces;

namespace Models.Entities
{
    public class Schedule : IEntity
    {
        [Key]
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public DateTime From { get; set; }
        
        public DateTime To { get; set; }

        public ScheduleTypeEnum ScheduleType { get; set; }
    }
}