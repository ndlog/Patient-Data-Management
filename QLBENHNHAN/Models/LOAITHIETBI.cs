//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QLBENHNHAN.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class LOAITHIETBI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public LOAITHIETBI()
        {
            this.THIETBIs = new HashSet<THIETBI>();
        }
    
        public string MALTB { get; set; }
        public string TENLOAITHIETBI { get; set; }
        public Nullable<System.DateTime> NGAYTAO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<THIETBI> THIETBIs { get; set; }
    }
}
