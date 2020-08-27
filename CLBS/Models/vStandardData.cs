using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Windows.Forms;

namespace CLBS.Models
{
    public class vStandardData
    {
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
    }

    public class CRUD_StandardData
    {
        [Required]
        public string SurfaceID { get; set; }
        [Required]
        public double SlopeX { get; set; }
        [Required]
        public double SlopeY { get; set; }
        public double constantX { get; set; }
        public double constantY { get; set; }
        [Required]
        public double Base { get; set; }
        [Required]
        public double Addition { get; set; }
        [Required]
        public string Material { get; set; }
        public string Design { get; set; }
        public string CreateBy { get; set; }
    }

    public class STDResult
    {
        public string SurfaceID { get; set; }
        public string Base { get; set; }
        public string Addition { get; set; }
        public string Material { get; set; }
        public STANDARDData STD { get; set; }

    }
}