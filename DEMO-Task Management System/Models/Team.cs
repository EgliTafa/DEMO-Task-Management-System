using System.Text.Json.Serialization;

namespace DEMO_Task_Management_System.Models
{
    public class Team
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [JsonIgnore]
        public List<User>? TeamMembers { get; set; }
    }
}
