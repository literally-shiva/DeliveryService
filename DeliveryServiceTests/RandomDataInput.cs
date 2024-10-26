namespace DeliveryServiceTests
{
    // Вспомогательный класс, который был необходим для генерации случайных входных данных
    internal class RandomDataInput
    {
        public static void DoRandomInput(int numberOfRecords)
        {
            int days = 0;
            int hours = 0;
            int minutes = 0;
            int seconds = 0;
            int kilograms = 0;
            int districtNumber = 0;

            Random rnd = new Random();

            using (StreamWriter writer = new StreamWriter("input.txt"))
            {
                for (int i = 1; i <= numberOfRecords; i++)
                {
                    days = rnd.Next(1, 8);
                    hours = rnd.Next(10, 21);
                    minutes = rnd.Next(0, 60);
                    seconds = rnd.Next(0, 60);
                    kilograms = rnd.Next(0, 51);
                    districtNumber = rnd.Next(1, 6);
                    writer.WriteLine($"{i};{kilograms};{districtNumber};2024-10-{days:d2} {hours:d2}:{minutes:d2}:{seconds:d2}");
                }
            }
        }
    }
}
