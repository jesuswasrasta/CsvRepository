#region Usings
using System;
using System.IO;
using System.Linq;

using NUnit.Framework;

using Persistence.Csv;


#endregion


namespace People.Tests.Integration
{
	// ReSharper disable InconsistentNaming
	[TestFixture]
	public class CustomerRepositoryTests
	{
		#region Setup / TearDown
		//NUnit Cheat Sheet: http://lukewickstead.wordpress.com/2013/01/16/nunit-cheat-sheet/
		[SetUp]
		public void SetUp()
		{
		}

		[TearDown]
		public void TearDown()
		{
		}
		#endregion


		#region Tests
		[Test(Description = "Add a Customer record in a csv file")]
		public void AddingACustomer()
		{
			var customer = new CustomerRecord
			{
				Firstname = "Niklaus",
				Lastname = "Wirth",
				BirthDay = "02/15/1934"
			};

			var csvFile = Path.GetTempFileName();
			var lineFile = new LineFile(csvFile);
			var repo = new CustomerRepository<CustomerRecord>(lineFile, ';');
			repo.Open();
			repo.Store(customer);
			repo.Close();

			var line = File.ReadAllText(csvFile);
			const string lineExpected = "Niklaus;Wirth;02/15/1934;\r\n";

			Assert.AreEqual(lineExpected, line);
		}

		[Test(Description = "Adds two customer records in a csv file")]
		public void Adding2Customers()
		{
			const string lineExpected1 = "Dennis;Ritchie;9/9/1941;";
			var customer1 = new CustomerRecord
			{
				Firstname = "Dennis",
				Lastname = "Ritchie",
				BirthDay = "9/9/1941"
			};
			const string lineExpected2 = "Ken;Thompson;2/4/1943;";
			var customer2 = new CustomerRecord
			{
				Firstname = "Ken",
				Lastname = "Thompson",
				BirthDay = "2/4/1943"
			};

			var csvFile = Path.GetTempFileName();
			var lineFile = new LineFile(csvFile);
			var repo = new CustomerRepository<CustomerRecord>(lineFile, ';');
			repo.Open();

			repo.Store(customer1);
			repo.Store(customer2);

			repo.Close();

			var lines = File.ReadAllLines(csvFile);
			Assert.AreEqual(lineExpected1, lines[0]);
			Assert.AreEqual(lineExpected2, lines[1]);
		}

		[Test(Description = "Modifies a customer record in a csv file")]
		public void UpdatingACustomer()
		{
			var customer = new CustomerRecord
			{
				Firstname = "Bjorn",
				Lastname = "Stroustrup",
				BirthDay = "12/30/1950"
			};

			var csvFile = Path.GetTempFileName();
			var lineFile = new LineFile(csvFile);
			var repo = new CustomerRepository<CustomerRecord>(lineFile, ';');
			repo.Open();
			repo.Store(customer);
			customer.Firstname = "Bjarne";
			repo.Store(customer);
			repo.Close();
			
			var lines = File.ReadAllLines(csvFile);

			const string lineExpected = "Bjarne;Stroustrup;12/30/1950;";

			Assert.IsTrue(lines.Length == 1);
			Assert.AreEqual(lineExpected, lines[0]);
		}

		[Test(Description = "Add a customer to the csv file and then tries to load it from")]
		public void AddAndRetrieveACustomer()
		{
			var customer = new CustomerRecord
			{
				Firstname = "James",
				Lastname = "Gosling",
				BirthDay = "05/19/1955"
			};

			var csvFile = Path.GetTempFileName();
			var lineFile = new LineFile(csvFile);
			var repo = new CustomerRepository<CustomerRecord>(lineFile, ';', true);
			repo.Open();
			repo.Store(customer);
			repo.Flush();

			var func = new Func<CustomerRecord, bool>(f => f.Lastname == "Gosling");
			var customerResult = repo.Select(func).FirstOrDefault();

			Assert.AreEqual(customer, customerResult);
		}
		#endregion
	}
	// ReSharper restore InconsistentNaming
}