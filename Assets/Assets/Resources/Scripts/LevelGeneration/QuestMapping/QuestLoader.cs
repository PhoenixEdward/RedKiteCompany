using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RedKite
{
    class QuestLoader
    {
        private static QuestLoader _instance = null;
        public static QuestLoader Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new QuestLoader();

                return _instance;
            }
        }

        public Quest Load(string questName)
        {

            TextAsset json = Resources.Load<TextAsset>("Scripts/Data/Quests");

            QuestBoard inventory = JsonUtility.FromJson<QuestBoard>(json.text);

            //need to do this for weapons. Way fucking simpler than my stupid loot dict.

            Debug.Log(inventory.Quests.Count);

            Quest output = inventory.Quests.First(x=> x.Name == questName);

            return output;
        }

        [Serializable]
        public class QuestBoard
        {
            public List<Quest> Quests;
        }
    }
}
