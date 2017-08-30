using AirVitamin.Drivers;
using System;
using System.Drawing.Text;

namespace AirVitamin.Client
{
    internal class FormResultData
    {
        public WorkerStateStage stage;

        /// <summary>
        /// Номер текущей выбранной услуги
        /// </summary>
        public NumberServiceEnum numberService;

        /// <summary>
        /// данные по текущей выбранной услуге
        /// </summary>
        public Service serv;

        public string retLogin;
        public string retPassword;

        /// <summary>
        /// ID текущего пользователя из базы
        /// </summary>
        public int CurrentUserId;

        /// <summary>
        /// максимальное время оказания услуги
        /// </summary>
        public int timework = 180;

        /// <summary>
        /// Реальное время оказания услуги
        /// </summary>
        public int realtimework = 0;

        /// <summary>
        /// время ознакомления с услугой и забор аксессуаров
        /// </summary>
        public int timeRecognize = 60;

        /// <summary>
        /// Наименование услуги
        /// </summary>
        public string ServName;

        /// <summary>
        /// номер текущего устройства
        /// </summary>
        public int numberCurrentDevice = 1;

        /// <summary>
        /// Драйвера устройств
        /// </summary>
        public MachineDrivers drivers;

        /// <summary>
        /// статистика по деньгам
        /// </summary>
        public MoneyStatistic statistic;

        /// <summary>
        /// Глобальный лог приложения
        /// </summary>
        public Log log;

        /// <summary>
        /// об ошибке принтера уже сообщили
        /// </summary>
        public bool PrinterError = false;

        /// <summary>
        /// Ошибка приемника
        /// </summary>
        public bool BillError = false;

        /// <summary>
        /// Загруженные шрифты
        /// </summary>
        public PrivateFontCollection FontCollection;

        private bool isSendSMS1 = false;
        private bool isSendSMS2 = false;
        private bool isSendSMS3 = false;
        private bool isSendSMS4 = false;

        public bool IsInterError1 = false;
        public bool IsInterError2 = false;
        public bool IsInterError3 = false;
        public bool IsInterError4 = false;

        public bool IsSendSMS1
        {
            get
            {
                if (Globals.ClientConfiguration.Settings.spanSendSMS1 == 0)
                {
                    // если 0 - отключим отправку смс вообще
                    lastTimeSendSMS1 = DateTime.Now;
                    return true;
                }

                return isSendSMS1;
            }
            set
            {
                if (DateTime.Now - lastTimeSendSMS1 > new TimeSpan(Globals.ClientConfiguration.Settings.spanSendSMS1, 0, 0))
                {
                    // меняем значение только если прошло время заданного гистрезиса
                    isSendSMS1 = value;

                    if (isSendSMS1 == true)
                    {
                        // обновляем время отсылки СМС если только что его отослали
                        lastTimeSendSMS1 = DateTime.Now;
                    }
                }
            }
        }

        public bool IsSendSMS2
        {
            get
            {
                if (Globals.ClientConfiguration.Settings.spanSendSMS2 == 0)
                {
                    // если 0 - отключим отправку смс вообще
                    lastTimeSendSMS2 = DateTime.Now;
                    return true;
                }

                return isSendSMS2;
            }
            set
            {
                if (DateTime.Now - lastTimeSendSMS2 > new TimeSpan(Globals.ClientConfiguration.Settings.spanSendSMS2, 0, 0))
                {
                    // меняем значение только если прошло время заданного гистрезиса
                    isSendSMS2 = value;

                    if (isSendSMS2 == true)
                    {
                        // обновляем время отсылки СМС если только что его отослали
                        lastTimeSendSMS2 = DateTime.Now;
                    }
                }
            }
        }

        public bool IsSendSMS3
        {
            get
            {
                if (Globals.ClientConfiguration.Settings.spanSendSMS3 == 0)
                {
                    // если 0 - отключим отправку смс вообще
                    lastTimeSendSMS3 = DateTime.Now;
                    return true;
                }

                return isSendSMS3;
            }
            set
            {
                if (DateTime.Now - lastTimeSendSMS3 > new TimeSpan(Globals.ClientConfiguration.Settings.spanSendSMS3, 0, 0))
                {
                    // меняем значение только если прошло время заданного гистрезиса
                    isSendSMS3 = value;

                    if (isSendSMS3 == true)
                    {
                        // обновляем время отсылки СМС если только что его отослали
                        lastTimeSendSMS3 = DateTime.Now;
                    }
                }
            }
        }

        public bool IsSendSMS4
        {
            get
            {
                if (Globals.ClientConfiguration.Settings.spanSendSMS4 == 0)
                {
                    // если 0 - отключим отправку смс вообще
                    lastTimeSendSMS4 = DateTime.Now;
                    return true;
                }

                return isSendSMS4;
            }
            set
            {
                if (DateTime.Now - lastTimeSendSMS4 > new TimeSpan(Globals.ClientConfiguration.Settings.spanSendSMS4, 0, 0))
                {
                    // меняем значение только если прошло время заданного гистрезиса
                    isSendSMS4 = value;

                    if (isSendSMS4 == true)
                    {
                        // обновляем время отсылки СМС если только что его отослали
                        lastTimeSendSMS4 = DateTime.Now;
                    }
                }
            }
        }

        DateTime lastTimeSendSMS1 = new DateTime(0);
        DateTime lastTimeSendSMS2 = new DateTime(0);
        DateTime lastTimeSendSMS3 = new DateTime(0);
        DateTime lastTimeSendSMS4 = new DateTime(0);

        public FormWaitVideoSecondScreen fr2;

        public FormResultData(Log log)
        {
            retLogin = "";
            retPassword = "";
            timework = 180;
            ServName = "";

            // Лог
            this.log = log;

            // инициализируем драйверы устройств
            drivers = new MachineDrivers(log);

            // прочтем из базы статистику
            statistic = GlobalDb.GlobalBase.GetMoneyStatistic();
        }
    }
}