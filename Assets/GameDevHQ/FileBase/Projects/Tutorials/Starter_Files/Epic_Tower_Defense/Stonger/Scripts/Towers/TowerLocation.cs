using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerLocation : MonoBehaviour
{
    [SerializeField] private bool _isOccupied;
    [SerializeField] private GameObject _vacantParticleEffect;

    //method to place tower
    //maintain reference to tower type (for upgrade functionality)

    private void OnEnable()
    {
        TowerPlacement.onBrowsingTowerLocations += ToggleVacantParticleEffect;
    }

    private void OnDisable()
    {
        TowerPlacement.onBrowsingTowerLocations -= ToggleVacantParticleEffect;
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

    private void ToggleVacantParticleEffect(bool isPlacingTower, bool onTowerLocation)
    {
        if (isPlacingTower && !_isOccupied)
            _vacantParticleEffect.SetActive(true);
        else
            _vacantParticleEffect.SetActive(false);
    }
}
