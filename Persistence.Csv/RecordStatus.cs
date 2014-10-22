namespace Persistence.Csv
{
	/// <summary>
	/// A flag to mark the status of a CSV record.
	/// </summary>
	public enum RecordStatus
	{
		/// <summary>
		/// The record in stored, you have to do nothing else
		/// </summary>
		Stored,
		/// <summary>
		/// The record changed since his creation, you have to update it
		/// </summary>
		Changed,
		/// <summary>
		/// New record to add to the file
		/// </summary>
		New,
		/// <summary>
		/// The record has to be deleted form file.
		/// </summary>
		Deleted
	}
}