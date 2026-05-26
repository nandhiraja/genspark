namespace MyExtenstion
{
    public static class MyStringExtension
    {
        public static int CountWords(this string s)
        {
           return s.Split(new[] { ' ', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length; 
        }
    }


}