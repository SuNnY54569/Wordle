using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    private static readonly KeyCode[] SUPPORTED_KEYS = new KeyCode[]
    {
        KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F,
        KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L,
        KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R,
        KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X,
        KeyCode.Y, KeyCode.Z
    };

    [SerializeField] private Row[] rows;

    [SerializeField] private string[] solutions;
    [SerializeField] private string[] validWords;
    [SerializeField] private string randomWord;

    [SerializeField] private int rowIndex;
    [SerializeField] private int columnIndex;
    
    [Header("States")] 
    public Tile.State emptyState;
    public Tile.State occupiedState;
    public Tile.State correctState;
    public Tile.State wrongSpotState;
    public Tile.State incorrectState;

    [Header("UI")] 
    [SerializeField] private TextMeshProUGUI warningText;
    [SerializeField] private TextMeshProUGUI showRandomWordText;
    [SerializeField] private Button tryAgainButton;
    [SerializeField] private Button newWordButton;
    [SerializeField] private Button winNewWordButton;
    [SerializeField] private Button showWordButton;
    
    private bool isWordVisible = false;
    private Row currentRow;
    

    private void Awake()
    {
        rows = GetComponentsInChildren<Row>();
        warningText.gameObject.SetActive(false);
    }

    private void Start()
    {
        //LoadData();
        //KeyboardKey.onKeyPressed += OnLetterButtonClick;
        NewGame();
    }

    public void NewGame()
    {
        ClearBoard();
        SetRandomWord();

        enabled = true;
    }

    public void TryAgain()
    {
        ClearBoard();
        
        enabled = true;
    }

    /*private void LoadData()
    {
        TextAsset textFile = Resources.Load("official_wordle_all") as TextAsset;
        validWords = textFile.text.Split('\n');
        
        textFile = Resources.Load("official_wordle_common") as TextAsset;
        solutions = textFile.text.Split('\n');
    }*/

    private void SetRandomWord()
    {
        /*randomWord = solutions[Random.Range(0, solutions.Length)];
        randomWord = randomWord.ToLower().Trim();*/
        
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        char[] wordArray = new char[5];
        int index = 0;

        while (index < 5)
        {
            char randomChar = alphabet[Random.Range(0, alphabet.Length)];

            if (!wordArray.Contains(randomChar))
            {
                wordArray[index] = randomChar;
                index++;
            }
        }

        randomWord = new string(wordArray).ToLower();
        showRandomWordText.gameObject.SetActive(false);
        isWordVisible = false;
        showRandomWordText.text = randomWord;

    }

    private void Update()
    {
        currentRow = rows[rowIndex];
        
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            columnIndex = Mathf.Max(columnIndex - 1, 0);
            
            currentRow.tiles[columnIndex].SetLetter('\0');
            currentRow.tiles[columnIndex].SetState(emptyState);
            
            warningText.gameObject.SetActive(false);
        }
        else if (columnIndex >= rows[rowIndex].tiles.Length)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                SubmitRow(currentRow);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                warningText.gameObject.SetActive(true);
                warningText.text = "Word not complete";
            }
            
            for (int i = 0; i < SUPPORTED_KEYS.Length; i++)
            {
                if (Input.GetKeyDown(SUPPORTED_KEYS[i]))
                {
                    warningText.gameObject.SetActive(false);
                    rows[rowIndex].tiles[columnIndex].SetLetter((char)SUPPORTED_KEYS[i]);
                    currentRow.tiles[columnIndex].SetState(occupiedState);
                    columnIndex++;
                    break;
                }
            }
        }
    }

    public void OnLetterButtonClick(string letter)
    {
        if (columnIndex < rows[rowIndex].tiles.Length)
        {
            warningText.gameObject.SetActive(false);
            rows[rowIndex].tiles[columnIndex].SetLetter(letter[0]);
            rows[rowIndex].tiles[columnIndex].SetState(occupiedState);
            columnIndex++;
        }
    }
    
    public void OnBackspaceButtonClick()
    {
        if (columnIndex > 0)
        {
            columnIndex = Mathf.Max(columnIndex - 1, 0);
            rows[rowIndex].tiles[columnIndex].SetLetter('\0');
            rows[rowIndex].tiles[columnIndex].SetState(emptyState);
            warningText.gameObject.SetActive(false);
        }
    }
    
    public void OnEnterButtonClick()
    {
        if (columnIndex >= rows[rowIndex].tiles.Length) // Ensure word is complete
        {
            SubmitRow(currentRow);
        }
        else
        {
            warningText.gameObject.SetActive(true);
            warningText.text = "Word not complete";
        }
    }
    
    public void ToggleRandomWord()
    {
        isWordVisible = !isWordVisible;
        showRandomWordText.gameObject.SetActive(isWordVisible);
    }

    private void SubmitRow(Row row)
    {
        /*if (!IsValidWord(row.word))
        {
            warningText.gameObject.SetActive(true);
            warningText.text = "Invalid Word";
            return;
        }*/
        
        string remaining = randomWord;

        for (int i = 0; i < row.tiles.Length; i++)
        {
            Tile tile = row.tiles[i];

            if (tile.letter == randomWord[i])
            {
                tile.SetState(correctState);

                remaining = remaining.Remove(i, 1);
                remaining = remaining.Insert(i, " ");
            }
            else if (!randomWord.Contains(tile.letter))
            {
                tile.SetState(incorrectState);
            }
        }

        for (int i = 0; i < row.tiles.Length; i++)
        {
            Tile tile = row.tiles[i];

            if (tile.state != correctState && tile.state != incorrectState)
            {
                if (remaining.Contains(tile.letter))
                {
                    tile.SetState(wrongSpotState);

                    int index = remaining.IndexOf(tile.letter);
                    remaining = remaining.Remove(index, 1);
                    remaining = remaining.Insert(index, " ");
                }
                else
                {
                    tile.SetState(incorrectState);
                }
            }
        }

        if (HasWon(row))
        {
            enabled = false;
            
            winNewWordButton.gameObject.SetActive(true);
            showRandomWordText.gameObject.SetActive(true);
            isWordVisible = true;
            tryAgainButton.gameObject.SetActive(false);
            newWordButton.gameObject.SetActive(false);

            return;
        }

        rowIndex++;
        columnIndex = 0;

        if (rowIndex >= rows.Length)
        {
            enabled = false;
        }
    }

    private void ClearBoard()
    {
        for (int row = 0; row < rows.Length; row++)
        {
            for (int col = 0; col < rows[row].tiles.Length; col++)
            {
                rows[row].tiles[col].SetLetter('\0');
                rows[row].tiles[col].SetState(emptyState);
            }
        }

        rowIndex = 0;
        columnIndex = 0;
    }

    /*private bool IsValidWord(string word)
    {
        for (int i = 0; i < validWords.Length; i++)
        {
            if (validWords[i] == word)
            {
                return true;
            }
        }

        return false;
    }*/

    private bool HasWon(Row row)
    {
        for (int i = 0; i < row.tiles.Length; i++)
        {
            if (row.tiles[i].state != correctState)
            {
                return false;
            }
        }

        return true;
    }

    private void OnEnable()
    {
        tryAgainButton.gameObject.SetActive(false);
        newWordButton.gameObject.SetActive(false);
        winNewWordButton.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        tryAgainButton.gameObject.SetActive(true);
        newWordButton.gameObject.SetActive(true);
    }
}
