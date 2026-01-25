using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace Maviray.Blazor.Components.Core.Extensions;

public static class LoggerExtensions
{
    extension(ILogger logger)
    {
        public void Information(string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
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
            [CallerFilePath] string filePath = "",
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
            [CallerFilePath] string filePath = "",
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
            [CallerFilePath] string filePath = "",
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
            [CallerFilePath] string filePath = "",
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
            [CallerFilePath] string filePath = "",
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
            [CallerFilePath] string filePath = "",
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
    }
}