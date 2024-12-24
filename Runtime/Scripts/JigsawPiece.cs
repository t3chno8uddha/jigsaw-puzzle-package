using UnityEngine;

public class JigsawPiece : MonoBehaviour
{
    public Vector2 coordinates;     // Piece coordinates on the grid.

    bool isHeld;        // Flag to see if the piece is held.

    SpriteRenderer pieceRenderer;
    Collider2D pieceCollider;

    Transform target;           // The target slot for the piece.

    float threshold;            // The threshold amount for snapping the piece.

    GameObject shadow;          // Piece shadow gameObject.

    Vector2 shadowOffset;       // Shadow position offset.
    float shadowScale = 1.125f; // Shadow size.
    float shadowAlpha;          // Shadow opacity.

    Animator pieceAnimator;                         // Piece animator.
    RuntimeAnimatorController animatorController;   // Piece animator controller.

    void Start()
    {
        // Get components.
        pieceRenderer = GetComponent<SpriteRenderer>();

        pieceAnimator = gameObject.AddComponent<Animator>();
        pieceAnimator.runtimeAnimatorController = new AnimatorOverrideController(animatorController);

        // Create colliders.
        pieceCollider = gameObject.AddComponent<PolygonCollider2D>();
        pieceCollider.isTrigger = true;


        // Create a shadow.
        shadow = new GameObject("Piece Shadow");
        shadow.transform.SetParent(transform);
        SpriteRenderer shadowRenderer = shadow.AddComponent<SpriteRenderer>();

        shadowRenderer.sprite = pieceRenderer.sprite;
        shadowRenderer.color = new Color(0, 0, 0, shadowAlpha);
        shadowRenderer.sortingOrder = -1;

        shadow.transform.localScale = new Vector3 (shadowScale, shadowScale, shadowScale);
        shadow.transform.localPosition = new Vector3 (shadowOffset.x, shadowOffset.y, 0);

        // Initialize other properties if needed.
    }

    void Update()
    {
        // Check if a touch event is happening.
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // Get the first touch input.

            Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
            touchPos.z = 0; // Set z to 0 to align with 2D.

            if (touch.phase == TouchPhase.Began)
            {
                // Raycast to check if the touch is on this piece.
                RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);
                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    isHeld = true;  // Begin holding the piece.
                    pieceRenderer.sortingOrder = 1;    // Set sorting order above the rest of the game.
                    if (pieceAnimator.GetBool("Held") == false) pieceAnimator.SetBool("Held", true);
                }
            }
            else if (touch.phase == TouchPhase.Moved && isHeld)
            {
                // Update the position while holding.
                if (transform.position != touchPos)
                {
                    transform.position = touchPos;
                }
            }
            else if (touch.phase == TouchPhase.Ended && isHeld)
            {
                if (pieceAnimator.GetBool("Held") == true) pieceAnimator.SetBool("Held", false);

                // Handle release logic.
                isHeld = false; // Stop holding the piece.
                pieceRenderer.sortingOrder = 0;    // Reset sorting order.

                // Check if the piece is close enough to snap to the target.
                if (Vector3.Distance(transform.position, target.position) < threshold)
                {
                    FinishMoving(); // Snap the piece into place.
                }
            }
        }
    }

    void FinishMoving()
    {
        transform.position = target.position;   // Move the piece into place.
        pieceRenderer.sortingOrder = -2;   // Set sorting order below the rest of the game.

        Destroy(shadow);            // Remove the shadow.
        Destroy(pieceCollider);     // Remove the collider to avoid further interaction.
        
        // Destroy(pieceAnimator);     // Remove the animator to avoid anything else.
        
        Destroy(this);              // Remove this script as it’s no longer needed.
    }

    // Assign a target slot to the piece.
    public void SetTarget (Transform pieceTarget) { target = pieceTarget; }

    // Assign a snapping threshold to the piece.
    public void SetThreshold (float pieceThreshold) { threshold = pieceThreshold; }

    // Assign the shadow offset to the piece.
    public void SetShadow (Vector2 offset, float alpha, float scale) { shadowOffset = offset; shadowAlpha = alpha; shadowScale = scale; }

    // Assign the animator to the piece.
    public void SetAnimator(RuntimeAnimatorController pieceAnimatorController) { animatorController = new AnimatorOverrideController(pieceAnimatorController); }
}