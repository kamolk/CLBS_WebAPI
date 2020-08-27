using CLBS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLBS.Models
{
    public class CLBS_Results
    {
        public vCLBSData DataResult { get; set; }
        public string StatusCode { get; set; }
        public string StatusDetails { get; set; }

    }
    public class CLBSList_Result
    {
        public List<vCLBSData> DataResult { get; set; }
        public string StatusCode { get; set; }
        public string StatusDetails { get; set; }
        public CLBSList_Result()
        {
            DataResult = new List<vCLBSData>();
        }
    }

    public class CLBSDatalist_Result
    {
        public List<CRUD_CLBSData> DataResult { get; set; }
        public string StatusCode { get; set; }
        public string StatusDetails { get; set; }
        public CLBSDatalist_Result()
        {
            DataResult = new List<CRUD_CLBSData>();
        }
    }
    public class STD_Result
    {
        public vStandardData DataResult { get; set; }
        public string StatusCode { get; set; }
        public string StatusDetails { get; set; }


    }
    public class STDList_Result
    {
        public List<vStandardData> DataResult { get; set; }
        public string StatusCode { get; set; }
        public string StatusDetails { get; set; }
        public STDList_Result()
        {
            DataResult = new List<vStandardData>();
        }
    }

    public class PrismCol_Result
    {
        public vPrismCol DataResult { get; set; }
        public string StatusCode { get; set; }
        public string StatusDetails { get; set; }

    }
    public class PrismColList_Result
    {
        public List<vPrismCol> DataResult { get; set; }
        public string StatusCode { get; set; }
        public string StatusDetails { get; set; }
        public PrismColList_Result()
        {
            DataResult = new List<vPrismCol>();
        }
    }

    public class CRUD_Data
    {
        public string StatusCode { get; set; }
        public string StatusDetails { get; set; }
        public List<string> Dup_data { get; set; }
        public string Prism_data { get; set; }
        public string foldername { get; set; }
        public List<string> No_STD { get; set; }

        public CRUD_Data()
        {
            Dup_data = new List<string>();
            No_STD = new List<string>();
        }
    }
}