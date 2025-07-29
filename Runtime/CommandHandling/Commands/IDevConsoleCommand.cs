namespace DevConsole.CommandHandling.Commands
{
    public interface IDevConsoleCommand
    {
        /// <summary>
        /// Command name as string, eg. "/getAll"
        /// </summary>
        string GetCommandHeaderPattern { get; }
        
        /// <summary>
        /// Stuff that will happen when the command is being executed
        /// </summary>
        /// <param name="parameters"></param>
        void ExecuteCommandAction(object[] parameters);
    }
}