using UnityEngine;
using UniRx;
using System;

public class AttackCont_BowUnit : MonoBehaviour
{
    private AttackCont_Bow bowCont;
    private int damage;
    private float lifetime;
    private float speed;
    private Vector3 direction;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] private float lineLength = 5f; // 線の長さ

    private void Awake()
    {
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2; // 開始点と終了点の2点
        }
    }

    public void Set_BowCont(AttackCont_Bow _bowCont)
    {
        bowCont = _bowCont;
    }
    public void Init(int _damage, float _lifetime, float _speed, Vector3 _direction)
    {
        this.damage = _damage;
        this.lifetime = _lifetime;
        this.speed = _speed;
        this.direction = _direction;

        Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(_ => ShotArrow()).AddTo(this);
        Observable.Timer(TimeSpan.FromSeconds(lifetime)).Subscribe(_ => ReturnToPool()).AddTo(this);
        this.gameObject.SetActive(true);

        lineRenderer.gameObject.SetActive(true);
        UpdateLineRenderer();
    }

    private void UpdateLineRenderer()
    {
        if (lineRenderer == null) return;

        // オブジェクトの現在位置を開始点に
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + this.direction * lineLength;

        // LineRendererの位置を設定
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
    }

    private void ReturnToPool()
    {
        this.gameObject.SetActive(false);
    }

    private void ShotArrow()
    {
        lineRenderer.gameObject.SetActive(false);
        var freeArrow = bowCont.Get_FreeArrow();
        freeArrow.transform.position = transform.position;
        freeArrow.Init(damage, 2.5f, speed * direction);
    }
}
