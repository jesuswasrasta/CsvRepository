#region Usings
using System.Collections.Generic;
using System.Reflection;


#endregion


namespace Persistence.Csv
{
	public class FieldAttributeComparer : IComparer<PropertyInfo>
	{
		public int Compare(PropertyInfo x, PropertyInfo y)
		{
			var xAttr = FieldsHelper.GetAttribute(x);
			var yAttr = FieldsHelper.GetAttribute(y);

			if (xAttr == null || yAttr == null)
			{
				return 0;
			}

			if (xAttr.Order == yAttr.Order)
			{
				return 0;
			}
			return (xAttr.Order > yAttr.Order) ? 1 : -1;
		}
	}
}