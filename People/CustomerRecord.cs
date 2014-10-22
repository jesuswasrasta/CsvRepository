#region Usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Persistence.Csv;


#endregion


namespace People
{
	/// <summary>
	/// Defines a <see cref="CsvRecord"/> for an hypotetical customer.
	/// </summary>
	public class CustomerRecord : CsvRecord
	{
		#region Properties
		[Field(0, "Firstname of the customer")]
		public string Firstname { get; set; }

		[Field(1, "Lastname of the customer")]
		public string Lastname { get; set; }

		[Field(2, "Birthday of the cusotmer")]
		public string BirthDay { get; set; }
		#endregion


		#region Constructors
		public CustomerRecord()
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

			return sb.ToString();
		}
		#endregion


		#region Overridden Methods
		//See: http://msdn.microsoft.com/en-us/library/ms173147%28VS.80%29.aspx
		public override bool Equals(object obj)
		{
			//var customerRecord = obj as CustomerRecord;
			//return customerRecord != null &&
			//	   customerRecord.Firstname != null &&
			//	   customerRecord.Firstname.Equals(Firstname) &&
			//	   customerRecord.Lastname != null &&
			//	   customerRecord.Lastname.Equals(Lastname) &&
			//	   customerRecord.BirthDay != null &&
			//	   customerRecord.BirthDay.Equals(BirthDay);

			var customerRecord = obj as CustomerRecord;
			return customerRecord != null && customerRecord.Id.Equals(Id);
		}

		public override int GetHashCode()
		{
			//see http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
			unchecked // Overflow is fine, just wrap
			{
				//int hash = 17;
				//// Remeber nullity checks, of course :)
				//hash = hash * 23 + (string.IsNullOrEmpty(Firstname) ? 0 : Firstname.GetHashCode());
				//hash = hash * 23 + (string.IsNullOrEmpty(Lastname) ? 0 : Lastname.GetHashCode());
				//hash = hash * 23 + (string.IsNullOrEmpty(BirthDay) ? 0 : BirthDay.GetHashCode());
				//return hash;

				int hash = 17;
				hash = hash * 23 + Id.GetHashCode();
				return hash;
			}
		}
		#endregion
	}
}