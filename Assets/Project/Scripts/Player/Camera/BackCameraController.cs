using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Câmera traseira exibida como um painel na HUD.
/// Renderiza o que está ATRÁS do jogador em uma RenderTexture
/// que é exibida por um RawImage na HUD — dando um efeito de "espelho traseiro" de horror.
///
/// ════════════════════════════════════════════════════════════════
/// COMO CONFIGURAR — PASSO A PASSO
/// ════════════════════════════════════════════════════════════════
///
/// ── CRIANDO A RENDER TEXTURE ──
/// 1. No Project, clique com botão direito > Create > Render Texture.
/// 2. Nomeie como "RT_BackCamera".
/// 3. Tamanho recomendado: 256x256 ou 512x512 (menor = melhor performance).
///
/// ── CRIANDO A CÂMERA TRASEIRA (na Hierarquia) ──
/// 4. Selecione o GameObject "Player" na Hierarquia.
/// 5. Clique com botão direito > Create Empty > renomeie para "BackCameraHolder".
/// 6. No Transform do "BackCameraHolder", coloque:
///      Position: X=0, Y=1.6, Z=0   (ou ajuste para a altura da cabeça)
///      Rotation: X=0,  Y=180, Z=0   ← isso faz ela apontar para TRÁS
/// 7. Com "BackCameraHolder" selecionado, clique com botão direito >
///    Camera — isso cria uma câmera filha.
/// 8. Renomeie a câmera filha para "BackCamera".
/// 9. No componente Camera:
///      - Target Texture: arraste a "RT_BackCamera" aqui
///      - Field of View: 80 (ou o valor que preferir)
///      - Clipping Planes Near: 0.1, Far: 50
///      - Clear Flags: Skybox
///      - Culling Mask: Everything (ou o que fizer sentido para o seu jogo)
///      - Depth: -2  (valor menor que a câmera principal para não sobrepor)
///
/// ── CRIANDO O PAINEL NA HUD ──
/// 10. No Canvas da HUD, crie um painel: botão direito > UI > Panel.
///     Renomeie para "BackCameraPanel".
///     Posicione-o abaixo da barra de vida (ajuste o RectTransform).
/// 11. Dentro do "BackCameraPanel", crie um RawImage:
///     botão direito em "BackCameraPanel" > UI > Raw Image.
///     Renomeie para "BackCameraImage".
///     - No componente Raw Image, no campo "Texture": arraste a "RT_BackCamera".
///     - Ajuste o RectTransform para preencher o painel (Anchor = Stretch/Stretch).
/// 12. (Opcional) Adicione uma borda: crie uma Image por cima do RawImage com
///     a mesma largura e uma sprite de moldura.
///
/// ── ADICIONANDO O SCRIPT ──
/// 13. Selecione o GameObject "BackCameraHolder" (ou "Player").
/// 14. Arraste o script "BackCameraController" para ele.
/// 15. No Inspector, preencha os campos:
///     • Back Camera     → arraste a câmera "BackCamera"
///     • Camera Panel    → arraste o "BackCameraPanel"
///     • Player Transform → arraste o Transform do "Player" (normalmente já é o pai)
///
/// ── OPCIONAIS ──
/// 16. "Show On Start": marque se quiser que o painel apareça logo ao iniciar.
/// 17. "Toggle Key": tecla para ativar/desativar o painel durante o jogo (ex: F).
/// ════════════════════════════════════════════════════════════════
/// </summary>
public class BackCameraController : MonoBehaviour
{
    [Header("Referências obrigatórias")]
    [Tooltip("A câmera filha do Player que aponta para trás (BackCamera).")]
    [SerializeField] private Camera backCamera;

    [Tooltip("O painel da HUD que contém o RawImage com a RT_BackCamera.")]
    [SerializeField] private GameObject cameraPanel;

    [Tooltip("Transform do Player (usado para posicionar a câmera corretamente). " +
             "Se deixar vazio, tenta pegar o pai deste GameObject.")]
    [SerializeField] private Transform playerTransform;

    [Header("Comportamento")]
    [Tooltip("Se true, o painel da câmera traseira aparece logo ao iniciar.")]
    [SerializeField] private bool showOnStart = true;

    [Tooltip("Tecla para ativar/desativar o painel durante o jogo (ex: F). " +
             "Deixe 'None' se não quiser atalho.")]
    [SerializeField] private KeyCode toggleKey = KeyCode.None;

    // ──────────────────────────────────────────────────────────────────

    private bool _isVisible;

    private void Awake()
    {
        // Fallback: usa o pai como playerTransform
        if (playerTransform == null)
            playerTransform = transform.parent != null ? transform.parent : transform;
    }

    private void Start()
    {
        if (backCamera == null)
        {
            Debug.LogError("[BackCameraController] 'Back Camera' NÃO atribuída no Inspector!", this);
            return;
        }

        if (cameraPanel == null)
        {
            Debug.LogError("[BackCameraController] 'Camera Panel' NÃO atribuído no Inspector!", this);
            return;
        }

        SetVisible(showOnStart);
    }

    private void Update()
    {
        // Alternância manual por tecla (opcional)
        if (toggleKey != KeyCode.None && Input.GetKeyDown(toggleKey))
        {
            SetVisible(!_isVisible);
        }
    }

    // ──────────────────────── API PÚBLICA ──────────────────────────────

    /// <summary>Ativa ou desativa o painel da câmera traseira.</summary>
    public void SetVisible(bool visible)
    {
        _isVisible = visible;

        if (cameraPanel != null)
            cameraPanel.SetActive(visible);

        if (backCamera != null)
            backCamera.enabled = visible;

        Debug.Log("[BackCameraController] Painel traseiro " + (visible ? "ATIVADO" : "DESATIVADO"));
    }

    /// <summary>Retorna true se o painel estiver visível.</summary>
    public bool IsVisible => _isVisible;
}
