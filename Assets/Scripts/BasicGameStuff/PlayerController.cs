using UnityEngine;

namespace BasicGameStuff
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance { get; private set; }

        public static bool MouseCaptured
        {
            get => !Cursor.visible;
            set
            {
                Cursor.visible = !value;
                Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
            }
        }

        [Header("Assigns")]
        public GameObject camPivot;
        public GameObject cameraObject;
        public GameObject grabPosObject;

        [Header("Player Settings")]
        public float speed = 5.0f;
        public float jumpVelocity = 4.5f;
        public float grabSpeed = 1f;
        public float sprintMultiplier = 1.25f;
        public LayerMask groundMask;
        public LayerMask pickupMask;

        [Header("Camera Settings")]
        public float xSensitivity = 0.2f;
        public float ySensitivity = 0.2f;

        [Header("Feature Set")]
        public bool allowJumping = true;
        public bool allowLookUp = true;
        public bool enableGrabbing = true;

        private Rigidbody grabbedBody;
        private int grabbedBodyLayer;
        private Rigidbody rb;
        private Vector3 velocity;

        private bool IsGrabbing => grabbedBody != null;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            Instance = this;
            MouseCaptured = true;
        }

        private void Update()
        {
            velocity = rb.linearVelocity;
            rb.angularVelocity = Vector3.zero;

            if (Input.GetKeyUp(KeyCode.Escape))
            {
                MouseCaptured = !MouseCaptured;
                PauseMenu.Instance.gameObject.SetActive(!MouseCaptured);
                Time.timeScale = MouseCaptured ? 1f : 0f;
            }

            if (MouseCaptured)
            {
                HandleInput();
                RotateCamera();
            }

            HandleGrabbing();
            ApplyVelocity();
        }

        private void Jump()
        {
            if (Input.GetKey(KeyCode.Space) && IsOnFloor() && allowJumping)
            {
                velocity.y = jumpVelocity;
            }
        }

        private void HandleInput()
        {
            Vector2 inputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            Vector3 direction = (transform.rotation * new Vector3(inputDir.x, 0, inputDir.y)).normalized;
            Move(direction);
            Jump();

            var lookingAtItem = Physics.Raycast(cameraObject.transform.position, cameraObject.transform.forward, out var hit, 5f, pickupMask);

            Crosshair.CrosshairColor = lookingAtItem ? Color.red : Color.white;

            if (Input.GetMouseButtonDown(0))
            {
                if (IsGrabbing)
                {
                    grabbedBody = null;
                    return;
                }

                else if (lookingAtItem && !IsGrabbing)
                {
                    var collider = hit.collider.gameObject;

                    if (!collider.GetComponent<PickUpItem>() || !collider.TryGetComponent(out grabbedBody))
                        return;

                    grabbedBodyLayer = grabbedBody.gameObject.layer;
                }
            }
        }

        private float speedPercentage = 1;

        private void Move(Vector3 direction)
        {
            float sprintSpeed = speed * (Input.GetKey(KeyCode.LeftShift) ? sprintMultiplier : 1f);
            float crouchSpeed = sprintSpeed * (Input.GetKey(KeyCode.LeftControl) ? 0.5f : 1f);
            float moveSpeed = crouchSpeed * speedPercentage;

            Vector3 forward = camPivot.transform.forward;
            Vector3 right = camPivot.transform.right;

            forward.y = 0;
            right.y = 0;

            Vector3 moveDir = (forward * direction.z + right * direction.x).normalized;

            velocity.x = Mathf.MoveTowards(velocity.x, moveDir.x * moveSpeed, moveSpeed * 10f * Time.deltaTime);
            velocity.z = Mathf.MoveTowards(velocity.z, moveDir.z * moveSpeed, moveSpeed * 10f * Time.deltaTime);
        }


        private void ApplyVelocity()
        {
            rb.linearVelocity = velocity;
        }

        private void RotateCamera()
        {
            Vector3 pivotRot = camPivot.transform.rotation.eulerAngles;
            Vector3 camRot = cameraObject.transform.localRotation.eulerAngles;

            float adjustedXSensitivity = xSensitivity * 0.1f;
            float adjustedYSensitivity = ySensitivity * 0.1f;

            pivotRot.y += Input.GetAxisRaw("Mouse X") * adjustedXSensitivity;

            float targetXRotation = camRot.x - Input.GetAxisRaw("Mouse Y") * adjustedYSensitivity;
            camRot.x = allowLookUp ? Mathf.Clamp((targetXRotation > 180 ? targetXRotation - 360 : targetXRotation), -90, 90) : 0;

            camPivot.transform.rotation = Quaternion.Euler(pivotRot);
            cameraObject.transform.localRotation = Quaternion.Euler(camRot);
        }


        private void HandleGrabbing()
        {
            if (!IsGrabbing)
            {
                speedPercentage = 1;
                return;
            }

            Vector3 distance = grabPosObject.transform.position - grabbedBody.position;
            grabbedBody.linearVelocity = distance * grabSpeed;
            grabbedBody.angularVelocity = Vector3.zero;

            if (grabbedBody.GetComponent<TrashItem>() != null)
            {
                speedPercentage = grabbedBody.GetComponent<TrashItem>().moveSpeedPercentage / 100;
            }

            speedPercentage = Mathf.Clamp(speedPercentage, 0, 1);

            if (grabbedBody.GetComponent<PickUpItem>().lookAtPlayer)
            {
                grabbedBody.transform.LookAt(cameraObject.transform.position);
                grabbedBody.transform.rotation = Quaternion.Euler(0, grabbedBody.transform.rotation.eulerAngles.y, 0);
            }
        }

        private bool IsOnFloor()
        {
            return Physics.Raycast(transform.position, Vector3.down, 1.1f, groundMask);
        }
    }
}
