using System;
using System.Collections.Generic;
using UnityEngine;

namespace GraveSDK.Data.Repositories
{
    public static class DialogueRepository
    {
        /// <summary>
        /// Makes a WorldGameObject say something with a speech bubble.
        /// </summary>
        /// <param name="speaker">The object speaking.</param>
        /// <param name="text">The text to say (will be localized if key exists).</param>
        /// <param name="type">The type of speech bubble (Talk, Think, InfoBox).</param>
        /// <param name="voice">Optional forced voice ID.</param>
        public static void Say(WorldGameObject speaker, string text, SpeechBubbleGUI.SpeechBubbleType type = SpeechBubbleGUI.SpeechBubbleType.Talk, SmartSpeechEngine.VoiceID voice = SmartSpeechEngine.VoiceID.None)
        {
            if (speaker == null) return;
            speaker.Say(text, null, null, type, voice);
        }

        /// <summary>
        /// Makes the player say something.
        /// </summary>
        public static void SayAsPlayer(string text, SpeechBubbleGUI.SpeechBubbleType type = SpeechBubbleGUI.SpeechBubbleType.Talk)
        {
            if (MainGame.me == null || MainGame.me.player == null) return;
            MainGame.me.player.Say(text, null, null, type, SmartSpeechEngine.VoiceID.Player, true);
        }

        /// <summary>
        /// Shows a list of options to the player and triggers a callback when one is selected.
        /// </summary>
        /// <param name="speaker">The NPC the player is talking to.</param>
        /// <param name="options">List of option IDs/strings. If they start with @, they are checked against unlocked phrases.</param>
        /// <param name="onChosen">Callback with the selected option ID.</param>
        public static void ShowOptions(WorldGameObject speaker, List<string> options, Action<string> onChosen)
        {
            if (speaker == null) return;

            List<AnswerVisualData> visualData = new List<AnswerVisualData>();
            foreach (var opt in options)
            {
                visualData.Add(new AnswerVisualData { id = opt });
            }

            speaker.ShowMultianswer(visualData, (chosen) => onChosen?.Invoke(chosen), null, null, speaker);
        }

        /// <summary>
        /// Shows a cinematic corner message (inner monologue).
        /// </summary>
        public static void ShowCornerMessage(string text)
        {
            if (GUIElements.me == null || GUIElements.me.corner_talk == null) return;
            GUIElements.me.corner_talk.Say(text, null);
        }

        /// <summary>
        /// Hides any active multi-answer dialog.
        /// </summary>
        public static void HideOptions()
        {
            MultiAnswerGUI.HideAnyctive();
        }
    }
}
