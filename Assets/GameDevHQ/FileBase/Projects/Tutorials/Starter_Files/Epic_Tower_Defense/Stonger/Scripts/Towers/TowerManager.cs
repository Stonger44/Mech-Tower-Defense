using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoSingleton<TowerManager>
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
    public GameObject CurrentTower { get; private set; }

    public GameObject TowerImagesContainer { get; set; }
    
    public bool IsPlacingTower { get; private set; }

    private Ray _rayOrigin;
    private RaycastHit _hitInfo;
    
    public static event Action<bool> onBrowsingTowerLocations; //When in "Placing Tower" Mode

    private bool _onVacantLocation;

    public override void Init()
    {
        IsPlacingTower = false;
        TowerImagesContainer = GameObject.Find("TowerImagesContainer");
    }

    private void OnEnable()
    {
        TowerLocation.onVacantLocationMouseOver_Vector3 += SnapTowerImageToTowerLocation;
        TowerLocation.onOccupiedLocationMouseOver += UpdateTowerImageToFollowMouse;
        TowerLocation.onLocationMouseExit += UpdateTowerImageToFollowMouse;
        TowerLocation.onPlaceTower += StopBrowsingTowerLocations;
    }

    private void OnDisable()
    {
        TowerLocation.onVacantLocationMouseOver_Vector3 -= SnapTowerImageToTowerLocation;
        TowerLocation.onOccupiedLocationMouseOver -= UpdateTowerImageToFollowMouse;
        TowerLocation.onLocationMouseExit -= UpdateTowerImageToFollowMouse;
        TowerLocation.onPlaceTower -= StopBrowsingTowerLocations;
    }

    // Update is called once per frame
    void Update()
    {
        //SelectTower();

        if (IsPlacingTower)
            SelectTowerLocation();
    }

    ////Test Code
    //private void SelectTower()
    //{
    //    if (Input.GetKeyDown(KeyCode.Alpha1))
    //    {
    //        //Cycle through GatlingGun, MissleLauncher, and NoTower (null)
    //        if (CurrentTowerImage == null)
    //        {
    //            //Gatling Gun
    //            CurrentTowerImage = _towerImageList[0];
    //            CurrentTower = _towerList[0];
    //        }
    //        else if (CurrentTowerImage == _towerImageList[0])
    //        {
    //            //Gatling Gun Upgrade
    //            CurrentTowerImage = _towerImageList[1];
    //            CurrentTower = _towerList[1];
    //        }
    //        else if (CurrentTowerImage == _towerImageList[1])
    //        {
    //            //Missile Launcher
    //            CurrentTowerImage = _towerImageList[2];
    //            CurrentTower = _towerList[2];
    //        }
    //        else if (CurrentTowerImage == _towerImageList[2])
    //        {
    //            //Missile Launcher Upgrade
    //            CurrentTowerImage = _towerImageList[3];
    //            CurrentTower = _towerList[3];
    //        }
    //        else if (CurrentTowerImage == _towerImageList[3])
    //        {
    //            //No Tower Selected
    //            CurrentTowerImage = null;
    //            CurrentTower = null;
    //        }

    //        //Update IsPlacingTower boolean appropriately
    //        IsPlacingTower = (CurrentTowerImage != null) ? true : false;
    //        onBrowsingTowerLocations?.Invoke(IsPlacingTower);

    //        //Put un-used images back in container (off screen)
    //        ResetTowerImages(false);
    //    }
    //}

    public void OnTowerSelectedForPlacement(GameObject selectedTowerImage)
    {
        CurrentTowerImage = selectedTowerImage;

        switch (CurrentTowerImage.name)
        {
            case "Gatling_Gun_Image":
                CurrentTower = _towerList[0];
                break;
            case "Missile_Launcher_Image":
                CurrentTower = _towerList[2];
                break;
            default:
                Debug.LogError("CurrentTowerImage name not recognized.");
                break;
        }


        //CurrentTowerImage = _towerImageList[0];
        //CurrentTower = _towerList[0];

        //Update IsPlacingTower boolean appropriately
        IsPlacingTower = (CurrentTowerImage != null) ? true : false;
        onBrowsingTowerLocations?.Invoke(IsPlacingTower);

        //Put un-used images back in container (off screen)
        ResetTowerImages(false);
    }

    private void SelectTowerLocation()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            StopBrowsingTowerLocations();
            return;
        }

        if (!_onVacantLocation)
        {
            //Cast a ray from the mouse position on the screen into the game world. Whoa.
            _rayOrigin = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(_rayOrigin, out _hitInfo))
            {
                //Update position of decoy tower (follow mouse position)
                CurrentTowerImage.transform.position = _hitInfo.point;
            }
        }
    }

    private void StopBrowsingTowerLocations()
    {
        IsPlacingTower = false;
        CurrentTowerImage = null;
        CurrentTower = null;
        ResetTowerImages(true);

        onBrowsingTowerLocations?.Invoke(IsPlacingTower);
    }

    private void ResetTowerImages(bool resetAllImages)
    {
        if (resetAllImages)
            CurrentTowerImage = null;

        foreach (var tower in _towerImageList)
        {
            if (tower != CurrentTowerImage)
                tower.transform.position = TowerImagesContainer.transform.position;
        }
    }

    private void SnapTowerImageToTowerLocation(Vector3 TowerLocation)
    {
        _onVacantLocation = true;

        if (CurrentTower != null)
            CurrentTowerImage.transform.position = TowerLocation;
    }

    private void UpdateTowerImageToFollowMouse()
        => _onVacantLocation = false;
}
