﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class recurso_coletavel : MonoBehaviour, SalvamentoEntreCenas
{
    [SerializeField] private item item;
    [SerializeField] private int qntd;
    [SerializeField] private float tempoParaLiberarColeta;
    [SerializeField] private BoxCollider2D areaColetavel;
    [SerializeField] private BoxCollider2D areaFisica;
    private GameObject NPCRelacionado = null;
    private Rigidbody2D rb;
    private SpriteRenderer icone;
    // Start is called before the first frame update
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        icone = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        icone.sprite = item.icone;
        if (areaColetavel.enabled == false)
            StartCoroutine(this.ligarColeta());
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<jogadorScript>().InterfaceJogador.AtualizaInventarioUI(item, qntd);
            SalvarEstado();
            Destroy(this.gameObject);
        }
    }
    public void DefineItem(item it)
    {
        item = it;
    }
    public item ReferenciaItem()
    {
        return item;
    }
    public void DefineQuantidadeItem(int q)
    {
        qntd = q;
    }
    public void LancaRecurso(float forca, float direcaoX, float direcaoY)
    {
        float localDeDropX;
        float localDeDropY;
        if (direcaoX != 0)        
            localDeDropX = direcaoX;        
        else
        {
            localDeDropX = Random.Range(-1f, 2f);
            if (localDeDropX == 0)
                localDeDropX = 1;
        }
        if (direcaoY != 0)
            localDeDropY = direcaoY;
        else
        {
            localDeDropY = Random.Range(-1f, 2f);
            if (localDeDropY == 0)
                localDeDropY = 1;
        }
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(localDeDropX, localDeDropY).normalized * forca);
    }
    IEnumerator ligarColeta()
    {
        yield return new WaitForSeconds(tempoParaLiberarColeta);
        areaColetavel.enabled = true;
        areaFisica.enabled = true;
    }
    public void SalvarEstado()
    {
        if (GetComponent<SalvarEstadoDoObjeto>() != null)
        {
            GetComponent<SalvarEstadoDoObjeto>().SalvarSeJaFoiModificado();
            if (NPCRelacionado != null)
                NPCRelacionado.GetComponent<NPCscript>().SalvarEstado();//salvar item necessário para quest
        }
    }
    public void AcaoSeEstadoJaModificado()
    {
        Destroy(this.gameObject);
    }
    public void SetNPC(GameObject npc)
    {
        NPCRelacionado = npc;
    }
}
