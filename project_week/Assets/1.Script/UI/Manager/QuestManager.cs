using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public class QuestManager : MonoBehaviour
    {
        public void dayQuestCheck()
        {
            DateTime now = DateTime.Now;
            if (true || BaseManager.userGameData.LastSave.ToString("MM/dd/yyyy").Equals(now.ToString("MM/dd/yyyy")))
            {

            }
            else 
            {
                BaseManager.userGameData.DayQuest = new int[3];

                int qSkin = 0;
                
                if (BaseManager.userGameData.HasSkin > 1) 
                {
                    List<int> lists = new List<int>();
                    for (SkinKeyList skin = SkinKeyList.snowman + 1; skin < SkinKeyList.max; skin++)
                    {
                        if ((BaseManager.userGameData.HasSkin & (1 << (int)skin)) > 0)
                        {
                            lists.Add((int)skin);
                        }
                    }
                    qSkin = UnityEngine.Random.Range(0, lists.Count); 
                }

                BaseManager.userGameData.QuestSkin = qSkin;
            }
        }
    }
}