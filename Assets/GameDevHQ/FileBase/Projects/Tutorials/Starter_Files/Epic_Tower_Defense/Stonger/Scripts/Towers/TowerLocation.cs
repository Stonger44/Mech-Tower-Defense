using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerLocation : MonoBehaviour
{
    [SerializeField] private bool _isOccupied;
    [SerializeField] private GameObject _vacantParticleEffect;
    [SerializeField] private GameObject _currentSelectedtower;
    [SerializeField] private GameObject _currentPlacedTower;

    private void OnEnable()
    {
        TowerBrowsing.onBrowsingTowerLocations += ToggleVacantParticleEffect;
    }

    private void OnDisable()
    {
        TowerBrowsing.onBrowsingTowerLocations -= ToggleVacantParticleEffect;
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

    private void OnMouseEnter()
    {
        _currentSelectedtower = Camera.main.GetComponent<TowerBrowsing>().CurrentTower;

        if (_currentSelectedtower != null)
            Debug.Log("Current Selected Tower: " + _currentSelectedtower.name);
    }

    private void OnMouseExit()
    {
        
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
