namespace web.net.Models;

public enum AppointmentStatus
{
    Scheduled = 0,
    Confirmed = 1,
    InService = 2,
    Completed = 3,
    Cancelled = 4
}

public enum RepairStatus
{
    PendingCheckIn = 0,
    InspectionDiagnostic = 1,
    InProgress = 2,
    WaitingParts = 3,
    QualityCheck = 4,
    ReadyForDelivery = 5,
    Delivered = 6
}

public enum InvoiceStatus
{
    Unpaid = 0,
    PartiallyPaid = 1,
    Paid = 2,
    Cancelled = 3
}
