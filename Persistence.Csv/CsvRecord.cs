namespace Persistence.Csv
{
	public abstract class CsvRecord : EntityBase
	{
		/// <summary>
		///     Loads a <see cref="CsvRecord" /> form a string line picked up from a CSV file.
		/// </summary>
		/// <returns></returns>
		public abstract bool Load(string csvRecordString, char separator);

		/// <summary>
		///     Exports the current <see cref="CsvRecord" /> to a valid CSV string.
		/// </summary>
		/// <param name="separator">The separator.</param>
		/// <returns></returns>
		public abstract string ToLine(char separator);
	}
}