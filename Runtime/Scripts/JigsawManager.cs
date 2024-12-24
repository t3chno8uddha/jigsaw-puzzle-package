using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JigsawManager : MonoBehaviour
{
    Dictionary<Vector2, JigsawPiece> jigsawPieces = new Dictionary<Vector2, JigsawPiece>(); // A Vector2 dictionary containing all the pieces.
    [SerializeField] List<JigsawPiece> allPieces = new List<JigsawPiece>();   // A list containing all the pieces.

    Transform pieces, slots;    // Parents to group objects in.

    [SerializeField] Transform boundsParent;    // Bounds for the random piece placements.
    Vector3 boundsMin, boundsMax;

    [SerializeField] float positionThreshold = 1;   // The threshold amount for snapping the piece.

    [SerializeField] JigsawPuzzle puzzle;   // The puzzle asset.

    [SerializeField] Vector2 shadowOffset;  // Shadow position offset.
    [SerializeField] float shadowScale = 1.125f;    // Shadow size.
    [Range (0,1)] [SerializeField] float shadowAlpha;   // Shadow opacity.

    [SerializeField] RuntimeAnimatorController pieceAnimator;

    JigsawEntryPoint _entryPoint;
    [SerializeField] Button homeButton;

    private void Awake()
    {
        homeButton.onClick.AddListener(FinishOnButton);
    }

    void FinishOnButton()
    {
        _entryPoint.InvokeGameFinished();
    }

    void Start()
    {
        // Create a parent for all the existing pieces.
        pieces = new GameObject("Puzzle Pieces").transform;
        pieces.SetParent(transform);
        pieces.localPosition = Vector3.zero;

        // Create a parent for all the empty slots.
        slots = new GameObject("Puzzle Slots").transform;
        slots.SetParent(transform);
        slots.localPosition = Vector3.zero + new Vector3(puzzle.slotOffset, -puzzle.slotOffset, 0);

        boundsMin = boundsParent.GetChild(0).position;
        boundsMax = boundsParent.GetChild(1).position;

        // Go over all the pieces on the Y axis
        for (int y = 0; y < puzzle.dimensions.y; y++)
        {
            // Same for X axis
            for (int x = 0; x < puzzle.dimensions.x; x++)
            {
                int pieceNumber = x + y * (int)puzzle.dimensions.x;    // Multiply the y by four to get the amoun we've already gone over previously.

                Vector2 pieceCoordinates = new Vector2(x, y);
                string pieceId = pieceNumber + " - (" + (int)pieceCoordinates.x + ", " + (int)pieceCoordinates.y + ")";

                GameObject pieceObject = new GameObject("Piece " + pieceId);
                //pieceObject.AddComponent<SoundPlayer>();
                pieceObject.AddComponent<SpriteRenderer>().sprite = puzzle.pieces[pieceNumber];

                JigsawPiece piece = pieceObject.gameObject.AddComponent<JigsawPiece>();

                piece.coordinates = pieceCoordinates;

                jigsawPieces.Add(pieceCoordinates, piece);
                allPieces.Add(piece);

                //  //  //  //  //  //  //  //  //  //  //  //  //  

                Transform pieceSlot = new GameObject("Slot " + pieceId).transform;
                pieceSlot.SetParent(slots);
                
                pieceSlot.localPosition = new Vector3(x * puzzle.pieceSize, -y * puzzle.pieceSize, 0);

                piece.SetTarget(pieceSlot);
                piece.SetThreshold(positionThreshold);
                piece.SetShadow(shadowOffset, shadowAlpha, shadowScale);
                piece.SetAnimator(pieceAnimator);

                //pieceSlot.gameObject.AddComponent<JigsawSlot>();  // Commented out because I decided against creating this.
            }
        }

        foreach (JigsawPiece piece in allPieces)
        {
            // Make sure every piece is in the appropriate parent.
            piece.transform.SetParent(pieces);

            float randomX = Random.Range(boundsMin.x, boundsMax.x);
            float randomY = Random.Range(boundsMin.y, boundsMax.y);

            piece.transform.position = new Vector3(randomX, randomY, 0);
        }
    }

    public void SetEntryPoint(JigsawEntryPoint entryPoint)
    {
        _entryPoint = entryPoint;
    }

    void SetFinishForPackage()
    {
        StartCoroutine(FinishAfterFireworks());
    }

    IEnumerator FinishAfterFireworks()
    {
        yield return new WaitForSecondsRealtime(5f);
        _entryPoint.InvokeGameFinished();
    }
}