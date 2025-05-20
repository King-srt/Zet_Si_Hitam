using UnityEngine;

public class Character2DClickMove : MonoBehaviour
{
    private static Character2DClickMove selectedCharacter;
    private SpriteRenderer spriteRenderer;
    public Color highlightColor = Color.yellow;
    private Color originalColor;

    public float moveSpeed = 5f;
    private Vector3 targetPosition;
    private bool isMoving = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        targetPosition = transform.position;
    }

    void OnMouseDown()
    {
        if (selectedCharacter != null && selectedCharacter != this)
        {
            selectedCharacter.spriteRenderer.color = selectedCharacter.originalColor;
        }

        selectedCharacter = this;
        spriteRenderer.color = highlightColor;
    }

    void Update()
    {
        if (selectedCharacter == this)
        {
            if (Input.GetMouseButtonDown(1)) // klik kanan
            {
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPos.z = 0f;
                targetPosition = mouseWorldPos;
                isMoving = true;
            }

            if (isMoving)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

                if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
                {
                    isMoving = false;
                }
            }
        }
    }

    void OnMouseEnter()
    {
        if (selectedCharacter != this)
        {
            spriteRenderer.color = highlightColor;
        }
    }

    void OnMouseExit()
    {
        if (selectedCharacter != this)
        {
            spriteRenderer.color = originalColor;
        }
    }
}