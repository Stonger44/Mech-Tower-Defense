using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacement : MonoBehaviour
{
    /*-----Towers-----*\
     0: Gatling Gun 
     1: Gatling Gun Upgrade
     2: Missle Launcher
     3: Missle Launcher Upgrade
    \*-----Towers-----*/
    [SerializeField] private List<GameObject> _towerImageList;
    private GameObject _currentTowerImage;
    public bool IsPlacingTower { get; private set; }
    private Ray _rayOrigin;
    private RaycastHit _hitInfo;
    [SerializeField] private GameObject _towerImagesContainer;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Test Code
        SelectTower();

        if (IsPlacingTower)
        {
            //Cast a ray from the mouse position on the screen into the game world. Whoa.
            //Update position of decoy tower (follow mouse position)
            //Check what the raycast hit (if hit placement spot placeholder)
            _rayOrigin = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(_rayOrigin, out _hitInfo))
            {
                _currentTowerImage.transform.position = _hitInfo.point;

                if (_hitInfo.transform.name == "TowerLocation")
                {
                    _currentTowerImage.transform.position = _hitInfo.transform.position;
                }
            }
        }

        
    }

    //Test Code
    private void SelectTower()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //Cycle through GatlingGun, MissleLauncher, and NoTower (null)
            if (_currentTowerImage == null)
            {
                _currentTowerImage = _towerImageList[0]; //Gatling Gun
            }
            else if (_currentTowerImage == _towerImageList[0])
            {
                _currentTowerImage = _towerImageList[2]; //Missle Launcher
            }
            else if (_currentTowerImage == _towerImageList[2])
            {
                _currentTowerImage = null;
            }

            //Update IsPlacingTower boolean appropriately
            IsPlacingTower = (_currentTowerImage != null) ? true : false;
            Debug.Log("IsPlacingTower: " + IsPlacingTower);

            //Put un-used images back in container (off screen)
            foreach (var tower in _towerImageList)
            {
                if (tower != _currentTowerImage)
                    tower.transform.position = _towerImagesContainer.transform.position;
            }
        }
    }

    private void PlaceTower()
    {

    }
}
