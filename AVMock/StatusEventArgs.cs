using AvMock.Enums;

namespace AvMock
{
    public class StatusEventArgsBase : EventArgs
    {
        public DateTime? TimeOfEvent { get; set; }

        public StatusEventArgsBase(DateTime? timeOfEvent)
        {
            TimeOfEvent = timeOfEvent;
        }
    }

    public class OnDemandStatusEventArgs : StatusEventArgsBase
    {
        public OnDemandScanStatuses OldStatus { get; set; }
        public OnDemandScanStatuses NewStatus { get; set; }

        public OnDemandStatusEventArgs(
            DateTime? timeOfEvent,
            OnDemandScanStatuses oldStatus,
            OnDemandScanStatuses newStatus) : base(timeOfEvent)
        {
            OldStatus = oldStatus;
            NewStatus = newStatus;
        }
    }

    public class RealTimeStatusEventArgs : StatusEventArgsBase
    {
        public RealTimeScanStatuses OldStatus { get; set; }
        public RealTimeScanStatuses NewStatus { get; set; }

        public RealTimeStatusEventArgs(
            DateTime? timeOfEvent,
            RealTimeScanStatuses oldStatus,
            RealTimeScanStatuses newStatus) : base(timeOfEvent)
        {
            OldStatus = oldStatus;
            NewStatus = newStatus;
        }
    }
}