namespace Chess.Common.Helpers
{
    public enum ClientEvent
    {
        [StringValue("invocationReceived")]
        Invocation,
        [StringValue("invocationAccepted")]
        InvocationAccepted,
        [StringValue("invocationDismissed")]
        InvocationDismissed,
        [StringValue("invocationCanceled")]
        InvocationCanceled,
        [StringValue("moveCommitted")]
        MoveCommitted,
        [StringValue("check")]
        Check,
        [StringValue("mate")]
        Mate,
        [StringValue("resign")]
        Resign,
        [StringValue("draw")]
        Draw
    }
}
