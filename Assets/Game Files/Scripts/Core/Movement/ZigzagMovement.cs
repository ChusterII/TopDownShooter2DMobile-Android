using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WarKiwiCode.Game_Files.Scripts.Core.Movement
{
    public class ZigzagMovement : EnemyMovement
    {
        [SerializeField] private float minimumDistanceToPlayer = 2f;
        [SerializeField] private float minimumZigZagDistance = 2f;
        
        
        private bool _pickedZigZagCoordinates;
        private Vector2 _zigPosition;
        private Vector2 _zagPosition;
        private bool _moveToZig;
        private bool _isMoving;
        private bool _queueEmpty;
        private readonly Queue<Vector2> _zigzagCoordinates = new Queue<Vector2>();
        private Vector2 _waypoint;
        
        // Testing
        //private Color _color;
        
        // Start is called before the first frame update
        void Start()
        {
            //_color = Random.ColorHSV();
            //
            while (!_pickedZigZagCoordinates)
            {
                if (CalculateInitialZigZagPositions()) continue;
                SetInitialWayPoint();
                CreateZigzagPositionsQueue();
            }
        }

        private void CreateZigzagPositionsQueue()
        {
            float yPosition = GetPosition().y;
            float yVariance = Random.Range(0.5f, 1f);
            if (yPosition >= 0)
            {
                // Upper half of the screen
                yPosition -= yVariance;
                while (yPosition > minimumDistanceToPlayer)
                {
                    EnqueueNextZigZagPosition(yPosition);
                    yPosition -= yVariance;
                }
            }
            else
            {
                // Lower half of the screen
                yPosition += yVariance;
                while (yPosition < -minimumDistanceToPlayer)
                {
                    EnqueueNextZigZagPosition(yPosition);
                    yPosition += yVariance;
                }
            }
        }

        private void EnqueueNextZigZagPosition(float yPosition)
        {
            if (_moveToZig)
            {
                // Calculate next ZAG position
                _zagPosition = new Vector2(_zagPosition.x, yPosition);
                _zigzagCoordinates.Enqueue(_zagPosition);
                _moveToZig = false;
            }
            else
            {
                // Calculate next ZIG position
                _zigPosition = new Vector2(_zigPosition.x, yPosition);
                _zigzagCoordinates.Enqueue(_zigPosition);
                _moveToZig = true;
            }
        }

        private void SetInitialWayPoint()
        {
            float distanceToZig = Vector2.Distance(GetPosition(), _zigPosition);
            float distanceToZag = Vector2.Distance(GetPosition(), _zagPosition);
            if (distanceToZig < distanceToZag)
            {
                // Need to move to Zig because it's closer
                _moveToZig = true;
                _zigzagCoordinates.Enqueue(_zigPosition);
            }
            else
            {
                // Need to move to Zag because it's closer
                _moveToZig = false;
                _zigzagCoordinates.Enqueue(_zagPosition);
            }
        }

        private bool CalculateInitialZigZagPositions()
        {
            // Get an x value between -3.5 and 3.5
            float xZig = Random.Range(-3.5f, 3.5f);
            float xZag = Random.Range(-3.5f, 3.5f);
            float minX = Mathf.Min(xZig, xZag);
            float maxX = Mathf.Max(xZig, xZag);

            // Check if the difference between both values is equal and greater than 2.
            if (!(minX + minimumZigZagDistance <= maxX)) return true;

            // Set initial zig and zag x value
            _pickedZigZagCoordinates = true;
            _zigPosition = new Vector2(minX, GetPosition().y);
            _zagPosition = new Vector2(maxX, GetPosition().y);
            return false;
        }

        // Update is called once per frame
        void Update()
        {
            Move();
        }

        protected override void Move()
        {
            step = moveSpeed * Time.deltaTime;
            if (disableMovement) return;
            if (!_queueEmpty)
            {
                // Keep getting zigzag coordinates
                if (!_isMoving)
                {
                    _waypoint = _zigzagCoordinates.Dequeue();
                    _isMoving = true;
                }
                
                // Move towards the next waypoint
                transform.position = Vector3.MoveTowards(transform.position, _waypoint, step);
                if (Vector3.Distance(transform.position, _waypoint) < 0.001f && _isMoving)
                {
                    _isMoving = false;
                    if (_zigzagCoordinates.Count == 0)
                    {
                        _queueEmpty = true;
                    }
                }

            }
            else
            {
                // Time to move towards the player
                if (canMove)
                {
                    transform.position = Vector3.MoveTowards(transform.position, playerPosition, step);
                }
                canMove = !(Vector3.Distance(transform.position, playerPosition) < 0.9f);
            }

        }
        
        /*private void OnDrawGizmos()
        {
            foreach (Vector2 position in _zigzagCoordinates)
            {
                Gizmos.color = _color;
                Gizmos.DrawSphere(position, 0.1f);
            }
        }*/
    }
}
