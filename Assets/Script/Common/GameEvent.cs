using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx;
using System;

public static class GameEvent
{
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



}
