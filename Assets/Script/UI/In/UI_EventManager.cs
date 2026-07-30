using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;

public class UI_EventManager : MonoBehaviour
{
    [SerializeField] private GameObject obj_StartCall;
    [SerializeField] private GameObject obj_EndCall;




    public async void StartInGame(int _waitTime)
    {
        obj_StartCall.SetActive(true);
        await UniTask.Delay(_waitTime * 1000);
        obj_StartCall.SetActive(false);
    }

    public async void EndGame(int _waitTime)
    {
        obj_EndCall.SetActive(true);
        await UniTask.Delay(_waitTime * 1000);
        obj_EndCall.SetActive(false);
    }


}
