namespace ServiceSaleMachine.Drivers
{
    public enum MessageEndPoint
    {
        Unknown = 0,
        Scaner = 1,
        BillAcceptor = 2,           // событие от купюро приемника
        BillAcceptorCredit = 3,     // событие получения денег
        BillAcceptorReturn = 3,     // событие возврата денег

    }
}