using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrecisionFullCoilHMI.Models
{
    public class DigitalInput
    {
          public int Id { get; set; }
  public string Name { get; set; }  = "";
  public bool Status { get; set; } = false;
    }
}