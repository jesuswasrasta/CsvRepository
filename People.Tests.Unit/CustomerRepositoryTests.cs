#region Usings
using System;
using System.IO;
using System.Linq;

using Moq;

using NUnit.Framework;

using Persistence.Csv;


#endregion


namespace People.Tests.Unit
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


		#region Open()
		[Test]
		public void UsingTheRepo_WithoutOpeningItBefore_ThrowsException()
		{
			var customerMock = new Mock<CustomerRecord>();
			var lineFileMock = new Mock<ILineFile>();
			var repo = new CustomerRepository<CustomerRecord>(lineFileMock.Object, ';');

			Assert.Throws<RepositoryException>(() => repo.Store(customerMock.Object));
			Assert.Throws<RepositoryException>(() => repo.Delete(customerMock.Object));
			Assert.Throws<RepositoryException>(() => repo.Select(customerMock.Object));
			Assert.Throws<RepositoryException>(() => repo.Select(new Func<CustomerRecord, bool>(c => true)));
		}
		#endregion
	}
	// ReSharper restore InconsistentNaming
}