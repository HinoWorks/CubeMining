using UnityEngine;
using System.Collections;
using Cysharp.Threading.Tasks;

public class HitFlash : MonoBehaviour
{
    private string colorProperty = "_Color"; // シェーダの色プロパ名
    float duration = 0.15f;          // 白フラッシュ時間
    [SerializeField] Renderer rend;
    MaterialPropertyBlock mpb;
    int colorId;
    Color baseColor;

    void Awake()
    {
        mpb = new MaterialPropertyBlock();
        mpb_crack = new MaterialPropertyBlock();

        colorId = Shader.PropertyToID(colorProperty);

        // 元色を取得（共有マテリアルから）
        baseColor = rend.sharedMaterial.HasProperty(colorId)
            ? rend.sharedMaterial.GetColor(colorId)
            : Color.white;

        mpb.SetColor(colorId, Color.white);
    }


    public void Init_Crack()
    {
        targetTightness = tightness_init;
        mpb_crack.SetFloat(ID_Tightness, targetTightness);
        rend.SetPropertyBlock(mpb_crack);
    }

    public void Flash()
    {
        StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        float t = 0f;

        // 立ち上がり：即白
        mpb.SetColor(colorId, Color.white);
        rend.SetPropertyBlock(mpb);

        while (t < duration)
        {
            t += Time.deltaTime;
            float k = 1f - Mathf.Clamp01(t / duration); // 1→0
            mpb.SetColor(colorId, Color.Lerp(baseColor, Color.white, k));
            rend.SetPropertyBlock(mpb);
            yield return null;
        }

        // 元色に戻して上書き解除
        mpb.SetColor(colorId, baseColor);
        rend.SetPropertyBlock(mpb);
        // あるいは完全クリアしたい場合：mpb.Clear(); rend.SetPropertyBlock(mpb);
    }





    // -----

    static readonly int ID_Tightness = Shader.PropertyToID("_GlowTightness");

    private float targetTightness = 3f;
    [SerializeField] float tightness_init = 3f;
    [SerializeField] float tightness_low = 6f;
    [SerializeField] float tightness_mid = 6f;
    [SerializeField] float tightness_high = 6f;
    MaterialPropertyBlock mpb_crack;

    public async void Set_Crack(float _hpRate)
    {
        if (_hpRate > 0.66f)
        {
            targetTightness = tightness_low;
        }
        else if (_hpRate > 0.33f)
        {
            targetTightness = tightness_mid;
        }
        else
        {
            targetTightness = tightness_high;
        }

        // 立ち上がり：即白
        mpb.SetColor(colorId, Color.white);
        rend.SetPropertyBlock(mpb);

        await UniTask.DelayFrame(2);
        mpb_crack.SetFloat(ID_Tightness, targetTightness);
        rend.SetPropertyBlock(mpb_crack);
    }


}
