using System.ComponentModel;

namespace DEMO_Task_Management_System.Data.Enums
{
    public enum TaskStatusList
    {
        [Description("Not Started")]
        NotStarted,
        [Description("In Progress")]
        InProgress,
        [Description("Completed")]
        Completed,
        // ...
    }
}
