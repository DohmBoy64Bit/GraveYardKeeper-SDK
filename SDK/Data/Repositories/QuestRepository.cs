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

        /// <summary>
        /// Registers a new quest into the game balance.
        /// </summary>
        public void AddQuest(QuestDefinition quest)
        {
            if (quest == null) return;
            string error = GameBalance.me.AddData(quest);
            if (string.IsNullOrEmpty(error))
            {
                GameBalance.me.CreateIDsCache();
            }
        }

        /// <summary>
        /// Forces a quest to be marked as completed in the current save.
        /// </summary>
        public void ForceComplete(string questId)
        {
            if (MainGame.me?.save?.quests == null) return;
            MainGame.me.save.quests.ForceQuestEnd(questId, true);
        }

        /// <summary>
        /// Checks if a quest is currently active.
        /// </summary>
        public bool IsQuestActive(string questId)
        {
            if (MainGame.me?.save?.quests == null) return false;
            return MainGame.me.save.quests.IsQuestCurrent(questId);
        }

        /// <summary>
        /// Checks if a quest has been completed successfully.
        /// </summary>
        public bool IsCompleted(string questId)
        {
            if (MainGame.me?.save?.quests == null) return false;
            return MainGame.me.save.quests.IsQuestSucced(questId);
        }

        /// <summary>
        /// Checks if a quest has been failed.
        /// </summary>
        public bool IsFailed(string questId)
        {
            if (MainGame.me?.save?.quests == null) return false;
            return MainGame.me.save.quests.IsQuestFaild(questId);
        }

        /// <summary>
        /// Starts a quest by its definition ID. The quest must exist in GameBalance.
        /// </summary>
        public void StartQuest(string questId)
        {
            if (MainGame.me?.save?.quests == null) return;
            var def = GameBalance.me?.GetDataOrNull<QuestDefinition>(questId);
            if (def != null)
            {
                MainGame.me.save.quests.StartQuest(def);
            }
        }

        /// <summary>
        /// Forces a quest to be marked as failed in the current save.
        /// </summary>
        public void ForceFail(string questId)
        {
            if (MainGame.me?.save?.quests == null) return;
            MainGame.me.save.quests.ForceQuestEnd(questId, false);
        }

        /// <summary>
        /// Triggers quest system key checks. Many quests listen for specific
        /// key strings (e.g., "end_of_day", "interact_gerry", "quest_finished").
        /// This allows mods to trigger quest progression with custom keys.
        /// </summary>
        public void CheckKey(string key)
        {
            if (MainGame.me?.save?.quests == null) return;
            MainGame.me.save.quests.CheckKeyQuests(key);
        }

        /// <summary>
        /// Gets all currently active quests as QuestState objects.
        /// </summary>
        public List<QuestState> GetCurrentQuests()
        {
            if (MainGame.me?.save?.quests == null) return new List<QuestState>();
            return MainGame.me.save.quests.GetCurrentQuests();
        }

        /// <summary>
        /// Checks if a quest was ever executed (started at least once).
        /// </summary>
        public bool WasEverStarted(string questId)
        {
            if (MainGame.me?.save?.quests == null) return false;
            return MainGame.me.save.quests.CheckIfQuestWasExecuted(questId);
        }
    }
}
