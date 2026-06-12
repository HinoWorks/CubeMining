using UnityEngine;
using UniRx;


namespace HoleGameSystem
{
    /*
    public class AroundGuradManager : MonoBehaviour
    {
        [SerializeField] GameObject pf_guard;
        [SerializeField] Transform parent_guards;

        private GameObject obj_guard_1;
        private GameObject obj_guard_2;


        private int currentLayerIndex = 0;
        private int currentLayerMaxSize = 0;
        private float offset_max = -0.4f;

        private int initialSize = 3;



        void Start()
        {
            GameEvent.InGame.OnNewGroundLayer.Subscribe(currentLayerIndex => Check_NewGroundLayer(currentLayerIndex)).AddTo(this);
            GameEvent.GameState.SetGameState.Subscribe(SetGameState).AddTo(this);

            obj_guard_1 = Instantiate(pf_guard, parent_guards) as GameObject;
            obj_guard_2 = Instantiate(pf_guard, parent_guards) as GameObject;
            obj_guard_1.transform.rotation = Quaternion.Euler(0, 0, 0);
            obj_guard_2.transform.rotation = Quaternion.Euler(0, 90, 0);
            Init();
        }

        private void Init()
        {
            currentLayerIndex = -1;
            parent_guards.transform.localPosition = new Vector3(0, 0, 0);
            obj_guard_1.transform.localPosition = new Vector3(initialSize + offset_max, 0, -initialSize / 2f);
            obj_guard_2.transform.localPosition = new Vector3(initialSize / 2f, 0, -initialSize - offset_max);

        }

        private void SetGameState(GameStateType state)
        {
            switch (state)
            {
                case GameStateType.InGame_Ready:
                    Init();
                    break;
            }
        }
        private void Check_NewGroundLayer(int _currentLayerIndex)
        {
            if (_currentLayerIndex == currentLayerIndex) return;
            currentLayerIndex = _currentLayerIndex;
            currentLayerMaxSize = BlockGenerateManager.Inst.currentLayerSize;

            parent_guards.transform.localPosition = new Vector3(0, -currentLayerIndex, 0);
            obj_guard_1.transform.localPosition = new Vector3(currentLayerMaxSize + offset_max, 0, -currentLayerMaxSize / 2f);
            obj_guard_2.transform.localPosition = new Vector3(currentLayerMaxSize / 2f, 0, -currentLayerMaxSize - offset_max);
        }

    }
    */
}