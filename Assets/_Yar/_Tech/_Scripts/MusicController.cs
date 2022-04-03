using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MusicController : Singleton<MusicController>
{
    [SerializeField] private List<AudioSource> _audioSources;
    [SerializeField] private List<AnimationCurve> _audioThresholds;
    private float _clipLength = 1;

    private float _agressionPercent = 0;

    private void Start()
    {
        _clipLength = _audioSources[0].clip.length;

        StartCoroutine(MusicCycle());
    }

    private IEnumerator MusicCycle() 
    {
        while (true)
        {
            for (int i = 0; i < _audioSources.Count; i++)
            {
                int id = i;
                _audioSources[id].volume = Mathf.Lerp(_audioSources[id].volume
                    , _audioThresholds[id].Evaluate(_agressionPercent), Time.deltaTime);
            }

            yield return null;

            /*for (int i = 0; i < _audioSources.Count; i++)
            {
                if (_audioSources[i].isPlaying == false && _audioThresholds[i].Evaluate(_agressionPercent) > .5f) 
                {
                    _audioSources[i].PlayOneShot(_audioSources[i].clip);
                }
            }

            yield return new WaitForSeconds(_clipLength - .1f);*/
        }
    }

    public void SetAgressionPercent(float agrPercent) => _agressionPercent = agrPercent;
}
