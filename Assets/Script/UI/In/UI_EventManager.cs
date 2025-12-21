using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;

public class UI_EventManager : MonoBehaviour
{
    [SerializeField] private GameObject obj_StartCall;
    [SerializeField] private GameObject obj_EndCall;




    public async void StateGame()
    {
        obj_StartCall.SetActive(true);
        await UniTask.Delay(2000);
        GameWatcher.Inst.SetGameState(GameStateType.InGame);

        obj_StartCall.SetActive(false);
    }

    public async void EndGame()
    {
        obj_EndCall.SetActive(true);
        await UniTask.Delay(2000);
        GameWatcher.Inst.SetGameState(GameStateType.Result);
        obj_EndCall.SetActive(false);
    }


}
