﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChonkJoustingLanceAttack : MonoBehaviour
{
    [SerializeField]
    private Transform m_lanceTransform;
    [SerializeField]
    private float m_knockbackForce;
    [SerializeField]
    private float m_knockupForce;
    [SerializeField]
    private float m_shieldAngle;
    [SerializeField]
    private float m_invincibilityFrame;
    [SerializeField]
    private float m_invincibilityFlashTime;
    private bool m_isInvincible;

    [Header("Data")]
    [SerializeField]
    private FloatReference m_scoreValue;
    [SerializeField]
    private FloatReference m_livesValue;


    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(new Ray(m_lanceTransform.position, m_lanceTransform.forward), out hit, 1))
        {
            if (hit.transform.CompareTag("Player"))
            {
                ChonkJoustingLanceAttack chonk = hit.transform.GetComponentInParent<ChonkJoustingLanceAttack>();
                if (chonk.m_isInvincible)
                    return;
                Attack(hit.transform.gameObject);
            }
        }
    }

    public void Attack(GameObject other)
    {
        float dot = Vector3.Dot(transform.forward, other.transform.forward);
        float shield = Mathf.Lerp(-1, 1, Mathf.InverseLerp(0, 180, m_shieldAngle));
        if (dot < shield)
        {
            Knockback(transform.forward * -1 * m_knockbackForce * 2, 0);
            other.GetComponentInParent<ChonkJoustingLanceAttack>().Knockback(transform.forward * m_knockbackForce * 2, 0);
        }
        else
        {
            m_scoreValue.Value++;
            other.GetComponentInParent<ChonkJoustingLanceAttack>().Hurt(transform.forward * m_knockbackForce, m_knockupForce);
        }
    }

    public void Knockback(Vector3 knockback, float knockup)
    {
        GetComponent<Rigidbody>().AddForce(Vector3.up * knockup, ForceMode.Impulse);
        GetComponent<Rigidbody>().AddForce(knockback, ForceMode.Impulse);
    }

    public void Hurt(Vector3 knockback, float knockup)
    {
        Knockback(knockback, knockup);
        m_livesValue.Value--;
        if (m_livesValue.Value <= 0)
            GetComponent<ChonkJoustingDeath>().OnDeath();
        else
            StartCoroutine(InvincibilityFrame());
    }

    private IEnumerator InvincibilityFrame()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        Color[] colours = new Color[renderers.Length];

        int i = 0;
        foreach (Renderer rend in renderers)
        {
            colours[i] = rend.material.color;
            i++;
        }

        m_isInvincible = true;
        float timer = m_invincibilityFrame;
        float flashTimer = m_invincibilityFlashTime;
        bool flash = true;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            flashTimer -= Time.deltaTime;

            i = 0;
            foreach (Renderer rend in renderers)
            {
                if (flash)
                {
                    rend.material.color = colours[i];
                    rend.material.color = new Color(rend.material.color.r, rend.material.color.g, rend.material.color.b, 0.4f);
                }
                else
                    rend.material.color = new Color(1, 0, 0, 0.3f);
                i++;
            }
            if (flashTimer <= 0)
            {
                flash = !flash;
                flashTimer = m_invincibilityFlashTime;
            }

            yield return null;
        }
        m_isInvincible = false;
        i = 0;
        foreach (Renderer rend in renderers)
        {
            rend.material.color = colours[i];
            i++;
        }
    }
}
