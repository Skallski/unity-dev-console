namespace DevConsole.CommandHandling.Commands.DevConsoleVariable
{
    /// <summary>
    /// Logs all dev console variables
    /// </summary>
    public class GetAllVariablesCommand : IDevConsoleCommand
    {
        public string GetCommandHeaderPattern => "/getAll";
        
        public void ExecuteCommandAction(object[] parameters)
        {
            (string, System.Type)[] fields = DevConsoleVariableRepository.GetAllFields();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            if (fields.Length == 0)
            {
                UnityEngine.Debug.LogError("No variables found!");
                return;
            }
            
            for (int i = 0; i < fields.Length; i++)
            {
                (string fieldName, System.Type fieldType) = fields[i];
                sb.Append($"{fieldName} [{fieldType}]{(i + 1 < fields.Length ? ", " : "")}");
            }

            UnityEngine.Debug.Log(sb.ToString());
        }
    }
}