using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using NUnit.Framework;


namespace Farmaci.Tests
{
	// ReSharper disable InconsistentNaming
	[TestFixture]
	public class FarmaciRepositoryTests
	{
		#region Setup/TearDown
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


		[Test]
		public void InserimentoDiUnFarmaco()
		{
			const string lineExpected = "020766010;PLASIL*INET 5F 2ML 10MG/2ML;1234567890123456";
			var farmaco = new FarmacoRecord
			{
				Minsan = "020766010", 
				Description = "PLASIL*INET 5F 2ML 10MG/2ML", 
				Targa = "1234567890123456"
			};

			var csvFile = Path.GetTempFileName();
			var repo = new FarmaciRepository<FarmacoRecord>(csvFile, ';');

			repo.Store(farmaco);

			var line = File.ReadAllText(csvFile);
			Assert.AreEqual(lineExpected, line);
		}
	}

	// ReSharper restore InconsistentNaming
}