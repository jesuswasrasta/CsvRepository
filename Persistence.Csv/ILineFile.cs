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
		IList<string> ReadAllLines();

		void DeleteLine(string line);

		void DeleteLines(string[] lines);

		void AddLine(string line);

		void AddLines(string[] lines);
	}
}