namespace DevConsole.CommandHandling.Commands.DevConsoleVariable
{
    /// <summary>
    /// Sets variable with new value and logs the result
    /// param 1: variable to modify
    /// param 2: value to set
    /// </summary>
    public class SetVariableCommand : IDevConsoleCommand
    {
        public string GetCommandHeaderPattern => @"\/set\s+([a-zA-Z_][a-zA-Z0-9_]*)\s+(\S+)";

        public void ExecuteCommandAction(object[] parameters)
        {
            if (parameters.Length == 0)
            {
                UnityEngine.Debug.LogError("No variable name provided!");
                return;
            }
            
            string variableName = (string) parameters[0];
            object value = parameters[1];
            
            if (DevConsoleVariableRepository.TrySetValue(variableName, value, out string logMessage))
            {
                UnityEngine.Debug.Log($"{variableName} [{value.GetType()}] => {value}");
            }
            else
            {
                UnityEngine.Debug.LogError(logMessage);
            }
        }
    }
}