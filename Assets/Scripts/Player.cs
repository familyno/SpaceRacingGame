using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private GameObject _player;

    [SerializeField] private Transform _prefabShip;
    [SerializeField] private Transform _targetLeftWay;
    [SerializeField] private Transform _targetCenterWay;
    [SerializeField] private Transform _targetRightWay;

    [SerializeField] private GameObject _camera;


    private bool _playerStarted;
    bool _isGameOver;
    bool isLeft, isCenter = true, isRight;

    int _currentPoint;
    int _detectedPoint = 1;

    float _offsetY = 0.5f;
    private float _speed = 0.02f;
    private float _velocity = 0.0001f;
    private float _speedJump = 5f;
    private float _textSpeed = 0.2f;

    private float _minSpeed = 0.2f;
    private float _maxSpeed = 0.0001f;

    private List<Vector3> _lcurve;
    private List<Vector3> _ccurve;
    private List<Vector3> _rcurve;

    private List<Vector3> _currentCurve;

    private List<Vector3>[] _ways;

    private Wayponts _wayponts;

    private void Awake()
    {
        _wayponts = GameObject.FindObjectOfType<Wayponts>();
    }

    private void Start()
    {
        _ways = _wayponts.GetWays();
        _lcurve = _ways[0];
        _ccurve = _ways[1];
        _rcurve = _ways[2];

        _currentCurve = _ccurve;

        if (isLocalPlayer)
        {
            _camera.SetActive(true);
            //StartCoroutine(StartFly());
        }
        else
        {
            _camera.SetActive(false);
        }

        _currentCurve = _ccurve;
        StartCoroutine(StartFly());
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (!isLeft && isCenter && !isRight)
                {
                    isLeft = true;
                    isCenter = false;
                    isRight = false;
                    _currentCurve = _lcurve;
                }
                if (!isLeft && !isCenter && isRight)
                {
                    isLeft = false;
                    isCenter = true;
                    isRight = false;
                    _currentCurve = _ccurve;
                }
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (!isLeft && isCenter && !isRight)
                {
                    isLeft = false;
                    isCenter = false;
                    isRight = true;
                    _currentCurve = _rcurve;
                }
                if (isLeft && !isCenter && !isRight)
                {
                    isLeft = false;
                    isCenter = true;
                    isRight = false;
                    _currentCurve = _ccurve;
                }
            }

            if (isLeft && !isCenter && !isRight)
                _prefabShip.position = Vector3.Lerp(_prefabShip.position, _targetLeftWay.position, Time.deltaTime * _speedJump);
            if (!isLeft && isCenter && isRight)
                _prefabShip.position = Vector3.Lerp(_prefabShip.position, _targetCenterWay.position, Time.deltaTime * _speedJump);

            if (!isLeft && !isCenter && isRight)
                _prefabShip.position = Vector3.Lerp(_prefabShip.position, _targetRightWay.position, Time.deltaTime * _speedJump);
            if (!isLeft && isCenter && !isRight)
                _prefabShip.position = Vector3.Lerp(_prefabShip.position, _targetCenterWay.position, Time.deltaTime * _speedJump);


            if (Input.GetKey(KeyCode.UpArrow))
            {
                _speed -= _velocity;
                _textSpeed += _velocity;
                if (_speed <= _maxSpeed)
                {
                    _speed = _maxSpeed;
                    _textSpeed = 0.28f;
                }
            }
            else
            {
                _speed += _velocity;
                _textSpeed -= _velocity;
                if (_speed >= _minSpeed)
                {
                    _speed = _minSpeed;
                    _textSpeed = 0.1f;
                }
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                _speed += _velocity;
                _textSpeed -= _velocity;
                if (_speed >= _minSpeed)
                {
                    _speed = _minSpeed;
                    _textSpeed = 0.1f;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Столкновение с " + other.name);
    }

    private void MoveOnWaypoints(GameObject player, List<Vector3> waypoints)
    {
        if (player == null)
            return;
        
        if (isLocalPlayer)
        {
            player.transform.position = new Vector3(waypoints[_currentPoint][0], waypoints[_currentPoint][1] + _offsetY, waypoints[_currentPoint][2]);
            player.transform.LookAt(waypoints[_currentPoint + _detectedPoint]);

            _wayponts.SetSpeed(_textSpeed);
        }
    }

    private IEnumerator StartFly()
    {
        while (true)
        {
            yield return new WaitForSeconds(_speed);
            if (_wayponts.IsStart)
            {
                if (_currentPoint + _detectedPoint < _currentCurve.Count && !_isGameOver)
                {
                    MoveOnWaypoints(gameObject, _ccurve);

                    MoveOnWaypoints(_player, _currentCurve);
                    _currentPoint++;

                    //Debug.Log(_currentPoint);
                }
                else if (!_isGameOver)
                {
                    _isGameOver = true;
                    Debug.Log("GAME OVER !!!");
                    break;
                }

            }
        }
    }
}
