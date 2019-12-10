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
    public GameObject CurrentlyViewedTower { get; private set; }
    public GameObject TowerImagesContainer { get; set; }
    
    public bool IsPlacingTower { get; private set; }
    public bool IsViewingTower { get; private set; }
    public bool IsDismantlingTower { get; private set; }

    private Ray _rayOrigin;
    private RaycastHit _hitInfo;
    
    public static event Action<bool> onBrowsingTowerLocations; //When in "Placing Tower" Mode

    public static event Action<GameObject> onStopViewingTower;
    public static event Action onStopViewingTowerUI;
    public static event Action<GameObject> onDismantleTower;
    public static event Action<GameObject, GameObject> onUpgradeTower;

    private bool _onVacantLocation;

    public override void Init()
    {
        IsPlacingTower = false;
        IsViewingTower = false;
        TowerImagesContainer = GameObject.Find("TowerImagesContainer");
    }

    private void OnEnable()
    {
        //Place Tower
        TowerLocation.onVacantLocationMouseOver_Vector3 += SnapTowerImageToTowerLocation;
        TowerLocation.onOccupiedLocationMouseOver += UpdateTowerImageToFollowMouse;
        TowerLocation.onLocationMouseExit += UpdateTowerImageToFollowMouse;
        TowerLocation.onPlaceTower += StopBrowsingTowerLocations;

        //View Tower
        TowerLocation.onViewingCurrentTower += StartViewingTower;
    }

    private void OnDisable()
    {
        //Place Tower
        TowerLocation.onVacantLocationMouseOver_Vector3 -= SnapTowerImageToTowerLocation;
        TowerLocation.onOccupiedLocationMouseOver -= UpdateTowerImageToFollowMouse;
        TowerLocation.onLocationMouseExit -= UpdateTowerImageToFollowMouse;
        TowerLocation.onPlaceTower -= StopBrowsingTowerLocations;

        //View Tower
        TowerLocation.onViewingCurrentTower -= StartViewingTower;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsPlacingTower)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                StopBrowsingTowerLocations();
                return;
            }

            SelectTowerLocation();
        }

        if (IsViewingTower)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                StopViewingTower();
            }
        }
    }

    /*----------View Tower----------*/
    private void StartViewingTower(GameObject currentlyViewedTower)
    {
        IsViewingTower = true;
        CurrentlyViewedTower = currentlyViewedTower;
    }

    public void StopViewingTower()
    {
        IsViewingTower = false;
        IsDismantlingTower = false;
        onStopViewingTower?.Invoke(CurrentlyViewedTower);
        onStopViewingTowerUI?.Invoke();
        CurrentlyViewedTower = null;
    }

    public void UpgradeTower()
    {
        GameObject towerToUpgradeTo = null;

        switch (CurrentlyViewedTower.tag)
        {
            case "Tower_Gatling_Gun":
                towerToUpgradeTo = _towerList[1]; //Gatling_Gun_Upgrade
                break;
            case "Tower_Missile_Launcher":
                towerToUpgradeTo = _towerList[3]; //Missile_Launcher_Upgrade
                break;
            default:
                Debug.LogError("CurrentlyViewedTower.tag not recognized!");
                break;
        }

        onUpgradeTower?.Invoke(CurrentlyViewedTower, towerToUpgradeTo);
    }

    public void DismantleTower()
    {
        IsDismantlingTower = true;
        onDismantleTower?.Invoke(CurrentlyViewedTower);
        StopViewingTower();
    }
    /*----------View Tower----------*/

    /*----------Place Tower----------*/
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

        //Update IsPlacingTower boolean appropriately
        IsPlacingTower = (CurrentTowerImage != null) ? true : false;
        onBrowsingTowerLocations?.Invoke(IsPlacingTower);

        //Put un-used images back in container (off screen)
        ResetTowerImages(false);
    }

    private void SelectTowerLocation()
    {
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
    /*----------Place Tower----------*/
}
