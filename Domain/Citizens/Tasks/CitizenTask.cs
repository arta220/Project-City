using Domain.Citizens.States;
using Domain.Map;

namespace Domain.Citizens.Tasks
{
    public class CitizenTask
    {
        public bool IsCompleted { get; private set; }
        public CitizenTaskType TaskType { get; }
        public Position Target { get; }
        public CitizenTask(CitizenTaskType taskType, Position target) 
        {
            TaskType = taskType;
            Target = target;
            IsCompleted = false;
        }
        public void MarkAsCompleted() => IsCompleted = true;
    }
}
