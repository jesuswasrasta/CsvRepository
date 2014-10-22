#region Usings
using System;


#endregion


namespace Persistence.Csv
{
	public class PersistedRecord<T> where T : CsvRecord
	{
		#region Properties
		public RecordStatus Status { get; private set; }

		public T Record { get; private set; }
		#endregion


		#region Constructors
		public PersistedRecord(T record, RecordStatus status)
		{
			if (record == null)
			{
				throw new ArgumentNullException("record");
			}
			Record = record;
			Status = status;
		}
		#endregion
	}
}