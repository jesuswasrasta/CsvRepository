namespace Persistence.Csv
{
	/// <summary>
	/// Represents a simple repository based on a CSV text file.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface ICsvRepository<T> : IRepository<T> where T : CsvRecord, new()
	{
		/// <summary>
		///     Opens the repository, loading records in memory.
		/// </summary>
		void Open();

		/// <summary>
		///     Closes the repository, saving records to csv file.
		/// </summary>
		void Close();
	}
}