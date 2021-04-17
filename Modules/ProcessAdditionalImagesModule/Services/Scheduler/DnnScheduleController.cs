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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Instrumentation;
using DotNetNuke.Services.Scheduling;
using Hotcakes.Modules.ProcessAdditionalImagesModule.Components;

namespace Hotcakes.Modules.ProcessAdditionalImagesModule.Services.Scheduler
{
    public class DnnScheduleController
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(DnnScheduleController));

        public ScheduleItem GetScheduledJob(int scheduledJobId)
        {
            Requires.NotNegative("scheduledJobId", scheduledJobId);

            try
            {
                var scheduledJob = SchedulingProvider.Instance().GetSchedule(scheduledJobId);

                return scheduledJob;
            }
            catch(Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw ex;
            }
        }

        public int CreateScheduledJob()
        {
            try
            {
                var scheduleId = Null.NullInteger;
                var scheduledJobs = SchedulingProvider.Instance().GetSchedule();  // retrieves all scheduled jobs 
                var jobList = ConvertArrayList(scheduledJobs);
                var job = jobList.FirstOrDefault(s => s.TypeFullName == Constants.SCHEDULED_JOB_TYPE);

                if (job == null || job.ScheduleID == Null.NullInteger)
                {
                    // the scheduled job doesn't exist yet
                    scheduleId = SchedulingProvider.Instance().AddSchedule(new ScheduleItem
                    {
                        TypeFullName = Constants.SCHEDULED_JOB_TYPE,
                        TimeLapseMeasurement = "h", // hours
                        TimeLapse = 24,
                        RetryTimeLapseMeasurement = "m", // minutes
                        RetryTimeLapse = 30,
                        RetainHistoryNum = 5,
                        FriendlyName = Constants.SCHEDULED_JOB_NAME,
                        Enabled = true,
                        CatchUpEnabled = false
                    });

                    Logger.Debug("Created the scheduled job for Hotcakes Image Import.");
                }
                else
                {
                    // it was manually created, or never saved to module settings for some reason
                    scheduleId = job.ScheduleID;
                }

                return scheduleId;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw ex;
            }
        }

        private List<ScheduleItem> ConvertArrayList(ArrayList jobs)
        {
            var newJobs = new List<ScheduleItem>();

            foreach(var item in jobs)
            {
                var newItem = (ScheduleItem)item;
                newJobs.Add(newItem);
            }

            return newJobs;
        }

        public void DisableScheduledJob(int scheduledJobId)
        {
            Requires.NotNegative("scheduledJobId", scheduledJobId);

            try
            {
                var scheduledJob = GetScheduledJob(scheduledJobId);

                if (scheduledJob != null && scheduledJob.ScheduleID == scheduledJobId)
                {
                    scheduledJob.Enabled = false;
                    DNNScheduler.Instance().UpdateScheduleWithoutExecution(scheduledJob);
                    Logger.Debug("Disabled the Hotcakes Image Import Scheduled Job");
                }
            }
            catch(Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw ex;
            }
        }

        public void EnableScheduledJob(int scheduledJobId)
        {
            Requires.NotNegative("scheduledJobId", scheduledJobId);

            try
            {
                var scheduledJob = GetScheduledJob(scheduledJobId);

                if (scheduledJob != null && scheduledJob.ScheduleID == scheduledJobId)
                {
                    scheduledJob.Enabled = true;
                    DNNScheduler.Instance().UpdateScheduleWithoutExecution(scheduledJob);
                    Logger.Debug("Enabled the Hotcakes Image Import Scheduled Job");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw ex;
            }
        }

        public bool ScheduledJobExists(int scheduledJobId)
        {
            Requires.NotNegative("scheduledJobId", scheduledJobId);

            try
            {
                var scheduledJob = GetScheduledJob(scheduledJobId);

                return (scheduledJob != null && scheduledJob.ScheduleID == scheduledJobId);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw ex;
            }
        }
    }
}