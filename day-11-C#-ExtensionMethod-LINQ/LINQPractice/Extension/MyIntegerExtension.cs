namespace MyExtenstion
{
    public static class MyIntegerExtension
    {
        public static double TaxCalculation( this int amount)
        {   
            if (amount <= 0)
            {
                return amount;
            }
            if(amount <= 1000)
            {
                return amount+(amount*0.18);
            }
            return amount+(amount*0.20);
        }
    }
}