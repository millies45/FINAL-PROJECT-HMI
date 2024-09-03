using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrecisionFullCoilHMI.Models
{
    public class AnalogOutput
    {
          public int Id { get; set; }
  public string Name { get; set; } = "";
  public double Value { get; set; } = 0;
    }
}