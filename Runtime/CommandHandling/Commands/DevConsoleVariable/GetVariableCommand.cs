namespace DevConsole.CommandHandling.Commands.DevConsoleVariable
{
    /// <summary>
    /// Logs variable and its value
    /// param: variable to modify
    /// </summary>
    public class GetVariableCommand : IDevConsoleCommand
    {
        public string GetCommandHeaderPattern => @"^\/get(?:\s+([a-zA-Z_][a-zA-Z0-9_]*))?$";
        
        public void ExecuteCommandAction(object[] parameters)
        {
            if (parameters.Length == 0)
            {
                UnityEngine.Debug.LogError("No variable name provided!");
                return;
            }
            
            string variableName = (string) parameters[0];
            
            if (DevConsoleVariableRepository.TryGetValue(variableName, out object value, out string response))
            {
                UnityEngine.Debug.Log($"{variableName} [{value.GetType()}] = {value}");
            }
            else
            {
                UnityEngine.Debug.LogError($"Variable '{variableName}' {response}");
            }
        }
    }
}