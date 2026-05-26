using System.Reflection.Metadata;
using System.Runtime.ConstrainedExecution;
using MyExtenstion;

namespace LINQPractice
{   
    internal class User
    {
        public string Name {get; set;} = string.Empty;
        public int Age {get; set;} 
        public bool IsActive {get; set;}

    }

    internal class Product
    {
        public string ProductName {get;set;} = string.Empty;
       public  int oldPrice{get;set;}
        public int currentPrice {get;set;}    

    }



    internal class Program
    {   
        public delegate void Mydelegate (int num, int num2);

        Program()
        {
             string s = "Hai let learn LINQ";
             Console.WriteLine(s.CountWords());


             int num = 10012;
              Console.WriteLine(num.TaxCalculation());

            List<User> users = new List<User> {
                    new User { Name = "Alice", Age = 25, IsActive = true },
                     new User { Name = "Bob", Age = 15, IsActive = true },
                    new User { Name = "Charlie", Age = 30, IsActive = false },
                    new User { Name = "David", Age = 40, IsActive = true }
                };
            users.CusomterFilter(c=>c.Age>25,c=>Console.WriteLine("Filter found names are : "+c.Name));


             List<Product> products = new List<Product> {
                    new Product { ProductName = "Apple", oldPrice = 90, currentPrice = 100 },
                     new Product { ProductName = "Orange", oldPrice = 100, currentPrice = 250 },
                    new Product { ProductName = "Grapes", oldPrice = 200, currentPrice = 300 },
                      new Product { ProductName = "Lemon", oldPrice = 140, currentPrice = 180 }
                };

                IEnumerable<int> items = products.ProductFilter(p=>{p.oldPrice-p.currentPrice});

        }

       static void Add (int number1,int number2)
        {
            Console.WriteLine($"Add :  {number1+number2}");
        }

        void sub (int number1,int number2)
        {
            Console.WriteLine($"sub :  {number1+number2}");
        }
        void divide (int number1,int number2)
        {
            Console.WriteLine($"div :  {number1+number2}");
        }
        

        static void Main(string[] args)
        {
            new Program();

            Mydelegate CustomDelegate = Add;
            CustomDelegate(2,43);
            
        }
    }
}