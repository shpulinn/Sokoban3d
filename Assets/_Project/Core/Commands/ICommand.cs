namespace _Project.Core.Commands
{
    public interface ICommand
    {
        void Execute();
        void Undo();
    }
}