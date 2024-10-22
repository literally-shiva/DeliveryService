using DeliveryService;
//using static DeliveryService.RandomDataInput;

//DoRandomInput(1000);

DateTime _firstDeliveryDateTime = new DateTime();
int _cityDistrict = 0;
string _deliveryOrder = "";
string _deliveryLog = "";

using (StreamReader initReader = new StreamReader("init.txt"))
{
    string? initLine;
    while ((initLine = initReader.ReadLine()) != null)
    {
        var temp = initLine.Split('=');

    }
}

try
{
    if (!int.TryParse(args[0], out _cityDistrict))
    {
        Console.WriteLine("Неправильный номер района.");
        return;
    }
    if (!DateTime.TryParseExact(args[1] + " " + args[2], "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out _firstDeliveryDateTime))
    {
        Console.WriteLine("Неправильный формат даты.");
        Console.WriteLine(args[1]);
        return;
    }
    _deliveryOrder = args[3];
    _deliveryLog = args[4];
}
catch
{
    Console.WriteLine("Произошла ошибка ввода.");
    return;
}

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
        if (order.DistrictNumber == _cityDistrict && order.Date >= _firstDeliveryDateTime && order.Date <= _firstDeliveryDateTime.AddMinutes(30))
            orderList.Add(order);
    }

    orderList = orderList.OrderBy(x => x.DistrictNumber).ThenBy(x => x.Date).ToList();

    foreach (var order in orderList)
    {
        orderWriter.WriteLine($"Id = {order.Id,4}; Weight = {order.Weight,2}, DistrictNumer = {order.DistrictNumber,3}, Date = {order.Date.ToString("yyyy-MM-dd HH:mm:ss")}");
    }

    logWriter.WriteLine($"{DateTime.Now} была произведена фильтрация по району номер {_cityDistrict} и времени первого заказа {_firstDeliveryDateTime}");
}