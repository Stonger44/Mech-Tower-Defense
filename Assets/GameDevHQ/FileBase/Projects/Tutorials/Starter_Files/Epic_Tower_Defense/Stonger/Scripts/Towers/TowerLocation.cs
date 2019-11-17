using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerLocation : MonoBehaviour
{
    [SerializeField] private bool _isOccupied;
    [SerializeField] private GameObject _vacantParticleEffect;
    [SerializeField] private GameObject _currentSelectedtower;
    [SerializeField] private GameObject _currentPlacedTower;

    public static event Action<Vector3> onVacantLocationMouseOver_Vector3;
    public static event Action onVacantLocationMouseOver;
    public static event Action onVacantLocationMouseExit;

    private void OnEnable()
    {
        TowerManager.onBrowsingTowerLocations += ToggleVacantParticleEffect;
    }

    private void OnDisable()
    {
        TowerManager.onBrowsingTowerLocations -= ToggleVacantParticleEffect;
    }

    // Start is called before the first frame update
    void Start()
    {
        _vacantParticleEffect.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseOver()
    {
        if (!_isOccupied)
        {
            onVacantLocationMouseOver_Vector3?.Invoke(this.transform.position);
            onVacantLocationMouseOver?.Invoke();
        }
    }

    private void OnMouseExit()
    {
        if (!_isOccupied)
        {
            onVacantLocationMouseExit?.Invoke();
        }
    }

    private void OnMouseDown()
    {
        
    }

    private GameObject GetCurrentSelectedTower()
    {
        return new GameObject();
    }

    private void ToggleVacantParticleEffect(bool isPlacingTower)
    {
        if (isPlacingTower && !_isOccupied)
            _vacantParticleEffect.SetActive(true);
        else
            _vacantParticleEffect.SetActive(false);
    }

    private void PlaceTower()
    {
        if (_isOccupied)
        {
            Debug.Log("Request Denied: Tower Location already occupied");
        }
        else
        {
            ////NEEDS WORK
            Debug.Log(_currentPlacedTower.name + " has been placed.");
            //Instantiate(currentTower, this.transform.position, Quaternion.identity);
            //_isOccupied = true;
        }
    }
}
