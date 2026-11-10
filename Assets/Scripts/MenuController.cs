using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Controla o menu principal: Jogar (Loja / Fases), Configurações e Sair.
/// Os botões são ligados por código em Awake, só é preciso preencher as referências.
/// </summary>
public class MenuController : MonoBehaviour
{
    [Header("Painéis")]
    public GameObject painelPrincipal;
    public GameObject painelJogar, painelConfig, painelFases, painelLoja;

    [Header("Principal")]
    public Button btnJogar;
    public Button btnConfig, btnSair;

    [Header("Jogar")]
    public Button btnLoja;
    public Button btnFases, btnJogarVoltar;

    [Header("Configurações")]
    public Button btnControleBotoes;
    public Button btnControleJoystick, btnConfigVoltar;
    public Slider sliderMusica, sliderSfx;

    [Header("Fases")]
    public Button btnFase1;
    public Button btnFase2, btnFasesVoltar;
    public TMP_Text txtFase2Estado;
    public string cenaFase1 = "Fase1", cenaFase2 = "Fase2";

    [Header("Loja")]
    public Button btnLojaVoltar;
    public Button btnDebugMoedas;
    public TMP_Text txtMoedas;
    public RectTransform conteudoLoja;
    public Sprite spriteSkin;
    public TMP_FontAsset fonte;

    [Header("Cores")]
    public Color corSelecionado = new Color(0.25f, 0.8f, 0.55f, 1f);
    public Color corNormal = new Color(1f, 1f, 1f, 0.85f);

    GameObject[] todosPaineis;

    void Awake()
    {
        Time.timeScale = 1f;
        todosPaineis = new[] { painelPrincipal, painelJogar, painelConfig, painelFases, painelLoja };

        btnJogar.onClick.AddListener(() => Mostrar(painelJogar));
        btnConfig.onClick.AddListener(() => { AtualizarConfig(); Mostrar(painelConfig); });
        btnSair.onClick.AddListener(Sair);

        btnLoja.onClick.AddListener(() => { AtualizarLoja(); Mostrar(painelLoja); });
        btnFases.onClick.AddListener(() => { AtualizarFases(); Mostrar(painelFases); });
        btnJogarVoltar.onClick.AddListener(() => Mostrar(painelPrincipal));

        btnControleBotoes.onClick.AddListener(() => { GameData.ControleJoystick = false; AtualizarConfig(); });
        btnControleJoystick.onClick.AddListener(() => { GameData.ControleJoystick = true; AtualizarConfig(); });
        sliderMusica.onValueChanged.AddListener(v => { GameData.VolMusica = v; AplicarAudio(); });
        sliderSfx.onValueChanged.AddListener(v => { GameData.VolSfx = v; AplicarAudio(); });
        btnConfigVoltar.onClick.AddListener(() => { GameData.Salvar(); Mostrar(painelPrincipal); });

        btnFase1.onClick.AddListener(() => AbrirFase(1, cenaFase1));
        btnFase2.onClick.AddListener(() => AbrirFase(2, cenaFase2));
        btnFasesVoltar.onClick.AddListener(() => Mostrar(painelJogar));

        btnLojaVoltar.onClick.AddListener(() => Mostrar(painelJogar));
        if (btnDebugMoedas != null)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            btnDebugMoedas.onClick.AddListener(() => { GameData.AdicionarMoedas(10); AtualizarLoja(); });
#else
            btnDebugMoedas.gameObject.SetActive(false);
#endif
        }

        Mostrar(painelPrincipal);
    }

    void Update()
    {
        // Botão voltar do Android / ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (painelFases.activeSelf || painelLoja.activeSelf) Mostrar(painelJogar);
            else if (painelJogar.activeSelf || painelConfig.activeSelf) Mostrar(painelPrincipal);
        }
    }

    void Mostrar(GameObject alvo)
    {
        foreach (var p in todosPaineis) p.SetActive(p == alvo);
    }

    // ---------- Sair ----------
    void Sair()
    {
        GameData.Salvar(); // salvamento automático
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ---------- Configurações ----------
    void AtualizarConfig()
    {
        bool joy = GameData.ControleJoystick;
        btnControleJoystick.image.color = joy ? corSelecionado : corNormal;
        btnControleBotoes.image.color = joy ? corNormal : corSelecionado;
        sliderMusica.SetValueWithoutNotify(GameData.VolMusica);
        sliderSfx.SetValueWithoutNotify(GameData.VolSfx);
    }

    void AplicarAudio()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.AplicarVolumes();
    }

    // ---------- Fases ----------
    void AtualizarFases()
    {
        bool lib = GameData.FaseLiberada(2);
        btnFase2.interactable = lib;
        if (txtFase2Estado != null)
            txtFase2Estado.text = lib ? "Fase 2" : "Fase 2\n<size=60%>Conclua a Fase 1</size>";
    }

    void AbrirFase(int fase, string cena)
    {
        if (!GameData.FaseLiberada(fase)) return;
        GameData.Salvar();
        SceneManager.LoadScene(cena);
    }

    // ---------- Loja ----------
    void AtualizarLoja()
    {
        txtMoedas.text = "Estrelas: " + GameData.Moedas;
        for (int i = conteudoLoja.childCount - 1; i >= 0; i--)
            Destroy(conteudoLoja.GetChild(i).gameObject);
        for (int i = 0; i < GameData.Skins.Length; i++) CriarCartao(i);
    }

    void CriarCartao(int i)
    {
        var skin = GameData.Skins[i];
        bool comprada = GameData.SkinComprada(i);
        bool equipada = GameData.SkinSelecionada == i;

        var card = Novo("Skin_" + i, conteudoLoja, typeof(Image), typeof(LayoutElement));
        card.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.55f);
        var le = card.GetComponent<LayoutElement>();
        le.preferredWidth = 300; le.preferredHeight = 460;

        var prev = Novo("Preview", card.transform, typeof(Image));
        Ancorar(prev, 0.1f, 0.40f, 0.9f, 0.92f);
        var img = prev.GetComponent<Image>();
        img.sprite = spriteSkin; img.preserveAspect = true; img.color = skin.cor;

        var nome = NovoTexto("Nome", card.transform, skin.nome, 30, Color.white);
        Ancorar(nome.gameObject, 0.05f, 0.27f, 0.95f, 0.40f);

        var btn = Novo("Botao", card.transform, typeof(Image), typeof(Button));
        Ancorar(btn, 0.1f, 0.04f, 0.9f, 0.23f);
        var b = btn.GetComponent<Button>();
        btn.GetComponent<Image>().color = equipada ? corSelecionado : Color.white;
        string rotulo;
        if (equipada) { rotulo = "Equipada"; b.interactable = false; }
        else if (comprada) { rotulo = "Equipar"; }
        else { rotulo = "Comprar (" + skin.preco + ")"; b.interactable = GameData.Moedas >= skin.preco; }
        NovoTexto("Texto", btn.transform, rotulo, 30, Color.black).rectTransform.anchorMax = Vector2.one;

        int idx = i;
        b.onClick.AddListener(() =>
        {
            if (!GameData.SkinComprada(idx)) { if (!GameData.ComprarSkin(idx)) return; }
            GameData.EquiparSkin(idx);
            AtualizarLoja();
        });
    }

    GameObject Novo(string nome, Transform pai, params System.Type[] comps)
    {
        var go = new GameObject(nome, typeof(RectTransform));
        foreach (var c in comps) go.AddComponent(c);
        go.transform.SetParent(pai, false);
        return go;
    }

    TMP_Text NovoTexto(string nome, Transform pai, string txt, float tam, Color cor)
    {
        var go = Novo(nome, pai, typeof(TextMeshProUGUI));
        var t = go.GetComponent<TextMeshProUGUI>();
        t.text = txt; t.fontSize = tam; t.color = cor; t.alignment = TextAlignmentOptions.Center;
        t.enableWordWrapping = true;
        if (fonte != null) t.font = fonte;
        var rt = t.rectTransform;
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.offsetMin = rt.offsetMax = Vector2.zero;
        return t;
    }

    void Ancorar(GameObject go, float x0, float y0, float x1, float y1)
    {
        var rt = (RectTransform)go.transform;
        rt.anchorMin = new Vector2(x0, y0); rt.anchorMax = new Vector2(x1, y1);
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }
}
