using System.Numerics;
using UnityEngine;

[CreateAssetMenu(menuName = "CubeMining/SO_PlayerLevelData", fileName = "SO_PlayerLevelData")]
public class SO_PlayerLevelData : ScriptableObject
{
    [Tooltip("要素0 = レベル1→2 に必要な経験値。以降も同様。空なら式で補完。")]
    public int[] expToNextLevel = new[] { 20, 40, 60, 80, 100, 150, 200, 300, 400, 500 };

    [Tooltip("レベルアップごとに未使用ポイントへ加算する量")]
    public int pointsPerLevelUp = 1;

    /// <summary>現在レベルから次へ進むために必要な経験値（レベル内ゲージ用）</summary>
    public BigInteger GetExpToNext(int currentLevel)
    {
        if (currentLevel < 1) currentLevel = 1;
        var idx = currentLevel - 1;
        if (expToNextLevel != null && idx >= 0 && idx < expToNextLevel.Length && expToNextLevel[idx] > 0)
            return expToNextLevel[idx];
        return 100 * currentLevel;
    }
}
