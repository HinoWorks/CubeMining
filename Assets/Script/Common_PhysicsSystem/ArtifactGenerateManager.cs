using UnityEngine;
using System.Collections.Generic;

public class ArtifactGenerateManager : MonoBehaviour
{
    public static ArtifactGenerateManager Inst;

    private List<MiningTarget_Artifact> list_targetArtifacts = new List<MiningTarget_Artifact>();
    public bool isGenerateArtifact { get; private set; } = false;
    private bool isArtifactAllGet = false;


    private float timer = 0f;
    private float checkInterval = 3f;

    void Awake()
    {
        if (Inst == null) { Inst = this; }
        else { Destroy(this); }
    }

    public void Init()
    {
        isGenerateArtifact = false;
        isArtifactAllGet = SaveLoader.Inst.Get_ArtifactIndex_NotGet().Length == 0;
    }

    public void ResetAll()
    {
        foreach (var targetArtifact in list_targetArtifacts)
        {
            targetArtifact.NotActivate();
        }
    }
    public void UnityUpDate()
    {
        timer += Time.deltaTime;
        if (timer < checkInterval) return;

        timer = 0f;
        Check_ArtifactGenerate();
    }

    public void Check_ArtifactGenerate()
    {
        if (isArtifactAllGet) return; // アーティファクト全て取得済み？
        if (isGenerateArtifact) return; // アーティファクト生成中？
        if (!GameParamManager.IsArtifactGenerate()) return; // アーティファクト生成条件を満たしている？

        Generate(BlockGenerateManager.Inst.generatePosition, Quaternion.Euler(BlockGenerateManager.Inst.generateRotation));
    }

    private bool ShouldGenerate()
    {
        return !isArtifactAllGet && !isGenerateArtifact && GameParamManager.IsArtifactGenerate();
    }

    private MiningTarget_Artifact Generate(Vector3 position, Quaternion rotation)
    {
        var artifactIndexes = SaveLoader.Inst.Get_ArtifactIndex_NotGet();
        if (artifactIndexes.Length == 0) return null;

        var targetArtifact = list_targetArtifacts.Find(x => x.isActiveAndEnabled == false);
        if (targetArtifact == null)
        {
            var newArtifact = Instantiate(SOLoader.BlockData.pf_Artifact, InGameManager.Inst.ParentPool) as GameObject;
            targetArtifact = newArtifact.GetComponent<MiningTarget_Artifact>();
            list_targetArtifacts.Add(targetArtifact);
        }

        var artifactIndex = artifactIndexes[Random.Range(0, artifactIndexes.Length)];
        targetArtifact.Init(artifactIndex);
        targetArtifact.transform.localPosition = position;
        targetArtifact.transform.localRotation = rotation;
        isGenerateArtifact = true;
        return targetArtifact;
    }
}
