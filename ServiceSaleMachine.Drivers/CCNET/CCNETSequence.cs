using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceSaleMachine.Drivers
{
    public enum CCNETSequenceEnum
    {
        PowerUpInitSequence,
        EnableSequence,
        BillAcceptingSequence,
        BillReturnedSequence,
        DropCasseteSequence,
    }
}
