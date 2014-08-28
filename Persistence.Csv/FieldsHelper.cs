#region Usings
using System.Linq;
using System.Reflection;


#endregion


namespace Persistence.Csv
{
	public static class FieldsHelper
	{
		#region Constructors
		static FieldsHelper()
		{
		}
		#endregion


		#region Fields Attributes
		public static FieldAttribute GetAttribute(PropertyInfo propertyInfo)
		{
			var attributes = propertyInfo.GetCustomAttributes(typeof (FieldAttribute), false);
			return attributes.OfType<FieldAttribute>().Select(attribute => attribute).FirstOrDefault();
		}
		#endregion
	}
}