using System;
using System.Collections.Generic;
using UnityEngine;

namespace GraveSDK.Data.Repositories
{
    /// <summary>
    /// Provides access to the game's SmartExpression evaluation system.
    /// SmartExpressions are the game's built-in scripting language used for quest triggers,
    /// interaction conditions, and crafting logic.
    /// 
    /// Available functions in SmartExpression strings:
    ///   Ppar("param_name")       - Get player parameter value
    ///   SetPpar("param", value)  - Set player parameter
    ///   AddPpar("param", value)  - Add to player parameter
    ///   DecPpar("param", value)  - Subtract from player parameter
    ///   WGOpar("param_name")     - Get parameter from the context WGO
    ///   GetDay()                 - Current game day number
    ///   IsDay() / IsNight()      - Time of day checks (returns 1 or 0)
    ///   GetTime()                - Current time as 0-1 float
    ///   Ife(a, b)                - Returns 1 if a equals b
    ///   Ifn(condition)           - Returns 1 if condition is true
    ///   HasItemInWGO("item_id")  - Check if WGO has an item
    ///   HasOverheadBody()        - Check if player is carrying a body
    /// 
    /// Example expressions:
    ///   "Ppar(\"slimes_killed\") >= 5"   -- True when player has killed 5+ slimes
    ///   "GetDay() > 10"                   -- True after day 10
    ///   "Ppar(\"money\") >= 10000"        -- True when player has 100+ gold
    /// </summary>
    public static class SmartExpressionRepository
    {
        /// <summary>
        /// Creates a SmartExpression from a string formula.
        /// The expression can use any of the game's built-in functions.
        /// </summary>
        public static SmartExpression Create(string expression)
        {
            var expr = new SmartExpression();
            expr.FromString(expression);
            return expr;
        }

        /// <summary>
        /// Evaluates a SmartExpression string and returns the float result.
        /// </summary>
        public static float EvaluateFloat(string expression, WorldGameObject context = null)
        {
            var expr = Create(expression);
            return expr.EvaluateFloat(context, MainGame.me?.player);
        }

        /// <summary>
        /// Evaluates a SmartExpression for side-effects only (e.g. "SetPpar(\"money\", 5000)").
        /// </summary>
        public static void Execute(string expression, WorldGameObject context = null)
        {
            var expr = Create(expression);
            expr.Evaluate(context, MainGame.me?.player);
        }

        /// <summary>
        /// Evaluates a SmartExpression string as a boolean.
        /// Returns true if the result is > 0.5.
        /// </summary>
        public static bool EvaluateBoolean(string expression, WorldGameObject context = null)
        {
            var expr = Create(expression);
            return expr.EvaluateBoolean(context, MainGame.me?.player);
        }

        /// <summary>
        /// Evaluates a SmartExpression string as a chance/probability.
        /// Used for triggers — returns true if the expression evaluates to true.
        /// </summary>
        public static bool EvaluateChance(string expression, WorldGameObject context = null)
        {
            var expr = Create(expression);
            return expr.EvaluateChance(context, MainGame.me?.player);
        }
    }

    /// <summary>
    /// Builder pattern for creating custom QuestDefinition objects with proper
    /// SmartExpression triggers. This allows modders to define quests like:
    /// 
    /// var quest = new QuestBuilder("mod_kill_slimes")
    ///     .SetStartKey("interact_gerry")
    ///     .SetSuccessTrigger("Ppar(\"slimes_killed\") >= 5")
    ///     .SetVisible(true)
    ///     .SetOneTime(true)
    ///     .Build();
    /// </summary>
    public class QuestBuilder
    {
        private readonly string _id;
        private string _startTriggerExpr = "";
        private string _successTriggerExpr = "";
        private string _failTriggerExpr = "";
        private List<string> _startKeys = new List<string>();
        private bool _oneTime = true;
        private bool _visible = true;
        private string _startScript = "";
        private string _successScript = "";
        private string _failScript = "";
        private string _arrowTag = "";
        private string _arrowObjId = "";
        private List<string> _successExpressions = new List<string>();
        private List<string> _failExpressions = new List<string>();

        public QuestBuilder(string questId)
        {
            _id = questId;
        }

        /// <summary>
        /// Sets the SmartExpression that must be true for the quest to auto-start.
        /// Example: "Ppar(\"gerry_talked\") >= 1"
        /// </summary>
        public QuestBuilder SetStartTrigger(string expression)
        {
            _startTriggerExpr = expression;
            return this;
        }

        /// <summary>
        /// Sets the SmartExpression that determines quest success.
        /// The QuestSystem evaluates this every time CheckQuestsState() is called.
        /// Example: "Ppar(\"slimes_killed\") >= 5"
        /// </summary>
        public QuestBuilder SetSuccessTrigger(string expression)
        {
            _successTriggerExpr = expression;
            return this;
        }

        /// <summary>
        /// Sets the SmartExpression that determines quest failure.
        /// Example: "GetDay() > 20" (fail if not done by day 20)
        /// </summary>
        public QuestBuilder SetFailTrigger(string expression)
        {
            _failTriggerExpr = expression;
            return this;
        }

        /// <summary>
        /// Sets the key(s) that the QuestSystem listens for to start this quest.
        /// The game calls CheckKeyQuests(key) at various points:
        ///   "end_of_day", "interact_{npc_id}", "quest_finished", etc.
        /// </summary>
        public QuestBuilder SetStartKey(params string[] keys)
        {
            _startKeys = new List<string>(keys);
            return this;
        }

        /// <summary>Whether this quest can only be completed once.</summary>
        public QuestBuilder SetOneTime(bool oneTime)
        {
            _oneTime = oneTime;
            return this;
        }

        /// <summary>Whether the quest shows in the quest log UI.</summary>
        public QuestBuilder SetVisible(bool visible)
        {
            _visible = visible;
            return this;
        }

        /// <summary>FlowScript to run when the quest starts.</summary>
        public QuestBuilder SetStartScript(string script)
        {
            _startScript = script;
            return this;
        }

        /// <summary>FlowScript to run when the quest succeeds.</summary>
        public QuestBuilder SetSuccessScript(string script)
        {
            _successScript = script;
            return this;
        }

        /// <summary>FlowScript to run when the quest fails.</summary>
        public QuestBuilder SetFailScript(string script)
        {
            _failScript = script;
            return this;
        }

        /// <summary>
        /// Adds a SmartExpression to evaluate when the quest succeeds.
        /// Use this for side-effects like "AddPpar(\"money\", 5000)".
        /// </summary>
        public QuestBuilder AddSuccessExpression(string expression)
        {
            _successExpressions.Add(expression);
            return this;
        }

        /// <summary>
        /// Adds a SmartExpression to evaluate when the quest fails.
        /// </summary>
        public QuestBuilder AddFailExpression(string expression)
        {
            _failExpressions.Add(expression);
            return this;
        }

        /// <summary>Custom tag of WGO to show quest arrow on.</summary>
        public QuestBuilder SetArrowTag(string tag)
        {
            _arrowTag = tag;
            return this;
        }

        /// <summary>Object ID of WGO to show quest arrow on.</summary>
        public QuestBuilder SetArrowObjId(string objId)
        {
            _arrowObjId = objId;
            return this;
        }

        /// <summary>
        /// Builds the QuestDefinition with all configured SmartExpression triggers.
        /// </summary>
        public QuestDefinition Build()
        {
            var quest = new QuestDefinition();
            quest.id = _id;
            quest.one_time_quest = _oneTime;
            quest.quest_visible = _visible;
            quest.start_script = _startScript;
            quest.success_script = _successScript;
            quest.fail_script = _failScript;
            quest.arrow_wgo_custom_tag = _arrowTag;
            quest.arrow_wgo_obj_id = _arrowObjId;
            quest.start_key = _startKeys;

            // Set up SmartExpression triggers
            if (!string.IsNullOrEmpty(_startTriggerExpr))
                quest.start_trigger.FromString(_startTriggerExpr);

            if (!string.IsNullOrEmpty(_successTriggerExpr))
                quest.success_trigger.FromString(_successTriggerExpr);

            if (!string.IsNullOrEmpty(_failTriggerExpr))
                quest.fail_trigger.FromString(_failTriggerExpr);

            // Set up reward/punishment expressions
            quest.success_expressions = new List<SmartExpression>();
            foreach (var expr in _successExpressions)
            {
                var se = new SmartExpression();
                se.FromString(expr);
                quest.success_expressions.Add(se);
            }

            quest.fail_expressions = new List<SmartExpression>();
            foreach (var expr in _failExpressions)
            {
                var se = new SmartExpression();
                se.FromString(expr);
                quest.fail_expressions.Add(se);
            }

            return quest;
        }

        /// <summary>
        /// Builds and registers the quest into GameBalance in one call.
        /// </summary>
        public QuestDefinition BuildAndRegister()
        {
            var quest = Build();
            string error = GameBalance.me?.AddData(quest);
            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError("[GraveSDK] QuestBuilder.Register error: " + error);
            }
            else
            {
                GameBalance.me?.CreateIDsCache();
                Debug.Log("[GraveSDK] Registered quest: " + _id);
            }
            return quest;
        }
    }
}
