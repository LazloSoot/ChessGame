export enum ClientEvent {
    InvocationReceived = 'invocationReceived',
    InvocationAccepted = 'invocationAccepted',
    InvocationDismissed = 'invocationDismissed',
    InvocationCanceled = 'invocationCanceled',
    MoveCommitted = 'moveCommitted',
    Check = 'check',
    Mate = 'mate',
    Resign = 'resign',
    Draw = 'draw'
}