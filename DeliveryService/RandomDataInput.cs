namespace DeliveryService
{
    internal class RandomDataInput
    {
        public static void DoRandomInput(int numberOfRecords)
        {
            //int days = 0;
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
                    //days = rnd.Next(1, 32);
                    hours = rnd.Next(0, 24);
                    minutes = rnd.Next(0, 60);
                    seconds = rnd.Next(0, 60);
                    kilograms = rnd.Next(0, 16);
                    districtNumber = rnd.Next(1, 3);
                    writer.WriteLine($"{i};{kilograms};{districtNumber};2024-10-22 {hours:d2}:{minutes:d2}:{seconds:d2}");
                }
            }
        }
    }
}
