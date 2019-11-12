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
    [SerializeField] private int _ScreenWidth = Screen.width;
    [SerializeField] private int _ScreenHeight = Screen.height;
    [SerializeField] private float _mousePosition;
    [SerializeField] private float _edgeScrollWidthPercent;
    [SerializeField] private float _edgeScrollMarginTop, _edgeScrollMarginLeft, _edgeScrollMarginRight, _edgeScrollMarginBottom;
    
    // Start is called before the first frame update
    void Start()
    {
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

    private void MouseEdgeScrollCamera()
    {
        //Mouse moves Camera toward screen edge if mouse is near the edge of the screen
        if (Input.mousePosition.x < _edgeScrollMarginLeft)
        {
            this.transform.Translate(Vector3.left * _panSpeed * Time.deltaTime);
        }
        if (Input.mousePosition.x > _edgeScrollMarginRight)
        {
            this.transform.Translate(Vector3.right * _panSpeed * Time.deltaTime);
        }

        if (Input.mousePosition.y < _edgeScrollMarginBottom)
        {
            this.transform.Translate(Vector3.down * _panSpeed * Time.deltaTime);
        }
        if (Input.mousePosition.y > _edgeScrollMarginTop)
        {
            this.transform.Translate(Vector3.up * _panSpeed * Time.deltaTime);
        }

        ConfineCamera();
    }

    private void ConfineCamera()
    {
        var xPos = Mathf.Clamp(this.transform.position.x, _xMin, _xMax);
        var yPos = Mathf.Clamp(this.transform.position.y, _yMin, _yMax);
        var zPos = Mathf.Clamp(this.transform.position.z, _zMin, _zMax);

        this.transform.position = new Vector3(xPos, yPos, zPos);
    }
}
