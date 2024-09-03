using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrecisionFullCoilHMI.Models
{
    public class Job
    {

        public short Id { get; set; } 

        [Display(Name = "Job Number")]
        public short JobNumber { get; set; } 
        public short Quantity { get; set; } = 0;
        public float SideA { get; set; } = 0;
        public float SideB { get; set; } = 0;
        public short DuctType { get; set; } = 0;
        public short Lock { get; set; } = 0;
        public short Connector { get; set; } = 0;
        public short Cleats { get; set; } = 0;
        public short CleatEdges { get; set; } = 0;
        public short SideAHoles { get; set; } = 0;
        public short SideBHoles { get; set; } = 0;
        public short HoleDie { get; set; } = 0;
        public short HoleSize { get; set; } = 0;
        public short Bead { get; set; } = 0;
        public short Insulation { get; set; } = 0;
        public short PinSpacing { get; set; } = 0;
        public short Sealant { get; set; } = 0;
        public short Gauge { get; set; } = 0;
        public DateTime LastUpdateDate { get; set; } = DateTime.UtcNow;
        public int RecipeId { get; set; } 
        public Recipe Recipe { get; set; }
        [NotMapped]
        public int CurrentJobNumber { get; set; }
    }
}
