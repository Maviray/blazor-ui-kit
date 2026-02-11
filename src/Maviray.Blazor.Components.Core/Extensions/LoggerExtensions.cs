using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using Maviray.Blazor.Components.Core.Options;

namespace Maviray.Blazor.Components.Core.Extensions;

public static class LoggerExtensions
{
    extension(ILogger logger)
    {
        public void Information(string message,
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(
                    "[{MemberName}:{LineNumber}] {Message}",
                    memberName,
                    lineNumber,
                    message);
            }
        }

        public void Information(string message,
            object?[] args,
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            if (!logger.IsEnabled(LogLevel.Information))
            {
                return;
            }

            var enhancedMessage = $"[{{MemberName}}:{{LineNumber}}] {message}";
            var enhancedArgs = new object?[args.Length + 2];
            enhancedArgs[0] = memberName;
            enhancedArgs[1] = lineNumber;
            Array.Copy(args, 0, enhancedArgs, 2, args.Length);

            logger.LogInformation(enhancedMessage, enhancedArgs);
        }

        public void Warning(string message,
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0)
        {

            if (!logger.IsEnabled(LogLevel.Warning))
            {
                return;
            }

            logger.LogWarning(
                "[{MemberName}:{LineNumber}] {Message}",
                memberName,
                lineNumber,
                message);
        }

        public void Warning(string message,
            object?[] args,
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            if (!logger.IsEnabled(LogLevel.Warning))
            {
                return;
            }

            var enhancedMessage = $"[{{MemberName}}:{{LineNumber}}] {message}";
            var enhancedArgs = new object?[args.Length + 2];
            enhancedArgs[0] = memberName;
            enhancedArgs[1] = lineNumber;
            Array.Copy(args, 0, enhancedArgs, 2, args.Length);

            logger.LogWarning(enhancedMessage, enhancedArgs);
        }

        public void Error(Exception? exception,
            string message,
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            if (!logger.IsEnabled(LogLevel.Error))
            {
                return;
            }

            logger.LogError(
                exception,
                "[{MemberName}:{LineNumber}] {Message}",
                memberName,
                lineNumber,
                message);
        }

        public void Error(Exception? exception,
            string message,
            object?[] args,
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            if (!logger.IsEnabled(LogLevel.Error))
            {
                return;
            }

            var enhancedMessage = $"[{{MemberName}}:{{LineNumber}}] {message}";
            var enhancedArgs = new object?[args.Length + 2];
            enhancedArgs[0] = memberName;
            enhancedArgs[1] = lineNumber;
            Array.Copy(args, 0, enhancedArgs, 2, args.Length);

            logger.LogError(exception, enhancedMessage, enhancedArgs);
        }

        public void Error(string message,
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            if (!logger.IsEnabled(LogLevel.Error))
            {
                return;
            }

            logger.LogError(
                "[{MemberName}:{LineNumber}] {Message}",
                memberName,
                lineNumber,
                message);
        }

        /// <summary>
        /// Logs a message if the configured log level allows it as per IMaviComponentOptions
        /// </summary>
        /// <param name="componentOptions">Component options for log level filtering (optional)</param>
        /// <param name="callerType">The type of the calling class</param>
        /// <param name="level">The log level</param>
        /// <param name="message">The log message</param>
        /// <param name="args">Optional message format arguments</param>
        /// <param name="memberName">caller member name</param>
        /// <param name="sourceLineNumber">source line number</param>
        public void LogIfEnabled(IMaviComponentOptions? componentOptions,
            Type callerType,
            LogLevel level,
            string message,
            object?[]? args = null,
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {

            if (componentOptions == null) return;

            // If component options are provided, respect the configured log level
            if (level < componentOptions.ComponentLogLevel) return;

            // Format: [ComponentType.MethodName] Message
            var enrichedMessage = $"[{callerType.Name}.{memberName}][{sourceLineNumber}] {message}";

            if (args != null && args.Length > 0)
            {
                logger.Log(level, enrichedMessage, args);
            }
            else
            {
                logger.Log(level, enrichedMessage);
            }
        }

        /// <summary>
        /// Logs a message if the configured log level allows it as per IMaviComponentOptions
        /// </summary>
        /// <param name="componentOptions">Component options for log level filtering (optional)</param>
        /// <param name="callerType">The type of the calling class</param>
        /// <param name="id">id of the executing component</param>
        /// <param name="message">The log message</param>
        /// <param name="args">Optional message format arguments</param>
        /// <param name="memberName">caller member name</param>
        /// <param name="sourceLineNumber">source line number</param>
        public void LogDebugLifeCycle(IMaviComponentOptions? componentOptions,
            Type callerType,
            string? id,
            string message,
            object?[]? args = null,
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (componentOptions == null) return;

            // If component options are provided, respect the configured log level
            if (!componentOptions.EnableLifecycleLogging) return;

            // Format: [ComponentType.MethodName] Message
            var enrichedMessage = $"[{callerType.Name}.{memberName}][{sourceLineNumber}]-[{id}]: {message}";

            if (args != null && args.Length > 0)
            {
                logger.Log(LogLevel.Information, enrichedMessage, args);
            }
            else
            {
                logger.Log(LogLevel.Information, enrichedMessage);
            }
        }

        /// <summary>
        /// Logs a message if the configured log level allows it as per IMaviComponentOptions
        /// </summary>
        /// <param name="componentOptions">Component options for log level filtering (optional)</param>
        /// <param name="callerType">The type of the calling class</param>
        /// <param name="args">Optional message format arguments</param>
        /// <param name="memberName">caller member name</param>
        /// <param name="sourceLineNumber">source line number</param>
        public void LogDebugLifeCycle(IMaviComponentOptions? componentOptions,
            Type callerType,
            object?[]? args = null,
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (componentOptions == null) return;

            // If component options are provided, respect the configured log level
            if (!componentOptions.EnableLifecycleLogging) return;

            // Format: [ComponentType.MethodName] Message
            var enrichedMessage = $"[Executing: {callerType.Name}.{memberName}][{sourceLineNumber}]";

            if (args != null && args.Length > 0)
            {
                logger.Log(LogLevel.Information, enrichedMessage, args);
            }
            else
            {
                logger.Log(LogLevel.Information, enrichedMessage);
            }
        }

        /// <summary>
        /// Logs a message if the configured log level allows it as per IMaviComponentOptions
        /// </summary>
        /// <param name="componentOptions">Component options for log level filtering (optional)</param>
        /// <param name="id">id of the executing component</param>
        /// <param name="callerType">The type of the calling class</param>
        /// <param name="args">Optional message format arguments</param>
        /// <param name="memberName">caller member name</param>
        /// <param name="sourceLineNumber">source line number</param>
        public void LogDebugLifeCycle(IMaviComponentOptions? componentOptions,
            string? id,
            Type callerType,
            object?[]? args = null,
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (componentOptions == null) return;

            // If component options are provided, respect the configured log level
            if (!componentOptions.EnableLifecycleLogging) return;

            // Format: [ComponentType.MethodName] Message
            var enrichedMessage = $"[Executing: {callerType.Name}.{memberName}][{sourceLineNumber}]-[{id}]";

            if (args != null && args.Length > 0)
            {
                logger.Log(LogLevel.Information, enrichedMessage, args);
            }
            else
            {
                logger.Log(LogLevel.Information, enrichedMessage);
            }
        }
    }
}