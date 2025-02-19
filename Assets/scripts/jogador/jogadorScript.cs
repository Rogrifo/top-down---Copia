﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class jogadorScript : MonoBehaviour, AcoesNoTutorial
{
    public static jogadorScript Instance { get; private set; }
    //variáveis publicas
    [Header("Valores Numéricos")]
    [SerializeField] private float velocidade;
    //private float vidaMaxima = 1;
    [SerializeField] private float velocidadeProjetil;
    [SerializeField] private float taxaDeDisparo;
    [SerializeField] private float taxaDeAtaqueMelee;
    [SerializeField] private float alcanceMelee;
    private Vector2 distanciaAtaqueMelee;
    [SerializeField] private float danoMelee;
    [SerializeField] private float forcaKnockbackNosInimigos;
    [SerializeField] private float duracaoKnockbakNosInimigos;
    [SerializeField] private float tempParalisacaoProjetil;
    [Header("Componentes")]
    //[SerializeField] private JogadorAnimScript animScript;
    public Camera mainCamera;
    [SerializeField] private Transform posicaoMelee;
    [SerializeField] private Transform pontoDeDisparo;
    [SerializeField] private GameObject projetilPrefab;
    [SerializeField] private LayerMask objetosAcertaveisLayer;
    public UIinventario InterfaceJogador;
    [SerializeField] private GameObject iconeInteracao;
    [SerializeField] private GameObject animacoesTutorial;
    public CinemachineBehaviour comportamentoCamera;
    //variáveis privadas
    //private float vidaAtual;
    //inputs
    private KeyCode inputDisparo = KeyCode.Mouse1;
    private KeyCode inputAtaqueMelee = KeyCode.Mouse0;
    private Vector2 movimento;
    private Rigidbody2D rb;
    private bool atirando = false;
    private bool atacando = false;
    private float forcaEmpurrao;
    private Vector2 direcaoEmpurrao;
    [Header("Não Mexer")]
    private Vector2 baguncarControles = new Vector2(0,0);
    public enum estados
    {
        EmAcao,
        Paralisado,
        EmContrucao,
        EmDialogo,
        SendoEmpurrado,
        EmUI
    };
    /*[HideInInspector]*/ public estados estadosJogador = estados.EmAcao;// 0 = em acao, 1 = em menus, 2 = em construcao 
    private ReceitaDeCrafting moduloCriado;
    private bool podeAnimar = true;
    private Vector2 direcaoProjetil = Vector2.zero;
    [SerializeField] private GameObject caixaComTudo;
    private GameObject balaDisparada = null;
    //[SerializeField] private Missao m;
    // Start is called before the first frame update
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        rb = GetComponent<Rigidbody2D>();
        distanciaAtaqueMelee.x = posicaoMelee.localPosition.x;
        distanciaAtaqueMelee.y = posicaoMelee.localPosition.y;
    }
    private void Start()
    {
        if (TutorialSetUp.Instance != null)
            Tutorial();
    }
    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.L))
        //    MissoesManager.Instance.AdicionarMissao(m);
        //if (Input.GetKeyDown(KeyCode.KeypadEnter))
        //    MissoesManager.Instance.ConcluirMissao(m);
            //    SoundManager.Instance.TocarSom(SoundManager.Som.ModuloExplodindo);
            //    Debug.Log(TutorialSetUp.Instance);
            //    SceneManager.LoadScene("testes");
            if (Input.GetKeyDown(KeyCode.P) && Input.GetKey(KeyCode.LeftShift))
        {
            Instantiate(caixaComTudo, transform.position + new Vector3(0f, 1f, 0f), Quaternion.identity);
        }
        if (/*!UIinventario.Instance.pausado*/!InterfaceMenu.Instance.pausado)
        {
            switch (estadosJogador)
            {
                case estados.EmAcao: //movimentando
                    MovimentoInput();
                    InputAtirar();
                    InputAtaqueMelee();
                    break;
                //case estados.EmContrucao://construindo
                //    //executando click em objetos
                //    break;
                case estados.EmDialogo:
                    InputProsseguirDialogo();
                    break;
                case estados.Paralisado:
                    if (TutorialSetUp.Instance != null)
                    {
                        if (TutorialSetUp.Instance.GetSequenciaDialogos() == 1)
                            InputAtirar();
                        else if (TutorialSetUp.Instance.GetSequenciaDialogos() == 2)
                            InputAtaqueMelee();
                    }
                    if (Input.GetKeyUp(inputDisparo) && atirando)
                        EncerrarDisparos();
                    break;
                default:
                    break;
            }
        }
    }
    private void FixedUpdate()
    {
         if (estadosJogador == estados.EmAcao)
         {
            rb.velocity = movimento * velocidade + baguncarControles;
         }
         else if (estadosJogador == estados.SendoEmpurrado)
         {
            //rb.AddForce(-forcaEmpurrao * direcaoEmpurrao);
            rb.velocity = -forcaEmpurrao * direcaoEmpurrao;
         }
         else
         {
            rb.velocity = Vector2.zero;
         }
    }
    private void MovimentoInput()
    {
        float movX = Input.GetAxisRaw("Horizontal");
        float movY = Input.GetAxisRaw("Vertical");
        if (TutorialSetUp.Instance != null && (movX != 0 || movY != 0))
            animacoesTutorial.SetActive(false);
        MudaAreaAtaque(movX, movY);
        movimento = new Vector2(movX, movY).normalized;//normalized faz com q o movimento seja igual para todas as direções, não passando de um limite de 1
    }
    public Vector3 PegaPosicoMouse()
    {
        Vector3 posicaoMouse = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        posicaoMouse.z = -1f;
        //Debug.Log(posicaoMouse);
        return posicaoMouse;
    }
    private void InputAtirar()//dispara ao apertar o botão direito do mouse
    {
        if (Input.GetKey(inputDisparo))
        {
            if (!atirando)
            {
                atirando = true;
                MudarEstadoJogador(1);
                Vector3 dirAnim = (PegaPosicoMouse() - transform.position).normalized;
                direcaoProjetil = (PegaPosicoMouse() - pontoDeDisparo.position).normalized;
                JogadorAnimScript.Instance.AnimarDisparo(dirAnim.x, dirAnim.y, taxaDeDisparo, atirando);
                //SoundManager.Instance.TocarSom(SoundManager.TipoSom.JogadorAtirando);
            }
        }
    }
    public void EncerrarDisparos()
    {
        atirando = false;
        JogadorAnimScript.Instance.AnimarDisparo(0f, 0f, taxaDeDisparo, atirando);
        if (TutorialSetUp.Instance != null)
        {
            if (TutorialSetUp.Instance.GetSequenciaDialogos() > 2)
                MudarEstadoJogador(0);
        }
        else
            MudarEstadoJogador(0);
    }
    public void Atira()
    {
        if (balaDisparada == null)
        {
            balaDisparada = Instantiate(projetilPrefab, pontoDeDisparo.position, Quaternion.identity);
            balaDisparada.transform.Rotate(new Vector3(0f, 0f, Mathf.Atan2(direcaoProjetil.y, direcaoProjetil.x) * Mathf.Rad2Deg));//rotaciona a bala
            balaDisparada.GetComponent<Rigidbody2D>().velocity = direcaoProjetil * velocidadeProjetil;//new Vector3(direcaoProjetil.x / Mathf.Abs(direcaoProjetil.x), direcaoProjetil.y / Mathf.Abs(direcaoProjetil.y), 0f)
            balaDisparada.GetComponent<balaHit>().SetDano(tempParalisacaoProjetil);
            balaDisparada = null;
        }
    }
    private void InputAtaqueMelee()// animação e inflinge dano caso encontre algo
    {
        if (Input.GetKeyDown(inputAtaqueMelee) && !atacando)
        {
            atacando = true;
            JogadorAnimScript.Instance.AnimarAtaqueMelee(posicaoMelee.localPosition.x, posicaoMelee.localPosition.y, taxaDeAtaqueMelee);
        }
    }
    public void ataqueMelee()
    {
        if (atacando)
        {
            Collider2D[] objetosAcertados = Physics2D.OverlapCircleAll(posicaoMelee.position, alcanceMelee, objetosAcertaveisLayer);//hit em objetos
            //if (objetosAcertados.Length == 0)
            //    SoundManager.Instance.TocarSom(SoundManager.Som.JogadorAtqMelee);
            foreach (Collider2D objeto in objetosAcertados)
            {
                if (objeto.gameObject.layer == 8)
                {
                    objeto.GetComponent<hitbox_inimigo>().inimigo.GetComponent<inimigoScript>().mudancaVida(-danoMelee, this.tag, forcaKnockbackNosInimigos, this.transform.position, duracaoKnockbakNosInimigos);
                }
                else if (objeto.gameObject.layer == 9 && !desastreManager.Instance.VerificarSeUmDesastreEstaAcontecendo())
                {
                    objeto.gameObject.GetComponentInParent<CentroDeRecurso>().RecebeuHit();
                }
            }
            atacando = false;
        }
    }
    private void InputProsseguirDialogo()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            DialogeManager.Instance.MostraProximoDialogo();
        }
    }
    public void MudaAreaAtaque(float movX, float movY)// atualiza a direção do ataque melee sempre que o jogador muda de direção
    {
        if (!atacando)
        {
            if (Input.GetButton("Horizontal")) //&& movX != 0)
            {
                posicaoMelee.localPosition =  new Vector3(movX * Mathf.Abs(distanciaAtaqueMelee.x), distanciaAtaqueMelee.y, 0f);
                pontoDeDisparo.localPosition = new Vector3(movX * Mathf.Abs(pontoDeDisparo.localPosition.x), 0f, 0f);
            } 
            if (Input.GetButton("Vertical")) //&& movY != 0)
            {
                posicaoMelee.localPosition = new Vector3(0f, movY * Mathf.Abs(distanciaAtaqueMelee.x), 0f);
                pontoDeDisparo.localPosition = new Vector3(pontoDeDisparo.localPosition.x, movY * Mathf.Abs(pontoDeDisparo.localPosition.y), 0f);
            }
        }
    }
    /*private void mousePrecionado()
    {
        Mydebug.mydebug.MyPrint("click do mouse");
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //Mydebug.mydebug.MyPrint("click do mouse");
            Vector2 mousePos = PegaPosicoMouse();
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit)
            {
                Clicavel clicavel = hit.collider.GetComponent<Clicavel>();
                clicavel?.Click(this);
            }
        }
    }*/
    public void MudarEstadoJogador(int i)
    {
        switch (i)
        {
            case 0:
                ControleManager();
                estadosJogador = estados.EmAcao;
                podeAnimar = true;
                break;
            case 1:
                ControleManager();
                estadosJogador = estados.Paralisado;
                podeAnimar = false;
                break;
            case 2:
                ControleManager();
                estadosJogador = estados.EmContrucao;
                podeAnimar = false;
                break;
            case 3:
                ControleManager();
                estadosJogador = estados.EmDialogo;
                podeAnimar = false;
                break;
            case 4:
                ControleManager();
                estadosJogador = estados.SendoEmpurrado;
                //atirando = false;
                podeAnimar = false;
                break;
            case 5:
                ControleManager();
                estadosJogador = estados.EmUI;
                podeAnimar = false;
                break;
        }
    }
    private void ControleManager()
    {
        switch ((int)estadosJogador)
        {
            case 2://em construção
                if (comportamentoCamera.GetFocoDaCamera() != this.transform /*&& BossAlho.Instance == null*/)
                    comportamentoCamera.MudaFocoCamera(transform, 0f);
                if (BaseScript.Instance != null)
                {
                    BaseScript.Instance.Ativar_DesativarVisualConstrucaoModulos(false);
                    //ClickEmObjetos.Instance.RetornarMaterialOriginal(); 
                }
                break;
            case 3://em dialogo
                if (DialogeManager.Instance.GetEstadoDialogo() && TutorialSetUp.Instance == null)
                {
                    Debug.Log("remover dialogo");
                    DialogeManager.Instance.FimDialogo();
                }
                break;
            case 5://em UI
                if (UIinventario.Instance.InventarioAberto)
                {
                    UIinventario.Instance.fecharTodoInventario();
                }
                break;
            default:
                break;
        }
    }
    public void Knockback(float duracaoEmpurrao, float forca, Transform obj)
    {
        MudarEstadoJogador(4);
        direcaoEmpurrao = (obj.transform.position - this.transform.position).normalized;
        forcaEmpurrao = forca;;
        StartCoroutine(this.duracaoKnockback(duracaoEmpurrao));
    }
    private IEnumerator duracaoKnockback(float duracaoEmpurrao)
    {
        yield return new WaitForSeconds(duracaoEmpurrao);
        MudarEstadoJogador(0);
    }
    public void mudancaRelogio(float valor, float duracaoStn)
    {
        if (atirando && estadosJogador == estados.Paralisado)
        {
            MudarEstadoJogador(0);
            atirando = false;
            JogadorAnimScript.Instance.AnimarDisparo(0f, 0f, taxaDeDisparo, atirando);
        }
        if (atacando)
        {
            atacando = false;
        }
        JogadorAnimScript.Instance.Hit(duracaoStn);
        desastreManager.Instance.AvisoDePerigoTimer();
        //if (desastreManager.Instance.VerificarSeUmDesastreEstaAcontecendo())
        //{
        //    desastreManager.Instance.MudarTempoAcumuladoParaDesastre(desastreManager.Instance.GetTempoAcumuladoParaDesastre() + Mathf.Clamp(valor, 0, desastreManager.Instance.GetIntervaloDeTempoEntreOsDesastres()));
        //}
        //else
        //{
        if (!desastreManager.Instance.VerificarSeUmDesastreEstaAcontecendo())
        {
            desastreManager.Instance.DiminuirTempoRestanteParaDesastre(valor);
            desastreManager.Instance.ConfigurarTimer(desastreManager.Instance.GetTempoRestanteParaDesastre(), 0f, false);
        }
        //}
        //vidaAtual += valor;
        //inventario.BarraDeVida.GetComponent<barraDeVida>().AtualizaBarraDeVida(vidaAtual);
        //if (vidaAtual > vidaMaxima)
        //{
        //    vidaAtual = vidaMaxima;
        //}
        //else if (vidaAtual <= 0)
        //{
        //    vidaAtual = 0f;
        //}
    }
    public void IndicarInteracaoPossivel(float botao, bool visivel)
    {
        if (visivel)
        {
            iconeInteracao.SetActive(true);
            iconeInteracao.GetComponent<Animator>().SetFloat("BOTOES", botao);
        }
        else
        {
            iconeInteracao.GetComponent<Animator>().SetFloat("BOTOES", 0f);
            iconeInteracao.SetActive(false);
        }
    }
    public void BaguncaControles(float unidadesX, float unidadesY, float forcaGeral)
    {
        baguncarControles = new Vector2(Random.Range(-unidadesX, unidadesX),Random.Range(-unidadesY, unidadesY)) * forcaGeral;
        if (baguncarControles.x == 0)
            baguncarControles.x = 1;
        if (baguncarControles.y == 0)
            baguncarControles.y = 1;
    }
    private void OnDrawGizmosSelected()
    {
        if (posicaoMelee == null)
            return;
        Gizmos.DrawWireSphere(posicaoMelee.position, alcanceMelee);
    }
    public bool GetPodeAnimar()
    {
        return podeAnimar;
    }
    public void Tutorial()
    {
        TutorialSetUp.Instance.SetupInicialJogador();
    }
    public void AoFinalizarDialogo(object origem, System.EventArgs args)
    {
        if(TutorialSetUp.Instance != null)
        {
            if (TutorialSetUp.Instance.GetSequenciaDialogos() == 1)
            {
                MudarEstadoJogador(1);
                //transform.position = TutorialSetUp.Instance.pontoDeCombateJogador.position;
                animacoesTutorial.SetActive(true);
                animacoesTutorial.GetComponent<Animator>().SetBool("DISP", true);
                JogadorAnimScript.Instance.Getanimator().SetFloat("HORIZONTAL", 1f);
                JogadorAnimScript.Instance.Getanimator().SetFloat("VERTICAL", 0f);
                //Time.timeScale = 0f;
            }
        }
        //else
            //comportamentoCamera.MudaFocoCamera(transform, 0);
    }
    public void AoLevantar()
    {
        JogadorAnimScript.Instance.Levantar(false);
        DialogeManager.Instance.DialogoFinalizado += AoFinalizarDialogo;
        TutorialSetUp.Instance.IniciarDialogo();
    }
    public void SetDirecaoDeMovimentacaoAleatoria(Vector2 vec)
    {
        baguncarControles = vec;
    }
    public ReceitaDeCrafting GetModuloConstruido()
    {
        return moduloCriado;
    }
    public void SetModuloConstruido(ReceitaDeCrafting modulo)
    {
        moduloCriado = ScriptableObject.CreateInstance<ReceitaDeCrafting>();
        moduloCriado = modulo;
    }
    public GameObject GetAnimacoesTutorial()
    {
        return animacoesTutorial;
    }
}
