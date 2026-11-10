using UnityEngine;

/// <summary>
/// Dados persistentes do jogo (moedas, progresso, skins e configurações).
/// Tudo é salvo em PlayerPrefs e gravado em disco a cada mudança.
/// Para a gameplay: GameData.AdicionarMoedas(n) ao coletar estrela,
/// GameData.CompletarFase(n) ao chegar na alga final.
/// </summary>
public static class GameData
{
    public struct SkinInfo
    {
        public string nome;
        public int preco;
        public Color cor; // tint aplicado no sprite da Kuro
        public SkinInfo(string n, int p, Color c) { nome = n; preco = p; cor = c; }
    }

    public const int TotalFases = 2;

    public static readonly SkinInfo[] Skins =
    {
        new SkinInfo("Kuro Original",    0,  Color.white),
        new SkinInfo("Glaucus Azul",     10, new Color(0.45f, 0.70f, 1.00f)),
        new SkinInfo("Hypselodoris Rosa",20, new Color(1.00f, 0.55f, 0.80f)),
        new SkinInfo("Flabellina Laranja",30,new Color(1.00f, 0.65f, 0.30f)),
        new SkinInfo("Chromodoris Roxa", 40, new Color(0.75f, 0.50f, 1.00f)),
    };

    const string K_MOEDAS = "Moedas";
    const string K_FASE = "FaseDesbloqueada";
    const string K_JOY = "ControleJoystick";
    const string K_MUS = "VolMusica";
    const string K_SFX = "VolSfx";
    const string K_SKINS = "SkinsCompradas";
    const string K_SKIN = "SkinSelecionada";

    // ---------- Moedas ----------
    public static int Moedas
    {
        get { return PlayerPrefs.GetInt(K_MOEDAS, 0); }
        private set { PlayerPrefs.SetInt(K_MOEDAS, Mathf.Max(0, value)); }
    }

    public static void AdicionarMoedas(int n) { Moedas = Moedas + n; Salvar(); }

    // ---------- Progresso ----------
    /// <summary>Maior fase liberada (começa em 1).</summary>
    public static int FaseDesbloqueada
    {
        get { return Mathf.Clamp(PlayerPrefs.GetInt(K_FASE, 1), 1, TotalFases); }
    }

    public static bool FaseLiberada(int fase) { return fase <= FaseDesbloqueada; }

    public static void CompletarFase(int fase)
    {
        if (fase >= FaseDesbloqueada && fase < TotalFases)
        {
            PlayerPrefs.SetInt(K_FASE, fase + 1);
            Salvar();
        }
    }

    // ---------- Skins ----------
    public static bool SkinComprada(int i)
    {
        return i == 0 || (PlayerPrefs.GetInt(K_SKINS, 0) & (1 << i)) != 0;
    }

    public static int SkinSelecionada
    {
        get { int s = PlayerPrefs.GetInt(K_SKIN, 0); return SkinComprada(s) ? s : 0; }
    }

    public static bool ComprarSkin(int i)
    {
        if (i < 0 || i >= Skins.Length || SkinComprada(i)) return false;
        if (Moedas < Skins[i].preco) return false;
        Moedas = Moedas - Skins[i].preco;
        PlayerPrefs.SetInt(K_SKINS, PlayerPrefs.GetInt(K_SKINS, 0) | (1 << i));
        Salvar();
        return true;
    }

    public static void EquiparSkin(int i)
    {
        if (!SkinComprada(i)) return;
        PlayerPrefs.SetInt(K_SKIN, i);
        Salvar();
    }

    // ---------- Configurações ----------
    /// <summary>true = joystick virtual; false = botões (setas).</summary>
    public static bool ControleJoystick
    {
        get { return PlayerPrefs.GetInt(K_JOY, 1) == 1; }
        set { PlayerPrefs.SetInt(K_JOY, value ? 1 : 0); Salvar(); }
    }

    public static float VolMusica
    {
        get { return PlayerPrefs.GetFloat(K_MUS, 0.7f); }
        set { PlayerPrefs.SetFloat(K_MUS, Mathf.Clamp01(value)); }
    }

    public static float VolSfx
    {
        get { return PlayerPrefs.GetFloat(K_SFX, 0.7f); }
        set { PlayerPrefs.SetFloat(K_SFX, Mathf.Clamp01(value)); }
    }

    public static void Salvar() { PlayerPrefs.Save(); }
}
