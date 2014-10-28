namespace Persistence.Csv
{
	/// <summary>
	///     Only properties decorated with this attribute will be exported to CSV file.
	/// </summary>
	public class FieldAttribute : CsvAttribute
	{
		#region Fields
		/// <summary>
		///     Gets the column order of the resulting CSV line exported.
		/// </summary>
		/// <value>
		///     The order.
		/// </value>
		public ushort Order { get; private set; }

		/// <summary>
		///     Gets the (optional) description of the field.
		/// </summary>
		/// <value>
		///     The description.
		/// </value>
		public string Description { get; private set; }
		#endregion


		#region Constructors
		public FieldAttribute(ushort order, string description = null)
		{
			Order = order;
			Description = description;
		}
		#endregion
	}
}