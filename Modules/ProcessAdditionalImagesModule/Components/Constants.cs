﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hotcakes.Modules.ProcessAdditionalImagesModule.Components
{
    public class Constants
    {
        public const string IMPORT_DOWNLOAD_FOLDER_PATH = "import/additional/";
        public const string IMPORT_ADDITIIONAL_FOLDER_PATH = "import/additional/Watermarked/";

        public const string LOCALIZATION_FILE_PATH = "~/DesktopModules/ProcessAdditionalImagesModule/App_LocalResources/View.ascx.resx";

        public const string SETTINGS_SCHEDULER_ENABLED = "Hcc.ImportAdditionalImages.SchedulerEnabled";
        public const string SETTINGS_SCHEDULER_ID = "Hcc.ImportAdditionalImages.ScheduleJobId";

        public const string SCHEDULED_JOB_TYPE = "Hotcakes.Modules.ProcessAdditionalImagesModule.Services.Scheduler.ProcessImagesScheduledJob, Hotcakes.Modules.ProcessAdditionalImagesModule";
        public const string SCHEDULED_JOB_NAME = "Hotcakes Commerce: Import Additional Images";
    }
}