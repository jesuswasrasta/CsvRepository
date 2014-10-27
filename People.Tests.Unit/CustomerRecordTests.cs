#region Usings
using System;
using System.IO;
using System.Linq;

using NUnit.Framework;


#endregion


namespace People.Tests.Unit
{
	// ReSharper disable InconsistentNaming
	[TestFixture]
	public class CustomerRecordTests
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


		#region Store()
		[Test(Description = "Customer loaded from csv is NOT the same as one created programmatically: Id is different")]
		public void LoadedCustomer_NOT_TheSameAsCreatedCustomer()
		{
			const char separator = ';';
			const string lineExpected = "Niklaus;Wirth;02/15/1934;\r\n";
			
			var customer = new CustomerRecord();
			
			customer.Load(lineExpected, separator);

			var expectedCustomer = new CustomerRecord
			{
				Firstname = "Niklaus",
				Lastname = "Wirth",
				BirthDay = "02/15/1934"
			};

			Assert.AreEqual(expectedCustomer.GetHeader(separator), customer.GetHeader(separator));

			Assert.AreEqual(expectedCustomer.BirthDay, customer.BirthDay);
			Assert.AreEqual(expectedCustomer.Firstname, customer.Firstname);
			Assert.AreEqual(expectedCustomer.Lastname, customer.Lastname);

			Assert.AreNotEqual(expectedCustomer, customer);
		}
		#endregion
	}
	// ReSharper restore InconsistentNaming
}