﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerClimbPlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private float m_rotateSpeed;
    [SerializeField]
    private float m_climbSpeed;
    [SerializeField]
    private FloatReference m_fallSpeed;
    [SerializeField]
    private float m_strafeSpeed;
    [SerializeField]
    private float m_autoClimbMoveSpeed;

    [Header("Sticky Obstacle")]
    [SerializeField]
    private float m_stickyMovementScale;

    [Header("Data")]
    [SerializeField]
    private FloatReference m_yPosValue;

    private Quaternion m_leftClimbRot;
    private Quaternion m_rightClimbRot;
    private Quaternion m_targetRot;
    private bool m_climbLeft;
    private bool m_atTargetRot;
    private bool m_playerHasControl;
    private bool m_playerFalling;
    private bool m_stopMoving;
    private Rigidbody m_rb;
    private float m_climbScale;

    private bool m_pressedLeft;
    private bool m_pressedRight;

    private void Awake()
    {
        m_leftClimbRot = Quaternion.Euler(20, 0, 0);
        m_rightClimbRot = Quaternion.Euler(-20, 0, 0);
        m_atTargetRot = true;
        m_playerHasControl = false;
        m_rb = GetComponent<Rigidbody>();
        TakeControl();
    }

    private void Start()
    {
        m_climbScale = 1;
    }

    public void GiveControl()
    {
        m_playerHasControl = true;
        GetComponent<InputMapper>().EnableInput();
    }

    public void StartFalling()
    {
        m_playerFalling = true;
    }

    public void StopFalling()
    {
        m_playerFalling = false;
    }

    public void TakeControl()
    {
        m_playerHasControl = false;
        GetComponent<InputMapper>().DisableInput();
    }

    public void MovePlayer(Vector2 joystick)
    {
        RaycastHit hit;
        Vector3 newPos = new Vector3(transform.position.x, transform.position.y, transform.position.z - joystick.x * m_strafeSpeed * m_climbScale * Time.deltaTime);
        if (!Physics.Raycast(transform.position, newPos - transform.position, out hit, 0.5f))
        {
            transform.position = newPos;
        }
    }

    public void Climb()
    {
        if (!m_atTargetRot)
            return;
        m_climbLeft = !m_climbLeft;
        if (m_climbLeft)
            m_targetRot = m_leftClimbRot;
        else
            m_targetRot = m_rightClimbRot;
        m_atTargetRot = false;
    }

    public void ClimbLeft(float trigger)
    {
        //if (trigger != 0)
        //    MovePlayer(Vector3.left);
        if (trigger <= 0 || m_pressedLeft || !m_atTargetRot || !m_climbLeft)
            return;
        if (m_climbLeft)
            m_targetRot = m_leftClimbRot;
        m_pressedLeft = true;
        m_pressedRight = false;
        m_climbLeft = !m_climbLeft;
        m_atTargetRot = false;
    }

    public void ClimbRight(float trigger)
    {
        //if (trigger != 0)
        //    MovePlayer(Vector3.right);
        if (trigger <= 0 || m_climbLeft || m_pressedRight || !m_atTargetRot)
            return;
        if (!m_climbLeft)
            m_targetRot = m_rightClimbRot;
        m_pressedLeft = false;
        m_pressedRight = true;
        m_climbLeft = !m_climbLeft;
        m_atTargetRot = false;
    }

    private void Update()
    {
        m_yPosValue.Value = transform.position.y;
        if (m_stopMoving)
            return;
        RaycastHit hit;
        bool hitUp = Physics.Raycast(transform.position, Vector3.up, out hit, 1.0f);
        if (hitUp && hit.collider.CompareTag("Mud"))
            hitUp = false;
        if (hitUp && hit.transform.CompareTag("StopClimber"))
        {
            m_stopMoving = true;
            return;
        }
        if (hitUp)
            transform.position = new Vector3(transform.position.x, hit.point.y - 1.0f, transform.position.z);
        if (!m_playerHasControl && !hitUp)
        {
            transform.position += Vector3.up * m_fallSpeed.Value * Time.deltaTime;
            Climb();
        }

        transform.rotation = Quaternion.RotateTowards(transform.rotation, m_targetRot, m_rotateSpeed * Time.deltaTime);
        if (transform.rotation == m_targetRot)
            m_atTargetRot = true;
        else if (m_playerHasControl && !hitUp)
            transform.position = new Vector3(transform.position.x, transform.position.y + m_climbSpeed * m_climbScale * Time.deltaTime, transform.position.z);
        if (m_playerFalling)
            transform.position += Vector3.down * m_fallSpeed.Value * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Mud"))
            m_climbScale = m_stickyMovementScale;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Mud"))
            m_climbScale = 1;
    }
}
