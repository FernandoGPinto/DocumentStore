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
    public enum FileVerificationResult
    {
        /// <summary>
        /// An error occured while scanning.
        /// </summary>
        [Description("Error")]
        Error,

        /// <summary>
        /// File not found.
        /// </summary>
        [Description("The file could not be found")]
        FileNotFound,

        /// <summary>
        /// File name contains invalid characters.
        /// </summary>
        [Description("File name contains invalid characters")]
        InvalidCharacters,

        /// <summary>
        /// File check passed.
        /// </summary>
        [Description("File check passed")]
        Passed,

        /// <summary>
        /// File name contains reserved words.
        /// </summary>
        [Description("File name contains reserved words")]
        ReservedWords,

        /// <summary>
        /// The scan timed out.
        /// </summary>
        [Description("Timeout")]
        Timeout,

        /// <summary>
        /// Failed file type check.
        /// </summary>
        [Description("Failed file type check")]
        TypeCheckFailed,

        /// <summary>
        /// Failed virus scan.
        /// </summary>
        [Description("Failed virus scan")]
        VirusScanFailed
    }
}
