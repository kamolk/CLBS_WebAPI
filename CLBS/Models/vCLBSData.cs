using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CLBS.Models
{
    public class vCLBSData
    {
        
        public string OPC { get; set; }
        public string Eye { get; set; }

        public string SurfaceID { get; set; }
        public string SlopeX { get; set; }

        public string SlopeY { get; set; }

        public double Base { get; set; }

        public double Addition { get; set; }

        public string Material { get; set; }

        public string Design { get; set; }

        public string CreateBy { get; set; }

        public DateTime? CreateDate { get; set; }
    }


    public class CRUD_CLBSData
    {
        [Required]
        public string OPC { get; set; }
        [Required]
        public string Eye { get; set; }
        [Required]
        public string SurfaceID { get; set; }
    
        public string CreateBy { get; set; }

        public DateTime? CreateDate { get; set; }
    }

    public class CLBS_File
    {
        public string OPC { get; set; }
        public string Eye { get; set; }
        public double Base { get; set; }
        public double Addition { get; set; }
        public string Material { get; set; }
    }

    public class SemiFinishes
    {
        public string cceCode { get; set; }
        public string slopeRx { get; set; }
        public string slopeRy { get; set; }
    }


    public class CLBS_Result
    {
        public List<CLBSData> CLBSlist { get; set; }
        public List<string> NotMatch_STD { get; set; }

        public CLBS_Result()
        {
            CLBSlist = new List<CLBSData>();
            NotMatch_STD = new List<string>();
        
        }
    }       
    
}