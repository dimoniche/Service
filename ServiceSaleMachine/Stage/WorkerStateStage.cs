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
    }
}
