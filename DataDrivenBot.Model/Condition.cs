//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataDrivenBot.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Condition : Step
    {
        public string Expression { get; set; }
        public Nullable<int> NextStepIDWhenTrue { get; set; }
        public Nullable<int> NextStepIDWhenFalse { get; set; }
    
        public virtual Step NextStepWhenFalse { get; set; }
        public virtual Step NextStepWhenTrue { get; set; }
    }
}
