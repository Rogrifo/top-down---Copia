﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class balaHit : MonoBehaviour
{
    public float DuracaoDaBala;
    private Animator animator;
    private float dano;
    private float duracaoStun;
    [SerializeField] private bool balaJogador = false;
    [SerializeField] private bool balaInimigo = false;

    private void Start()
    {
        StartCoroutine(this.DestruirProjetil());
    }
    IEnumerator DestruirProjetil()
    {
        yield return new WaitForSeconds(DuracaoDaBala);
        Destroy(this.gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (balaInimigo)
        {
            if (collision.gameObject.tag == "Player")
            {
                collision.gameObject.GetComponent<jogadorScript>().mudancaRelogio(dano, 1f);
                Destroy(this.gameObject);
            }
            destruirAoTocar(collision);
        }
        else if (balaJogador)
        {
            if (collision.gameObject.tag == "inimigo")
            {
                collision.gameObject.GetComponentInChildren<hitbox_inimigo>().inimigo.GetComponent<inimigoScript>().mudancaVida(dano, this.tag, 0f, this.transform.position, 0f);
                Destroy(this.gameObject);
            }
            destruirAoTocar(collision);
        }
    }
    private void destruirAoTocar(Collider2D collision)
    {
        if (collision.gameObject.tag == "obstaculo")
        {
            Destroy(this.gameObject);
        }
    }
    public void SetDano(float d)
    {
        dano = d;
    }
    public Animator GetAnimator()
    {
        animator = GetComponent<Animator>();
        return animator;
    }
    public void SetDuracaoStun(float f)
    {
        duracaoStun = f;
    }
}
