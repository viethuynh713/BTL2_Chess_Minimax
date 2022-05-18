using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("UI settings")]
    public Text WhoWinText;
    public GameObject GameOverPanel;
    public Text blackLevelText;
    public Text whiteLevelText;
    public Slider whileSlider;
    public Slider blackSlider;

    AlphaBeta ab = new AlphaBeta();
    private bool _kingDead = false;
    float timer = 0;
    Board _board;
    bool isWhiteTurn = true;
    int maxDepthWhite = 100;
    int maxDepthBlack = 1;
    void Start()
    {
        _board = Board.Instance;
        _board.SetupBoard();
        blackSlider.value = 1;
        whileSlider.value = 1;
        GameOverPanel.SetActive(false);

    }
    public void ClickPlay(){
        isPlay = true;
    }
    void Update()
    {
        if (!isPlay)
        {
            if (blackSlider.value == 0)
            {
                maxDepthBlack = 100;
                blackLevelText.text = "0";
            }
            else
            {
                maxDepthBlack = (int)blackSlider.value;
                blackLevelText.text = maxDepthBlack.ToString();
            }
            if (whileSlider.value == 0)
            {
                maxDepthWhite = 100;
                whiteLevelText.text = "0";
            }
            else
            {
                maxDepthWhite = (int)whileSlider.value;
                whiteLevelText.text = maxDepthWhite.ToString();
            }
        }
        else
        {

            if (isWhiteTurn)
            {
                if (_kingDead)
                {
                    Debug.Log("WINNER!");
                    WhoWinText.text = "Black Win!";
                    GameOverPanel.SetActive(true);
                    isPlay = false;
                }

                if (timer < 0.5f)
                {
                    timer += Time.deltaTime;
                }
                else if (timer >= 0.5f)
                {
                    Move move = ab.GetMove(maxDepthWhite, isWhiteTurn);
                    _DoAIMove(move);
                    isWhiteTurn = !isWhiteTurn;
                    timer = 0;
                }
            }
            else
            {
                if (_kingDead)
                {
                    Debug.Log("WINNER!");
                    WhoWinText.text = "White Win!";
                    GameOverPanel.SetActive(true);
                    isPlay = false;
                }

                if (timer < 0.5f)
                {
                    timer += Time.deltaTime;
                }
                else if (timer >= 0.5f)
                {
                    Move move = ab.GetMove(maxDepthBlack, isWhiteTurn);
                    _DoAIMove(move);
                    isWhiteTurn = !isWhiteTurn;
                    timer = 0;
                }
            }
        }

    }

    public bool playerTurn = true;
    public bool isPlay = false;

    void _DoAIMove(Move move)
    {
        Tile firstPosition = move.firstPosition;
        Tile secondPosition = move.secondPosition;

        if (secondPosition.CurrentPiece && secondPosition.CurrentPiece.Type == Piece.pieceType.KING)
        {
            SwapPieces(move);
            _kingDead = true;
        }
        else
        {
            SwapPieces(move);
        }
    }

    public void SwapPieces(Move move)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Highlight");
        foreach (GameObject o in objects)
        {
            Destroy(o);
        }

        Tile firstTile = move.firstPosition;
        Tile secondTile = move.secondPosition;

        firstTile.CurrentPiece.MovePiece(new Vector3(-move.secondPosition.Position.x, 0, move.secondPosition.Position.y));

        if (secondTile.CurrentPiece != null)
        {
            if (secondTile.CurrentPiece.Type == Piece.pieceType.KING)
                _kingDead = true;
            Destroy(secondTile.CurrentPiece.gameObject);
        }


        secondTile.CurrentPiece = move.pieceMoved;
        firstTile.CurrentPiece = null;
        secondTile.CurrentPiece.position = secondTile.Position;
        secondTile.CurrentPiece.HasMoved = true;

        playerTurn = !playerTurn;
    }
}
