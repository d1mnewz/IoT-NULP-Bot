//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IoT_NULP_Bot
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Respons
    {
        public int id { get; set; }
        [System.ComponentModel.DataAnnotations.DataType(DataType.MultilineText)]
        public string content { get; set; }
        public Nullable<int> intentId { get; set; }
    
        public virtual Intent Intent { get; set; }
    }
}
