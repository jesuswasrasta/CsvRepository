using System.Collections.Generic;


namespace Persistence.Csv
{
	public abstract class CsvRecord : EntityBase
	{
		public virtual List<ICsvField> Fields { get; protected set; }
	}
}