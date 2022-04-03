using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : Singleton<CamController>
{
    private float _rotationSpeed = 10;

    private float _inertia = 0;

    private bool _isRotating = true;

    private void Awake()
    {
        _rotationSpeed = 360;
    }

    void Update()
    {
        if (GameController.Instance.CurrentGameState() == GameController.GameState.GameLoop)
        {
            if (_isRotating)
            {
                if (Input.GetMouseButton(0))
                {
                    _inertia = Mathf.Lerp(_inertia, Input.GetAxis("Mouse X"), 20 * Time.deltaTime);

                    transform.RotateAround(Vector3.zero, Vector3.up, _rotationSpeed * _inertia * Time.deltaTime);
                }

                else
                {
                    _inertia = Mathf.Lerp(_inertia, 0, 1f * Time.deltaTime);

                    transform.RotateAround(Vector3.zero, Vector3.up, _rotationSpeed * _inertia * Time.deltaTime);
                }
            }
        }
    }

    public void SetRotatingFlag(bool isRotating) => _isRotating = isRotating;
}
