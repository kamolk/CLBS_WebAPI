using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CLBS.Domain
{
    [Table("PrismCol")]
    public partial class PrismCol
    {

            [ Key,ForeignKey("CLBSData")]
            [StringLength(50)]
            [Required]
            public string cceCode { get; set; }
            [Required]
            public string slopeRx { get; set; }
            [Required]
            public string slopeRy { get; set; }


            public virtual CLBSData CLBSData { get; set; }
        
    }
}