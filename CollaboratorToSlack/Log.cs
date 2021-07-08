using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;

namespace CollaboratorToSlack
{
	public class Log : IDisposable
	{
		#region Privates 

		private static string _logPath;
		private static LogLevel _logLevel = LogLevel.Disabled;
		private static FileStream _fs;
		private static FileInfo _fi;
		private static StreamWriter _sw;

		#endregion

		#region C'tor
		public Log()
		{
			NameValueCollection appSettings = ConfigurationManager.AppSettings;
			if (appSettings["log_path"] == null)
			{
				return;
			}

			if (appSettings["log_level"] == null)
			{
				return;
			}

			_logPath = appSettings["log_path"];
			if (!_logPath.EndsWith("\\"))
			{
				_logPath = string.Concat(_logPath, "\\");
			}

			if (!Enum.TryParse(appSettings["log_level"], out _logLevel))
			{
				_logLevel = LogLevel.Disabled;
				_logPath = string.Empty;
				return;
			}

			OpenFile();
		}

		~Log()
		{
			CloseFile();
		}

		#endregion


		public void Dispose()
		{

		}

		public void LogMessage(LogLevel level, string message, string method, int messageId, string stackTrace = "")
		{
			if (_logLevel == LogLevel.Disabled || level == LogLevel.Disabled)
			{
				return;
			}

			if (string.IsNullOrWhiteSpace(message))
			{
				return;
			}

			if (!_fs.CanWrite)
			{
				return;
			}

			_sw.WriteLine($"{GetDateTime()}\t{level}\t{method}\t{messageId}\t{message}");
			if (level is LogLevel.Critical or LogLevel.Error && !string.IsNullOrWhiteSpace(stackTrace))
			{
				_sw.WriteLine(stackTrace);
			}
		}

		private static void OpenFile()
		{
			if (string.IsNullOrWhiteSpace(_logPath))
			{
				_logLevel = LogLevel.Disabled;
				return;
			}

			string fileName = GetFileName();
			_fi = new FileInfo(fileName);
			_fs = new FileStream(fileName, FileMode.CreateNew);
			_sw = new StreamWriter(_fs);
		}

		private static void CloseFile()
		{
			if (!_fs.CanWrite)
			{
				return;
			}

			try
			{
				_sw.Flush();
				_fs.Flush();

				_sw.Close();
				_fs.Close();
			}
			catch
			{
				//nothing to do here
			}
		}

		private static string GetFileName()
		{
			return $"{_logPath}{DateTime.UtcNow:yyyyMMdd-HHmmss-ff}.log";
		}

		private static string GetDateTime()
		{
			return $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.ff}";
		}
	}
}
