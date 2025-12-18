using UnityEngine;




public class GameEvent_In
{

}

public class GameEvent_Out
{

}



public class GameWatcher : MonoBehaviour
{

    public static GameWatcher Inst;
    private void Awake()
    {
        if (Inst == null) { Inst = this; }
        else { Destroy(this); }
    }

}
