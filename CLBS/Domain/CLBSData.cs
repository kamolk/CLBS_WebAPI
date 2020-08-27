using CLBS.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace CLBS
{
    [Table("CLBSData")]
    public partial class CLBSData
    {
        public CLBSData()
        {
            PrismCol = new PrismCol();
        }

        [Key]
        [StringLength(50)]
        public string OPC { get; set; }
        public string Eye { get; set; }
        // [Required]
        //[StringLength(30)]
        public string SurfaceID { get; set; }
        public string CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }

        [ForeignKey("SurfaceID")]
        public virtual STANDARDData STANDARDData { get; set; }


        //[ForeignKey("CreateBy")]
        //public virtual User User { get; set; }

        public virtual PrismCol PrismCol { get; set; }

    }

}

