using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentStore.Core.Enums
{
    /// <summary>
    /// The enum containing section names.
    /// </summary>
    public enum Sections
    {
        [Description("Company Policies Section")]
        CompanyPolicies = 1,

        [Description("FAQs Section")]
        FAQs = 2,

        [Description("Employee Handbooks Section")]
        EmployeeHandbooks = 3,

        [Description("Legal Documentation Section")]
        LegalDocumentation = 4,

        [Description("System Guidelines Section")]
        SystemGuidelines = 5,

        [Description("System Specifications Section")]
        SystemSpecs = 6
    }
}
