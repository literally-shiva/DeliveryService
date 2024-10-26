using DeliveryService;

namespace DeliveryServiceTests
{
    [TestClass]
    public class DeliveryTest
    {
        [TestMethod]
        public void DeliveryFiltrationtTestWithoutParams()
        {
            //arrange
            string[] args = [];
            var expected = File.ReadAllText("DataForTests\\expectedTestWithoutParams.txt");

            //act
            Delivery.DeliveryFiltration(args);

            //assert
            var atual = File.ReadAllText("deliveryOrder.txt");
            Assert.AreEqual(expected, atual);
        }
        [TestMethod]
        public void DeliveryFiltrationtTestWithDistrict()
        {
            //arrange
            string[] args = ["3"];
            var expected = File.ReadAllText("DataForTests\\expectedTestWithDistrict.txt");

            //act
            Delivery.DeliveryFiltration(args);

            //assert
            var atual = File.ReadAllText("deliveryOrder.txt");
            Assert.AreEqual(expected, atual);
        }
        [TestMethod]
        public void DeliveryFiltrationtTestWithDateTime()
        {
            //arrange
            string[] args = ["3", "2024-10-02", "17:00:00"];
            var expected = File.ReadAllText("DataForTests\\expectedTestWithDateTime.txt");

            //act
            Delivery.DeliveryFiltration(args);

            //assert
            var atual = File.ReadAllText("deliveryOrder.txt");
            Assert.AreEqual(expected, atual);
        }
        [TestMethod]
        public void DeliveryFiltrationtTestWithInterval()
        {
            //arrange
            string[] args = ["3", "2024-10-02", "17:00:00", "01:00:00"];
            var expected = File.ReadAllText("DataForTests\\expectedTestWithInterval.txt");

            //act
            Delivery.DeliveryFiltration(args);

            //assert
            var atual = File.ReadAllText("deliveryOrder.txt");
            Assert.AreEqual(expected, atual);
        }
        [TestMethod]
        public void DeliveryFiltrationtTestWithOrderPath()
        {
            //arrange
            string[] args = ["3", "2024-10-02", "17:00:00", "01:00:00", "Order1.txt"];
            var expected = File.ReadAllText("DataForTests\\expectedTestWithOrderPath.txt");

            //act
            Delivery.DeliveryFiltration(args);

            //assert
            var atual = File.ReadAllText(args[4]);
            Assert.AreEqual(expected, atual);
        }
        [TestMethod]
        public void DeliveryFiltrationtTestWithLogPath()
        {
            //arrange
            string[] args = ["3", "2024-10-02", "17:00:00", "01:00:00", "Order1.txt", "Log1.txt"];
            var expected = File.ReadAllText("DataForTests\\expectedTestWithLogPath.txt");

            //act
            Delivery.DeliveryFiltration(args);

            //assert
            var atual = File.ReadAllText(args[4]);
            Assert.AreEqual(expected, atual);
        }
    }
}