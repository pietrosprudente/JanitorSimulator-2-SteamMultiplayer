using System;
using System.Threading.Tasks;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Steamworks;
using Steamworks.Data;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using Color = UnityEngine.Color;

namespace BasicGameStuff
{
    public class PlayerController : NetworkBehaviour
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
        public Transform tpsPos;
        public Camera myCamera;
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

        public readonly SyncVar<string> nickname = new();
        public readonly SyncVar<SteamId> steamID = new();
        public TMP_Text nicknameTMP;
        public Renderer face;
        public GameObject broomObject;

        private bool IsGrabbing => grabbedBody != null;

        public override void OnStartClient()
        {
            rb = GetComponent<Rigidbody>();
            if (!IsOwner) {
                Destroy(myCamera.gameObject);
                return;
            }
            Instance = this;
            MouseCaptured = true;
            SetupSteamPlayer(SteamClient.IsLoggedOn ? SteamClient.Name : "Player " + Random.Range(1111, 9999), SteamClient.SteamId);
        }

        private GameObject spawnedBroom;

        [ServerRpc]
        private void SetupSteamPlayer(string newName, SteamId newID)
        {
            nickname.Value = newName;
            steamID.Value = newID;
            var broom = Instantiate(broomObject, transform.position, Quaternion.identity);
            Spawn(broom, Owner);
            spawnedBroom = broom.transform.GetChild(0).gameObject;
        }

        [ServerRpc(RequireOwnership = false, RunLocally = true)]
        private void RetrieveStupidBroom()
        {
            spawnedBroom.GetComponent<PickUpItem>().NetworkObject.GiveOwnership(this.Owner);
            spawnedBroom.TryGetComponent(out Rigidbody rb);
            rb.isKinematic = true;
            rb.transform.position = transform.position;
            rb.transform.rotation = Quaternion.identity;
            rb.isKinematic = false;
            rb.position = this.rb.position;
            rb.rotation = Quaternion.identity;
        }

        public readonly SyncVar<bool> canPush = new(true);

        [ServerRpc]
        public void Push()
        {
            if (!canPush.Value) return;
            canPush.Value = false;
            Invoke(nameof(ResetCanPush), 0.6f);
            Ray ray = new()
            {
                origin = cameraObject.transform.position,
                direction = cameraObject.transform.forward,
            };
            bool hasHit = Physics.Raycast(ray, out RaycastHit hit, 5f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            if (hasHit)
            {
                Vector3 force = -hit.normal * 2;
                hit.rigidbody?.AddForce(force);
                hit.collider.TryGetComponent(out PlayerController controller);
                if (controller)
                {
                    controller.PlayerPush(controller.Owner, force);
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void ResetCanPush()
        {
            canPush.Value = false;
        }

        [TargetRpc]
        public void PlayerPush(NetworkConnection conn, Vector3 force)
        {
            if(conn == Owner)
            rb.AddForce(force);
        }

        public bool tps = false;
        
        
        private async Task<Image?> GetAvatar()
        {
            try
            {
                // Get Avatar using await
                return await SteamFriends.GetLargeAvatarAsync(steamID.Value);
            }
            catch ( Exception e )
            {
                // If something goes wrong, log it
                print(e);
                return null;
            }
        }
        
        private Texture2D ConvertToTexture2D(Image image)
        {
            // Create a new Texture2D
            var avatar = new Texture2D( (int)image.Width, (int)image.Height, TextureFormat.ARGB32, false );
	
            // Set filter type, or else its really blury
            avatar.filterMode = FilterMode.Trilinear;

            // Flip image
            for ( int x = 0; x < image.Width; x++ )
            {
                for ( int y = 0; y < image.Height; y++ )
                {
                    var p = image.GetPixel( x, y );
                    avatar.SetPixel( x, (int)image.Height - y, new UnityEngine.Color( p.r / 255.0f, p.g / 255.0f, p.b / 255.0f, p.a / 255.0f ) );
                }
            }
	
            avatar.Apply();
            return avatar;
        }
        
        private void Update()
        {
            nicknameTMP.text = nickname.Value;
            UpdatePlayerAvatar();
            if (!IsOwner) return;
            rb.isKinematic = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 1;
            myCamera.transform.position = tps ? tpsPos.position : cameraObject.transform.position;
            myCamera.transform.rotation = tps ? tpsPos.rotation : cameraObject.transform.rotation;
            velocity = rb.linearVelocity;
            rb.angularVelocity = Vector3.zero;

            if (Input.GetKeyUp(KeyCode.Escape))
            {
                MouseCaptured = !MouseCaptured;
                PauseMenu.Instance.gameObject.SetActive(!MouseCaptured);
                //Time.timeScale = MouseCaptured ? 1f : 0f;
            }

            if (MouseCaptured)
            {
                HandleInput();
                RotateCamera();
            }

            HandleGrabbing();
            ApplyVelocity();
        }

        private async Task UpdatePlayerAvatar()
        {
            var avatar = await GetAvatar();
            if(avatar == null) return;
            if (face.material.mainTexture != ConvertToTexture2D(avatar.Value));
            {
                face.material.mainTexture = ConvertToTexture2D(avatar.Value);
            }
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
            if (Input.GetKeyDown(KeyCode.Q))
            {
                RetrieveStupidBroom();
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                tps = !tps;
            }

            var lookingAtItem = Physics.Raycast(cameraObject.transform.position, cameraObject.transform.forward, out var hit, 5f, pickupMask);

            Crosshair.CrosshairColor = lookingAtItem ? Color.green : Color.red;

            if (Input.GetMouseButtonDown(0))
            {
                if (IsGrabbing)
                {
                    grabbedBody.GetComponent<PickUpItem>().DropItem();
                    grabbedBody = null;
                    return;
                }

                else if (lookingAtItem && !IsGrabbing)
                {
                    var collider = hit.collider.gameObject;

                    if (!collider.TryGetComponent(out grabbedBody))
                        return;

                    grabbedBody.GetComponent<PickUpItem>().GrabItem(LocalConnection);
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
