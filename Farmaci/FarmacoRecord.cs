#region Usings
using System.Collections.Generic;

using Persistence;
using Persistence.Csv;


#endregion


namespace Farmaci
{
	public class FarmacoRecord : CsvRecord
	{
		#region Properties
		[Ignored]
		public override int Id
		{
			get { return GetHashCode(); }
		}

		public string Minsan { get; set; }

		public string Description { get; set; }

		public string Targa { get; set; }

		[Ignored]
		public override List<ICsvField> Fields { get; protected set; }
		#endregion


		#region Constructors
		public FarmacoRecord()
		{
		}
		#endregion
	}
}