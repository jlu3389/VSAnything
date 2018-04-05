using System;
using System.IO;
using System.Diagnostics;
namespace SCLCoreCLR
{
	public class Log
	{
		private static StreamWriter m_Stream;

		private static bool m_OpenedDefaultLogFile;

		private static LogCallback m_LogCallback;

		public static void OpenFile(string path)
		{
			Log.OpenFile(path, 8388608);
		}

		public static void OpenFile(string path, int max_size)
		{
			try
			{
				string directoryName = Path.GetDirectoryName(path);
				if (directoryName != "" && !Directory.Exists(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}
				if (Log.m_Stream != null)
				{
					Log.m_Stream.Close();
					Log.m_Stream = null;
				}
				string path2 = path;
				for (int i = 1; i < 100; i++)
				{
					try
					{
						Log.m_Stream = new StreamWriter(path, true);
						break;
					}
					catch (Exception)
					{
					}
					path = Path.Combine(Path.GetDirectoryName(path2), Path.GetFileNameWithoutExtension(path2)) + i + Path.GetExtension(path2);
				}
				FileInfo fileInfo = new FileInfo(path);
				if (fileInfo.Length > (long)max_size)
				{
					Log.m_Stream.Close();
					Log.m_Stream = null;
					char[] buffer = new char[max_size];
					StreamReader expr_AE = new StreamReader(path);
					expr_AE.BaseStream.Seek(fileInfo.Length - (long)max_size, SeekOrigin.Begin);
					expr_AE.Read(buffer, 0, max_size);
					expr_AE.Close();
					Log.m_Stream = new StreamWriter(path);
					Log.m_Stream.Write("Cut...");
					Log.m_Stream.Write(buffer);
				}
			}
			catch (Exception)
			{
			}
		}

		public static void SetLogCallback(LogCallback callback)
		{
			Log.m_LogCallback = callback;
		}

		public static void Write(string message)
		{
			Log.Write(message, LogVerbosity.Normal);
		}

		public static void Write(string message, LogVerbosity verbosity)
		{
			try
			{
				if (verbosity != LogVerbosity.Verbose && Log.m_LogCallback != null)
				{
					Log.m_LogCallback(message);
				}
				if (Log.m_Stream == null && !Log.m_OpenedDefaultLogFile)
				{
					Log.OpenFile("log.txt");
					Log.m_OpenedDefaultLogFile = true;
				}
				if (Log.m_Stream != null)
				{
					message = DateTime.Now.ToString() + ": " + message;
					message = message.Replace("\n", "\r\n");
					Log.m_Stream.Write(message);
				}
				Log.m_Stream.Flush();
			}
			catch (Exception)
			{
			}
		}

		public static void WriteLine(string message, LogVerbosity verbosity)
		{
            Trace.WriteLine(message);
            Log.Write(message + "\n", verbosity);
		}
        public static void logTrace(string message)
        {
            Trace.WriteLine(message);
        }
		public static void WriteLine(string message)
		{
            Trace.WriteLine(message);
            Log.WriteLine(message, LogVerbosity.Normal);
		}
	}
}
