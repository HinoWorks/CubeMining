using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx;
using System;


public enum GameStateType
{
    Title,
    InGame_Ready,
    InGame,
    InGame_End,
    Result,
    OutGame,
}


public static class GameEvent
{

    public static class GameState
    {
        private static readonly Subject<GameStateType> setGameState = new();
        public static IObservable<GameStateType> SetGameState => setGameState.AsObservable();
        public static void PublishGameState(GameStateType state)
        {
            setGameState.OnNext(state);
        }
    }



    public static class Input
    {
        private static readonly Subject<Vector3> pointerMove = new();
        public static IObservable<Vector3> PointerMove => pointerMove.AsObservable();
        public static void PublishPointerMove(Vector3 pos)
        {
            pointerMove.OnNext(pos);
        }

        private static readonly Subject<bool> pointerAreaIn = new();
        public static IObservable<bool> PointerAreaIn => pointerAreaIn.AsObservable();
        public static void PublishPointerAreaIn(bool isAreaIn)
        {
            pointerAreaIn.OnNext(isAreaIn);
        }
    }


    public static class UI
    {
        private static readonly Subject<bool> resultOpen = new();
        public static IObservable<bool> ResultOpen => resultOpen.AsObservable();
        public static void PublishResultOpen(bool isOpen)
        {
            resultOpen.OnNext(isOpen);
        }


        private static readonly Subject<float> timeLimit = new();
        public static IObservable<float> TimeLimit => timeLimit.AsObservable();
        public static void PublishTimeLimit(float time)
        {
            timeLimit.OnNext(time);
        }
    }



}
