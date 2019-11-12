using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float _xInput, _yInput, _zInput, _scrollInput;
    [SerializeField] private float _xMin, _xMax;
    [SerializeField] private float _yMin, _yMax;
    [SerializeField] private float _zMin, _zMax;
    [SerializeField] private float _panSpeed, _scrollSpeed;
    [SerializeField] private float _fieldOfViewMin, _fieldOfViewMax;
    [SerializeField] private float _xDelta, _yDelta, _zDelta;

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
        _xInput = Input.GetAxis("Horizontal") * _panSpeed * Time.deltaTime;
        _yInput = Input.GetAxis("Vertical") * _panSpeed * Time.deltaTime;

        //Move camera
        if (_xInput != 0)
            this.transform.Translate(Vector3.right * _xInput);
        if (_yInput != 0)
            this.transform.Translate(Vector3.up * _yInput);

        ConfineCamera();
    }

    private void ZoomCamera()
    {
        _scrollInput = Input.GetAxis("Mouse ScrollWheel") * _scrollSpeed * Time.deltaTime;

        if (_scrollInput != 0)
        {
            if (_scrollInput > 0)
            {
                //Zoom In
                Camera.main.fieldOfView -= _yDelta;
                
                if (Camera.main.fieldOfView >= _fieldOfViewMin && Camera.main.fieldOfView <= _fieldOfViewMax)
                {
                    //Adjust Camera confined area
                    _xMin -= _xDelta;
                    _xMax += _xDelta;
                    _zMin -= _zDelta;
                    _zMax += _zDelta; 
                }
            }
            else if (_scrollInput < 0)
            {
                //Zoom Out
                Camera.main.fieldOfView += _yDelta;

                if (Camera.main.fieldOfView >= _fieldOfViewMin && Camera.main.fieldOfView <= _fieldOfViewMax)
                {
                    //Adjust Camera confined area
                    _xMin += _xDelta;
                    _xMax -= _xDelta;
                    _zMin += _zDelta;
                    _zMax -= _zDelta; 
                }
            }

            //Confine Zoom
            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, _fieldOfViewMin, _fieldOfViewMax);
        }

        ConfineCamera();
    }

    private void MouseMoveCamera()
    {
        //Mouse moves Camera toward mouse direction if mouse is at the edge of the screen

        //If (Input.mousPosition.x > )
        //Screen.width
        //transform.translate(Vector3.left * Time.deltaTime);
    }

    private void ConfineCamera()
    {
        var xPos = Mathf.Clamp(this.transform.position.x, _xMin, _xMax);
        var yPos = Mathf.Clamp(this.transform.position.y, _yMin, _yMax);
        var zPos = Mathf.Clamp(this.transform.position.z, _zMin, _zMax);

        this.transform.position = new Vector3(xPos, yPos, zPos);
    }
}
