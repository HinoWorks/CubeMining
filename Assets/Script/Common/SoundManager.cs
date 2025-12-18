using UnityEngine;
using System;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Inst;

    [Range(0, 100)]
    public float Volume_BGM;
    [Range(0, 100)]
    public float Volume_Sound;

    // 音量
    //public SoundVolume volume = new SoundVolume();

    // === AudioSource ===
    private AudioSource BGMsource;
    private AudioSource[] SEsources = new AudioSource[16];
    private SO_SoundElement soundData_BGM;

    // -- SE同時になってしまう現状回避 --
    private float timer;
    private float duration_SE = 0.2f;
    private bool isSEPlayed = false;




    void Awake()
    {
        if (Inst == null)
        {
            Inst = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this);
            return;
        }

        // BGM AudioSource
        BGMsource = gameObject.AddComponent<AudioSource>();
        BGMsource.loop = true;

        // SE AudioSource
        for (int i = 0; i < SEsources.Length; i++)
        {
            SEsources[i] = gameObject.AddComponent<AudioSource>();
        }
    }


    void Start()
    {
        // BGM Start
        //Instance.PlayBGM(0);
    }


    void Update()
    {
        if (isSEPlayed)
        {
            timer += Time.deltaTime;
            if (timer >= duration_SE)
            {
                isSEPlayed = false;
                timer = 0;
            }
        }
        /*
        // ミュート設定
        BGMsource.mute = volume.Mute;
        foreach (AudioSource source in SEsources)
        {
            source.mute = volume.Mute;
        }
        foreach (AudioSource source in VoiceSources)
        {
            source.mute = volume.Mute;
        }
        */

        /*
        // ボリューム設定
        BGMsource.volume = volume.BGM;
        foreach (AudioSource source in SEsources)
        {
            source.volume = volume.SE;
        }
        foreach (AudioSource source in VoiceSources)
        {
            source.volume = volume.Voice;
        }
        */
    }







    #region -- BGM --
    public void PlayBGM(int _index)
    {
        var getData = DataBase.Inst.mSO_SoundData.Get_SoundData_BGM(_index);
        if (getData == null) return;

        // -- set 
        BGMsource.Stop();
        soundData_BGM = getData;
        BGMsource.clip = soundData_BGM.clip;
        BGMsource.Play();
        ChangeVolume_ForBGM();
    }

    public void StopBGM()
    {
        BGMsource.Stop();
        BGMsource.clip = null;
    }
    public void ChangeVolume_ForBGM()
    {
        BGMsource.volume = Volume_BGM / 100f;
    }
    #endregion


    #region -- SE --
    public void PlaySE_BallCol(bool _isForceON = false)
    {
        if (!_isForceON && isSEPlayed) return;
        isSEPlayed = true;
        SO_SoundElement getData;
        getData = DataBase.Inst.mSO_SoundData.Get_SoundData_SE_Random(0, 4);

        PlaySE(getData);
    }
    public void PlaySE_BallTap(bool _isForceON = true)
    {
        if (!_isForceON && isSEPlayed) return;
        isSEPlayed = true;
        SO_SoundElement getData;
        getData = DataBase.Inst.mSO_SoundData.Get_SoundData_SE(11);

        PlaySE(getData);
    }
    public void PlaySE_SPBallGenerate(bool _isForceON = true)
    {
        if (!_isForceON && isSEPlayed) return;
        isSEPlayed = true;
        SO_SoundElement getData;
        getData = DataBase.Inst.mSO_SoundData.Get_SoundData_SE(12);

        PlaySE(getData);
    }

    public void PlaySE(SO_SoundElement getData)
    {
        //if (!_isForceON && isSEPlayed) return;
        //isSEPlayed = true;
        //SO_SoundElement getData;
        //getData = DataBase.Inst.mSO_SoundData.Get_SoundData_SE(_index);

        // 再生中で無いAudioSouceで鳴らす
        foreach (AudioSource source in SEsources)
        {
            if (source.isPlaying == false)
            {
                source.clip = getData.clip;
                source.volume = getData.Volume * Volume_Sound / 100f;
                source.Play();
                return;
            }
        }
    }
    // SE停止
    public void StopSE()
    {
        // 全てのSE用のAudioSouceを停止する
        foreach (AudioSource source in SEsources)
        {
            source.Stop();
            source.clip = null;
        }
    }
    #endregion 
}
