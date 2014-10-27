#region Usings
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;


#endregion


namespace Persistence.Csv
{
	/// <summary>
	///     Defines a record in a CSV file.
	/// </summary>
	public abstract class CsvRecord : EntityBase
	{
		public string GetHeader(char separator)
		{
			var propertyInfos = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
			var properties = new List<PropertyInfo>(propertyInfos);
			var comparer = new FieldAttributeComparer();
			properties.Sort(comparer);

			var header = new StringBuilder();
			foreach (PropertyInfo propertyInfo in properties)
			{
				var fieldAttribute = propertyInfo.GetCustomAttributes(typeof (FieldAttribute), false).FirstOrDefault() as FieldAttribute;
				if (fieldAttribute != null)
				{
					header.AppendFormat("{0}{1}", propertyInfo.Name, separator);
				}
			}
			return header.ToString();
		}

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