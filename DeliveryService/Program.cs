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
        WriteLogAndConsole($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Чтение файла конфигурации", logWriter);
        string? configLine;
        while ((configLine = configReader.ReadLine()) != null)
        {
            var temp = configLine.Split('=');
            try
            {
                switch (temp[0])
                {
                    case "_cityDistrict":
                        if (!int.TryParse(temp[1], out _cityDistrict))
                        {
                            _cityDistrict = 1;
                            throw new Exception("не удалось считать переменную _cityDistrict");
                        }
                        WriteLogAndConsole($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Район: {_cityDistrict}", logWriter);
                        break;
                    case "_firstDeliveryDateTime":
                        if (!DateTime.TryParse(temp[1], out _firstDeliveryDateTime))
                        {
                            _firstDeliveryDateTime = DateTime.Now;
                            throw new Exception("не удалось считать переменную _firstDeliveryDateTime");
                        }
                        WriteLogAndConsole($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Время первой доставки: {_firstDeliveryDateTime.ToString("yyyy-MM-dd HH:mm:ss")}", logWriter);
                        break;
                    case "_intervalDeliveryDateTime":
                        if (!TimeSpan.TryParse(temp[1], out _intervalDeliveryTimeSpan))
                        {
                            _intervalDeliveryTimeSpan = new TimeSpan(0, 30, 0);
                            throw new Exception("не удалось считать переменную _intervalDeliveryDateTime");
                        }
                        WriteLogAndConsole($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Интервал: {_intervalDeliveryTimeSpan}", logWriter);
                        break;
                    case "_deliveryOrder":
                        if (!isFileNameValid(temp[1]))
                            throw new Exception("не удалось считать переменную _deliveryOrder");
                        _deliveryOrder = temp[1];
                        WriteLogAndConsole($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Путь к файлу с результатом выборки: {_deliveryOrder}", logWriter);
                        break;
                    case "_deliveryLog":
                        if (!isFileNameValid(temp[1]))
                            throw new Exception("не удалось считать переменную _deliveryLog");
                        _deliveryLog = temp[1];
                        WriteLogAndConsole($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Путь к файлу с логами: {_deliveryLog}", logWriter);
                        break;
                    default:
                        throw new Exception("обнаружен неизвестный параметр");
                }
            }
            catch (Exception e)
            {
                WriteLogAndConsole($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Ошибка при чтении файла конфигурации: {e.Message}", logWriter);
            }
        }
        WriteLogAndConsole($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Файл конфигурации прочитан", logWriter);
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

    WriteLogAndConsole($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Начало обработки параметров консоли", logWriter);
    try
    {
        switch (args.Length)
        {
            case 0:
                WriteLogAndConsole($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Дополнительные параметры запуска отсутствуют и будут взяты из конфигурационного файла", logWriter);
                break;
            case 1:
                if (!int.TryParse(args[0], out _cityDistrict))
                {
                    _cityDistrict = 1;
                    throw new Exception("не удалось считать переменную _cityDistrict");
                }
                WriteLogAndConsole($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Район: {_cityDistrict}", logWriter);
                break;
            case 2:
                throw new Exception("не удалось считать переменную _firstDeliveryDateTime");
            case 3:
                if (!int.TryParse(args[0], out _cityDistrict))
                {
                    _cityDistrict = 1;
                    throw new Exception("не удалось считать переменную _cityDistrict");
                }
                WriteLogAndConsole($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Район: {_cityDistrict}", logWriter);
                if (!DateTime.TryParse(args[1] + " " + args[2], out _firstDeliveryDateTime))
                {
                    _firstDeliveryDateTime = DateTime.Now;
                    throw new Exception("не удалось считать переменную _firstDeliveryDateTime");
                }
                WriteLogAndConsole($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Время первой доставки: {_firstDeliveryDateTime}", logWriter);
                break;
            case 4:
                if (!int.TryParse(args[0], out _cityDistrict))
                {
                    _cityDistrict = 1;
                    throw new Exception("не удалось считать переменную _cityDistrict");
                }
                WriteLogAndConsole($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Район: {_cityDistrict}", logWriter);
                if (!DateTime.TryParse(args[1] + " " + args[2], out _firstDeliveryDateTime))
                {
                    _firstDeliveryDateTime = DateTime.Now;
                    throw new Exception("не удалось считать переменную _firstDeliveryDateTime");
                }
                WriteLogAndConsole($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Время первой доставки: {_firstDeliveryDateTime.ToString("yyyy-MM-dd HH:mm:ss")}", logWriter);
                if (!TimeSpan.TryParse(args[3], out _intervalDeliveryTimeSpan))
                {
                    _intervalDeliveryTimeSpan = new TimeSpan(0, 30, 0);
                    throw new Exception("не удалось считать переменную _intervalDeliveryDateTime");
                }
                WriteLogAndConsole($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Интервал: {_intervalDeliveryTimeSpan}", logWriter);
                break;
            case 5:
                if (!int.TryParse(args[0], out _cityDistrict))
                {
                    _cityDistrict = 1;
                    throw new Exception("не удалось считать переменную _cityDistrict");
                }
                WriteLogAndConsole($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Район: {_cityDistrict}", logWriter);
                if (!DateTime.TryParse(args[1] + " " + args[2], out _firstDeliveryDateTime))
                {
                    _firstDeliveryDateTime = DateTime.Now;
                    throw new Exception("не удалось считать переменную _firstDeliveryDateTime");
                }
                WriteLogAndConsole($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Время первой доставки: {_firstDeliveryDateTime.ToString("yyyy-MM-dd HH:mm:ss")}", logWriter);
                if (!TimeSpan.TryParse(args[3], out _intervalDeliveryTimeSpan))
                {
                    _intervalDeliveryTimeSpan = new TimeSpan(0, 30, 0);
                    throw new Exception("не удалось считать переменную _intervalDeliveryDateTime");
                }
                WriteLogAndConsole($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Интервал: {_intervalDeliveryTimeSpan}", logWriter);
                if (!isFileNameValid(args[4]))
                    throw new Exception("не удалось считать переменную _deliveryOrder");
                _deliveryOrder = args[4];
                WriteLogAndConsole($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Путь к файлу с результатом выборки: {_deliveryOrder}", logWriter);
                break;
            case 6:
                if (!int.TryParse(args[0], out _cityDistrict))
                {
                    _cityDistrict = 1;
                    throw new Exception("не удалось считать переменную _cityDistrict");
                }
                WriteLogAndConsole($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Район: {_cityDistrict}", logWriter);
                if (!DateTime.TryParse(args[1] + " " + args[2], out _firstDeliveryDateTime))
                {
                    _firstDeliveryDateTime = DateTime.Now;
                    throw new Exception("не удалось считать переменную _firstDeliveryDateTime");
                }
                WriteLogAndConsole($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Время первой доставки: {_firstDeliveryDateTime.ToString("yyyy-MM-dd HH:mm:ss")}", logWriter);
                if (!TimeSpan.TryParse(args[3], out _intervalDeliveryTimeSpan))
                {
                    _intervalDeliveryTimeSpan = new TimeSpan(0, 30, 0);
                    throw new Exception("не удалось считать переменную _intervalDeliveryDateTime");
                }
                WriteLogAndConsole($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Интервал: {_intervalDeliveryTimeSpan}", logWriter);
                if (!isFileNameValid(args[4]))
                    throw new Exception("не удалось считать переменную _deliveryOrder");
                _deliveryOrder = args[4];
                WriteLogAndConsole($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Путь к файлу с результатом выборки: {_deliveryOrder}", logWriter);
                if (!isFileNameValid(args[5]))
                    throw new Exception("не удалось считать переменную _deliveryLog");
                _deliveryLog = args[5];
                WriteLogAndConsole($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Путь к файлу с логами: {_deliveryLog}", logWriter);
                break;
            case > 6:
                throw new Exception("на вход получено больше параметров, чем ожидалось");
        }
    }
    catch (Exception e)
    {
        WriteLogAndConsole($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Ошибка при считывании параметров с консоли: {e.Message}", logWriter);
    }
    WriteLogAndConsole($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Конец обработки параметрво консоли", logWriter);
    #endregion

    #region Обработка входных данных
    WriteLogAndConsole($"""
        [{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Начало фильтрации c параметрами:
        [{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Район: {_cityDistrict}
        [{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Время первой доставки: {_firstDeliveryDateTime.ToString("yyyy-MM-dd HH:mm:ss")}
        [{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Интервал: {_intervalDeliveryTimeSpan}
        [{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Путь к файлу с результатом выборки: {_deliveryOrder}
        [{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Путь к файлу с логами: {_deliveryLog}
        """, logWriter);

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
        WriteLogAndConsole($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Найдено {orderList.Count} записей.", logWriter);
    }
    #endregion
}

#region Вспомогательные функции
// Функция для проверки имени файла на корректность
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

// Функция вывода информации в консоль и логирования
void WriteLogAndConsole(string message, StreamWriter logWriter)
{
    Console.WriteLine(message);
    logWriter.WriteLine(message);
}
#endregion