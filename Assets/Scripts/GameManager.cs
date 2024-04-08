using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private int boardHeight, boardWidth;
    [SerializeField]
    private GameObject[] colorIcons;

    private GameObject _board;
    private GameObject[,] _gameBoard;
    private Vector3 _offset = new Vector3(0, 0, -1);
    private List<GameObject> _matchLines;

    [SerializeField]
    private TextMeshProUGUI scoreText;
    private int _score = 0;

    void Start()
    {
        // oyun ba�lad���nda rastgele dizilme.
        _board = GameObject.Find("3x5Gameboard");
        _gameBoard = new GameObject[boardHeight, boardWidth];
        _matchLines = new List<GameObject>();
        for (int i = 0; i < boardHeight; i++)
        {
            for (int j = 0; j < boardWidth; j++)
            {
                GameObject gridPosition = _board.transform.Find(i + " " + j).gameObject;
                GameObject pieceType = colorIcons[Random.Range(0, colorIcons.Length)];
                GameObject thisPiece = Instantiate(pieceType, gridPosition.transform.position + _offset, Quaternion.identity);
                thisPiece.name = pieceType.name;
                thisPiece.transform.parent = gridPosition.transform;
                _gameBoard[i, j] = thisPiece;
            }
        }
    }

    public void Spin()
    // spin butonuna bas�ld���nda rastgele bir �ekilde her sat�r ve s�tunu yeniden ayarlamaya yarar.
    {
        //�nceki �izgileri temizle
        foreach (GameObject l in _matchLines)
        {
            GameObject.Destroy(l);
        }
        _matchLines.Clear();
        for (int i = 0; i < boardHeight; i++)
        {
            for (int j = 0; j < boardWidth; j++)
            {
                GameObject gridPosition = _board.transform.Find(i + " " + j).gameObject;
                if (gridPosition.transform.childCount > 0)
                {
                    GameObject destroyPiece = gridPosition.transform.GetChild(0).gameObject;
                    Destroy(destroyPiece);
                }
                GameObject pieceType = colorIcons[Random.Range(0, colorIcons.Length)];
                GameObject thisPiece = Instantiate(pieceType, gridPosition.transform.position + _offset, Quaternion.identity);
                thisPiece.name = pieceType.name;
                thisPiece.transform.parent = gridPosition.transform;
                _gameBoard[i, j] = thisPiece;
            }
        }
        CheckForMatches();
    }

    private void CheckForMatches()
    {
        //Diagonal Matches (soldan sa�a)
        for (int i = 0; i < boardHeight - 2; i++)
        {
            for (int j = 0; j < boardWidth - 2; j++)
            {
                if (_gameBoard[i, j].name == _gameBoard[i + 1, j + 1].name &&
                    _gameBoard[i, j].name == _gameBoard[i + 2, j + 2].name)
                {
                    // Diagonal kazan�l� line'� g�rselle�tir
                    Vector3 start = _gameBoard[i, j].transform.position + _offset;
                    Vector3 end = _gameBoard[i + 2, j + 2].transform.position + _offset;
                    DrawLine(start, end);
                }
            }
        }

        //Diagonal Matches (sa�dan sola)
        for (int i = 0; i < boardHeight - 2; i++)
        {
            for (int j = 2; j < boardWidth; j++)
            {
                if (_gameBoard[i, j].name == _gameBoard[i + 1, j - 1].name &&
                    _gameBoard[i, j].name == _gameBoard[i + 2, j - 2].name)
                {
                    // Diagonal kazan�l� line'� g�rselle�tir
                    Vector3 start = _gameBoard[i, j].transform.position + _offset;
                    Vector3 end = _gameBoard[i + 2, j - 2].transform.position + _offset;
                    DrawLine(start, end);
                }
            }
        }


        //Vertical Matches
        for (int i = 0; i < boardWidth; i++)
        {
            int matchLength = 1;
            GameObject matchBegin = _gameBoard[0, i];
            GameObject matchEnd = null;
            for (int j = 1; j < boardHeight; j++)
            {
                if (_gameBoard[j, i].name == _gameBoard[j - 1, i].name)
                {
                    matchLength++;
                }
                else
                {
                    if (matchLength >= 3)
                    {
                        matchEnd = _gameBoard[j - 1, i];
                        _score += 10 * (matchLength - 2);
                        DrawLine(matchBegin.transform.position + _offset, matchEnd.transform.position + _offset);
                    }
                    matchBegin = _gameBoard[j, i];
                    matchLength = 1;
                }
            }
            if (matchLength >= 3)
            {
                matchEnd = _gameBoard[boardHeight - 1, i];
                _score += 10 * (matchLength - 2);
                DrawLine(matchBegin.transform.position + _offset, matchEnd.transform.position + _offset);
            }
        }

        //Horizontal Matches
        for (int i = 0; i < boardHeight; i++)
        {
            int matchLength = 1;
            GameObject matchBegin = _gameBoard[i, 0];
            GameObject matchEnd = null;
            for (int j = 0; j < boardWidth - 1; j++)
            {
                if (_gameBoard[i, j].name == _gameBoard[i, j + 1].name)
                {
                    matchLength++;
                }
                else
                {
                    if (matchLength >= 3)
                    {
                        matchEnd = _gameBoard[i, j];
                        _score += 10 * (matchLength - 2);
                        DrawLine(matchBegin.transform.position + _offset, matchEnd.transform.position + _offset);
                    }
                    matchBegin = _gameBoard[i, j + 1];
                    matchLength = 1;
                }
            }
            if (matchLength >= 3)
            {
                matchEnd = _gameBoard[i, boardWidth - 1];
                DrawLine(matchBegin.transform.position + _offset, matchEnd.transform.position + _offset);
            }
            scoreText.text = _score.ToString();
        }
        //Five in a row
        List<GameObject> points = new List<GameObject>();
        List<string> names = new List<string>();
        for (int i = 0; i < boardHeight; i++)
        {
            points.Clear();
            GameObject startPoint = _gameBoard[i, 0];
            if (names.Contains(startPoint.name))
                continue;
            else
                names.Add(startPoint.name);
            points.Add(startPoint);
            for (int j = 1; j < boardWidth; j++)
            {
                bool notFound = false;
                for (int k = 0; k < boardHeight; k++)
                {
                    if (startPoint.name == _gameBoard[k, j].name)
                    {
                        points.Add(_gameBoard[k, j]);
                        break;
                    }
                    if (k == boardHeight - 1)
                        notFound = true;
                }
                if (notFound)
                    break;
            }
            if (points.Count == boardWidth)
            {
                GameObject myLine = new GameObject();
                myLine.transform.position = startPoint.transform.position + _offset;
                myLine.AddComponent<LineRenderer>();
                LineRenderer lr = myLine.GetComponent<LineRenderer>();
                lr.positionCount = boardWidth;
                lr.startWidth = .1f;
                lr.endWidth = .1f;
                for (int a = 0; a < points.Count; a++)
                    lr.SetPosition(a, points[a].transform.position + _offset);
                _matchLines.Add(myLine);

                // Five in a row oldu�unda skoru g�ncelle
                _score += 10; // �rnek bir skor art���
            }
        }

    }
        void DrawLine(Vector3 start, Vector3 end)
        {
            GameObject myLine = new GameObject();
            myLine.transform.position = start;
            myLine.AddComponent<LineRenderer>();
            LineRenderer lr = myLine.GetComponent<LineRenderer>();
            lr.startWidth = .1f;
            lr.endWidth = .1f;
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
            _matchLines.Add(myLine);
        }
    }





