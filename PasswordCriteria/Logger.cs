using System;

namespace PasswordCriteria
{
	public class ClassIWannaLog
	{
		public Logger<ClassIWannaLog> _logger => new Logger<ClassIWannaLog>();
	}

	public class Logger<T>
	{
		public Logger() { }

		public string Log(string msg) => $"[{typeof(T).Name}] {msg}";
	}

	public class Logger
	{
		private Type _t;

		public Logger(Type t)
		{
			_t = t;
		}
	}
}