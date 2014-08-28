#region Usings
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using Persistence;
using Persistence.Csv;


#endregion


namespace Farmaci
{
	public class FarmacoRecord : CsvRecord
	{
		#region Properties
		public override int Id
		{
			get { return GetHashCode(); }
		}

		[Field(0, "Minsan del prodotto")]
		public string Minsan { get; set; }

		[Field(1, "Descrizione del farmaco in Banca Dati Federfarma")]
		public string Description { get; set; }

		[Field(2, "Targatura della scatola")]
		public string Targa { get; set; }
		#endregion


		#region Constructors
		public FarmacoRecord()
		{
		}
		#endregion


		#region Public Methods
		public override bool Load(string csvRecordString, char separator)
		{
			if (csvRecordString == null)
			{
				return false;
			}
			var values = csvRecordString.Split(separator);

			var propertyInfos = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
			var properties = new List<PropertyInfo>(propertyInfos);
			var comparer = new FieldAttributeComparer();
			properties.Sort(comparer);

			foreach (PropertyInfo propertyInfo in properties)
			{
				var fieldAttribute = propertyInfo.GetCustomAttributes(typeof(FieldAttribute), false).FirstOrDefault() as FieldAttribute;
				if (fieldAttribute != null)
				{
					propertyInfo.SetValue(this, values[fieldAttribute.Order], null);
				}
			}
			return true;
		}

		public override string ToLine(char separator)
		{
			var sb = new StringBuilder();
			var propertyInfos = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
			var properties = new List<PropertyInfo>(propertyInfos);
			var comparer = new FieldAttributeComparer();
			properties.Sort(comparer);

			foreach (PropertyInfo propertyInfo in properties)
			{
				var fieldAttribute = propertyInfo.GetCustomAttributes(typeof(FieldAttribute), false).FirstOrDefault() as FieldAttribute;
				if (fieldAttribute != null)
				{
					var propertyValue = propertyInfo.GetValue(this, null) ?? string.Empty;
					sb.AppendFormat("{0}{1}", propertyValue, separator);
				}
			}
			sb.Append(Environment.NewLine);

			return sb.ToString();
		}
		#endregion
	}
}