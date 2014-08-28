#region Usings
using System;
using System.IO;
using System.Linq;

using NUnit.Framework;


#endregion


namespace Farmaci.Tests
{
	// ReSharper disable InconsistentNaming
	[TestFixture]
	public class FarmaciRepositoryTests
	{
		//NUnit Cheat Sheet: http://lukewickstead.wordpress.com/2013/01/16/nunit-cheat-sheet/
		[SetUp]
		public void SetUp()
		{
		}

		[TearDown]
		public void TearDown()
		{
		}


		#region Store()
		[Test(Description = "Inserisce il record di un farmaco in un file csv")]
		public void InserimentoDiUnFarmaco()
		{
			const string lineExpected = "020766010;PLASIL*INET 5F 2ML 10MG/2ML;1234567890123456;\r\n";
			var farmaco = new FarmacoRecord
			{
				Minsan = "020766010",
				Description = "PLASIL*INET 5F 2ML 10MG/2ML",
				Targa = "1234567890123456"
			};

			var csvFile = Path.GetTempFileName();
			var repo = new FarmaciRepository<FarmacoRecord>(csvFile, ';');
			repo.Open();
			
			repo.Store(farmaco);

			repo.Close();

			var line = File.ReadAllText(csvFile);
			Assert.AreEqual(lineExpected, line);
		}

		[Test(Description = "Inserisce due record di farmaci in un file csv")]
		public void InserimentoDi2Farmaci()
		{
			const string lineExpected1 = "020766010;PLASIL*INET 5F 2ML 10MG/2ML;1234567890123456;";
			var farmaco1 = new FarmacoRecord
			{
				Minsan = "020766010",
				Description = "PLASIL*INET 5F 2ML 10MG/2ML",
				Targa = "1234567890123456"
			};
			const string lineExpected2 = "027849102;CEDAX*OS GRAT SOSP 36MG/ML 15G;9999999999999999;";
			var farmaco2 = new FarmacoRecord
			{
				Minsan = "027849102",
				Description = "CEDAX*OS GRAT SOSP 36MG/ML 15G",
				Targa = "9999999999999999"
			};

			var csvFile = Path.GetTempFileName();
			var repo = new FarmaciRepository<FarmacoRecord>(csvFile, ';');
			repo.Open();

			repo.Store(farmaco1);
			repo.Store(farmaco2);

			repo.Close();

			var lines = File.ReadAllLines(csvFile);
			Assert.AreEqual(lineExpected1, lines[0]);
			Assert.AreEqual(lineExpected2, lines[1]);
		}

		[Test(Description = "Modifica il record di un farmaco in un file csv")]
		public void AggiornamentoDiUnFarmaco()
		{
			const string lineExpected = "020766010;PLASIL*INET 5F 2ML 10MG/2ML;9999999999999999;";
			var farmaco = new FarmacoRecord
			{
				Minsan = "020766010",
				Description = "PLASIL*INET 5F 2ML 10MG/2ML",
				Targa = "1234567890123456"
			};

			var csvFile = Path.GetTempFileName();
			var repo = new FarmaciRepository<FarmacoRecord>(csvFile, ';');
			repo.Open();

			repo.Store(farmaco);

			//Modifico il farmaco
			farmaco.Targa = "9999999999999999";
			
			//Lo risalvo
			repo.Store(farmaco);

			repo.Close();
			
			var lines = File.ReadAllLines(csvFile);
			Assert.IsTrue(lines.Length == 1);
			Assert.AreEqual(lineExpected, lines[0]);
		}

		#endregion


		#region Select()
		[Test(Description = "Prova ad inserire il record di un farmaco e poi a recuperarlo")]
		public void InserimentoELetturaDiUnFarmaco()
		{
			var farmaco = new FarmacoRecord
			{
				Minsan = "020766010",
				Description = "PLASIL*INET 5F 2ML 10MG/2ML",
				Targa = "1234567890123456"
			};

			var csvFile = Path.GetTempFileName();
			var repo = new FarmaciRepository<FarmacoRecord>(csvFile, ';');
			repo.Open();

			repo.Store(farmaco);

			var func = new Func<FarmacoRecord, bool>(f => f.Minsan == "020766010");
			var farmacoResult = repo.Select(func);

			Assert.AreEqual(farmaco, farmacoResult.FirstOrDefault());
		}
		#endregion

	}
	// ReSharper restore InconsistentNaming
}