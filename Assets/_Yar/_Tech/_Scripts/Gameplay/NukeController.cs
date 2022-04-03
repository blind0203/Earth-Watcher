using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class NukeController : Singleton<NukeController>
{
    [SerializeField] private GameObject _rocketPrefab;
    [SerializeField] private List<GameObject> _objectsToTurnOff;

    public void StartWar() 
    {
        StartCoroutine(War());
    }

    private IEnumerator War() 
    {
        for (int i = 0; i < _objectsToTurnOff.Count; i++)
        {
            _objectsToTurnOff[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < 50; i++)
        {
            for (int j = 0; j < Random.Range(1, 3); j++)
            {
                SendRocket();
            }
            yield return null;
        }

        yield return new WaitForSeconds(10f);

        GameController.Instance.EndGame();

        yield return new WaitForSeconds(8f);

        SceneManager.LoadScene(0);
    }

    private void SendRocket() 
    {
        Vector3 startPoint = new Vector3(Random.Range(-1f, 1f)
            , Random.Range(-1f, 1f)
            , Random.Range(-1f, 1f)).normalized * 5.2f;

        Vector3 endPoint = Quaternion.Euler(Random.Range(-30f, 30)
                            , Random.Range(-30f, 30f)
                            , Random.Range(-30f, 30))  * startPoint;

        Vector3 midPoint = ((startPoint + endPoint) / 2).normalized * 6;


        Vector3[] path = new Vector3[3]
        {
           startPoint,
           midPoint,
           endPoint
        }; 

        Transform rocket = Instantiate(_rocketPrefab, startPoint, Quaternion.LookRotation(endPoint), null).transform;

        rocket.DOPath(path, Random.Range(10f, 15f), PathType.CatmullRom, PathMode.Full3D, 10, null)
            .SetEase(Ease.InCubic).OnComplete(() =>
            {
                StartCoroutine(Explode(rocket));
            });
    }

    private IEnumerator Explode(Transform rocketTransform) 
    {
        rocketTransform.GetComponentInChildren<ParticleSystem>().Play();

        yield return new WaitForSeconds(6f);

        Destroy(rocketTransform.gameObject);
    }
}
