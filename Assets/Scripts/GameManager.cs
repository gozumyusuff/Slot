using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] ScoreManager scoreManager;

    [SerializeField]
    private int boardHeight, boardWidth;
    [SerializeField]
    private GameObject[] fruitTypeObjects;


    [SerializeField] private GameObject board;
    private GameObject[,] gameBoard;
    private Vector3 offset = new(0, 0, -1);
    private List<GameObject> matchLines;

    private void Awake()
    {
        gameBoard = new GameObject[boardHeight, boardWidth];
        matchLines = new List<GameObject>();        
    }

    void Start()
    {
        CreateBoard();
        CheckForMatches();        
    }

    public void Spin()
    {
        ClearBoard();
        CreateBoard();
        CheckForMatches();

    }

    private void ClearBoard()
    {
        foreach (GameObject l in matchLines)
        {
            GameObject.Destroy(l);
        }
        matchLines.Clear();
    }

    private void CreateBoard()
    {
        for (int i = 0; i < boardHeight; i++)
        {
            for (int j = 0; j < boardWidth; j++)
            {
                GameObject gridPosition = board.transform.Find(i + " " + j).gameObject;
                if (gridPosition.transform.childCount > 0)
                {
                    GameObject destroyPiece = gridPosition.transform.GetChild(0).gameObject;
                    Destroy(destroyPiece);
                }
                GameObject pieceType = fruitTypeObjects[Random.Range(0, fruitTypeObjects.Length)];
                GameObject thisPiece = Instantiate(pieceType, gridPosition.transform.position + offset, Quaternion.identity);
                
                thisPiece.name = pieceType.name;
                thisPiece.transform.parent = gridPosition.transform;
                gameBoard[i, j] = thisPiece;
            }
        }
    }

    private void CheckForMatches()
    {
        DiagonalCheck();
        VerticalCheck();
        HorizontalCheck();
        FiveInARowCheck();

    }

    private void FiveInARowCheck()
    {
        //Five in a row
        List<GameObject> points = new List<GameObject>();
        List<string> names = new List<string>();
        for (int i = 0; i < boardHeight; i++)
        {
            points.Clear();
            GameObject startPoint = gameBoard[i, 0];
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
                    if (startPoint.name == gameBoard[k, j].name)
                    {
                        points.Add(gameBoard[k, j]);
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
                myLine.transform.position = startPoint.transform.position + offset;
                myLine.AddComponent<LineRenderer>();
                LineRenderer lr = myLine.GetComponent<LineRenderer>();
                lr.positionCount = boardWidth;
                lr.startWidth = .1f;
                lr.endWidth = .1f;
                for (int a = 0; a < points.Count; a++)
                    lr.SetPosition(a, points[a].transform.position + offset);
                matchLines.Add(myLine);

                // Five in a row olduðunda skoru güncelle
                scoreManager.AddScore();

            }
        }
    }

    private void HorizontalCheck()
    {
        //Horizontal Matches
        for (int i = 0; i < boardHeight; i++)
        {
            int matchLength = 1;
            GameObject matchBegin = gameBoard[i, 0];
            GameObject matchEnd = null;
            for (int j = 0; j < boardWidth - 1; j++)
            {
                if (gameBoard[i, j].name == gameBoard[i, j + 1].name)
                {
                    matchLength++;
                }
                else
                {
                    if (matchLength >= 3)
                    {
                        matchEnd = gameBoard[i, j];
                        scoreManager.AddScore(matchLength);
                        DrawLine(matchBegin.transform.position + offset, matchEnd.transform.position + offset);
                    }
                    matchBegin = gameBoard[i, j + 1];
                    matchLength = 1;
                }
            }
            if (matchLength >= 3)
            {
                matchEnd = gameBoard[i, boardWidth - 1];
                scoreManager.AddScore(matchLength);
                DrawLine(matchBegin.transform.position + offset, matchEnd.transform.position + offset);
            }
        }
    }

    private void VerticalCheck()
    {
        //Vertical Matches
        for (int i = 0; i < boardWidth; i++)
        {
            int matchLength = 1;
            GameObject matchBegin = gameBoard[0, i];
            GameObject matchEnd = null;
            for (int j = 1; j < boardHeight; j++)
            {
                if (gameBoard[j, i].name == gameBoard[j - 1, i].name)
                {
                    matchLength++;
                }
                else
                {
                    if (matchLength >= 3)
                    {
                        matchEnd = gameBoard[j - 1, i];
                        scoreManager.AddScore(10 * (matchLength - 2));
                        DrawLine(matchBegin.transform.position + offset, matchEnd.transform.position + offset);
                    }
                    matchBegin = gameBoard[j, i];
                    matchLength = 1;
                }
            }
            if (matchLength >= 3)
            {
                matchEnd = gameBoard[boardHeight - 1, i];
                scoreManager.AddScore(matchLength);
                DrawLine(matchBegin.transform.position + offset, matchEnd.transform.position + offset);
            }
        }
    }

    private void DiagonalCheck()
    {
        //Diagonal Matches (soldan saða)
        for (int i = 0; i < boardHeight - 2; i++)
        {
            for (int j = 0; j < boardWidth - 2; j++)
            {
                if (gameBoard[i, j].name == gameBoard[i + 1, j + 1].name &&
                    gameBoard[i, j].name == gameBoard[i + 2, j + 2].name)
                {
                    // Diagonal kazançlý line'ý görselleþtir
                    Vector3 start = gameBoard[i, j].transform.position + offset;
                    Vector3 end = gameBoard[i + 2, j + 2].transform.position + offset;
                    DrawLine(start, end);
                    scoreManager.AddScore();

                }
            }
        }

        //Diagonal Matches (saðdan sola)
        for (int i = 0; i < boardHeight - 2; i++)
        {
            for (int j = 2; j < boardWidth; j++)
            {
                if (gameBoard[i, j].name == gameBoard[i + 1, j - 1].name &&
                    gameBoard[i, j].name == gameBoard[i + 2, j - 2].name)
                {
                    // Diagonal kazançlý line'ý görselleþtir
                    Vector3 start = gameBoard[i, j].transform.position + offset;
                    Vector3 end = gameBoard[i + 2, j - 2].transform.position + offset;
                    DrawLine(start, end);
                    scoreManager.AddScore();


                }
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
        matchLines.Add(myLine);
    }
}





