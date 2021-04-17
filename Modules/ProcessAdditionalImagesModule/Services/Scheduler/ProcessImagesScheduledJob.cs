/*
' Copyright (c) 2020 Upendo Ventures, LLC
'  All rights reserved.
' 
' Permission is hereby granted, free of charge, to any person obtaining a copy 
' of this software and associated documentation files (the "Software"), to deal 
' in the Software without restriction, including without limitation the rights 
' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
' copies of the Software, and to permit persons to whom the Software is 
' furnished to do so, subject to the following conditions:
' 
' The above copyright notice and this permission notice shall be included in all 
' copies or substantial portions of the Software.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
' SOFTWARE.
*/

using DotNetNuke.Entities.Controllers;
using DotNetNuke.Instrumentation;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Scheduling;
using System;
using System.Linq;
using Hotcakes.Modules.ProcessAdditionalImagesModule.Services.Data;

namespace Hotcakes.Modules.ProcessAdditionalImagesModule.Services.Scheduler
{
    /// <summary>
    /// A scheduled job that can be managed in DNN.
    /// </summary>
    /// <remarks>
    /// <para>The following values are recommended values for the DNN scheduler:</para>
    /// <para />
    /// 
    /// </remarks>
    public class ProcessImagesScheduledJob : SchedulerClient
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(ProcessImagesScheduledJob));

        /// <summary>
        /// Gets things started...
        /// </summary>
        /// <param name="oItem"></param>
        public ProcessImagesScheduledJob(ScheduleHistoryItem oItem) : base()
        {
            ScheduleHistoryItem = oItem;
        }

        /// <summary>
        /// This method does all of the real work.
        /// </summary>
        public override void DoWork()
        {
            try
            {
                // Perform required items for logging
                Progressing();

                ScheduleHistoryItem.AddLogNote("ProcessImagesScheduledJob Starting");
                Logger.Debug("ProcessImagesScheduledJob Starting");

                // begin...
                var ctlImages = new ImageProcessingController();
                ctlImages.ImportProductImages();
                // end... 

                ScheduleHistoryItem.AddLogNote("ProcessImagesScheduledJob Completed");
                Logger.Debug("ProcessImagesScheduledJob Completed");

                // Show success
                ScheduleHistoryItem.Succeeded = true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                ScheduleHistoryItem.Succeeded = false;
                ScheduleHistoryItem.AddLogNote("Exception:: " + ex.ToString());
                Exceptions.LogException(ex);
            }
        }
    }
}