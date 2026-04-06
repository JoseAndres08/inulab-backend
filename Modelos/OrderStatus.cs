namespace BackendLimpio.Models
{
    public enum OrderStatus
    {
        Pending,
        Assigned,
        MotoEnCamino,
        MotoArrived,
        PickupInProgress,
        SampleReceived,
        ArrivedAtLab,
        Processing,
        ResultsUploaded,
        Completed
    }
}