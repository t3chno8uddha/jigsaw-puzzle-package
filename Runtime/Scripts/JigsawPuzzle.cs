using UnityEngine;

[CreateAssetMenu(fileName = "Puzzle", menuName = "Jigsaw/New Jigsaw Puzzle")]
public class JigsawPuzzle : ScriptableObject
{
    public Texture2D puzzle;    // The puzzle texture image.

    public Vector2 dimensions = new Vector2(4, 4);  // The amount of pieces in our puzzle.
    public Sprite[] pieces; // All the individual pieces.

    public float pieceSize = 1; // The physical size of each piece. Needed to ensure correctly sized gaps.
    public float slotOffset = 0;
}
