namespace DeliveryService
{
    public class Order
    {
        public int Id { get; set; }
        public int Weight { get; set; }
        public int DistrictNumber { get; set; }
        public DateTime Date { get; set; }
        public Order(int id, int weight, int districtNumber, DateTime date)
        {
            Id = id;
            Weight = weight;
            DistrictNumber = districtNumber;
            Date = date;
        }
    }
}
