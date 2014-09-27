#region Usings
using System.Collections.Generic;


#endregion


namespace Persistence.Csv
{
	public abstract class CsvRecord : EntityBase
	{
		public virtual List<ICsvField> Fields { get; protected set; }
	}
}