namespace CLBS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public User()
        //{
        //    CLBSDatas = new HashSet<CLBSData>();
        //    STANDARDDatas = new HashSet<STANDARDData>();
        //}

        [Key]
        [StringLength(50)]
        public string ID { get; set; }


        public string Username { get; set; }


        public string FirstName { get; set; }


        public string LastName { get; set; }


        public string UserEmail { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<CLBSData> CLBSDatas { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<STANDARDData> STANDARDDatas { get; set; }
    }
}
