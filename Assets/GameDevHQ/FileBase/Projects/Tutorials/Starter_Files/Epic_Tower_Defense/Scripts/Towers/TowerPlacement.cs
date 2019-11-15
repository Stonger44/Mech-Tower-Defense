using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacement : MonoBehaviour
{
    [SerializeField] private List<GameObject> _towerImageList;

    private Ray _rayOrigin;
    private RaycastHit _hitInfo;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {

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
