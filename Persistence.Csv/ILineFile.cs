#region Usings
using System;
using System.Collections.Generic;


#endregion


namespace Persistence.Csv
{
	/// <summary>
	/// Read and write to a text file considering a line as a unit measure.
	/// </summary>
	public interface ILineFile : IDisposable
	{
		void AddLine(string line);

		void AddLines(string[] lines);

		void DeleteLine(string line);

		void DeleteLines(string[] lines);

		IList<string> ReadAllLines();

		string ReadLine(int index);
	}
}