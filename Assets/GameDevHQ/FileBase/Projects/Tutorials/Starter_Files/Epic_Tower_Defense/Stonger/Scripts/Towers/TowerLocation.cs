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
        //Subscribe to onPlacingTower event
        TowerPlacement.onTogglePlacingTower += ToggleVacantParticleEffect;
    }

    private void OnDisable()
    {
        //Unsubscribe from onPlacingTower event
        TowerPlacement.onTogglePlacingTower -= ToggleVacantParticleEffect;
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

    private void ToggleVacantParticleEffect(bool isPlacingTower)
    {
        if (isPlacingTower && !_isOccupied)
            _vacantParticleEffect.SetActive(true);
        else
            _vacantParticleEffect.SetActive(false);
    }
}
