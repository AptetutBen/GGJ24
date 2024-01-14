using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public static class WeekendLogger
{
	public enum LoggingLevel {All, Minimum, ErrorsOnly, Off}

	public static string NETWORKING_SERVER_LOG_COLOUR = "#be7efc";
	public static string LOBBY_LOG_COLOUR = "#FFBF00";

	private static readonly int DEFAULT_SIZE = 12;

	public static bool IsEnabled = true;

	public static int FontSize = 12;

	#region LogFilter

	private static IList<string> m_objects = new List<string>();

	public static void Add(object obj)
	{
		var name = obj.GetType().FullName;
		if (!m_objects.Contains(name))
		{
			m_objects.Add(name);
		}
	}

	public static void Remove(object obj)
	{
		var name = obj.GetType().FullName;
		if (m_objects.Contains(name))
		{
			m_objects.Remove(name);
		}
	}

	public static void Log(object obj, object message)
	{
		var name = obj.GetType().FullName;
		if (m_objects.Contains(name))
		{
			Log("[" + name + "]: " + message);
		}
	}

	public static void LogWarning(object obj, object message)
	{
		var name = obj.GetType().FullName;
		if (m_objects.Contains(name))
		{
			LogWarning("[" + name + "]: " + message);
		}
	}

	public static void LogError(object obj, object message)
	{
		var name = obj.GetType().FullName;
		if (m_objects.Contains(name))
		{
			LogError("[" + name + "]: " + message);
		}
	}

	public static void LogBold(object obj, object message)
	{
		var name = obj.GetType().FullName;
		if (m_objects.Contains(name))
		{
			Log(ApplyStyle("[" + name + "]: " + "<b>" + message + "</b>"));
		}
	}

	public static void LogItalic(object obj, object message)
	{
		var name = obj.GetType().FullName;
		if (m_objects.Contains(name))
		{
			Log(ApplyStyle("[" + name + "]: " + "<i>" + message + "</i>"));
		}
	}

	public static void LogColor(object obj, object message, string color)
	{
		var name = obj.GetType().FullName;
		if (m_objects.Contains(name))
		{
			Log(ApplyStyle("[" + name + "]: " + "<color=" + color + ">" + message + "</color>"));
		}
	}

	#endregion

	#region Log RichText

	public static void LogBold(object message)
	{
		Log(ApplyStyle("<b>" + message + "</b>"));
	}

	public static void LogItalic(object message)
	{
		Log(ApplyStyle("<i>" + message + "</i>"));
	}

	public static void LogColor(object message, string color)
	{
		Log(ApplyStyle("<color=" + color + ">" + message + "</color>"));
	}


	#endregion

	#region Log

	public static void Log(object message)
	{
		if (IsEnabled)
		{
			UnityEngine.Debug.Log(ApplyStyle(message));
		}
	}

	public static void Log(object message, UnityEngine.Object context)
	{
		if (IsEnabled)
		{
			UnityEngine.Debug.Log(ApplyStyle(message), context);
		}
	}
	public static void LogFormat(string format, params object[] args)
	{
		if (IsEnabled)
		{
			UnityEngine.Debug.LogFormat(format, args);
		}
	}
	public static void LogFormat(Object context, string format, params object[] args)
	{
		if (IsEnabled)
		{
			UnityEngine.Debug.LogFormat(context, format, args);
		}
	}

	#endregion


	// Logging for the Network Server
	#region LogNetworkServer

	private static LoggingLevel networkServerLoggingLevel = LoggingLevel.All;

	public static void SetNetworkServerLoggingLevel(LoggingLevel loggingLevel)
	{
		networkServerLoggingLevel = loggingLevel;
	}

	// Normal Logging
	public static void LogNetworkServer(object message)
	{
		if (IsEnabled && networkServerLoggingLevel == LoggingLevel.All)
		{
			Debug.Log(ApplyStyle($"<color={NETWORKING_SERVER_LOG_COLOUR}>{message}</color>"));
		}
	}

	// Warning Logging
	public static void LogNetworkServerWarning(object message)
	{
		if (IsEnabled && (networkServerLoggingLevel == LoggingLevel.All || networkServerLoggingLevel == LoggingLevel.Minimum))
		{
			Debug.LogWarning(ApplyStyle($"<color=\"#c49029\">Network Server Warning: </color> <color={NETWORKING_SERVER_LOG_COLOUR}>{message}</color>"));
		}
	}

	//Error Logging
	public static void LogNetworkServerError(object message)
	{
		if (IsEnabled && networkServerLoggingLevel != LoggingLevel.Off)
		{
			Debug.LogError(ApplyStyle($"<color=\"#c25033\">Network Server Error: </color> <color={NETWORKING_SERVER_LOG_COLOUR}>{message}</color>"));
		}
	}

	#endregion

	#region LogNetworkServer

	private static LoggingLevel lobbyLoggingLevel = LoggingLevel.All;

	public static void SetLobbyLoggingLevel(LoggingLevel loggingLevel)
	{
		lobbyLoggingLevel = loggingLevel;
	}

	// Normal Logging
	public static void LogLobby(object message)
	{
		if (IsEnabled && lobbyLoggingLevel == LoggingLevel.All)
		{
			Debug.Log(ApplyStyle($"<color={LOBBY_LOG_COLOUR}>{message}</color>"));
		}
	}

	// Warning Logging
	public static void LogLobbyWarning(object message)
	{
		if (IsEnabled && (lobbyLoggingLevel == LoggingLevel.All || lobbyLoggingLevel == LoggingLevel.Minimum))
		{
			Debug.LogWarning(ApplyStyle($"<color=\"#c49029\">Lobby Warning: </color> <color={LOBBY_LOG_COLOUR}>{message}</color>"));
		}
	}

	//Error Logging
	public static void LogLobbyError(object message)
	{
		if (IsEnabled && lobbyLoggingLevel != LoggingLevel.Off)
		{
			Debug.LogError(ApplyStyle($"<color=\"#c25033\">Lobby Error: </color> <color={LOBBY_LOG_COLOUR}>{message}</color>"));
		}
	}

	#endregion


	#region Error

	public static void LogError(object message)
	{
		if (IsEnabled)
		{
			UnityEngine.Debug.LogError(ApplyStyle(message));
		}
	}
	public static void LogError(object message, UnityEngine.Object context)
	{
		if (IsEnabled)
		{
			UnityEngine.Debug.LogError(ApplyStyle(message), context);
		}
	}
	public static void LogErrorFormat(string format, params object[] args)
	{
		if (IsEnabled)
		{
			UnityEngine.Debug.LogErrorFormat(format, args);
		}
	}
	public static void LogErrorFormat(Object context, string format, params object[] args)
	{
		if (IsEnabled)
		{
			UnityEngine.Debug.LogErrorFormat(context, format, args);
		}
	}

	#endregion

	#region Warning

	public static void LogWarning(object message)
	{
		if (IsEnabled)
		{
			UnityEngine.Debug.LogWarning(ApplyStyle(message));
		}
	}
	public static void LogWarning(object message, Object context)
	{
		if (IsEnabled)
		{
			UnityEngine.Debug.LogWarning(ApplyStyle(message), context);
		}
	}
	public static void LogWarningFormat(string format, params object[] args)
	{
		if (IsEnabled)
		{
			UnityEngine.Debug.LogWarningFormat(format, args);
		}
	}
	public static void LogWarningFormat(Object context, string format, params object[] args)
	{
		if (IsEnabled)
		{
			UnityEngine.Debug.LogWarningFormat(context, format, args);
		}
	}

	#endregion

	#region Exception

	public static void LogException(System.Exception exception)
	{
		if (IsEnabled)
		{
			UnityEngine.Debug.LogException(exception);
		}
	}
	public static void LogException(System.Exception exception, UnityEngine.Object context)
	{
		if (IsEnabled)
		{
			UnityEngine.Debug.LogException(exception, context);
		}
	}

	#endregion

	private static object ApplyStyle(object message)
	{
		object log = message;
		if (DEFAULT_SIZE != FontSize)
		{
			log = "<size=" + FontSize.ToString() + ">" + message + "</size>";
		}
		log += "\n";

		return log;
	}
}
