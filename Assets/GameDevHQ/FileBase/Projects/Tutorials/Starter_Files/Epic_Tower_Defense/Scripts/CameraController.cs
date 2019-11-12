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
    //[SerializeField] private Dictionary<float, List<int>> _fieldOfViewBoundaries = new Dictionary<float, List<int>>();

    // Start is called before the first frame update
    void Start()
    {
        //GenerateFieldOfViewBoundaries();
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

        //Confine camera
        ConfineCamera();
    }

    private void ZoomCamera()
    {
        _scrollInput = Input.GetAxis("Mouse ScrollWheel") * _scrollSpeed * Time.deltaTime;

        if (_scrollInput != 0)
        {
            if (_scrollInput > 0)
            {
                //Change Field of View by one
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
                //Change Field of View by one
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

            //Zoom in/out
            //Camera.main.fieldOfView -= _scrollInput;

            //Confine Zoom
            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, _fieldOfViewMin, _fieldOfViewMax);

            ////Change camera min/max confine values based on field of view (zoom):
            ////As we zoom in (field of view decreases), confined area increases
            ////Change Camera hardcoded confine values based on Field of View values
            //List<int> fieldOfViewBoundary = _fieldOfViewBoundaries[Camera.main.fieldOfView];

            //_xMin = fieldOfViewBoundary[0];
            //_xMax = fieldOfViewBoundary[1];
            //_zMin = fieldOfViewBoundary[2];
            //_zMax = fieldOfViewBoundary[3];
        }

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

    //private void GenerateFieldOfViewBoundaries()
    //{
    //    //FoVBoundares.Add(fieldOfView, new List<int>() { xMin, xMax, zMin, zMax });
    //    _fieldOfViewBoundaries.Add(20, new List<int>() { -60, -44, -16, -2 });
    //    _fieldOfViewBoundaries.Add(21, new List<int>() { -59, -45, -16, -2 });
    //    _fieldOfViewBoundaries.Add(22, new List<int>() { -59, -46, -15, -3 });
    //    _fieldOfViewBoundaries.Add(23, new List<int>() { -58, -47, -15, -3 });
    //    _fieldOfViewBoundaries.Add(24, new List<int>() { -58, -47, -14, -4 });
    //    _fieldOfViewBoundaries.Add(25, new List<int>() { -58, -47, -14, -4 });
    //    _fieldOfViewBoundaries.Add(26, new List<int>() { -57, -48, -13, -5 });
    //    _fieldOfViewBoundaries.Add(27, new List<int>() { -57, -49, -13, -5 });
    //    _fieldOfViewBoundaries.Add(28, new List<int>() { -56, -50, -13, -5 });
    //    _fieldOfViewBoundaries.Add(29, new List<int>() { -55, -51, -12, -6 });
    //    _fieldOfViewBoundaries.Add(30, new List<int>() { -54, -52, -12, -6 });
    //}

}
