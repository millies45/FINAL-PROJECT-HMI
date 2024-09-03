using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrecisionFullCoilHMI.Models
{
    public class EncoderInput
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int Value { get; set; } = 0;
    }
}