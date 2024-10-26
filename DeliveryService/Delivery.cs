namespace DeliveryService
{
    public class Delivery
    {
        public static void DeliveryFiltration(string[] args)
        {
            // Инициализация необходимых переменных значениями по умолчанию
            int _cityDistrict = 1;                                                  // Идентификатор района для фильтрации
            DateTime _firstDeliveryDateTime = DateTime.Now;                         // Время первой доставки, от которой будем искать заказы (время обращения "с")
            TimeSpan _intervalDeliveryTimeSpan = new TimeSpan(0, 30, 0);            // Интервал, вперед на который будем смотреть для поиска подходящих заказов (по умолчанию, исходя из условия, 30 минут)
            string _deliveryOrder = "deliveryOrder.txt";                            // Путь к файлу с результатом выборки
            string _deliveryLog = "deliveryLog.txt";                                // Путь к файлу с логами

            #region Чтение файла конфигурации для первичного определения параметров
            try
            {
                using (StreamReader configReader = new StreamReader("config.txt"))
                {
                    Console.WriteLine("[INFO] Чтение файла конфигурации");
                    string? configLine;
                    while ((configLine = configReader.ReadLine()) != null)
                    {
                        var temp = configLine.Split('=');
                        try
                        {
                            switch (temp[0])
                            {
                                case "_cityDistrict":
                                    ParseCityDistrict(temp[1], out _cityDistrict);
                                    break;
                                case "_firstDeliveryDateTime":
                                    ParseFirstDeliveryDateTime(temp[1], out _firstDeliveryDateTime);
                                    break;
                                case "_intervalDeliveryDateTime":
                                    ParseIntervalDeliveryDateTime(temp[1], out _intervalDeliveryTimeSpan);
                                    break;
                                case "_deliveryOrder":
                                    ParseDeliveryOrder(temp[1], out _deliveryOrder);
                                    break;
                                case "_deliveryLog":
                                    ParseDeliveryLog(temp[1], out _deliveryLog);
                                    break;
                            }
                        }
                        catch
                        {
                            Console.WriteLine($"[INFO] Ошибка при чтении файла конфигурации: не удалось прочитать значение параметра");
                        }
                    }
                    Console.WriteLine("[INFO] Чтение файла конфигурации окончено");
                }
            }
            catch
            {
                Console.WriteLine("[INFO] Файл конфигурации отсутствует");
            }
            #endregion

            #region Считывание параметров с консоли
            Console.WriteLine("[INFO] Чтение параметров консоли");

            switch (args.Length)
            {
                case 0:
                    Console.WriteLine($"[INFO] Дополнительные параметры запуска отсутствуют и будут взяты из конфигурационного файла или установлены по умолчанию");
                    break;
                case 1:
                    ParseCityDistrict(args[0], out _cityDistrict);
                    break;
                case 2:
                    ParseCityDistrict(args[0], out _cityDistrict);
                    ParseFirstDeliveryDateTime(args[1], out _firstDeliveryDateTime);
                    break;
                case 3:
                    ParseCityDistrict(args[0], out _cityDistrict);
                    ParseFirstDeliveryDateTime(args[1] + " " + args[2], out _firstDeliveryDateTime);
                    break;
                case 4:
                    ParseCityDistrict(args[0], out _cityDistrict);
                    ParseFirstDeliveryDateTime(args[1] + " " + args[2], out _firstDeliveryDateTime);
                    ParseIntervalDeliveryDateTime(args[3], out _intervalDeliveryTimeSpan);
                    break;
                case 5:
                    ParseCityDistrict(args[0], out _cityDistrict);
                    ParseFirstDeliveryDateTime(args[1] + " " + args[2], out _firstDeliveryDateTime);
                    ParseIntervalDeliveryDateTime(args[3], out _intervalDeliveryTimeSpan);
                    ParseDeliveryOrder(args[4], out _deliveryOrder);
                    break;
                case >= 6:
                    ParseCityDistrict(args[0], out _cityDistrict);
                    ParseFirstDeliveryDateTime(args[1] + " " + args[2], out _firstDeliveryDateTime);
                    ParseIntervalDeliveryDateTime(args[3], out _intervalDeliveryTimeSpan);
                    ParseDeliveryOrder(args[4], out _deliveryOrder);
                    ParseDeliveryLog(args[5], out _deliveryLog);
                    break;
            }
            Console.WriteLine("[INFO] Чтение параметров консоли окончено");
            #endregion

            #region Обработка входных данных
            try
            {
                using (StreamWriter logWriter = new StreamWriter(_deliveryLog, true))
                {
                    #region Демонстрация выбранных параметров для фильтрации с учётом файла конфигурации и параметров консоли
                    WriteLogAndConsole($"""
                        Инициализация фильтрации c параметрами:
                            Район: {_cityDistrict}
                            Время первой доставки: {_firstDeliveryDateTime.ToString("yyyy-MM-dd HH:mm:ss")}
                            Интервал: {_intervalDeliveryTimeSpan}
                            Путь к файлу с результатом выборки: {_deliveryOrder}
                            Путь к файлу с логами: {_deliveryLog}
                        """, logWriter);
                    #endregion
                    try
                    {
                        using (StreamWriter orderWriter = new StreamWriter(_deliveryOrder))
                        {
                            try
                            {
                                using (StreamReader orderReader = new StreamReader("input.txt"))
                                {
                                    string? line;
                                    List<Order> orderList = new List<Order>();
                                    int lineIndex = 1;

                                    while ((line = orderReader.ReadLine()) != null)
                                    {
                                        try
                                        {
                                            var tempArr = line.Split(";");

                                            Order order = new Order(int.Parse(tempArr[0]),
                                                int.Parse(tempArr[1]),
                                                int.Parse(tempArr[2]),
                                                DateTime.Parse(tempArr[3]));
                                            if (order.DistrictNumber == _cityDistrict &&
                                                    order.Date >= _firstDeliveryDateTime &&
                                                    order.Date <= _firstDeliveryDateTime + _intervalDeliveryTimeSpan)
                                                orderList.Add(order);
                                        }
                                        catch
                                        {
                                            WriteLogAndConsole($"Не удалось прочитать {lineIndex} строку входных данных", logWriter);
                                        }
                                        lineIndex++;
                                    }

                                    orderList = orderList.OrderBy(x => x.Date).ToList();

                                    WriteLogAndConsole($"Найденные записи: ", logWriter);
                                    foreach (var order in orderList)
                                    {
                                        orderWriter.WriteLine($"{order.Id};{order.Weight};{order.DistrictNumber};{order.Date.ToString("yyyy-MM-dd HH:mm:ss")}");
                                        WriteLogAndConsole($"{order.Id};{order.Weight};{order.DistrictNumber};{order.Date.ToString("yyyy-MM-dd HH:mm:ss")}", logWriter);
                                    }
                                    WriteLogAndConsole($"Всего найдено {orderList.Count} записей.", logWriter);
                                }
                            }
                            catch
                            {
                                WriteLogAndConsole("Ошибка: файл входных данных отсутствует", logWriter);
                            }
                        }
                    }
                    catch
                    {
                        WriteLogAndConsole("Ошибка: не получилось найти часть пути к файлу с результатом выборки", logWriter);
                    }
                }
            }
            catch
            {
                Console.WriteLine("[INFO] Ошибка: не получилось найти часть пути к файлу логов");
            }
            #endregion

            #region Вспомогательные методы
            // Метод для проверки имени файла на корректность
            bool isFileNameValid(string fileName)
            {
                if ((fileName == null) || (fileName.IndexOfAny(Path.GetInvalidPathChars()) != -1))
                    return false;
                try
                {
                    var tempFileInfo = new FileInfo(fileName);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            // Метод вывода информации в консоль и логирования
            void WriteLogAndConsole(string message, StreamWriter logWriter)
            {
                Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] {message}");
                logWriter.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] {message}");
            }

            #region Методы парсинга и установки параметров из файла конфигурации или по умолчанию в случае неудачи
            // Метод парсинга _cityDistrict
            void ParseCityDistrict(string strToParse, out int cityDistrict)
            {
                var tempCityDistrict = _cityDistrict;
                if (!int.TryParse(strToParse, out cityDistrict))
                {
                    cityDistrict = tempCityDistrict;
                    Console.WriteLine("[INFO] Не удалось считать _cityDistrict");
                }
            }

            // Метод парсинга _firstDeliveryDateTime
            void ParseFirstDeliveryDateTime(string strToParse, out DateTime firstDeliveryDateTime)
            {
                var tempFirstDeliveryDateTime = _firstDeliveryDateTime;
                if (!DateTime.TryParse(strToParse, out firstDeliveryDateTime))
                {
                    firstDeliveryDateTime = tempFirstDeliveryDateTime;
                    Console.WriteLine("[INFO] Не удалось считать _firstDeliveryDateTime");
                }
            }

            // Метод парсинга _intervalDeliveryDateTime
            void ParseIntervalDeliveryDateTime(string strToParse, out TimeSpan intervalDeliveryTimeSpan)
            {
                var tempIntervalDeliveryTimeSpan = _intervalDeliveryTimeSpan;
                if (!TimeSpan.TryParse(strToParse, out intervalDeliveryTimeSpan))
                {
                    intervalDeliveryTimeSpan = tempIntervalDeliveryTimeSpan;
                    Console.WriteLine("[INFO] Не удалось считать _intervalDeliveryTimeSpan");
                }
            }

            // Метод парсинга _deliveryOrder
            void ParseDeliveryOrder(string strToParse, out string deliveryOrder)
            {
                if (!isFileNameValid(strToParse))
                {
                    strToParse = _deliveryOrder;
                    Console.WriteLine("[INFO] Не удалось считать _deliveryOrder");
                }
                deliveryOrder = strToParse;
            }

            // Метод парсинга _deliveryLog
            void ParseDeliveryLog(string strToParse, out string deliveryLog)
            {
                if (!isFileNameValid(strToParse))
                {
                    strToParse = _deliveryLog;
                    Console.WriteLine("[INFO] Не удалось считать _deliveryLog");
                }
                deliveryLog = strToParse;
            }
            #endregion

            #endregion
        }
    }
}
