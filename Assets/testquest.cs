using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testquest : MonoBehaviour
{
    public void TestQuest()
    {
        QuestManager.instance.MoveToNextQuest();
    }
}
