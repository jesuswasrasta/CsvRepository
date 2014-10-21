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
		[Test(Description = "Customer loaded from csv is the same as one created programmatically")]
		public void LoadedCustomerSameAsCreatedCustomer()
		{
			const string lineExpected = "Niklaus;Wirth;02/15/1934;\r\n";
			var customer = new CustomerRecord();
			customer.Load(lineExpected, ';');

			var expectedCustomer = new CustomerRecord
			{
				Firstname = "Niklaus",
				Lastname = "Wirth",
				BirthDay = "02/15/1934"
			};

			Assert.AreEqual(expectedCustomer, customer);
		}


		#endregion
	}
	// ReSharper restore InconsistentNaming
}