using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _xInput, _xMin, _xMax;
    [SerializeField] private float _yInput, _yMin, _yMax;
    [SerializeField] private float _zInput, _zMin, _zMax;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveCamera();
    }

    private void MoveCamera()
    {
        PanCamera();
        ZoomCamera();
        MouseMoveCamera();
    }

    private void PanCamera()
    {
        //Get player input
        _xInput = Input.GetAxis("Horizontal") * _speed * Time.deltaTime;
        _yInput = Input.GetAxis("Vertical") * _speed * Time.deltaTime;

        //Move camera
        if (_xInput != 0)
            this.transform.Translate(Vector3.right * _xInput);
        if (_yInput != 0)
            this.transform.Translate(Vector3.up * _yInput);

        //Confine camera
        var xPos = Mathf.Clamp(this.transform.position.x, _xMin, _xMax);
        var yPos = Mathf.Clamp(this.transform.position.y, _yMin, _yMax);
        var zPos = Mathf.Clamp(this.transform.position.z, _zMin, _zMax);

        this.transform.position = new Vector3(xPos, yPos, zPos);

    }

    private void ZoomCamera()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            //Zoom in/out
        }
    }

    private void MouseMoveCamera()
    {
        //Mouse moves Camera toward mouse direction if mouse is at the edge of the screen

        //If (Input.mousPosition.x > )
        //Screen.width
        //transform.translate(Vector3.left * Time.deltaTime);
    }

}
