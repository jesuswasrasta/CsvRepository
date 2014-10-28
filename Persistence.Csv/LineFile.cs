#region Usings
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Threading;


#endregion


namespace Persistence.Csv
{
	/// <summary>
	/// Simplify acces to a text file as a collection of lines to add or delete.
	/// </summary>
	public class LineFile : ILineFile
	{
		#region Fields
		private static readonly object _fileAccessLocker = new object();
		readonly EventWaitHandle _eventWaitHandle;
		private readonly string _fullpath;
		private bool _disposed;
		#endregion


		#region Constructors
		public LineFile(string fullpath)
		{
			_fullpath = fullpath;
			_eventWaitHandle = new EventWaitHandle(true, EventResetMode.AutoReset, new FileInfo(_fullpath).Name);
		}
		#endregion


		#region Public Methods
		public void AddLine(string line)
		{
			if (line == null)
			{
				return;
			}
			AddLines(new[] {line});
		}

		public void AddLines(string[] lines)
		{
			if (lines == null || !lines.Any())
			{
				return;
			}

			_eventWaitHandle.WaitOne();
			lock (_fileAccessLocker)
			{
				using (var fileStream = new FileStream(_fullpath, FileMode.OpenOrCreate, FileSystemRights.FullControl, FileShare.Read, 4096, FileOptions.None))
				{
					fileStream.Seek(0, SeekOrigin.End); //Accoda...
					using (var sw = new StreamWriter(fileStream))
					{
						sw.AutoFlush = true;
						foreach (var line in lines)
						{
							sw.Write("{0}{1}", line, Environment.NewLine);
						}
					}
				}
			}
			_eventWaitHandle.Set();
		}

		public IList<string> ReadAllLines()
		{
			_eventWaitHandle.WaitOne();
			var lines = new List<string>();
			lock (_fileAccessLocker)
			{
				using (var fileStream = new FileStream(_fullpath, FileMode.OpenOrCreate, FileSystemRights.Read, FileShare.Read, 4096, FileOptions.SequentialScan))
				{
					using (var sr = new StreamReader(fileStream))
					{
						string line;
						while ((line = sr.ReadLine()) != null)
						{
							lines.Add(line);
						}
					}
				}
			}
			_eventWaitHandle.Set();
			return lines;
		}

		public string ReadLine(int index)
		{
			_eventWaitHandle.WaitOne();
			string line = string.Empty;
			lock (_fileAccessLocker)
			{
				var lines = File.ReadAllLines(_fullpath);
				if (lines.Length > 0)
				{
					line = lines.Skip(index).Take(1).First();	
				}
			}
			_eventWaitHandle.Set();
			return line;
		}

		public void DeleteLine(string line)
		{
			if (line == null)
			{
				return;
			}
			DeleteLines(new[] {line});
		}

		public void DeleteLines(string[] lines)
		{
			if (lines == null || !lines.Any())
			{
				return;
			}

			var existingLines = ReadAllLines();
			var linesToWrite = existingLines.Where(existingLine => !lines.Contains(existingLine)).ToList();
			
			_eventWaitHandle.WaitOne();
			lock (_fileAccessLocker)
			{
				using (var fileStream = new FileStream(_fullpath, FileMode.Open, FileSystemRights.FullControl, FileShare.ReadWrite, 4096, FileOptions.SequentialScan))
				{
					fileStream.SetLength(0); //Empty file
					using (var sw = new StreamWriter(fileStream))
					{
						sw.AutoFlush = true;
						foreach (var lineToWrite in linesToWrite)
						{
							sw.Write("{0}{1}", lineToWrite, Environment.NewLine);
						}
					}
				}
			}
			_eventWaitHandle.Set();
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
				}
				// Unmanaged resources are released here.
				_disposed = true;
			}
		}

		~LineFile()
		{
			Dispose(false);
		}
		#endregion
	}
}
