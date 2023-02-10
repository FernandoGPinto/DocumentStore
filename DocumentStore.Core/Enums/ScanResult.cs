using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentStore.Core.Enums
{
    /// <summary>
    /// Result of the file scan.
    /// </summary>
    public enum ScanResult
    {
        /// <summary>
        /// No threat was found.
        /// </summary>
        [Description("No threat found")]
        NoThreatFound,

        /// <summary>
        /// A threat was found.
        /// </summary>
        [Description("Threat found")]
        ThreatFound,

        /// <summary>
        /// File not found.
        /// </summary>
        [Description("The file could not be found")]
        FileNotFound,

        /// <summary>
        /// The scan timed out.
        /// </summary>
        [Description("Timeout")]
        Timeout,

        /// <summary>
        /// An error occured while scanning.
        /// </summary>
        [Description("Error")]
        Error
    }
}
