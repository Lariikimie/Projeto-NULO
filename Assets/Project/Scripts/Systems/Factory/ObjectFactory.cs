using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Factory pattern implementation for spawning registered game object prefabs.
/// Register prefabs by ID at design time and instantiate them at runtime via
/// <see cref="Create"/>.
/// </summary>
public class ObjectFactory : MonoBehaviour
{
    public static ObjectFactory Instance { get; private set; }

    [System.Serializable]
    public class PrefabEntry
    {
        [Tooltip("Identificador único do prefab. Ex: \"Enemy\", \"Item_Key\"")]
        public string id;
        public GameObject prefab;
    }

    [Header("Banco de Prefabs")]
    [SerializeField] private List<PrefabEntry> prefabEntries = new List<PrefabEntry>();

    private Dictionary<string, GameObject> prefabDict = new Dictionary<string, GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (var entry in prefabEntries)
        {
            if (entry == null)
            {
                Debug.LogWarning("[ObjectFactory] Entrada nula encontrada na lista de prefabs.");
                continue;
            }

            if (string.IsNullOrEmpty(entry.id))
            {
                Debug.LogWarning("[ObjectFactory] Entrada com ID vazio ignorada.");
                continue;
            }

            if (entry.prefab == null)
            {
                Debug.LogWarning($"[ObjectFactory] Prefab nulo para o ID \"{entry.id}\" ignorado.");
                continue;
            }

            if (prefabDict.ContainsKey(entry.id))
            {
                Debug.LogWarning($"[ObjectFactory] ID duplicado ignorado: {entry.id}");
                continue;
            }

            prefabDict.Add(entry.id, entry.prefab);
        }

        Debug.Log($"[ObjectFactory] Inicializado. Prefabs registrados: {prefabDict.Count}");
    }

    /// <summary>
    /// Instantiates the prefab registered under <paramref name="id"/> at the given
    /// position and rotation.
    /// </summary>
    /// <param name="id">Registered prefab ID.</param>
    /// <param name="position">World position for the new instance.</param>
    /// <param name="rotation">World rotation for the new instance.</param>
    /// <returns>The instantiated <see cref="GameObject"/>, or <c>null</c> if the ID is not found.</returns>
    public GameObject Create(string id, Vector3 position, Quaternion rotation)
    {
        if (!prefabDict.TryGetValue(id, out GameObject prefab))
        {
            Debug.LogWarning($"[ObjectFactory] Prefab não encontrado: {id}");
            return null;
        }

        GameObject instance = Instantiate(prefab, position, rotation);
#if UNITY_EDITOR
        Debug.Log($"[ObjectFactory] Criado: {id} em {position}");
#endif
        return instance;
    }

    /// <summary>
    /// Instantiates the prefab registered under <paramref name="id"/> at the origin
    /// with no rotation.
    /// </summary>
    public GameObject Create(string id)
    {
        return Create(id, Vector3.zero, Quaternion.identity);
    }

    /// <summary>Returns true if a prefab with the given ID is registered.</summary>
    public bool HasPrefab(string id) => prefabDict.ContainsKey(id);
}
