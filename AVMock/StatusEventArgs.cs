using AvMock.Enums;

namespace AvMock
{
    public class StatusEventArgs : EventArgs
    {
        public DateTime? TimeOfEvent { get; set; }
        public OnDemandScanStatuses NewStatus { get; set; }
        public OnDemandScanStatuses OldStatus { get; set; }

        public StatusEventArgs(DateTime timeOfEvent)
            => TimeOfEvent = timeOfEvent;

        public StatusEventArgs(DateTime timeOfEvent, OnDemandScanStatuses newStatus, OnDemandScanStatuses oldStatus)
        {
            TimeOfEvent = timeOfEvent;
            NewStatus = newStatus;
            OldStatus = oldStatus;
        }
    }
}