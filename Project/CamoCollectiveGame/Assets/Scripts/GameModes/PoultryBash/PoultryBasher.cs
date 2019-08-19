﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoultryBasher : MonoBehaviour
{
    [Header("Punching")]
    [SerializeField]
    private float m_punchRadius;
    [SerializeField]
    private float m_punchDistance;
    [SerializeField]
    private float m_knockback;
    [SerializeField]
    private float m_knockup;

    [Header("Animation")]
    [SerializeField]
    private Animator m_animator;

    [Header("Data")]
    [SerializeField]
    private BoolReference m_deadValue;

    private bool m_punchedRight;
    private bool m_punchedLeft;

    private InputMapper m_input;

    private void Start()
    {
        m_input = GetComponent<InputMapper>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Finish"))
        {
            m_deadValue.Value = true;
            m_input.DisableInput();
        }
    }

    public void LeftPunch(float trigger)
    {
        if (trigger == 0)
        {
            m_punchedLeft = false;
            return;
        }
        if (m_punchedLeft)
            return;
        m_punchedLeft = true;
        m_animator.SetTrigger("LeftPunch");
    }

    public void RightPunch(float trigger)
    {
        if (trigger == 0)
        {
            m_punchedRight = false;
            return;
        }
        if (m_punchedRight)
            return;
        m_punchedRight = true;
        m_animator.SetTrigger("RightPunch");
    }

    public void Punch()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position - transform.forward, m_punchRadius, transform.forward, out hit, m_punchDistance, 1<<10))
        {
            Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * m_knockback, ForceMode.Impulse);
            rb.AddForce(Vector3.up * m_knockup, ForceMode.Impulse);
        }
    }
}
