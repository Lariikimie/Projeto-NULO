using UnityEngine;

/// <summary>
/// Trava o cursor no centro da tela durante o jogo.
/// ESC libera o cursor (útil para sair no editor ou abrir menus).
/// Qualquer clique com o mouse retrava o cursor.
///
/// COMO CONFIGURAR:
/// 1. Crie um GameObject vazio na raiz da Hierarquia (ex: "CursorManager").
/// 2. Arraste este script para esse GameObject.
/// 3. Ajuste os parâmetros no Inspector conforme necessário.
/// 4. Certifique-se de que NÃO há outro script travando/destravando o cursor
///    ao mesmo tempo (verifique o PlayerMovement se ele tiver HandleCursorLock).
/// </summary>
public class LockCursor : MonoBehaviour
{
    [Header("Configurações")]
    [Tooltip("Se true, o cursor é travado automaticamente ao iniciar o jogo/cena.")]
    [SerializeField] private bool lockOnStart = true;

    [Tooltip("Tecla que LIBERA o cursor (útil para pausar ou sair do editor).")]
    [SerializeField] private KeyCode releaseKey = KeyCode.Escape;

    [Tooltip("Se true, qualquer clique do mouse retrava o cursor enquanto ele estiver solto.")]
    [SerializeField] private bool relockOnClick = true;

    // ──────────────────────────────────────────────────────────────────

    private void Start()
    {
        if (lockOnStart)
            Lock();
    }

    private void Update()
    {
        // Solta o cursor ao pressionar a tecla de release
        if (Input.GetKeyDown(releaseKey) && Cursor.lockState == CursorLockMode.Locked)
        {
            Unlock();
        }

        // Retrava o cursor com qualquer clique do mouse (botão 0, 1 ou 2)
        if (relockOnClick && Cursor.lockState != CursorLockMode.Locked)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            {
                Lock();
            }
        }
    }

    // ──────────────────────── API PÚBLICA ──────────────────────────────

    /// <summary>Trava o cursor no centro da tela e o torna invisível.</summary>
    public void Lock()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>Libera o cursor e o torna visível (para menus/pause).</summary>
    public void Unlock()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>Retorna true se o cursor está atualmente travado.</summary>
    public bool IsLocked => Cursor.lockState == CursorLockMode.Locked;
}
