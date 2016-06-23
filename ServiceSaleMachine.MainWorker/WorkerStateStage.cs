using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceSaleMachine.MainWorker
{
    public enum WorkerStateStage
    {
        None,
        Init,
        Setting,
        Wait,
        FailRules,
        Rules,
        ChooseService,
        PayService,
        StartService,
        ContinueService,


    }
}
