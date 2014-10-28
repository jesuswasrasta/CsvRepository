#region Usings
using System;
using System.Collections.Generic;
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
		#region Fields
		private bool _disposed;
		private readonly object _openLock = new object();
		private readonly object _flushLocker = new object();
		private bool _opened;
	
		private readonly ILineFile _lineFile;
		private readonly char _separator;
		private readonly bool _hasHeader;
		private readonly IDictionary<T, RecordStatus> _entities;
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

		public void Flush()
		{
			lock (_flushLocker)
			{
				//Delete lines from file
				var entitiesToDelete = _entities.Where(e => e.Value == RecordStatus.Deleted || e.Value == RecordStatus.Changed);
				var linesToDelete = entitiesToDelete.Select(pair => pair.Key.ToLine(_separator)).ToList();
				_lineFile.DeleteLines(linesToDelete.ToArray());

				//Header
				if (_hasHeader)
				{
					var firstEntityHeader = _entities.FirstOrDefault().Key.GetHeader(_separator);
					var header = _lineFile.ReadLine(0);
					if (string.IsNullOrEmpty(header))
					{
						_lineFile.AddLine(firstEntityHeader);
					}
					else if (header != firstEntityHeader)
					{
						_lineFile.AddLine(firstEntityHeader);
					}
				}

				//Adds lines to file
				var entitiesToAdd = _entities.Where(e => e.Value == RecordStatus.New || e.Value == RecordStatus.Changed);
				var linesToAdd = entitiesToAdd.Select(pair => pair.Key.ToLine(_separator)).ToList();
				_lineFile.AddLines(linesToAdd.ToArray());
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

				Flush();
				_opened = false;
			}
		}

		public void Dispose()
		{
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
		#endregion
	}
}