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
    private const float MinJumpForce = 2500f;
    private const float MaxJumpForce = 75000f;
    private const float MinGravity = 90f;
    private const float MaxGravity = 2700f;
    private const float MinMovementSpeed = 300f;
    private const float MaxMovementSpeed = 9000f;
    private const float MinMoveVelocity = 3f;
    private const float MaxMoveVelocity = 90f;

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
    private string[] GroundLayers = new string[] { "Ground", "HeldItem", "Obstacle" };
    private bool m_onGround = false;

    // Growing/Shrink
    private const float MinGrowSpeed = 10f;
    private const float MaxGrowSpeed = 40f;
    private const float MinScale = 1f;
    private const float MaxScale = 200f;
    public float Scale { get { return transform.localScale.x; } }
    public float ScaleRatio { get { return (Scale - MinScale) / (MaxScale - MinScale); } }
    private const float HeadRaycastDistance = 1f;

    // Items
    private const float MinHeldItemBoundsRatio = 0.02f;
    private const float MaxHeldItemBoundsRatio = 0.3f;
    private const float PickupItemDistance = 2.5f;
    private HeldItem m_heldItem;
    private bool m_grabbedThisFrame = false;

    // Use this for initialization
    void Start ()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!GameManager.Instance.GameActive)
        {
            return;
        }

        m_grabbedThisFrame = false;

        CheckGround();
        RotateCameraAndCapsule(PlayerCamera);
        HandleSize();
        HandleMovement();
        HandlePressingButtons();
        HandleDroppingItems();
        HandlePickingUpItems();
    }

    /// <summary>
    /// Check collisions with things.
    /// </summary>
    /// <param name="col"></param>
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("KillPlane"))
        {
            transform.position = Vector3.zero;
            transform.localScale = Vector3.one;
        }

        if (col.gameObject.layer == LayerMask.NameToLayer("MouseDoor") && m_heldItem != null && m_heldItem.name == "SilverKey")
        {
            col.gameObject.transform.parent.GetComponent<MouseDoor>().Open();
        }

        if (col.gameObject.layer == LayerMask.NameToLayer("ExitDoor") && m_heldItem != null && m_heldItem.name == "GoldKey")
        {
            GameManager.Instance.WinGame();
        }
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
        float growSpeed = Mathf.Lerp(MinGrowSpeed, MaxGrowSpeed, ScaleRatio);
        if(Input.GetButton("Grow") && transform.localScale.x < MaxScale && CanGrow())
        {
            transform.localScale += Vector3.one * growSpeed * Time.deltaTime;
        }
        else if (Input.GetButton("Shrink") && transform.localScale.x > MinScale)
        {
            transform.localScale -= Vector3.one * growSpeed * Time.deltaTime;
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
        bool tooBig = false;
        RaycastHit hitInfo;
        if(Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.forward, out hitInfo, PickupItemDistance * Scale, LayerMask.GetMask("HeldItem")))
        {
            HeldItem lookedAtItem = hitInfo.transform.GetComponent<HeldItem>();
            if(lookedAtItem == null)
            {
                lookedAtItem = hitInfo.transform.GetComponentInParent<HeldItem>();
            }
            
            if(CanSelectItemBasedOnScale(lookedAtItem, out tooBig))
            {
                selectedItem = lookedAtItem;
                lookedAtItem.SetSelectionVisual(true);
            }
            else
            {
                lookedAtItem.SetSelectionVisual(false);
            }
        }

        // If a valid item is looked at, you can pick it up with a button.
        if(Input.GetButtonDown("Grab") && !m_grabbedThisFrame)
        {
            m_grabbedThisFrame = true;

            if (selectedItem == null)
            {
                GameManager.Instance.GameCanvas.PlayGrabFailAnimation(tooBig);
            }
            else
            {
                PickUpItem(selectedItem);
            }
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

        if (Input.GetButtonDown("Grab") && !m_grabbedThisFrame)
        {
            m_grabbedThisFrame = true;
            DropItem();
        }
    }

    /// <summary>
    /// Handle selecting buttons.
    /// </summary>
    private void HandlePressingButtons()
    {
        // Look at a button
        RaycastHit hitInfo;
        if (Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.forward, out hitInfo, PickupItemDistance * Scale, LayerMask.GetMask("GrowButton", "HeldItem")))
        {
            GrowButton lookedAtItem = hitInfo.transform.GetComponent<GrowButton>();

            if (lookedAtItem != null)
            {
                bool tooBig = false;
                if(CanSelectItemBasedOnScale(lookedAtItem, out tooBig))
                {
                    // If a valid item is looked at, you can pick it up with a button.
                    if (Input.GetButtonDown("Grab") && !m_grabbedThisFrame)
                    {
                        m_grabbedThisFrame = true;
                        PressButton(lookedAtItem);
                    }

                    lookedAtItem.SetSelectionVisual(true);
                }
                else
                {
                    // If an invalid item is looked at, do the animation
                    if (Input.GetButtonDown("Grab") && !m_grabbedThisFrame)
                    {
                        m_grabbedThisFrame = true;
                        GameManager.Instance.GameCanvas.PlayGrabFailAnimation(tooBig);
                    }

                    lookedAtItem.SetSelectionVisual(false);
                }

                
            }
            else if(lookedAtItem != null)
            {
                
            }
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

    private void PressButton(SelectableItem item)
    {
        item.GetComponent<GrowButton>().Press();
    }

    /// <summary>
    /// Can I pick it up.
    /// </summary>
    private bool CanSelectItemBasedOnScale(SelectableItem item, out bool tooBig)
    {
        float itemBounds = item.GetMaxBounds();

        Vector3 boundVector = Visual.GetComponent<Renderer>().bounds.size * Scale;
        float selfBounds = Mathf.Max(Mathf.Max(boundVector.x, boundVector.y), boundVector.z);

        float boundsRatio = itemBounds / selfBounds;
        Debug.Log(string.Format("Bounds Ratio: {0} Self Bounds: {1} Item Bounds: {2}", boundsRatio, selfBounds, itemBounds));
        tooBig = boundsRatio < MinHeldItemBoundsRatio;
        return boundsRatio <= MaxHeldItemBoundsRatio && boundsRatio >= MinHeldItemBoundsRatio;
    }

    /// <summary>
    /// Can grow
    /// </summary>
    private bool CanGrow()
    {
        if(Physics.Raycast(PlayerCamera.transform.position, Vector3.up, HeadRaycastDistance))
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Room can force the player to scale down.
    /// </summary>
    /// <param name="newRatio"></param>
    public void ForceScaleDown(float newRatio)
    {
        float newScale = Mathf.Lerp(MinScale, MaxScale, newRatio);
        transform.localScale = Vector3.one * newScale;
    }
}
