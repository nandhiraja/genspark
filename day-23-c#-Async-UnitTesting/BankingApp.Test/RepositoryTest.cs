using BankingAPI.Contexts;
using BankingAPI.Interfaces;
using BankingAPI.Models;
using BankingAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;


namespace BankingApp.Test;


       public class RepositoryTest
    {
        IRepository<int,Customer> customerRepository;
        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<BankingContext>().UseInMemoryDatabase("BankingDb").Options;
            BankingContext bankingContext = new BankingContext(options);
            customerRepository = new Repository<int, Customer>(bankingContext);
        }

        [Test]
        public async Task AddCustomerPassTest()
        {
            Customer customer = new Customer
            {
                Id = 2,
                Name = "John Doe",
                Email = "john@test.com",
                DateOfBirth = new DateTime(2000, 1, 1),
                Phone = "1234567890",
                Status = "Active",
                Username = null
            };
            var result =  customerRepository.Create(customer);
           
            Assert.That(result.Id, Is.EqualTo(customer.Id));

        }

       [Test]
        public async Task GetCustomerPassTest()
        {
            //Arrange
            Customer customer = new Customer
            {
                Id = 2,
                Name = "John Doe",
                Email = "john@test.com",
                DateOfBirth = new DateTime(2000, 1, 1),
                Phone = "1234567890",
                Status = "Active",
                Username = null
            };
            var customer1 =  customerRepository.Create(customer);


            //Action
            var result =  customerRepository.Get(customer1.Id);

            //Assert
            Assert.That(result.Name, Is.EqualTo(customer1.Name));

        }

         [Test]
        public async Task GetCustomerFailTest()
        {
            //Arrange
            Customer customer = new Customer
            {
                Id = 2,
                Name = "John Doe",
                Email = "john@test.com",
                DateOfBirth = new DateTime(2000, 1, 1),
                Phone = "1234567890",
                Status = "Active",
                Username = null
            };
            var customer1 =  customerRepository.Create(customer);


            //Action
            var result =  customerRepository.Get(customer1.Id);

            //Assert
            Assert.That(result.Name, Is.EqualTo("Ragul"));

        }

}
