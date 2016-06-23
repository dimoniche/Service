using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceSaleMachine.MainWorker
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
        /// выход из программы
        /// </summary>
        ExitProgram,                       //

    }
}
