using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentStore.Core.Utilities
{
    public static class FileUtil
    {
        /// <summary>
        /// Calls javascript function saveAsFile (helpers.js) which enables the use of the browser's file download tools.
        /// </summary>
        /// <param name="js"></param>
        /// <param name="filename"></param>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        public async static Task SaveAs(IJSRuntime js, string filename, byte[] fileStream)
        {
            await js.InvokeAsync<object>("saveAsFile", filename, fileStream);
        }
    }
}
