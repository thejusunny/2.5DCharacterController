using System;
using UnityEngine;
namespace Controller
{
    [Serializable]
    public class CharacterCollision
    {
        private Transform playerTransform;
        [SerializeField] private float groundRaylength;
        [SerializeField] private float movingPlatformRaylength=3f;
        [SerializeField] private float wallRaylength;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] bool isOnGround;
        [SerializeField] bool isOnWall;
        [SerializeField] bool isOnCeiling;
        [SerializeField]bool isOnMovingPlatform;
        public bool OnGround => leftOnGround || rightOnGround;
        public bool OnWall => leftWall || rightWall;
        IMovingPlatform platformInContact;
        RaycastHit movingPlatformRayHit;
        bool leftOnGround;
        bool rightOnGround;
        bool leftWall;
        bool rightWall;
       
        bool leftRayOnCeiling;
        bool righRayOnCeiling;
        public int WallDirection { get => leftWall ? -1 : rightWall ? 1 : 0; }
        public bool OnCeiling => leftRayOnCeiling || righRayOnCeiling;
        public bool LeftCelingHit => leftRayOnCeiling;
        public bool RightCelingHit => righRayOnCeiling;
        private RaycastHit leftCeilingRayHit;
        private RaycastHit rightCeilingRayHit;
        public bool BothRaysCelingHit => leftRayOnCeiling && righRayOnCeiling;
        public Vector3 LeftCeilingRayOffset => (Vector3.up * 0.2f - Vector3.right * 0.3f);
        public Vector3 RightCeilingRayOffset => (Vector3.up * 0.2f + Vector3.right * 0.3f);

        public RaycastHit LeftCeilingRayHit { get => leftCeilingRayHit; }
        public RaycastHit RightCeilingRayHit { get => rightCeilingRayHit; }
        public IMovingPlatform PlatformInContact { get => platformInContact; }
        public bool IsOnMovingPlatform { get => platformInContact!=null; }

        public void Init(Transform player)
        {
            playerTransform = player;
        }
        public void ProcessCollisions()
        {
            DetectMovingPlatform();
            DetectGroundCollisions();
            DetectWallCollisions();
            DetectCeilingCollisions();
            isOnGround = OnGround;
            isOnWall = OnWall;
            isOnCeiling = OnCeiling;
        }

        private void DetectMovingPlatform()
        {
            isOnMovingPlatform = Physics.Raycast(playerTransform.position, Vector3.down, out movingPlatformRayHit, movingPlatformRaylength, groundLayer);
            if (isOnMovingPlatform)
            {

                IMovingPlatform platform = movingPlatformRayHit.transform.GetComponent<IMovingPlatform>();
                if (platform != null)
                    platformInContact = platform;

            }
            else
                platformInContact = null;
        }

        private void DetectCeilingCollisions()
        {
            leftRayOnCeiling = Physics.Raycast(playerTransform.position + LeftCeilingRayOffset, Vector3.up, out leftCeilingRayHit, groundRaylength, groundLayer);
            righRayOnCeiling = Physics.Raycast(playerTransform.position + RightCeilingRayOffset, Vector3.up, out rightCeilingRayHit, groundRaylength, groundLayer);
        }

        private void DetectGroundCollisions()
        {
            leftOnGround = Physics.Raycast(playerTransform.position - Vector3.right * 0.5f, Vector3.down, groundRaylength, groundLayer);
            rightOnGround = Physics.Raycast(playerTransform.position + Vector3.right * 0.5f, Vector3.down, groundRaylength, groundLayer);
        }
        private void DetectWallCollisions()
        {
            leftWall = Physics.Raycast(playerTransform.position - Vector3.right * 0.5f, Vector3.left, wallRaylength, groundLayer);
            rightWall = Physics.Raycast(playerTransform.position + Vector3.right * 0.5f, Vector3.right, wallRaylength, groundLayer);
        }
    }
}