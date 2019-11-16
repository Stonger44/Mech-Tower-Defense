using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBrowsing : MonoBehaviour
{
    /*-----Towers-----*\
     0: Gatling Gun 
     1: Gatling Gun Upgrade
     2: Missle Launcher
     3: Missle Launcher Upgrade
    \*-----Towers-----*/
    [SerializeField] private List<GameObject> _towerImageList;
    public GameObject CurrentTowerImage { get; private set; }
    [SerializeField] private List<GameObject> _towerList;

    [SerializeField] private GameObject _towerImagesContainer;
    public GameObject CurrentTower { get; private set; }
    public bool IsPlacingTower { get; private set; }

    private Ray _rayOrigin;
    private RaycastHit _hitInfo;
    
    public static event Action<bool> onBrowsingTowerLocations; //When in "Placing Tower" Mode

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        SelectTower();
        SelectTowerLocation();
    }

    //Test Code
    private void SelectTower()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //Cycle through GatlingGun, MissleLauncher, and NoTower (null)
            if (CurrentTowerImage == null)
            {
                //Gatling Gun
                CurrentTowerImage = _towerImageList[0];
                CurrentTower = _towerList[0];
            }
            else if (CurrentTowerImage == _towerImageList[0])
            {
                //Missle Launcher
                CurrentTowerImage = _towerImageList[2];
                CurrentTower = _towerList[2];
            }
            else if (CurrentTowerImage == _towerImageList[2])
            {
                CurrentTowerImage = null;
                CurrentTower = null;
            }

            //Update IsPlacingTower boolean appropriately
            IsPlacingTower = (CurrentTowerImage != null) ? true : false;
            onBrowsingTowerLocations?.Invoke(IsPlacingTower);

            //Put un-used images back in container (off screen)
            ResetTowerImages();
        }
    }

    private void SelectTowerLocation()
    {
        if (IsPlacingTower)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                StopBrowsingTowerLocations();
                return;
            }

            //Cast a ray from the mouse position on the screen into the game world. Whoa.
            _rayOrigin = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(_rayOrigin, out _hitInfo))
            {
                //Update position of decoy tower (follow mouse position)
                CurrentTowerImage.transform.position = _hitInfo.point;

                //Move this into TowerLocation script using OnMouseEnter/Exit/Down
                ////Check what the raycast hit (if hit placement spot placeholder)
                //if (_hitInfo.transform.tag == "TowerLocation")
                //{
                //    _currentTowerImage.transform.position = _hitInfo.transform.position;
                //    BroadcastBrowsingTowerLocations(IsPlacingTower, true);

                //    //Place Tower
                //    if (Input.GetKeyDown(KeyCode.Mouse0))
                //    {
                //        onPlaceTowerClick?.Invoke(_hitInfo.transform.name, _currentTower);
                //    }
                //}
                //else
                //{
                //    BroadcastBrowsingTowerLocations(IsPlacingTower, false);
                //}
            }
        }
    }

    private void StopBrowsingTowerLocations()
    {
        IsPlacingTower = false;
        CurrentTowerImage = null;
        CurrentTower = null;
        ResetTowerImages();

        onBrowsingTowerLocations?.Invoke(IsPlacingTower);
    }

    private void ResetTowerImages()
    {
        foreach (var tower in _towerImageList)
        {
            if (tower != CurrentTowerImage)
                tower.transform.position = _towerImagesContainer.transform.position;
        }
    }
}
