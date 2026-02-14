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
    ResultEnd_ToOutGame,
    ResultEnd_ToIngameReady,
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


    public static class InGame
    {
        // インゲームでゲーム記録データが変化した時のイベント、主に1回のインゲーム中の記録データ変化を通知
        private static readonly Subject<(GameRecordData_Type, System.Numerics.BigInteger)> gameRecordDataMod_Ingame = new();
        public static IObservable<(GameRecordData_Type, System.Numerics.BigInteger)> GameRecordDataMod_Ingame => gameRecordDataMod_Ingame.AsObservable();
        public static void PublishGameRecordDataMod_Ingame(GameRecordData_Type gameRecordData_Type, System.Numerics.BigInteger mod)
        {
            gameRecordDataMod_Ingame.OnNext((gameRecordData_Type, mod));
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


        private static readonly Subject<IDamagable> pointerDamage = new();
        public static IObservable<IDamagable> PointerDamage => pointerDamage.AsObservable();
        public static void PublishPointerDamage(IDamagable target)
        {
            pointerDamage.OnNext(target);
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

        private static readonly Subject<System.Numerics.BigInteger> coinMod = new();
        public static IObservable<System.Numerics.BigInteger> CoinMod => coinMod.AsObservable();
        public static void PublishCoinMod(System.Numerics.BigInteger mod)
        {
            coinMod.OnNext(mod);
        }


        // インゲームリザルト, アウトゲームでリソースが変化した時のイベント
        private static readonly Subject<(ResourceType, System.Numerics.BigInteger)> resourceMod = new();
        public static IObservable<(ResourceType, System.Numerics.BigInteger)> ResourceMod => resourceMod.AsObservable();
        public static void PublishResourceMod(ResourceType resourceType, System.Numerics.BigInteger mod)
        {
            resourceMod.OnNext((resourceType, mod));
        }

        // インゲームでリソースが変化した時のイベント、主に1回のインゲーム中のリソース変化を通知
        private static readonly Subject<(ResourceType, System.Numerics.BigInteger)> resourceMod_Ingame = new();
        public static IObservable<(ResourceType, System.Numerics.BigInteger)> ResourceMod_Ingame => resourceMod_Ingame.AsObservable();
        public static void PublishResourceMod_Ingame(ResourceType resourceType, System.Numerics.BigInteger mod)
        {
            resourceMod_Ingame.OnNext((resourceType, mod));
        }

        private static readonly Subject<int> depthCount = new();
        public static IObservable<int> DepthCount => depthCount.AsObservable();
        public static void PublishDepthCount(int depth)
        {
            depthCount.OnNext(depth);
        }

    }



}
