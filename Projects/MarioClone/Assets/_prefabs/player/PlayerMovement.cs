using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{


    #region Properties

    public bool UseFixDeltaTime { get { return _useFixDeltaTime; } set { _useFixDeltaTime = value; } }

    #endregion

    //Check for movement
    public LayerMask _levelLayerMask;
    public Transform _topRightCorner;
    public Transform _topLeftCorner;
    public Transform _bottomRightCorner;
    public Transform _bottomLeftCorner;

    //Horizontal movement
    public float _gravity;
    public float _jumpForcePerSecond;
    public float _maxTimeOfJumpForce;

    //Vertical movement
    public float _maxNormalVelocity;
    public float _maxSprintVelocity;

    private float _timeRemaingJump;

    private bool _useFixDeltaTime = false;

    // Use this for initialization
    void Start()
    {
        _timeRemaingJump = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {

    }

    #region Public methods

    public void Move(float right, float jump)
    {
        float deltaTime = _useFixDeltaTime ? 0.02f : Time.deltaTime;

        bool grounded = IsPlayerGrounded();
        bool ceiling = IsPlayerOnCeiling();


        Vector2 move = deltaTime * _maxNormalVelocity * new Vector2(right, 0);


        //Add gravity
        if (!grounded) move += -Vector2.up * deltaTime * _gravity;
        else _timeRemaingJump = 0f;

        if (ceiling) _timeRemaingJump = 0f;

        //If Grunded and jump is selected -> jump
        if (grounded && jump > 0f)
        {
            _timeRemaingJump = _maxTimeOfJumpForce;
        }

        //Add Jump force;
        if (_timeRemaingJump > 0f)
        {
            move += Vector2.up * deltaTime * _jumpForcePerSecond;
            _timeRemaingJump -= deltaTime;
        }

        MoveWithCollision(move);
    }

    #endregion

    #region Private methods

    private void MoveWithCollision(Vector2 move)
    {
        //Check up/down movement
        if (move.y > 0f)
        {
            //Move up -> ceiling check
            RaycastHit2D leftTop = Physics2D.Raycast(_topLeftCorner.transform.position + new Vector3(0.005f, 0f, 0f), move.normalized, move.magnitude, _levelLayerMask);
            RaycastHit2D rightTop = Physics2D.Raycast(_topRightCorner.transform.position + new Vector3(-0.005f, 0f, 0f), move.normalized, move.magnitude, _levelLayerMask);

            //If no collision occured, do nothing
            if (leftTop.collider != null || rightTop.collider != null)
            {
                //Left top collider was hit
                if (leftTop.collider != null && Mathf.Abs(leftTop.point.y - leftTop.collider.bounds.min.y) <= 0.001f)
                {
                    //Hit top left
                    move.y = Mathf.Abs(leftTop.collider.bounds.min.y - _topLeftCorner.transform.position.y);
                }

                //Right top collider was hit
                if (rightTop.collider != null && Mathf.Abs(rightTop.point.y - rightTop.collider.bounds.min.y) <= 0.001f)
                {
                    float moveDistance = Mathf.Abs(rightTop.collider.bounds.min.y - _topRightCorner.transform.position.y);
                    if (move.y >= moveDistance) move.y = moveDistance;
                }
            }
        }
        else if (move.y < 0f)
        {
            //Move down -> ground check
            RaycastHit2D leftBottom = Physics2D.Raycast(_bottomLeftCorner.transform.position + new Vector3(0.005f, 0f, 0f), move.normalized, move.magnitude, _levelLayerMask);
            RaycastHit2D rightBottom = Physics2D.Raycast(_bottomRightCorner.transform.position + new Vector3(-0.005f, 0f, 0f), move.normalized, move.magnitude, _levelLayerMask);

            //If no collision occured, do nothing
            if (leftBottom.collider != null || rightBottom.collider != null)
            {
                //Left top collider was hit
                if (leftBottom.collider != null && Mathf.Abs(leftBottom.point.y - leftBottom.collider.bounds.max.y) <= 0.001f)
                {
                    //Hit top left
                    move.y = -Mathf.Abs(leftBottom.collider.bounds.max.y - _bottomLeftCorner.transform.position.y);
                }

                //Right top collider was hit
                if (rightBottom.collider != null && Mathf.Abs(rightBottom.point.y - rightBottom.collider.bounds.max.y) <= 0.001f)
                {
                    float moveDistance = -Mathf.Abs(_bottomRightCorner.transform.position.y - rightBottom.collider.bounds.max.y);
                    if (move.y <= moveDistance) move.y = moveDistance;
                }
            }

        }

        //Horitontal movement check
        if (move.x > 0f)
        {
            RaycastHit2D rightTop = Physics2D.Raycast(_topRightCorner.transform.position + new Vector3(0f, -0.005f, 0f), move.normalized, move.magnitude, _levelLayerMask);
            RaycastHit2D rightBottom = Physics2D.Raycast(_bottomRightCorner.transform.position + new Vector3(0f, 0.005f, 0f), move.normalized, move.magnitude, _levelLayerMask);

            //No collision -> do nothing
            if (rightTop.collider != null || rightBottom.collider != null)
            {
                //Check top right point
                if (rightTop.collider != null && Mathf.Abs(rightTop.point.x - rightTop.collider.bounds.min.x) <= 0.001f)
                {
                    move.x = Mathf.Abs(rightTop.collider.bounds.min.x - _topRightCorner.position.x);
                }

                if (rightBottom.collider != null && Mathf.Abs(rightBottom.point.x - rightBottom.collider.bounds.min.x) <= 0.001f)
                {
                    float moveDistance = Mathf.Abs(rightBottom.collider.bounds.min.x - _bottomRightCorner.position.x);
                    if (move.x >= moveDistance) move.x = moveDistance;
                }
            }

        }
        else if (move.x < 0f)
        {
            RaycastHit2D lefTop = Physics2D.Raycast(_topLeftCorner.transform.position + new Vector3(0f, -0.005f, 0f), move.normalized, move.magnitude, _levelLayerMask);
            RaycastHit2D rightBottom = Physics2D.Raycast(_bottomLeftCorner.transform.position + new Vector3(0f, 0.005f, 0f), move.normalized, move.magnitude, _levelLayerMask);

            //No collision -> do nothing
            if (lefTop.collider != null || rightBottom.collider != null)
            {
                //Check top right point
                if (lefTop.collider != null && Mathf.Abs(lefTop.point.x - lefTop.collider.bounds.max.x) <= 0.001f)
                {
                    move.x = -Mathf.Abs(lefTop.collider.bounds.max.x - _topLeftCorner.position.x);
                }

                if (rightBottom.collider != null && Mathf.Abs(rightBottom.point.x - rightBottom.collider.bounds.max.x) <= 0.001f)
                {
                    float moveDistance = -Mathf.Abs(rightBottom.collider.bounds.max.x - _bottomLeftCorner.position.x);
                    if (move.x <= moveDistance) move.x = moveDistance;
                }
            }
        }

        this.transform.position = this.transform.position + new Vector3(move.x, move.y);
    }

    private bool IsPlayerGrounded()
    {
        bool grounded = Physics2D.OverlapArea(_bottomLeftCorner.transform.position + new Vector3(0.05f, 0f, 0f), _bottomRightCorner.transform.position + new Vector3(-0.05f, -0.005f, 0f), _levelLayerMask);
        //Debug.Log(grounded);
        return grounded;
    }

    private bool IsPlayerOnCeiling()
    {
        bool ceiling = Physics2D.OverlapArea(_topLeftCorner.transform.position + new Vector3(0.05f, 0f, 0f), _topRightCorner.transform.position + new Vector3(-0.05f, 0.005f, 0f), _levelLayerMask);
        return ceiling;
    }

    #endregion
}
