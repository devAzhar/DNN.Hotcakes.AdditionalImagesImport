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

using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Localization;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Scheduling;
using System;
using Hotcakes.Modules.ProcessAdditionalImagesModule.Components;
using Hotcakes.Modules.ProcessAdditionalImagesModule.Services.Scheduler;

namespace Hotcakes.Modules.ProcessAdditionalImagesModule
{
    public abstract class ProcessAdditionalImagesModuleSettingsBase : ModuleSettingsBase
    {
        #region Properties 
        private bool _schedulerEnabled = false;
        private int _scheduleJobId = Null.NullInteger;
        private ScheduleItem _scheduledJob;

        protected bool SchedulerEnabled
        {
            get
            {
                if (Settings.ContainsKey(Constants.SETTINGS_SCHEDULER_ENABLED))
                {
                    bool.TryParse(Settings[Constants.SETTINGS_SCHEDULER_ENABLED].ToString(), out _schedulerEnabled);
                }
                else
                {
                    _schedulerEnabled = false;
                }

                return _schedulerEnabled;
            }
        }

        protected int ScheduledJobId
        {
            get
            {
                if (Settings.ContainsKey(Constants.SETTINGS_SCHEDULER_ID))
                {
                    int.TryParse(Settings[Constants.SETTINGS_SCHEDULER_ID].ToString(), out _scheduleJobId);
                }
                else
                {
                    _scheduleJobId = Null.NullInteger;
                }

                return _scheduleJobId;
            }
        }

        protected ScheduleItem ScheduledJob
        {
            get
            {
                if(_scheduledJob != null)
                {
                    return _scheduledJob;
                }

                if (ScheduledJobId > Null.NullInteger)
                {
                    var ctlScheduler = new DnnScheduleController();
                    _scheduledJob = ctlScheduler.GetScheduledJob(ScheduledJobId);
                }

                return _scheduledJob;
            }
        }

        protected string HostOnlyMessage
        {
            get
            {
                return GetLocalizedString("HostOnly");
            }
        }
        #endregion

        #region Localization

        protected string GetLocalizedString(string LocalizationKey)
        {
            if (!string.IsNullOrEmpty(LocalizationKey))
            {
                return Localization.GetString(LocalizationKey, this.LocalResourceFile);
            }
            else
            {
                return string.Empty;
            }
        }

        protected string GetLocalizedString(string LocalizationKey, string LocalResourceFilePath)
        {
            if (!string.IsNullOrEmpty(LocalizationKey))
            {
                return Localization.GetString(LocalizationKey, this.LocalResourceFile);
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion
    }
}