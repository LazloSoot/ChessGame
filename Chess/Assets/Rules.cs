using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Assets;
using ChessGame = Chess.BL.Chess;

public class Rules : MonoBehaviour {
    
    private const int coordOffset = 7;
    private readonly DragAndDrop dnd;
    private List<string> markedSpaces;
    private static ChessGame chess = new ChessGame();

    public Rules()
    {
        markedSpaces = new List<string>();
        dnd = new DragAndDrop();
        dnd.PickedUp += Dnd_PickedUp;
        dnd.DropedDown += Dnd_DropedDown;
    }


    public void Start()
    {
        ShowFigues();
	}
	
	void Update()
    {
        if(dnd.Action())
        {
            var from = GetSpace(dnd.FromPos);
            var to = GetSpace(dnd.ToPos);
            
           string figure = chess.GetFigureAt((int)((dnd.FromPos.x + coordOffset) / 2.0f), (int)((dnd.FromPos.y + coordOffset) / 2.0f)).ToString();
           string move = figure + from + to;
           chess = chess.Move(move);
           ShowFigues();
        }
	}

    string GetSpace(Vector2 position)
    {
        var x = Convert.ToInt32((position.x + coordOffset) / 2.0f);
        var y = Convert.ToInt32((position.y + coordOffset) / 2.0f);
        return ((char)('a' + x)).ToString() + (y + 1);
    }

    void ShowFigues()
    {
        int squareIndex = 0;
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                string figure = chess.GetFigureAt(x, y).ToString();
                if (figure == ".")
                    continue;

                PlaceFigure(squareIndex++ ,figure, x, y);
            }
        }
        
        for (;squareIndex < 32; squareIndex++)
        {
            PlaceFigure(squareIndex, "q", 9, 9);
        }
    }

    void PlaceFigure(int squareIndex, string figureName, int posX, int posY)
    {
        var figure = GameObject.Find("f" + squareIndex);
        var figureObj = GameObject.Find(figureName);
        var space = GameObject.Find("" + posY + posX);

        var figureObjSprite = figureObj.GetComponent<SpriteRenderer>();
        var figureSprite = figure.GetComponent<SpriteRenderer>();
        figureSprite.sprite = figureObjSprite.sprite;

        figure.transform.position = space.transform.position;
    }

    void MarkSpace(int x, int y, bool isMarked = false)
    {
        var targetSpace = GameObject.Find("" + y + x);
        string color = (((x + y) % 2 == 0) ? "bs" : "ws") + ((isMarked) ? "" : "Marked");
        var space = GameObject.Find(color);

        targetSpace.GetComponent<SpriteRenderer>().sprite = space.GetComponent<SpriteRenderer>().sprite;
        
    }

    void MarkSpace(string spaceName, bool isMarked = false)
    {
        if (spaceName[0] >= 'a' &&
                spaceName[0] <= 'h' &&
                spaceName[1] >= '1' &&
                spaceName[1] <= '8')
        {
            var x = spaceName[0] - 'a';
            var y = spaceName[1] - '1';
            MarkSpace(x, y, isMarked);
        }
        else
        {
            return;
        }
    }

    private void Dnd_PickedUp(object sender, EventArgs e)
    {
        
        markedSpaces = chess.GetAllValidMovesForFigureAt(Convert.ToInt32((dnd.FromPos.x + coordOffset) / 2.0f), Convert.ToInt32((dnd.FromPos.y + coordOffset) / 2.0f));
        foreach (var s in markedSpaces)
        {
            MarkSpace(s);
        }
    }


    private void Dnd_DropedDown(object sender, EventArgs e)
    {
        foreach (var s in markedSpaces)
        {
            MarkSpace(s, true);
        }

        markedSpaces.Clear();
    }


}
