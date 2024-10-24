using DeliveryService;
//using static DeliveryService.RandomDataInput;

//DoRandomInput(10000);

// Инициализация необходимых переменных значениями по умолчанию
int _cityDistrict = 1;                                                  // Идентификатор района для фильтрации
DateTime _firstDeliveryDateTime = DateTime.Now;                         // Время первой доставки, от которой будем искать заказы (время обращения "с")
TimeSpan _intervalDeliveryTimeSpan = new TimeSpan(0, 30, 0);            // Интервал, вперед на который будем смотреть для поиска подходящих заказов (по умолчанию, исходя из условия, 30 минут)
string _deliveryOrder = "deliveryOrder.txt";                            // Путь к файлу с результатом выборки
string _deliveryLog = "deliveryLog.txt";                                // Путь к файлу с логами

using (StreamWriter logWriter = new StreamWriter(_deliveryLog, true))
{
    #region Чтение файла конфигурации для первичного определения параметров
    using (StreamReader configReader = new StreamReader("config.txt"))
    {
        WriteLogAndConsole($"Чтение файла конфигурации", logWriter);
        string? configLine;
        while ((configLine = configReader.ReadLine()) != null)
        {
            var temp = configLine.Split('=');
            try
            {
                switch (temp[0])
                {
                    case "_cityDistrict":
                        ParseCityDistrictWithLog(temp[1], out _cityDistrict, logWriter);
                        break;
                    case "_firstDeliveryDateTime":
                        ParseFirstDeliveryDateTimeWithLog(temp[1], out _firstDeliveryDateTime, logWriter);
                        break;
                    case "_intervalDeliveryDateTime":
                        ParseIntervalDeliveryDateTimeWithLog(temp[1], out _intervalDeliveryTimeSpan, logWriter);
                        break;
                    case "_deliveryOrder":
                        ParseDeliveryOrderWithLog(temp[1], out _deliveryOrder, logWriter);
                        break;
                    case "_deliveryLog":
                        ParseDeliveryLogWithLog(temp[1], out _deliveryLog, logWriter);
                        break;
                    default:
                        throw new Exception("обнаружен неизвестный параметр");
                }
            }
            catch (Exception e)
            {
                WriteLogAndConsole($"Ошибка при чтении файла конфигурации: {e.Message}", logWriter);
            }
        }
        WriteLogAndConsole($"Файл конфигурации прочитан", logWriter);
    }
    #endregion

    #region Считывание параметров с консоли
    // Параметры с консоли приоритетнее заданных в конфигурационном файле.
    // Не обязательно задавать их все. Отсутствующие будут считаны с конфигурационного файла или установлены по умолчанию
    // Переменные в консоли идут строго в следующем порядке:
    //      Идентификатор района для фильтрации
    //      Время первой доставки
    //      Интервал
    //      Путь к файлу с результатом выборки
    //      Путь к файлу с логами

    WriteLogAndConsole($"Начало обработки параметров консоли", logWriter);
    try
    {
        switch (args.Length)
        {
            case 0:
                WriteLogAndConsole($"Дополнительные параметры запуска отсутствуют и будут взяты из конфигурационного файла", logWriter);
                break;
            case 1:
                ParseCityDistrictWithLog(args[0], out _cityDistrict, logWriter);
                break;
            case 2:
                throw new Exception("не удалось считать переменную _firstDeliveryDateTime");
            case 3:
                ParseCityDistrictWithLog(args[0], out _cityDistrict, logWriter);
                ParseFirstDeliveryDateTimeWithLog(args[1] + " " + args[2], out _firstDeliveryDateTime, logWriter);
                break;
            case 4:
                ParseCityDistrictWithLog(args[0], out _cityDistrict, logWriter);
                ParseFirstDeliveryDateTimeWithLog(args[1] + " " + args[2], out _firstDeliveryDateTime, logWriter);
                ParseIntervalDeliveryDateTimeWithLog(args[3], out _intervalDeliveryTimeSpan, logWriter);
                break;
            case 5:
                ParseCityDistrictWithLog(args[0], out _cityDistrict, logWriter);
                ParseFirstDeliveryDateTimeWithLog(args[1] + " " + args[2], out _firstDeliveryDateTime, logWriter);
                ParseIntervalDeliveryDateTimeWithLog(args[3], out _intervalDeliveryTimeSpan, logWriter);
                ParseDeliveryOrderWithLog(args[4], out _deliveryOrder, logWriter);
                break;
            case 6:
                ParseCityDistrictWithLog(args[0], out _cityDistrict, logWriter);
                ParseFirstDeliveryDateTimeWithLog(args[1] + " " + args[2], out _firstDeliveryDateTime, logWriter);
                ParseIntervalDeliveryDateTimeWithLog(args[3], out _intervalDeliveryTimeSpan, logWriter);
                ParseDeliveryOrderWithLog(args[4], out _deliveryOrder, logWriter);
                ParseDeliveryLogWithLog(args[5], out _deliveryLog, logWriter);
                break;
            case > 6:
                throw new Exception("на вход получено больше параметров, чем ожидалось");
        }
    }
    catch (Exception e)
    {
        WriteLogAndConsole($"Ошибка при считывании параметров с консоли: {e.Message}", logWriter);
    }
    WriteLogAndConsole($"Конец обработки параметрво консоли", logWriter);
    #endregion

    #region Демонстрация выбранных параметров для фильтрации с учётом файла конфигурации и параметров консоли
    WriteLogAndConsole($"""
        Начало фильтрации c параметрами:
            Район: {_cityDistrict}
            Время первой доставки: {_firstDeliveryDateTime.ToString("yyyy-MM-dd HH:mm:ss")}
            Интервал: {_intervalDeliveryTimeSpan}
            Путь к файлу с результатом выборки: {_deliveryOrder}
            Путь к файлу с логами: {_deliveryLog}
        """, logWriter);
    #endregion

    #region Обработка входных данных


    using (StreamReader orderReader = new StreamReader("input.txt"))
    using (StreamWriter orderWriter = new StreamWriter(_deliveryOrder))
    {
        string? line;
        List<Order> orderList = new List<Order>();

        while ((line = orderReader.ReadLine()) != null)
        {
            var tempArr = line.Split(";");

            Order order = new Order(int.Parse(tempArr[0]),
                int.Parse(tempArr[1]),
                int.Parse(tempArr[2]),
                DateTime.Parse(tempArr[3]));
            if (order.DistrictNumber == _cityDistrict && order.Date >= _firstDeliveryDateTime && order.Date <= _firstDeliveryDateTime + _intervalDeliveryTimeSpan)
                orderList.Add(order);
        }

        orderList = orderList.OrderBy(x => x.Date).ToList();

        foreach (var order in orderList)
        {
            orderWriter.WriteLine($"Id={order.Id,5}; Weight={order.Weight,2}; DistrictNumer={order.DistrictNumber,1}; Date={order.Date.ToString("yyyy-MM-dd HH:mm:ss")}");
        }
        WriteLogAndConsole($"Найдено {orderList.Count} записей.", logWriter);
    }
    #endregion
}

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
    catch (NotSupportedException)
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

// Метод парсинга _cityDistrict
void ParseCityDistrictWithLog(string strToParse, out int cityDistrict, StreamWriter logWriter)
{
    if (!int.TryParse(strToParse, out cityDistrict))
    {
        cityDistrict = 1;
        throw new Exception("не удалось считать переменную _cityDistrict");
    }
    WriteLogAndConsole($"Район: {cityDistrict}", logWriter);
}

// Метод парсинга _firstDeliveryDateTime
void ParseFirstDeliveryDateTimeWithLog(string strToParse, out DateTime firstDeliveryDateTime, StreamWriter logWriter)
{
    if (!DateTime.TryParse(strToParse, out firstDeliveryDateTime))
    {
        firstDeliveryDateTime = DateTime.Now;
        throw new Exception("не удалось считать переменную _firstDeliveryDateTime");
    }
    WriteLogAndConsole($"Время первой доставки: {firstDeliveryDateTime.ToString("yyyy-MM-dd HH:mm:ss")}", logWriter);
}

// Метод парсинга _intervalDeliveryDateTime
void ParseIntervalDeliveryDateTimeWithLog(string strToParse, out TimeSpan intervalDeliveryTimeSpan, StreamWriter logWriter)
{
    if (!TimeSpan.TryParse(strToParse, out intervalDeliveryTimeSpan))
    {
        intervalDeliveryTimeSpan = new TimeSpan(0, 30, 0);
        throw new Exception("не удалось считать переменную _intervalDeliveryDateTime");
    }
    WriteLogAndConsole($"Интервал: {intervalDeliveryTimeSpan}", logWriter);
}

// Метод парсинга _deliveryOrder
void ParseDeliveryOrderWithLog(string strToParse, out string deliveryOrder, StreamWriter logWriter)
{
    if (!isFileNameValid(strToParse))
        throw new Exception("не удалось считать переменную _deliveryOrder");
    deliveryOrder = strToParse;
    WriteLogAndConsole($"Путь к файлу с результатом выборки: {deliveryOrder}", logWriter);
}

// Метод парсинга _deliveryLog
void ParseDeliveryLogWithLog(string strToParse, out string deliveryLog, StreamWriter logWriter)
{
    if (!isFileNameValid(strToParse))
        throw new Exception("не удалось считать переменную _deliveryLog");
    deliveryLog = strToParse;
    WriteLogAndConsole($"Путь к файлу с логами: {deliveryLog}", logWriter);
}
#endregion