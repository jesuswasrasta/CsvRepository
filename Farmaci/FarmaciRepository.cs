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
	public class FarmaciRepository<T> : ICsvRepository<T> where T : CsvRecord, new()
	{
		#region Fields
		private readonly string _csvFullpath;
		private readonly char _separator;
		#endregion


		#region Constructors
		public FarmaciRepository(string csvFullpath, char separator = ',')
		{
			if (csvFullpath == null)
			{
				throw new ArgumentNullException("csvFullpath");
			}
			_csvFullpath = csvFullpath;
			_separator = separator;

			/* TODO:
			 * - apri file
			 * - leggi la prima riga e vedi se ci sono intestazioni campi
			 * - mappa campi?!
			 */
		}
		#endregion


		#region Public Methods
		public void Store(T entity)
		{
			var sb = new StringBuilder();
			var propertyInfos = entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
			var properties = new List<PropertyInfo>(propertyInfos);

			foreach (var propertyInfo in properties)
			{
				var attributes = propertyInfo.GetCustomAttributes(typeof (CsvAttribute), false);
				if(attributes.Contains(new IgnoredAttribute()))
				{
					continue;
				}
				var propertyValue = propertyInfo.GetValue(entity, null) ?? string.Empty;
				sb.AppendFormat("{0}{1}", propertyValue, _separator);
			}
			sb.Append(Environment.NewLine);
			
			using (var sw = new StreamWriter(_csvFullpath))
			{
				sw.Write(sb.ToString());
			}
		}

		public T Select(T entity)
		{
			throw new NotImplementedException();
		}

		public IQueryable<T> Select(Predicate<T> predicate)
		{
			throw new NotImplementedException();
		}

		public void Delete(T entity)
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}