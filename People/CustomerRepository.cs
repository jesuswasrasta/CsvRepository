#region Usings
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using Persistence.Csv;


#endregion


namespace People
{
	/// <summary>
	/// A very bare CSV repository fo <see cref="CustomerRecord"/>.
	/// <remarks>
	///		This in a very dumb implementation, just for simple and small CSV files.
	///		It lacks header row management.
	/// </remarks>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class CustomerRepository<T> : ICsvRepository<T> where T : CsvRecord, new()
	{
		#region Costants
		private const int BufferSize = 64 * 1024;
		#endregion
		

		#region Fields
		private readonly string _csvFullpath;
		private readonly char _separator;
		private readonly IList<T> _entities;

		private bool _disposed = false;
		private bool _opened = false;
		private static object _openLock = new object();
		#endregion


		#region Constructors
		public CustomerRepository(string csvFullpath, char separator)
		{
			if (csvFullpath == null)
			{
				throw new ArgumentNullException("csvFullpath");
			}
			_csvFullpath = csvFullpath;
			_separator = separator;
			_entities = new List<T>();
		}
		#endregion


		#region Public Methods
		public void Open()
		{
			if (_opened)
			{
				return;
			}
			lock (_openLock)
			{
				if (_opened)
				{
					return;
				}

				//nando20140827: leggo tutto il csv e carico le entità in una collezione locale
				//BUG! 20140827: devo skippare la prima riga nel caso contenesse le intestazioni delle colonne
				//string[] lines = File.ReadAllLines(_csvFullpath);
				//foreach (var line in lines)
				//{
				//	var entity = new T();
				//	var loaded = entity.Load(line, _separator);
				//	if (loaded)
				//	{
				//		_entities.Add(entity);
				//	}
				//}
				//_opened = true;

				using (var sr = new StreamReader(_csvFullpath, Encoding.ASCII))
				{
					string line = sr.ReadLine();
					var entity = new T();
					var loaded = entity.Load(line, _separator);
					if (loaded)
					{
						_entities.Add(entity);
					}
				}
				_opened = true;
			}
		}

		public void Store(T entity)
		{
			if (!_opened)
			{
				throw new RepositoryClosedException("Il repository è chiuso.");
			}

			if (_entities.Contains(entity))
			{
				_entities.Remove(entity);
			}
			_entities.Add(entity);
		}

		public T Select(T entity)
		{
			if (!_opened)
			{
				throw new RepositoryClosedException("Il repository è chiuso.");
			}

			return _entities.FirstOrDefault(e => e.Equals(entity));
		}

		public IQueryable<T> Select(Func<T, bool> func)
		{
			if (!_opened)
			{
				throw new RepositoryClosedException("Il repository è chiuso.");
			}

			var result = _entities.Where(func);
			return result.AsQueryable();
		}

		public void Delete(T entity)
		{
			if (!_opened)
			{
				throw new RepositoryClosedException("Il repository è chiuso.");
			}

			if (_entities.Contains(entity))
			{
				_entities.Remove(entity);
			}
		}

		public void Close()
		{
			if (!_opened)
			{
				return;
			}
			lock (_openLock)
			{
				if (!_opened)
				{
					return;
				}


				//nando20140828: File.WriteAllLines dovrebbe essere Lazy by design, verifica

				//TODO 20140827: salva le entità nel file di testo, salva e chiudi il file
				using (var sw = new StreamWriter(_csvFullpath, false, Encoding.ASCII, BufferSize))
				{
					foreach (var entity in _entities)
					{
						string line = entity.ToLine(_separator);
						sw.Write(line);
					}
					sw.Flush();
				}
				_opened = false;
			}
		}

		public void Dispose()
		{
			Close();
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion


		#region Private Methods
		private void Update(T entity)
		{
			throw new NotImplementedException();
		}

		private void Insert(T entity)
		{
			var sb = new StringBuilder();
			var propertyInfos = entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
			var properties = new List<PropertyInfo>(propertyInfos);
			var comparer = new FieldAttributeComparer();
			properties.Sort(comparer);

			foreach (PropertyInfo propertyInfo in properties)
			{
				var attributes = propertyInfo.GetCustomAttributes(typeof(CsvAttribute), false);
				if (!attributes.Contains(typeof (CsvAttribute)))
				{
					continue;
				}
				var propertyValue = propertyInfo.GetValue(entity, null) ?? string.Empty;
				sb.AppendFormat("{0}{1}", propertyValue, _separator);
			}
			sb.Append(Environment.NewLine);
			
			using (var sw = new StreamWriter(_csvFullpath, true))
			{
				sw.Write(sb.ToString());
			}
		}
		#endregion


		#region IDisposable implementation
		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					//TODO 20140827: scrivi i record nel file csv e chiudilo
				}
				// Unmanaged resources are released here.

				_disposed = true;
			}
		}

		~CustomerRepository()
		{
			Dispose(false);
		}
		#endregion
	}
}