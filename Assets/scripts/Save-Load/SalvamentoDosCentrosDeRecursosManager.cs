﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SalvamentoDosCentrosDeRecursosManager : MonoBehaviour
{
    public static SalvamentoDosCentrosDeRecursosManager Instance { get; private set; }
    static Dictionary<string, float> TemposDeSaidaDasFases = new Dictionary<string, float>();
    private List<GameObject> centrosDeRecursosEmCena = new List<GameObject>();
    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        string IndexFaseBase = SceneUtility.GetScenePathByBuildIndex(SceneManager.GetActiveScene().buildIndex);//pega o caminho da cena na pasta de arquivos
        string cenaAtual = IndexFaseBase.Substring(0, IndexFaseBase.Length - 6).Substring(IndexFaseBase.LastIndexOf('/') + 1);
        if (!TemposDeSaidaDasFases.ContainsKey(cenaAtual))
        {
            TemposDeSaidaDasFases.Add(cenaAtual, 0f);
        }
    }
    public float TempoDeSaidaDaFase(int BuildIndex)
    {
        string IndexFaseBase = SceneUtility.GetScenePathByBuildIndex(BuildIndex);//pega o caminho da cena na pasta de arquivos
        string cena = IndexFaseBase.Substring(0, IndexFaseBase.Length - 6).Substring(IndexFaseBase.LastIndexOf('/') + 1);
        if (TemposDeSaidaDasFases.ContainsKey(cena))
        {
            //Mydebug.mydebug.MyPrint("salvei tempo de saida da fase");
            return TemposDeSaidaDasFases[cena];
        }
        else
            return 0f;
    }
    public void SalvarTempoDeSaida()
    {
        string IndexFaseBase = SceneUtility.GetScenePathByBuildIndex(SceneManager.GetActiveScene().buildIndex);//pega o caminho da cena na pasta de arquivos
        string cenaAtual = IndexFaseBase.Substring(0, IndexFaseBase.Length - 6).Substring(IndexFaseBase.LastIndexOf('/') + 1);
        if (TemposDeSaidaDasFases.ContainsKey(cenaAtual))
        {
            TemposDeSaidaDasFases[cenaAtual] = Time.time;
        }
    }
    public void AdicionarCentroALista(GameObject CDRI)
    {
        if(CDRI.GetComponent<SalvarEstadoDoObjeto>() != null)
            centrosDeRecursosEmCena.Add(CDRI);
    }
    public void SalvarCentrosDeRecursoDaCenaAtual()
    {
        for (int i = 0; i < centrosDeRecursosEmCena.Count; i++)
            centrosDeRecursosEmCena[i].GetComponent<CentroDeRecursoInfinito>().SalvarEstado();
    }
    public void LimparDados()
    {
        TemposDeSaidaDasFases.Clear();
        centrosDeRecursosEmCena.Clear();
    }
}
