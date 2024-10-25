using DeliveryService;

namespace DeliveryServiceTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            //arrange
            string[] args = [];
            var expected = File.ReadAllText("Test1\\deliveryOrder1.txt");

            //act
            Delivery.DeliveryFiltration(args);

            //assert
            var atual = File.ReadAllText("deliveryOrder.txt");
            Assert.AreEqual(expected, atual);
        }
    }
}