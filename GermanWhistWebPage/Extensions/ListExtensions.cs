namespace GermanWhistWebPage.Extensions
{
    public static class ListExtensions
    {
        public static void Shuffle<T>(this List<T> list)
        {
            Random rnd = new Random();
            int n = list.Count-1;
            while (n > 0)
            {
                int k = rnd.Next(0, n+1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
                n--;
            }
        }
    }
}
