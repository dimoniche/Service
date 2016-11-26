namespace ServiceSaleMachine.Drivers
{
    public enum DeviceEvent
    {
        Unknown = 0,
        Scaner = 1,                             // событие считывания кода
        BillAcceptor = 2,                       // событие от купюро приемника
        BillAcceptorCredit = 3,                 // событие получения денег
        BillAcceptorReturn = 4,                 // событие возврата денег
        DropCassetteBillAcceptor = 5,           // событие выемки денег
        DropCassetteFullBillAcceptor = 6,       // событие переполнения касеты с деньгами
        BillAcceptorError = 7,                  // у приемника проблемы
        returnBillAcceptor = 8,                 // возврат купюры

        NoCOMPort = 9,                          // нет ком портов
        NeedSettingProgram = 10,                // Необходима настройка приложения
        BadSetting = 11,                        // плохие настройки
        InitializationOK = 12,                  // инициализация прошла успешно
        BillAcceptorEscrow = 13,                // событие задержки денег
        ConnectBillError = 14,                  // нет связи с приемником
        ConnectBillErrorEnd = 15,               // связь с приемником возобновилась
    }
}