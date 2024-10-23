using DeliveryService;
//using static DeliveryService.RandomDataInput;

//DoRandomInput(10000);

// Инициализация необходимых переменных значениями по умолчанию
int _cityDistrict = 0;                                                  // Идентификатор района для фильтрации
DateTime _firstDeliveryDateTime = new DateTime();                       // Время первой доставки, от которой будем искать заказы (время обращения "с")
TimeSpan _intervalDeliveryTimeSpan = new TimeSpan();                    // Интервал, вперед на который будем смотреть для поиска подходящих заказов (по умолчанию, исходя из условия, 30 минут)
string _deliveryOrder = "deliveryOrder.txt";                            // Путь к файлу с результатом выборки
string _deliveryLog = "deliveryLog.txt";                                // Путь к файлу с лонгами

#region Чтение файла конфигурации для переопределения переменных
using (StreamReader configReader = new StreamReader("config.txt"))
{
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
                        throw new Exception("не удалось считать переменную _cityDistrict");
                    break;
                case "_firstDeliveryDateTime":
                    if (!DateTime.TryParse(temp[1], out _firstDeliveryDateTime))
                        throw new Exception("не удалось считать переменную _firstDeliveryDateTime");
                    break;
                case "_intervalDeliveryDateTime":
                    if (!TimeSpan.TryParse(temp[1], out _intervalDeliveryTimeSpan))
                        throw new Exception("не удалось считать переменную _intervalDeliveryDateTime");
                    break;
                case "_deliveryOrder":
                    if (!isFileNameValid(temp[1]))
                        throw new Exception("не удалось считать переменную _deliveryOrder");
                    else
                        _deliveryOrder = temp[1];
                    break;
                case "_deliveryLog":
                    if (!isFileNameValid(temp[1]))
                        throw new Exception("не удалось считать переменную _deliveryLog");
                    else
                        _deliveryLog = temp[1];
                    break;
                default:
                    throw new Exception("обнаружен неизвестный параметр");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ошибка при чтении файла конфигурации: {e.Message}");
        }
    }

    Console.WriteLine($"Файл конфигурации прочитан.");
}
#endregion

#region Считывание параметров с консоли и их замена в зависимости от количества (наличия тех или иных)
// Параметры с консоли приоритетнее заданных в конфигурационном файле.
//
// Переменные в консоли идут строго в следующем порядке:
//      Идентификатор района для фильтрации
//      Время первой доставки
//      Интервал
//      Путь к файлу с результатом выборки
//      Путь к файлу с лонгами
//
// Не обязательно задавать все параметры. Отсутствующие будут заданы конфигурационным фалом или установлены по умолчанию

try
{
    switch (args.Length)
    {
        case 0:
            Console.WriteLine("Дополнительные параметры запуска отсутствуют. Они будут взяты из конфигурационного файла.");
            break;
        case 1:
            if (!int.TryParse(args[0], out _cityDistrict))
                throw new Exception("не удалось считать переменную _cityDistrict");
            break;
        case 2:
            throw new Exception("не удалось считать переменную _firstDeliveryDateTime");
        case 3:
            if (!int.TryParse(args[0], out _cityDistrict))
                throw new Exception("не удалось считать переменную _cityDistrict");
            if (!DateTime.TryParse(args[1] + args[2], out _firstDeliveryDateTime))
                throw new Exception("не удалось считать переменную _firstDeliveryDateTime");
            break;
        case 4:
            if (!int.TryParse(args[0], out _cityDistrict))
                throw new Exception("не удалось считать переменную _cityDistrict");
            if (!DateTime.TryParse(args[1] + args[2], out _firstDeliveryDateTime))
                throw new Exception("не удалось считать переменную _firstDeliveryDateTime");
            if (!TimeSpan.TryParse(args[3], out _intervalDeliveryTimeSpan))
                throw new Exception("не удалось считать переменную _intervalDeliveryDateTime");
            break;
        case 5:
            if (!int.TryParse(args[0], out _cityDistrict))
                throw new Exception("не удалось считать переменную _cityDistrict");
            if (!DateTime.TryParse(args[1] + args[2], out _firstDeliveryDateTime))
                throw new Exception("не удалось считать переменную _firstDeliveryDateTime");
            if (!TimeSpan.TryParse(args[3], out _intervalDeliveryTimeSpan))
                throw new Exception("не удалось считать переменную _intervalDeliveryDateTime");
            if (!isFileNameValid(args[4]))
                throw new Exception("не удалось считать переменную _deliveryOrder");
            break;
        case 6:
            if (!int.TryParse(args[0], out _cityDistrict))
                throw new Exception("не удалось считать переменную _cityDistrict");
            if (!DateTime.TryParse(args[1] + args[2], out _firstDeliveryDateTime))
                throw new Exception("не удалось считать переменную _firstDeliveryDateTime");
            if (!TimeSpan.TryParse(args[3], out _intervalDeliveryTimeSpan))
                throw new Exception("не удалось считать переменную _intervalDeliveryDateTime");
            if (!isFileNameValid(args[4]))
                throw new Exception("не удалось считать переменную _deliveryOrder");
            if (!isFileNameValid(args[5]))
                throw new Exception("не удалось считать переменную _deliveryLog");
            break;
        case > 6:
            throw new Exception("на вход получено больше параметров, чем ожидалось");
    }
}
catch (Exception e)
{
    Console.WriteLine($"Ошибка при считывании параметров с консоли: {e.Message}");
}
#endregion

#region Обработка входных данных
Console.WriteLine($"""
    [{DateTime.Now}] фильтрация c параметрами:
        Район:                                  {_cityDistrict}
        Время первой доставки:                  {_firstDeliveryDateTime}
        Интервал:                               {_intervalDeliveryTimeSpan}
        Путь к файлу с результатом выборки:     {_deliveryOrder}
        Путь к файлу с логами:                  {_deliveryLog}
    """);

using (StreamReader orderReader = new StreamReader("input.txt"))
using (StreamWriter orderWriter = new StreamWriter(_deliveryOrder))
using (StreamWriter logWriter = new StreamWriter(_deliveryLog, true))
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

    logWriter.WriteLine($"""
        [{DateTime.Now}] фильтрация c параметрами:
            Район:                              {_cityDistrict}
            Время первой доставки:              {_firstDeliveryDateTime}
            Интервал:                           {_intervalDeliveryTimeSpan}
            Путь к файлу с результатом выборки: {_deliveryOrder}
        """);
}
#endregion

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