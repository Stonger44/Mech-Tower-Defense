using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float _scrollInput;
    [SerializeField] private float _xMin, _xMax;
    [SerializeField] private float _yMin, _yMax;
    [SerializeField] private float _zMin, _zMax;
    [SerializeField] private float _xPanSpeed, _yPanSpeed, _scrollSpeed;
    [SerializeField] private float _fieldOfViewMin, _fieldOfViewMax;
    [SerializeField] private float _xDelta, _yDelta, _zDelta;
    [SerializeField] private int _ScreenWidth = Screen.width;
    [SerializeField] private int _ScreenHeight = Screen.height;
    [SerializeField] private float _mousePosition;
    [SerializeField] private float _edgeScrollWidthPercent;
    [SerializeField] private float _edgeScrollMarginTop, _edgeScrollMarginLeft, _edgeScrollMarginRight, _edgeScrollMarginBottom;
    [SerializeField] private Vector3 _newCameraPosition, _confinedCameraPosition;

    private Camera _camera;

    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;

        if (_camera == null)
            Debug.LogError("_camera is NULL.");

        //Cache _edgeScrollMargin values based on _edgeScrollWidthPercent
        _edgeScrollMarginLeft = Screen.width * _edgeScrollWidthPercent;
        _edgeScrollMarginRight = Screen.width - (Screen.width * _edgeScrollWidthPercent);

        _edgeScrollMarginTop = Screen.height - (Screen.height * _edgeScrollWidthPercent);
        _edgeScrollMarginBottom = Screen.height * _edgeScrollWidthPercent;
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
        MouseEdgeScrollCamera();
    }

    private void PanCamera()
    {
        //Get player input
        _newCameraPosition.x = Input.GetAxis("Horizontal") * _xPanSpeed * Time.deltaTime;
        _newCameraPosition.y = Input.GetAxis("Vertical") * _yPanSpeed * Time.deltaTime;

        //Move camera
        this.transform.Translate(_newCameraPosition);

        ConfineCamera();
    }

    private void ZoomCamera()
    {
        //Mathf.Lerp with scroll wheel

        _scrollInput = Input.GetAxis("Mouse ScrollWheel") * _scrollSpeed * Time.deltaTime;

        if (_scrollInput != 0)
        {
            if (_scrollInput > 0)
            {
                //Zoom In
                _camera.fieldOfView -= _yDelta;
                
                if (_camera.fieldOfView >= _fieldOfViewMin && _camera.fieldOfView <= _fieldOfViewMax)
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
                _camera.fieldOfView += _yDelta;

                if (_camera.fieldOfView >= _fieldOfViewMin && _camera.fieldOfView <= _fieldOfViewMax)
                {
                    //Adjust Camera confined area
                    _xMin += _xDelta;
                    _xMax -= _xDelta;
                    _zMin += _zDelta;
                    _zMax -= _zDelta; 
                }
            }

            //Confine Zoom
            _camera.fieldOfView = Mathf.Clamp(_camera.fieldOfView, _fieldOfViewMin, _fieldOfViewMax);
        }

        ConfineCamera();
    }

    private void MouseEdgeScrollCamera()
    {
        //Mouse moves Camera toward screen edge if mouse is near the edge of the screen
        if (Input.mousePosition.x < _edgeScrollMarginLeft)
        {
            this.transform.Translate(Vector3.left * _xPanSpeed * Time.deltaTime);
        }
        if (Input.mousePosition.x > _edgeScrollMarginRight)
        {
            this.transform.Translate(Vector3.right * _xPanSpeed * Time.deltaTime);
        }

        if (Input.mousePosition.y < _edgeScrollMarginBottom)
        {
            this.transform.Translate(Vector3.down * _yPanSpeed * Time.deltaTime);
        }
        if (Input.mousePosition.y > _edgeScrollMarginTop)
        {
            this.transform.Translate(Vector3.up * _yPanSpeed * Time.deltaTime);
        }

        ConfineCamera();
    }

    private void ConfineCamera()
    {
        _confinedCameraPosition.x = Mathf.Clamp(this.transform.position.x, _xMin, _xMax);
        _confinedCameraPosition.y = Mathf.Clamp(this.transform.position.y, _yMin, _yMax);
        _confinedCameraPosition.z = Mathf.Clamp(this.transform.position.z, _zMin, _zMax);

        this.transform.position = _confinedCameraPosition;
    }
}
