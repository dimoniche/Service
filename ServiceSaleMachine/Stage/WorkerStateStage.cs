using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceSaleMachine
{
    public enum WorkerStateStage
    {
        None,

        /// <summary>
        /// Инициализация
        /// </summary>
        Init,                       //

        /// <summary>
        /// настройка
        /// </summary>
        Setting,                    //

        /// <summary>
        /// Ожидание клиента
        /// </summary>
        Wait,                       //

        /// <summary>
        /// отказ от услуги - переход в ожидание клиента
        /// </summary>
        Fail,                       //

        /// <summary>
        /// Ознакомление с правилами услуги
        /// </summary>
        Rules,                      //

        /// <summary>
        /// Вход пользователя
        /// </summary>
        InterUser,                  //

        /// <summary>
        /// Выбор услуги
        /// </summary>
        ChooseService,              //

        /// <summary>
        /// Выбор способа оплаты
        /// </summary>
        ChoosePay,                  //

        /// <summary>
        /// Оплата чеком
        /// </summary>
        PayCheckService,            //

        /// <summary>
        /// Оплата деньгами
        /// </summary>
        PayBillService,             //

        /// <summary>
        /// Запуск услуги
        /// </summary>
        StartService,               // 

        /// <summary>
        /// Оказывание услуги
        /// </summary>
        ContinueService,            //

        /// <summary>
        /// окончание оказания услуги
        /// </summary>
        EndService,                       //

        /// <summary>
        /// Ожидание клиента
        /// </summary>
        AgainWait,                       //

        /// <summary>
        /// выход из программы
        /// </summary>
        ExitProgram,                       //

        /// <summary>
        /// Ручной вход в настройки
        /// </summary>
        ManualSetting,

        /// <summary>
        /// авторизация  из выбора сервиса
        /// </summary>
        UserRequestService,

        /// <summary>
        /// выемка денег
        /// </summary>
        DropCassettteBill,

        /// <summary>
        /// конец выемки денег
        /// </summary>
        EndDropCassette,

        /// <summary>
        /// Необходимость обслуживания
        /// </summary>
        NeedService,

        /// <summary>
        /// Обслуживание проведено
        /// </summary>
        EndNeedService,

        /// <summary>
        /// выход из формы в начало по таймауту
        /// </summary>
        TimeOut,

        /// <summary>
        /// Нужно настроить программу
        /// </summary>
        NeedSettingProgram,

        /// <summary>
        /// Нет ком портов
        /// </summary>
        NoCOMPort,

        /// <summary>
        /// Главный экран
        /// </summary>
        MainScreen,

        /// <summary>
        /// Ошибка в управляющих уситройствах
        /// </summary>
        ErrorControl,

        /// <summary>
        /// Ощибка в управляющих устройствах завершилась
        /// </summary>
        ErrorEndControl,

        /// <summary>
        /// Переход в меню с разницей
        /// </summary>
        WhatsDiff,

        /// <summary>
        /// Показ философии проекта
        /// </summary>
        Philosof,

        /// <summary>
        /// приемник полон деньгами
        /// </summary>
        BillFull,

        /// <summary>
        /// Выработан установленный ресурс
        /// </summary>
        ResursEnd,

        /// <summary>
        /// Ошибка купюроприемника
        /// </summary>
        ErrorBill,

        /// <summary>
        /// Купюроприемник обслужили
        /// </summary>
        EndBillFull,

        /// <summary>
        /// бумага закончилась
        /// </summary>
        PaperEnd,

        /// <summary>
        /// ошибка принтера
        /// </summary>
        ErrorPrinter,

        /// <summary>
        /// ошибка при регистрации нового пользователя
        /// </summary>
        ErrorRegisterNewUser,

        /// <summary>
        /// Авторизовали пользователя
        /// </summary>
        AuthorizeUser,

        /// <summary>
        /// Зарегистрировали нового пользователя
        /// </summary>
        RegisterNewUser,

        /// <summary>
        /// Не авторизовали пользователя
        /// </summary>
        NotAuthorizeUser,
        FindPhone,
        NotFindPhone,

        /// <summary>
        /// Ошибка приемника снялась
        /// </summary>
        BillErrorEnd,
    }
}
