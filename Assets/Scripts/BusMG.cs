using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saigai.Studios;

public class BusMg : MonoBehaviour {

    // Camera for pixel mapping
    public Camera cam;

    public Piece selected_piece;

    void Start() {
        // Initialize game grid
        Interop.init_game(0);

        // Load the blocks into the rust-end
        var foundPieceObjects = GameObject.FindObjectsOfType<Piece>();

        for (int i = 0; i < foundPieceObjects.Length; i++) {
            // add a new new piece
            uint id = Interop.add_piece();
            Debug.Log(id);
            // pieces.Add(foundPieceObjects[i]);
            foundPieceObjects[i].SetId(id);
            // add each cell
            for (int j = 0; j < foundPieceObjects[i].cells.Count; j++) {
                Debug.Log("Cell");
                Debug.Log(j);
                var cell = foundPieceObjects[i].cells[j];
                Interop.add_coordinate(id, cell);
            }
        }
        Debug.Log("Pieces initialized.");
        // bool[,] grid = new bool[8, 8];
        // load random number of people pieces to fit in the grid

        // game_loop(grid);

        Vector3[] corners = new Vector3[4];
        GetComponent<RectTransform>().GetWorldCorners(corners);
        for (int i = 0; i < 4; ++i) {
            corners[i] = cam.WorldToScreenPoint(corners[i]);
        }

        // Debug.Log(corners[0].x);
        // Debug.Log(corners[0].y);
        // Debug.Log((corners[3].x - corners[0].x));
        // Debug.Log((corners[2].y - corners[0].y));

        Interop.set_grid_space(corners[0].x, corners[0].y, (corners[3].x - corners[0].x), (corners[2].y - corners[0].y));
    }

    void game_loop(bool[,] grid) {

    }

    public void PlaceOnBoard(uint id) {
        Debug.Log("Placing on board.");
    }
}

