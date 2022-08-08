using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Wayponts : NetworkBehaviour
{
    [Header("[PLAYER]")]
    [SerializeField] private GameObject _prefabPlayer;

    [Header("[WAYS]")]

    [SerializeField] private GameObject _brushL;
    [SerializeField] private GameObject _brushC;
    [SerializeField] private GameObject _brushR;

    [Header("[UI]")]
    [SerializeField] private GameObject _canvas;
    [SerializeField] private Text _speedText;
    [SerializeField] private Text _timerText;

    private bool _isGameOver;
    
    [SyncVar]
    public bool IsStart;

    private int _countPoints = 0;
    private int _countPointsOriginal = 0;

    private int hours = 0;
    private int minuts = 0;
    private int sec = 0;

    private int beginLeftCurve = 0;
    private int beginCenterCurve = 0;
    private int beginRightCurve = 0;

    private string _nameFileWaypoints = "Text/Seg03Waypoints";
    private string _path;
    private string _maFile;
    private string[] nameObj;

    private string hour;
    private string min;
    private string secs;

    [SyncVar(hook = nameof(SetTime))]
    private string currentTime;
    private string spentTime;


    private List<Vector3> _lcurve;
    private List<Vector3> _ccurve;
    private List<Vector3> _rcurve;

    private List<Vector3>[] _ways;

    private DateTime _timeNow = new DateTime();

    private void Start()
    {
        _maFile = Resources.Load<TextAsset>(_nameFileWaypoints).ToString();

        string[] arrayString = _maFile.Split('\n');

        for (int i = 0; i < arrayString.Length; i++)
        {
            if (arrayString[i].Contains("createNode transform"))
            {
                nameObj = arrayString[i].Split('"');
                string[] numbs = arrayString[i + 4].Split(' ');

                if (_countPoints == 0)
                {
                    _countPoints = Convert.ToInt32(numbs[1]); //2581
                    _countPointsOriginal = _countPoints + 1; //2582
                }
                if (nameObj[1] == "lcurve")
                {
                    beginLeftCurve = GetBeginPoints(arrayString, i);
                }
                else if (nameObj[1] == "ccurve")
                {
                    beginCenterCurve = GetBeginPoints(arrayString, i);
                }
                else if (nameObj[1] == "rcurve")
                {
                    beginRightCurve = GetBeginPoints(arrayString, i);
                }
            }
        }

        if (beginLeftCurve > 0 && beginCenterCurve > 0 && beginRightCurve > 0)
        {
            _lcurve = FillListPoint(arrayString, beginLeftCurve);
            _ccurve = FillListPoint(arrayString, beginCenterCurve);
            _rcurve = FillListPoint(arrayString, beginRightCurve);
        }

        _ways = new List<Vector3>[3];
        _ways[0] = _lcurve;
        _ways[1] = _ccurve;
        _ways[2] = _rcurve;

        if(isServer)
        {
            StartCoroutine(TimerTick());
        }

        StartCoroutine(DrawRace());
        _canvas.SetActive(true);
    }

    private void Update()
    {
        if (Menu.GameType == Menu.TypeGame.SINGLE)
        {
            if (NetworkServer.connections.Count > 0 && !IsStart)
            {
                IsStart = true;
            }
        }
        else
        {
            if (NetworkServer.connections.Count > 1 && !IsStart)
            {
                IsStart = true;
            }
        }
    }
    
    private IEnumerator TimerTick()
    {
        for (int hrs = hours; hrs <= 24; hrs++)
        {
            for (int mnts = minuts; mnts <= 59; mnts++)
            {
                for (int secunds = sec; secunds <= 59; secunds++)
                {

                    hour = String.Format("{0:d2}", hrs);
                    min = String.Format("{0:d2}", mnts);
                    secs = String.Format("{0:d2}", secunds);

                    yield return new WaitForSeconds(1);


                    if (mnts == 15)
                    {
                        Application.Quit();
                    }

                    currentTime = hour + ":" + min + ":" + secs;
                    spentTime = min + " мин. " + secs + " сек.";
                    _timerText.text = currentTime;

                    if (_isGameOver)
                        break;
                }
            }
        }

    }

    private IEnumerator DrawRace()
    {
        for (int i = 0; i < _ccurve.Count; i++)
        {
            yield return new WaitForSeconds(0.0001f);
            _brushL.transform.position = _lcurve[i];//new Vector3(curve[i][0], curve[i][1], curve[i][2]);
            _brushC.transform.position = _ccurve[i];
            _brushR.transform.position = _rcurve[i];
        }
    }

    private int GetBeginPoints(string[] stringsArray, int begin)
    {
        string str = "";
        string str1 = "";
        for (int i = begin + 6; i < stringsArray.Length; i++)
        {
            str = _countPoints.ToString();
            str1 = _countPointsOriginal.ToString();

            if (stringsArray[i].Contains(str1) && stringsArray[i - 1].Contains(str))
            {
                return i + 1;
            }
        }
        return 0;
    }

    private List<Vector3> FillListPoint(string[] listPoint, int beginPoint)
    {
        List<Vector3> listPoints = new List<Vector3>();
        string currentString;
        string[] xyzValue;
        float x, y, z;
        
        for (int i = beginPoint; i < beginPoint + _countPointsOriginal; i++)
        {
            currentString = listPoint[i];
            xyzValue = currentString.Split(' ');
            
            if(xyzValue.Length == 3)
            {
                x = float.Parse(xyzValue[0], CultureInfo.InvariantCulture);
                y = float.Parse(xyzValue[1], CultureInfo.InvariantCulture);
                z = float.Parse(xyzValue[2], CultureInfo.InvariantCulture);

                listPoints.Add(new Vector3(x, y, z));
            }
        }
        return listPoints;
    }

    private void SetTime(string oldTime, string newTime)
    {
        _timerText.text = newTime;
    }

    public void SetSpeed(float speed)
    {
        _speedText.text = "SPEED: " + Mathf.Round(speed * 1000);
    }

    public List<Vector3>[] GetWays()
    {
        if (_ways != null)
            return _ways;
        return null;
    }
}
