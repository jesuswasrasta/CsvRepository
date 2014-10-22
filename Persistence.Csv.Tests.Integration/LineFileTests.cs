#region Usings
using System;
using System.Collections.Generic;
using System.IO;

using NUnit.Framework;


#endregion


namespace Persistence.Csv.Tests.Integration
{
	// ReSharper disable InconsistentNaming
	[TestFixture]
	public class LineFileTests
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


		#region AddLines()
		[Test(Description = "Adding a line to a file creates it")]
		public void AddALine_ToAnInexistentFile_CreatesFile()
		{
			var filename = Path.Combine(Path.GetTempPath(), string.Format("{0}.txt", DateTime.Now.Ticks));
			var lineFileWriter = new LineFile(filename);
			lineFileWriter.AddLine("hello!");

			Assert.IsTrue(File.Exists(filename));
		}

		[Test, Timeout(1000), MaxTime(1000), Description("Adding 1000 lines does not take more than a second")]
		public void AddingManyLines()
		{
			var filename = Path.Combine(Path.GetTempPath(), string.Format("{0}.txt", DateTime.Now.Ticks));
			var lineFileWriter = new LineFile(filename);

			var lines = new List<string>();
			for (int i = 0; i < 1000; i++)
			{
				lines.Add(string.Format("This is a medium lenght line, and, incidentally, this is the {0} line :)", i));
			}
			lineFileWriter.AddLines(lines.ToArray());

			Assert.IsTrue(File.Exists(filename));
		}

		[Test, Timeout(5000), MaxTime(5000), Description("Adding 1.000.000 lines does not take more than 5 seconds")]
		public void AddingTooMuchLines()
		{
			var filename = Path.Combine(Path.GetTempPath(), string.Format("{0}.txt", DateTime.Now.Ticks));
			var lineFileWriter = new LineFile(filename);

			var lines = new List<string>();
			for (int i = 0; i < 1000000; i++)
			{
				lines.Add(string.Format("This is a medium lenght line, and, incidentally, this is the {0} line :)", i));
			}
			lineFileWriter.AddLines(lines.ToArray());

			Assert.IsTrue(File.Exists(filename));
		}
		#endregion


		#region ReadAllLines()
		[Test(Description = "Reading all lines")]
		public void ReadAllLines()
		{
			var filename = Path.Combine(Path.GetTempPath(), string.Format("{0}.txt", DateTime.Now.Ticks));
			var lineFileWriter = new LineFile(filename);

			var lines = new List<string>();
			for (int i = 0; i < 1000000; i++)
			{
				lines.Add(string.Format("This is a medium lenght line, and, incidentally, this is the {0} line :)", i));
			}
			lineFileWriter.AddLines(lines.ToArray());

			var readLines = lineFileWriter.ReadAllLines();

			Assert.AreEqual(1000000, readLines.Count);
		}
		#endregion


		#region DeleteLines()
		[Test(Description = "Deleting some lines")]
		public void DeletingSomeLines()
		{
			const int totalLines = 10000;

			var filename = Path.Combine(Path.GetTempPath(), string.Format("{0}.txt", DateTime.Now.Ticks));
			var lineFileWriter = new LineFile(filename);

			var linesToAdd = new List<string>();
			for (int i = 0; i < totalLines; i++)
			{
				linesToAdd.Add(string.Format("This is a medium lenght line, and, incidentally, this is the {0} line :)", i));
			}
			lineFileWriter.AddLines(linesToAdd.ToArray());

			var linesToDelete = new List<string>();
			for (int i = 0; i < totalLines; i += 10)
			{
				linesToDelete.Add(string.Format("This is a medium lenght line, and, incidentally, this is the {0} line :)", i));
			}
			lineFileWriter.DeleteLines(linesToDelete.ToArray());

			var readLines = lineFileWriter.ReadAllLines();

			Assert.AreEqual(totalLines - (totalLines / 10), readLines.Count);
		}
		#endregion

	}
	// ReSharper restore InconsistentNaming
}