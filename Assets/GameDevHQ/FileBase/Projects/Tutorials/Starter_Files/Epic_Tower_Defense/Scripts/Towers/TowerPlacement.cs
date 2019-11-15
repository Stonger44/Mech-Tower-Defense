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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Test Code
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //Cycle through Tower1, Tower2, and NoTower (null)
            if (_currentTowerImage == null)
            {
                _currentTowerImage = _towerImageList[0]; //Gatling Gun
            }
            else if (_currentTowerImage == _towerImageList[0])
            {
                _currentTowerImage = _towerImageList[3]; //Missle Launcher
            }
            else if (_currentTowerImage == _towerImageList[3])
            {
                _currentTowerImage = null;
            }

            //Update IsPlacingTower boolean appropriately
            IsPlacingTower = (_currentTowerImage != null) ? true : false;
            Debug.Log("IsPlacingTower: " + IsPlacingTower);
        }

        //Cast a ray from the mouse position on the screen into the game world. Whoa.
        //Update postiion of decoy tower (follow mouse position)
        //Check what the raycast hit (if hit placement spot placeholder)
        _rayOrigin = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(_rayOrigin, out _hitInfo))
        {

            //_towerImage.transform.position = _hitInfo.point;
        }
    }


}
