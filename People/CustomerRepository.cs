#region Usings
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Timers;

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
	public class CustomerRepository<T> : ICsvRepository<T> where T : CustomerRecord, new()
	{
		#region Costants
		private const int BufferSize = 64 * 1024;
		#endregion
		

		#region Fields
		private readonly ILineFile _lineFile;
		private readonly char _separator;
		private readonly bool _hasHeader;
		private readonly IDictionary<T, RecordStatus> _entities;

		private bool _disposed;
		private bool _opened;
		private static readonly object _openLock = new object();
		//private Timer _timer;
		#endregion


		#region Constructors
		public CustomerRepository(ILineFile lineFile, char separator, bool hasHeader = false)
		{
			if (lineFile == null)
			{
				throw new ArgumentNullException("lineFile");
			}
			_lineFile = lineFile;
			_separator = separator;
			_hasHeader = hasHeader;
			_entities = new Dictionary<T, RecordStatus>();

			//_timer = new Timer();
			//_timer.AutoReset = true;
			//_timer.Interval = 1000;
			//_timer.Elapsed += UpdateFile;
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

				var lines = _lineFile.ReadAllLines();

				foreach (var line in lines)
				{
					if (_hasHeader)
					{
						//Skip first line...
						continue;
					}
					var entity = new T();
					var loaded = entity.Load(line, _separator);
					if (loaded)
					{
						_entities.Add(new KeyValuePair<T, RecordStatus>(entity, RecordStatus.Stored));
					}
				}

				_opened = true;
				//_timer.Start();
			}
		}

		public void Store(T entity)
		{
			if (!_opened)
			{
				throw new RepositoryException("Repository is closed.");
			}

			if (_entities.Keys.Contains(entity))
			{
				_entities.Remove(entity);
			}
			_entities.Add(new KeyValuePair<T, RecordStatus>(entity, RecordStatus.Changed));
		}

		public T Select(T entity)
		{
			if (!_opened)
			{
				throw new RepositoryException("Repository is closed.");
			}

			return _entities.FirstOrDefault(e => e.Key.Equals(entity)).Key;
		}

		public IQueryable<T> Select(Func<T, bool> func)
		{
			if (!_opened)
			{
				throw new RepositoryException("Repository is closed.");
			}

			var result = _entities.Keys.Where(func);
			return result.AsQueryable();
		}

		public void Delete(T entity)
		{
			if (!_opened)
			{
				throw new RepositoryException("Repository is closed.");
			}

			if (_entities.Keys.Contains(entity))
			{
				_entities.Remove(entity);
			}
			_entities.Add(new KeyValuePair<T, RecordStatus>(entity, RecordStatus.Deleted));
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

				//_timer.Stop();
				UpdateFile(this, null);
				
				//BUG: and the header?!

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


		#region IDisposable implementation
		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					Close();
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


		#region Private Methods
		private string GetHeader()
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
					var propertyName = propertyInfo.Name;
					sb.AppendFormat("{0}{1}", propertyName, _separator);
				}
			}
			sb.Append(Environment.NewLine);
			return sb.ToString();
		}


		private void UpdateFile(object sender, ElapsedEventArgs args)
		{
			lock (_openLock)
			{
				//Delete lines from file
				var entitiesToDelete = _entities.Where(e => e.Value == RecordStatus.Deleted || e.Value == RecordStatus.Changed);
				var linesToDelete = entitiesToDelete.Select(pair => pair.Key.ToLine(_separator)).ToList();
				_lineFile.DeleteLines(linesToDelete.ToArray());

				//Adds lines to file
				var entitiesToAdd = _entities.Where(e => e.Value == RecordStatus.New || e.Value == RecordStatus.Changed);
				var linesToAdd = entitiesToAdd.Select(pair => pair.Key.ToLine(_separator)).ToList();
				_lineFile.AddLines(linesToAdd.ToArray());
			}
		}
		#endregion
	}
}