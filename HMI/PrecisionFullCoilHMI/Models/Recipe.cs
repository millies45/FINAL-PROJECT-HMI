using System.ComponentModel.DataAnnotations;

namespace PrecisionFullCoilHMI.Models
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Name { get; set; } = "New Recipe";
        public string Description { get; set; } = string.Empty;
        public string UserId { get; set; } = "1"; 
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdateDate { get; set; } = DateTime.UtcNow;
        public short NumberOfJobs { get; set; } = 1;

        [Range(1, 1000, ErrorMessage = "Please enter a valid number between 1 and 1000")]
        public ICollection<Job> Jobs { get; set; } = new List<Job>();

        public Recipe()
        {
            Jobs = new HashSet<Job>();
        }
    }
}
