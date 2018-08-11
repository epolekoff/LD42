using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform GroundCollisionCheckPoint;
    public Camera PlayerCamera;
    public Transform HeldItemBone;
    public Renderer Visual;

    // Movement
    private const float MinJumpForce = 2000f;
    private const float MaxJumpForce = 60000f;
    private const float MinGravity = 100f;
    private const float MaxGravity = 3000f;
    private const float MinMovementSpeed = 400f;
    private const float MaxMovementSpeed = 12000f;
    private const float MinMoveVelocity = 4f;
    private const float MaxMoveVelocity = 120f;

    // Camera
    private float sensitivityX = 3F;
    private float sensitivityY = 3F;
    private float webGL_sensitivityX = 1f;
    private float webGL_sensitivityY = 1f;
    private float minimumX = -360F;
    private float maximumX = 360F;
    private float minimumY = -60F;
    private float maximumY = 60F;
    private float rotationY = 0F;

    // Jumping
    private string[] GroundLayers = new string[] { "Ground" };
    private bool m_onGround = false;

    // Growing/Shrink
    private const float GrowSpeed = 10f;
    private const float ShrinkSpeed = 10f;
    private const float MinScale = 1f;
    private const float MaxScale = 100f;
    public float Scale { get { return transform.localScale.x; } }
    public float ScaleRatio { get { return (Scale - MinScale) / (MaxScale - MinScale); } }

    // Items
    private const float MinHeldItemBoundsRatio = 0.1f;
    private const float MaxHeldItemBoundsRatio = 0.3f;
    private const float PickupItemDistance = 2f;
    private HeldItem m_heldItem;

    // Use this for initialization
    void Start ()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        CheckGround();
        RotateCameraAndCapsule(PlayerCamera);
        HandleSize();
        HandleMovement();
        HandleDroppingItems();
        HandlePickingUpItems();
    }

    /// <summary>
    /// Can I jump? Store it off.
    /// </summary>
    private void CheckGround()
    {
        m_onGround = Physics.Raycast(GroundCollisionCheckPoint.position, Vector3.down, 0.2f * Scale, LayerMask.GetMask(GroundLayers));
    }

    /// <summary>
    /// Move
    /// </summary>
    private void HandleMovement()
    {
        // Gravity
        float sizeScaledGravity = Mathf.Lerp(MinGravity, MaxGravity, ScaleRatio);
        GetComponent<Rigidbody>().AddForce(-Vector3.up * sizeScaledGravity);

        // Jump
        if(m_onGround && Input.GetButtonDown("Jump"))
        {
            float sizeScaledJumpForce = Mathf.Lerp(MinJumpForce, MaxJumpForce, ScaleRatio);
            GetComponent<Rigidbody>().AddForce(Vector3.up * sizeScaledJumpForce);
        }

        // Movement
        float SizeScaledMovementSpeed = Mathf.Lerp(MinMovementSpeed, MaxMovementSpeed, ScaleRatio);
        GetComponent<Rigidbody>().AddForce(transform.forward * Input.GetAxis("Vertical") * SizeScaledMovementSpeed);
        GetComponent<Rigidbody>().AddForce(transform.right * Input.GetAxis("Horizontal") * SizeScaledMovementSpeed);

        // Clamp the Movement Velocity.
        Vector3 clampedVelocity = GetComponent<Rigidbody>().velocity;
        float ClampVelocityBound = Mathf.Lerp(MinMoveVelocity, MaxMoveVelocity, ScaleRatio);
        clampedVelocity.x = Mathf.Clamp(clampedVelocity.x, -ClampVelocityBound, ClampVelocityBound);
        clampedVelocity.z = Mathf.Clamp(clampedVelocity.z, -ClampVelocityBound, ClampVelocityBound);

        GetComponent<Rigidbody>().velocity = clampedVelocity;
    }

    /// <summary>
    /// Rotate the camera using the mouse.
    /// </summary>
    private void RotateCameraAndCapsule(Camera camera)
    {
        float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

        rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
        rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

        camera.transform.localEulerAngles = new Vector3(-rotationY, 0, 0);

        // Rotate the player to match.
        transform.localEulerAngles = new Vector3(0, rotationX, 0);
    }

    /// <summary>
    /// Grow and shrink
    /// </summary>
    private void HandleSize()
    {
        if(Input.GetButton("Grow") && transform.localScale.x < MaxScale)
        {
            transform.localScale += Vector3.one * GrowSpeed * Time.deltaTime;
        }
        else if (Input.GetButton("Shrink") && transform.localScale.x > MinScale)
        {
            transform.localScale -= Vector3.one * ShrinkSpeed * Time.deltaTime;
        }
    }

    /// <summary>
    /// Handle selecting and picking up items.
    /// </summary>
    private void HandlePickingUpItems()
    {
        if(m_heldItem != null)
        {
            return;
        }

        // Look at an item.
        HeldItem selectedItem = null;
        RaycastHit hitInfo;
        if(Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.forward, out hitInfo, PickupItemDistance * Scale, LayerMask.GetMask("HeldItem")))
        {
            HeldItem lookedAtItem = hitInfo.transform.GetComponent<HeldItem>();

            if(CanPickUpItemBasedOnScale(lookedAtItem))
            {
                selectedItem = lookedAtItem;
                lookedAtItem.SetSelectionVisual(true);
            }
            else
            {
                lookedAtItem.SetSelectionVisual(false);
            }
        }
        if(selectedItem == null)
        {
            return;
        }

        // If a valid item is looked at, you can pick it up with a button.
        if(Input.GetButtonDown("Grab"))
        {
            PickUpItem(selectedItem);
        }
    }

    /// <summary>
    /// Check if the player drops their item.
    /// </summary>
    private void HandleDroppingItems()
    {
        if (m_heldItem == null)
        {
            return;
        }

        if (Input.GetButtonDown("Grab"))
        {
            DropItem();
        }
    }

    /// <summary>
    /// Pick up an item.
    /// </summary>
    /// <param name="item"></param>
    private void PickUpItem(HeldItem item)
    {
        m_heldItem = item;

        item.PickUp();

        // Set the item's parent.
        item.transform.parent = HeldItemBone.transform;
        item.transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// Drop the item.
    /// </summary>
    private void DropItem()
    {
        // Un-parent the item.
        m_heldItem.transform.parent = null;

        m_heldItem.Drop();

        // Clear the held item.
        m_heldItem = null;
    }

    /// <summary>
    /// Can I pick it up.
    /// </summary>
    private bool CanPickUpItemBasedOnScale(HeldItem item)
    {
        float itemBounds = item.GetMaxBounds();

        Vector3 boundVector = Visual.GetComponent<Renderer>().bounds.size * Scale;
        float selfBounds = Mathf.Max(Mathf.Max(boundVector.x, boundVector.y), boundVector.z);

        float boundsRatio = itemBounds / selfBounds;
        Debug.Log(string.Format("Bounds Ratio: {0} Self Bounds: {1} Item Bounds: {2}", boundsRatio, selfBounds, itemBounds));
        return boundsRatio <= MaxHeldItemBoundsRatio && boundsRatio >= MinHeldItemBoundsRatio;
    }
}
