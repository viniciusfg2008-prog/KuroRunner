using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>HUD, pausa, vitória e derrota da fase.</summary>
public class GerenciadorFase : MonoBehaviour
{
    public static GerenciadorFase Instance;

    public int numeroFase = 1;
    public string proximaCena = "Fase2";
    public string cenaMenu = "Menu";

    public TMP_Text txtVidas, txtMoedas, txtResultado;
    public GameObject painelVitoria, painelDerrota, painelPausa;
    public GameObject joystickRoot, botoesDirecaoRoot;

    bool pausado, fim;

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
        if (painelVitoria) painelVitoria.SetActive(false);
        if (painelDerrota) painelDerrota.SetActive(false);
        if (painelPausa) painelPausa.SetActive(false);
        bool joy = GameData.ControleJoystick;
        if (joystickRoot) joystickRoot.SetActive(joy);
        if (botoesDirecaoRoot) botoesDirecaoRoot.SetActive(!joy);
    }

    void OnDestroy() { Time.timeScale = 1f; }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !fim) AlternarPausa();
    }

    public static void Atualizar(int vidas, int moedas)
    {
        if (Instance == null) return;
        if (Instance.txtVidas) Instance.txtVidas.text = "Vidas: " + vidas;
        if (Instance.txtMoedas) Instance.txtMoedas.text = "Estrelas: " + moedas;
    }

    public void AlternarPausa()
    {
        if (fim) return;
        pausado = !pausado;
        Time.timeScale = pausado ? 0f : 1f;
        if (painelPausa) painelPausa.SetActive(pausado);
    }

    public void Vitoria(int moedas)
    {
        if (fim) return;
        fim = true;
        GameData.AdicionarMoedas(moedas);
        GameData.CompletarFase(numeroFase);
        if (txtResultado) txtResultado.text = "Fase concluída!\nEstrelas: " + moedas;
        if (painelVitoria) painelVitoria.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Derrota(int moedas)
    {
        if (fim) return;
        fim = true;
        if (painelDerrota) painelDerrota.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Reiniciar() { Time.timeScale = 1f; SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void ProximaFase() { Time.timeScale = 1f; SceneManager.LoadScene(proximaCena); }
    public void VoltarMenu() { Time.timeScale = 1f; SceneManager.LoadScene(cenaMenu); }
}
