using System;
using UnityEngine;

namespace BirdTracks.Game.Core
{
    [CreateAssetMenu(menuName = "Game/Log")]
    public sealed class LogAsset : ScriptableObject, ILogger
    {
        private static readonly object[] _EmptyArgs = new object[0];
        [SerializeField] private string m_Tag = "Default";
        [SerializeField] private Color m_Color = Color.white;
        [SerializeField] private bool m_LogEnabled = true;
        [SerializeField] private LogType m_FilterLogType = LogType.Log;
        private ILogHandler _logHandler;


        ILogHandler ILogger.logHandler
        {
            get { return _logHandler ?? Debug.unityLogger; }
            set { _logHandler = value; }
        }

        public bool logEnabled
        {
            get { return m_LogEnabled; }
            set { m_LogEnabled = value; }
        }

        public LogType filterLogType
        {
            get { return m_FilterLogType; }
            set { m_FilterLogType = value; }
        }


        bool ILogger.IsLogTypeAllowed(LogType logType)
        {
            if (!logEnabled)
            {
                return false;
            }

            if (logType > filterLogType)
            {
                return logType == LogType.Exception;
            }

            return true;
        }

        private static string GetString(object message)
        {
            if (message == null)
            {
                return "Null";
            }

            return message.ToString();
        }

        void ILogger.Log(LogType logType, object message)
        {
            ((ILogHandler)this).LogFormat(logType, null, "{0}", GetString(message));
        }

        void ILogger.Log(LogType logType, object message, UnityEngine.Object context)
        {
            ((ILogHandler)this).LogFormat(logType, context, "{0}", GetString(message));
        }

        void ILogger.Log(LogType logType, string tag, object message)
        {
            ((ILogHandler)this).LogFormat(logType, null, "[{0}] {1}", tag, GetString(message));
        }

        void ILogger.Log(LogType logType, string tag, object message, UnityEngine.Object context)
        {
            ((ILogHandler)this).LogFormat(logType, context, "[{0}] {1}", tag, GetString(message));
        }

        public void Log(object message)
        {
            ((ILogHandler)this).LogFormat(LogType.Log, null, "{0}", GetString(message));
        }

        public void Log(object message, UnityEngine.Object context)
        {
            ((ILogHandler)this).LogFormat(LogType.Log, context, "{0}", GetString(message));
        }

        void ILogger.Log(string tag, object message)
        {
            ((ILogHandler)this).LogFormat(LogType.Log, null, "[{0}] {1}", tag, GetString(message));
        }

        void ILogger.Log(string tag, object message, UnityEngine.Object context)
        {
            ((ILogHandler)this).LogFormat(LogType.Log, context, "[{0}] {1}", tag, GetString(message));
        }

        public void LogError(object message)
        {
            ((ILogHandler)this).LogFormat(LogType.Error, null, "{0}", GetString(message));
        }

        public void LogError(object message, UnityEngine.Object context)
        {
            ((ILogHandler)this).LogFormat(LogType.Error, context, "{0}", GetString(message));
        }

        void ILogger.LogError(string tag, object message)
        {
            ((ILogHandler)this).LogFormat(LogType.Error, null, "[{0}] {1}", tag, GetString(message));
        }

        void ILogger.LogError(string tag, object message, UnityEngine.Object context)
        {
            ((ILogHandler)this).LogFormat(LogType.Error, context, "[{0}] {1}", tag, GetString(message));
        }

        public void LogException(Exception exception)
        {
            ((ILogHandler)this).LogException(exception, null);
        }

        public void LogException(Exception exception, UnityEngine.Object context)
        {
            ((ILogger)this).logHandler.LogException(exception, context);
        }

        void ILogger.LogFormat(LogType logType, string format, params object[] args)
        {
            ((ILogHandler)this).LogFormat(logType, null, format, args);
        }

        void ILogHandler.LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
        {
            if (!((ILogger)this).IsLogTypeAllowed(logType))
            {
                return;
            }

            ((ILogger)this).logHandler.LogFormat(logType, context, $"<color=#{ColorUtility.ToHtmlStringRGB(m_Color)}>[{m_Tag}]</color> {format}", args ?? _EmptyArgs);
        }

        public void LogWarning(object message)
        {
            ((ILogHandler)this).LogFormat(LogType.Warning, null, "{0}", GetString(message));
        }

        public void LogWarning(object message, UnityEngine.Object context)
        {
            ((ILogHandler)this).LogFormat(LogType.Warning, context, "{0}", GetString(message));
        }

        void ILogger.LogWarning(string tag, object message)
        {
            ((ILogHandler)this).LogFormat(LogType.Warning, null, "[{0}] {1}", tag, GetString(message));
        }

        void ILogger.LogWarning(string tag, object message, UnityEngine.Object context)
        {
            ((ILogHandler)this).LogFormat(LogType.Warning, context, "[{0}] {1}", tag, GetString(message));
        }
    }
}