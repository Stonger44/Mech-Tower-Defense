using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private GameObject _camera;
    private Vector3 _lookDirection;
    private Vector3 _verticalOnlyLookDirection;

    [SerializeField] private GameObject _rootObject;
    [SerializeField] private Image _healthBar;

    [SerializeField] private Color _healthGreen;
    [SerializeField] private Color _healthYellow;
    [SerializeField] private Color _healthRed;

    private void OnEnable()
    {
        Enemy.onHealthUpdate += UpdateHealth;
        Gatling_Gun.onHealthUpdate += UpdateHealth;
        Missile_Launcher.onHealthUpdate += UpdateHealth;
        GameManager.onHealthUpdate += UpdateHealth;
    }

    private void OnDisable()
    {
        Enemy.onHealthUpdate -= UpdateHealth;
        Gatling_Gun.onHealthUpdate -= UpdateHealth;
        Missile_Launcher.onHealthUpdate -= UpdateHealth;
        GameManager.onHealthUpdate -= UpdateHealth;
    }

    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main.gameObject;

        if (_camera == null)
            Debug.LogError("_camera is NULL.");

        if (_rootObject == null)
            Debug.LogError("_rootObject is NULL.");
    }

    // Update is called once per frame
    void Update()
    {
        //Only Mech health bars need to rotate to face the camera
        if (_rootObject.CompareTag("Mech1") || _rootObject.CompareTag("Mech2"))
            LookAtCamera();
    }

    private void LookAtCamera()
    {
        _lookDirection = _camera.transform.position - this.transform.position;
        this.gameObject.transform.rotation = Quaternion.LookRotation(_lookDirection);
    }

    private void UpdateHealth(GameObject objectToUpdate, float healthPercent)
    {
        if (objectToUpdate == _rootObject)
        {
            _healthBar.fillAmount = healthPercent;

            //Not for Mechs (Mech health bars are already red)
            if (_rootObject.CompareTag("Mech1") == false && _rootObject.CompareTag("Mech2") == false)
            {
                if (healthPercent <= GameManager.Instance.HealthWarningThreshold)
                    _healthBar.color = _healthRed;
                else if (healthPercent <= GameManager.Instance.HealthCautionThreshold)
                    _healthBar.color = _healthYellow;
                else
                    _healthBar.color = _healthGreen;
            }
        }
    }
}
