using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saigai.Studios;

public class BusMg : MonoBehaviour {

    public Piece selected_piece;

    void Start() {
        // Load the blocks into the rust-end
        var foundPieceObjects = GameObject.FindObjectsOfType<Piece>();

        for (int i = 0; i < foundPieceObjects.Length; i++) {
            // add a new new piece
            uint id = Interop.add_piece();
            // pieces.Add(foundPieceObjects[i]);
            foundPieceObjects[i].SetId(id);
            // add each cell
            for (int j = 0; j < foundPieceObjects[i].cells.Count; j++) {
                var cell = foundPieceObjects[i].cells[j];
                Interop.add_coordinate(id, cell);
            }
        }
        Debug.Log("Pieces initialized.");
        // bool[,] grid = new bool[8, 8];
        // load random number of people pieces to fit in the grid

        // game_loop(grid);
    }

    void game_loop(bool[,] grid) {

    }

    public void PlaceOnBoard(uint id) {
        Debug.Log("Placing on board.");
    }
}

