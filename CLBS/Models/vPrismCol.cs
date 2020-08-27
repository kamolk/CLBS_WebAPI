using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CLBS.Models
{
    public class vPrismCol
    {
        [Required]
        public string cceCode { get; set; }
        [Required]
        public string slopeRx { get; set; }
        [Required]
        public string slopeRy { get; set; }
    }
}