using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _xInput, _xMin, _xMax;
    [SerializeField] private float _yInput, _yMin, _yMax;
    [SerializeField] private float _zInput, _zMin, _zMax;
    [SerializeField] private float _scrollInput, _scrollSpeed;
    [SerializeField] private float _fieldOfViewMin, _fieldOfViewMax;

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
        ConfineCamera();
    }

    private void ZoomCamera()
    {
        _scrollInput = Input.GetAxis("Mouse ScrollWheel") * _scrollSpeed * Time.deltaTime;

        Debug.Log("Scroll Input: " + _scrollInput);

        //Zoom in/out
        Camera.main.fieldOfView -= _scrollInput;

        //Confine Zoom
        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, _fieldOfViewMin, _fieldOfViewMax);

        //Change transform min/max values based on magnitude of transform:
        //As field of view increases, min/max values decrease
        //Need to use LERP or something to make this dynamically smooth
        if (Camera.main.fieldOfView <= 23)
        {
            _xMin = -60;
            _xMax = -44;

            _zMin = -16;
            _zMax = -2;
        }
        else if (Camera.main.fieldOfView > 23 && Camera.main.fieldOfView < 27)
        {
            _xMin = -58;
            _xMax = -47;

            _zMin = -14;
            _zMax = -4;
        }
        else if (Camera.main.fieldOfView >= 27)
        {
            _xMin = -54;
            _xMax = -52;

            _zMin = -12;
            _zMax = -6;
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

}
