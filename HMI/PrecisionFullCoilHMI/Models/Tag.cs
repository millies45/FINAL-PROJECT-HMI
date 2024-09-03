using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PrecisionFullCoilHMI.Models
{
    public class Tag
    {
        //public int Id { get; set; }
        //public string NodeId { get; set; } = string.Empty; // ns=4;s=V1
        //public string Name { get; set; } = string.Empty; // V1 Identefire from UaExpert
        //public string? Description { get; set; } = string.Empty ;
        //public bool Subscribe { get; set; } = false ;
        //public string? Definetion { get; set; } = string.Empty;// The definition for tags like DO, DI, Maintinance which could be help us in control 

        public int Id { get; set; }
        [MaxLength(100)]
        public string NodeId { get; set; }
        [MaxLength(50)]
        public string Name { get; set; } // Identifire
        [MaxLength(150)]
        public string? Description { get; set; }
        public bool Subscribe { get; set; }
        public int Definition { get; set; } // for the tag itself
        [MaxLength(20)]
        public string? UnitOfMeasurement { get; set; }
        public bool IsArray { get; set; }
        public int? ArrayStart { get; set; }
        public int? ArrayEnd { get; set; }
        public int MaintenancePurpose { get; set; } // New field for maintenance page DO, PB_Maintinance
        [NotMapped]
        public object? Value { get; set; }
    }

    public enum TagDefinition
    {
        Setting = 1,
        Alarm,
        ManualPushButton,
        RealTimeValue,
        Recipe
    }

    public enum MaintenancePurpose
    {
        None = 0,
        DO = 1,
        PB_Maintenance = 2,
    }
}