using UnityEngine;
using UniRx;
using System;

public class AttackCont_BowUnit : MonoBehaviour
{
    [SerializeField] SimpleAnimation anim;
    private AttackCont_Bow bowCont;
    private int damage;
    private float lifetime;
    private float speed;
    private Vector3 direction;
    [SerializeField] LineRenderer[] lineRenderers;
    [SerializeField] private float lineLength = 5f; // 線の長さ

    // -- level2
    private bool isAddArrow = false;
    private Vector3[] shotDirections_level2 = new Vector3[2];
    private float Get_RandomShotDirection_Level2 => UnityEngine.Random.Range(10f, 30f);


    // -- anim
    private string animName_shot = "Shot";
    private string animName_return = "Return";
    private string animaName_spawn = "Default";



    private void Awake()
    {
        if (lineRenderers != null)
        {
            foreach (var lineRenderer in lineRenderers)
            {
                lineRenderer.positionCount = 2; // 開始点と終了点の2点
                lineRenderer.gameObject.SetActive(false);
            }
        }
    }

    public void Set_BowCont(AttackCont_Bow _bowCont)
    {
        bowCont = _bowCont;
    }
    public void Init(int _damage, float _lifetime, float _speed, Vector3 _direction, bool _isAddArrow)
    {
        this.damage = _damage;
        this.lifetime = _lifetime;
        this.speed = _speed;
        this.direction = _direction;
        this.isAddArrow = _isAddArrow;

        this.gameObject.SetActive(true);
        anim.Play(animaName_spawn);

        Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(_ => StartShot()).AddTo(this);
        Observable.Timer(TimeSpan.FromSeconds(lifetime)).Subscribe(_ => ReturnToPool()).AddTo(this);

        UpdateLineRenderer();
    }

    private void UpdateLineRenderer()
    {
        if (lineRenderers == null) return;

        // オブジェクトの現在位置を開始点に
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + this.direction * lineLength;
        SetLineRenderer(0, startPosition, endPosition);
        lineRenderers[0].gameObject.SetActive(true);

        // -- level2 check
        if (!isAddArrow) return;

        if (bowCont.IsVertical) // 真上から撃ち落とす
        {
            var offsetAngle = (-this.transform.rotation.eulerAngles.x + 90f) * Mathf.Deg2Rad;
            var shotDirection = Get_RandomShotDirection_Level2;
            for (int i = 0; i < 2; i++)
            {
                var plusMinus = i == 0 ? 1 : -1; // 扇状にうつ
                lineRenderers[i + 1].gameObject.SetActive(true);
                endPosition = new Vector3(Mathf.Sin(offsetAngle + shotDirection * plusMinus * Mathf.Deg2Rad), -Mathf.Cos(offsetAngle + shotDirection * plusMinus * Mathf.Deg2Rad), 0f);
                SetLineRenderer(i + 1, startPosition, startPosition + endPosition * lineLength);
                shotDirections_level2[i] = endPosition;
            }
        }
        else // 側面から射つ
        {
            var offsetAngle = (-this.transform.rotation.eulerAngles.y + 90f) * Mathf.Deg2Rad;
            var shotDirection = Get_RandomShotDirection_Level2;
            for (int i = 0; i < 2; i++)
            {
                var plusMinus = i == 0 ? 1 : -1; // 扇状にうつ
                lineRenderers[i + 1].gameObject.SetActive(true);
                endPosition = new Vector3(Mathf.Cos(offsetAngle + shotDirection * plusMinus * Mathf.Deg2Rad), 0f, Mathf.Sin(offsetAngle + shotDirection * plusMinus * Mathf.Deg2Rad));
                SetLineRenderer(i + 1, startPosition, startPosition + endPosition * lineLength);
                shotDirections_level2[i] = endPosition;
            }
        }
    }

    private void SetLineRenderer(int _index, Vector3 _startPosition, Vector3 _endPosition)
    {
        lineRenderers[_index].SetPosition(0, _startPosition);
        lineRenderers[_index].SetPosition(1, _endPosition);
    }



    private void StartShot()
    {
        Shot_Level1();

        //  -- level2 check
        if (!isAddArrow) return;
        Shot_Level2();
    }

    private void Shot_Level1()
    {
        ShotArrow(direction, 0);
    }
    private void Shot_Level2()
    {
        var count = 1;
        foreach (var shotDirection in shotDirections_level2)
        {
            ShotArrow(shotDirection, count);
            count++;
        }
    }

    private void ShotArrow(Vector3 _setDirection, int _count)
    {
        anim.Play(animName_shot);
        lineRenderers[_count].gameObject.SetActive(false);
        var freeArrow = bowCont.Get_FreeArrow();
        freeArrow.transform.position = transform.position;
        freeArrow.Init(damage, 2.5f, speed * _setDirection);
    }



    private void ReturnToPool()
    {
        anim.Play(animName_return);
        //this.gameObject.SetActive(false);
    }
}
