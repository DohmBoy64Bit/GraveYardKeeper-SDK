using System.Collections.Generic;
using System.Linq;
using GraveSDK.Data.Models;

namespace GraveSDK.Data.Repositories
{
    public class QuestRepository
    {
        private readonly Dictionary<string, QuestModel> _cache;

        public QuestRepository()
        {
            _cache = new Dictionary<string, QuestModel>();
        }

        public QuestModel GetQuest(string questId)
        {
            if (string.IsNullOrEmpty(questId)) return null;
            if (_cache.TryGetValue(questId, out var cached)) return cached;

            QuestDefinition def = GameBalance.me?.GetData<QuestDefinition>(questId);
            if (def == null) return null;

            var model = ConvertToModel(def);
            _cache[questId] = model;
            return model;
        }

        public List<QuestModel> GetAllQuests()
        {
            if (GameBalance.me?.quests_data == null) return new List<QuestModel>();
            return GameBalance.me.quests_data
                .Select(d => GetQuest(d.id))
                .Where(m => m != null)
                .ToList();
        }

        private QuestModel ConvertToModel(QuestDefinition def)
        {
            return new QuestModel
            {
                Id = def.id,
                DisplayName = GJL.L(def.id),
                IsOneTime = def.one_time_quest,
                IsVisible = def.quest_visible,
                IsStarted = MainGame.me?.save?.quests?.IsQuestCurrent(def.id) ?? false,
                IsCompleted = MainGame.me?.save?.quests?.IsQuestSucced(def.id) ?? false,
                IsFailed = MainGame.me?.save?.quests?.IsQuestFaild(def.id) ?? false,
                StartScript = def.start_script,
                SuccessScript = def.success_script,
                FailScript = def.fail_script
            };
        }

        public void ClearCache()
        {
            _cache.Clear();
        }
    }
}
