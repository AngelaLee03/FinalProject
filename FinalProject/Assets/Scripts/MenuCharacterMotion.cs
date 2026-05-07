using UnityEngine;

public class MenuCharacterMotion : MonoBehaviour
{
    public enum MoveOrientation
    {
        LeftRight,
        ForwardBack,
        UpDown
    }

    [Header("Movement Orientation")]
    public MoveOrientation orientation = MoveOrientation.LeftRight;

    [Header("Slide Settings")]
    public float slideDistance = 1.5f;
    public float slideSpeed = 0.6f;

    [Header("Bounce Settings")]
    public float bounceHeight = 0.2f;
    public float bounceSpeed = 3.0f;

    [Header("Y Rotation")]
    public float forwardYRotation = 0f;
    public float backwardYRotation = 180f;
    public float rotationSmooth = 6f;

    private Vector3 startPosition;
    private Quaternion targetRotation;
    private float lastSlideValue;

    void Start()
    {
        startPosition = transform.position;
        targetRotation = transform.rotation;
    }

    void Update()
    {
        float slide = Mathf.Sin(Time.time * slideSpeed) * slideDistance;
        float bounce = Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;

        Vector3 slideDirection = GetSlideDirection();
        Vector3 bounceDirection = Vector3.up;

        transform.position = startPosition + (slideDirection * slide) + (bounceDirection * bounce);

        float direction = slide - lastSlideValue;

        if (direction > 0.001f)
        {
            targetRotation = Quaternion.Euler(0f, forwardYRotation, 0f);
        }
        else if (direction < -0.001f)
        {
            targetRotation = Quaternion.Euler(0f, backwardYRotation, 0f);
        }

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * rotationSmooth
        );

        lastSlideValue = slide;
    }

    Vector3 GetSlideDirection()
    {
        switch (orientation)
        {
            case MoveOrientation.LeftRight:
                return Vector3.right;

            case MoveOrientation.ForwardBack:
                return Vector3.forward;

            case MoveOrientation.UpDown:
                return Vector3.up;

            default:
                return Vector3.right;
        }
    }
}