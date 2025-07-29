using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using DevConsole.CommandHandling.Commands;

namespace DevConsole.CommandHandling
{
    /// <summary>
    /// This class handles command execution
    /// </summary>
    public static class DevConsoleCommandHandler
    {
        private static List<IDevConsoleCommand> ConsoleCommands;
        
        [UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            ConsoleCommands = GetAllConsoleCommands();
        }

        /// <summary>
        /// Checks for matched command from commands list
        /// </summary>
        /// <param name="command"> passed command </param>
        internal static void HandleCommand(string command)
        {
            if (command.StartsWith("/") == false)
            {
                goto InvalidCommand;
            }

            foreach (IDevConsoleCommand consoleCommand in ConsoleCommands)
            {
                Match match = Regex.Match(command, consoleCommand.GetCommandHeaderPattern);
                if (match.Success)
                {
                    object[] parameters = match.Groups
                        .Cast<Group>()
                        .Skip(1)
                        .Where(g => g.Success)
                        .Select(g => (object)g.Value)
                        .ToArray();

                    consoleCommand.ExecuteCommandAction(parameters);
                    return;
                }
            }
            
            InvalidCommand:
            {
                UnityEngine.Debug.LogError($"Invalid command: '{command}'");
            }
        }
        
        private static List<IDevConsoleCommand> GetAllConsoleCommands()
        {
            Type interfaceType = typeof(IDevConsoleCommand);

            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly =>
                {
                    try
                    {
                        return assembly.GetTypes();
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        return ex.Types.Where(t => t != null);
                    }
                })
                .Where(type => interfaceType.IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                .Select(type =>
                {
                    try
                    {
                        return Activator.CreateInstance(type) as IDevConsoleCommand;
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.LogWarning($"Could not create instance of {type?.Name}: {ex.Message}");
                        return null;
                    }
                })
                .Where(cmd => cmd != null)
                .ToList();
        }
    }
}