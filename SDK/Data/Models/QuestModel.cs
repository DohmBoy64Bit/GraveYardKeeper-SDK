using System.Collections.Generic;

namespace GraveSDK.Data.Models
{
    public class QuestModel
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public bool IsOneTime { get; set; }
        public bool IsVisible { get; set; }
        public bool IsStarted { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsFailed { get; set; }
        public string StartScript { get; set; }
        public string SuccessScript { get; set; }
        public string FailScript { get; set; }
    }
}
