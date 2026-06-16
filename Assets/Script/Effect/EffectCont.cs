using UnityEngine;

public class EffectCont : MonoBehaviour
{
    private ParticleSystem ps;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    public void SetParticle3DSize(float _x, float _y, float _z)
    {
        var main = ps.main;
        main.startSize3D = true;

        main.startSizeX = _x;
        main.startSizeY = _y;
        main.startSizeZ = _z; // 測った距離をそのまま直接代入！
    }

}
