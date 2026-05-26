namespace MyExtenstion
{
    public static class CustomLINQ
    {
         delegate int productDelegate(int value1,int value2);
        public static void CusomterFilter<T>(this IEnumerable<T> source , Func<T,bool> condition ,Action<T> operation)
        {   
            foreach(var item in source)
            {
                if(condition(item))
                {
                    operation(item);
                }
            }
            
        }

        public static IEnumerable<int> ProductFilter<T>(this IEnumerable<T> source ,Func<T, int> productOperation)
        {
            foreach(var item in source)
            {   int value = productOperation(item);
                yield return value;
            }
        }
    }
} 
