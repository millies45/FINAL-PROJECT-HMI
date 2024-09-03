using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrecisionFullCoilHMI.Models
{
    public class DigitalOutput
    {
           public int Id { get; set; }
   public string Name { get; set; } = "";
   public bool Status { get; set; } = false;
   public bool JogStatus { get; set; } = false;
    }
}