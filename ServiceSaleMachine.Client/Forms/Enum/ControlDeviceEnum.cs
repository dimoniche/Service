using System.ComponentModel;

namespace AirVitamin.Client
{
    public enum ControlDeviceEnum
    {
        MixBefore = 1,
        MixAfter = 2,

        [Description("Шланг")]
        Pipe,
        [Description("Мундштук")]
        Holder,
        [Description("Мусор")]
        Garbage
    }
}