namespace CLBS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("STANDARDData")]
    public partial class STANDARDData
    {
        public STANDARDData()
        {
            CLBSDatas = new HashSet<CLBSData>();
        }

        [Key]
        [StringLength(30)]
        [Required]
        public string SurfaceID { get; set; }
        public string SlopeX { get; set; }
        public string SlopeY { get; set; }
        public string constantX { get; set; }
        public string constantY { get; set; }
        public double Base { get; set; }
        public double Addition { get; set; }
       
        public string Material { get; set; }
        
        public string Design { get; set; }
        public string CreateBy { get; set; }


        //[ForeignKey("CreateBy")]
        //public virtual User User { get; set; }
        public virtual ICollection<CLBSData> CLBSDatas { get; set; }

    }
}
