using System.Collections.Generic;
using UnityEngine;

// Este script guarda o invent�rio l�gico do jogador:
// - Quantidade de pilhas
// - Lista de chaves
// - Lista de notas (NoteData)
public class PlayerInventory : MonoBehaviour
{
    [Header("Pilhas (Bateria extra)")]
    [SerializeField] private int batteryCount = 0;

    [Header("Chaves")]
    [SerializeField] private List<string> keys = new List<string>();

    [Header("Notas (Di�rio)")]
    [SerializeField] private List<NoteData> notes = new List<NoteData>();

    /// <summary>
    /// Disparado sempre que uma nova nota é adicionada ao inventário.
    /// Assine este evento para reagir (ex.: trocar textura na nota 3D).
    /// </summary>
    public event System.Action<NoteData> OnNoteAdded;

    // ==== PILHAS =====================================================

    public void AddBattery(int amount)
    {
        batteryCount += amount;
        if (batteryCount < 0)
            batteryCount = 0;

        Debug.Log("[Invent�rio] Pilhas: " + batteryCount);
    }

    public bool UseBattery(int amount = 1)
    {
        if (batteryCount >= amount)
        {
            batteryCount -= amount;
            Debug.Log("[Invent�rio] Usou pilha. Restam: " + batteryCount);
            return true;
        }

        Debug.Log("[Invent�rio] Tentou usar pilha, mas n�o tem suficientes.");
        return false;
    }

    public int GetBatteryCount()
    {
        return batteryCount;
    }

    // ==== CHAVES =====================================================

    public void AddKey(string keyId)
    {
        if (!keys.Contains(keyId))
        {
            keys.Add(keyId);
            Debug.Log("[Invent�rio] Pegou chave: " + keyId);
        }
        else
        {
            Debug.Log("[Invent�rio] J� tinha a chave: " + keyId);
        }
    }

    public bool HasKey(string keyId)
    {
        return keys.Contains(keyId);
    }

    public List<string> GetAllKeys()
    {
        return keys;
    }

    // ==== NOTAS ======================================================

    public void AddNote(NoteData note)
    {
        if (note == null)
        {
            Debug.LogWarning("[Invent�rio] Tentou adicionar nota nula.");
            return;
        }

        if (!notes.Contains(note))
        {
            notes.Add(note);
            Debug.Log("[Invent�rio] Pegou nota: " + note.title);
            OnNoteAdded?.Invoke(note);
        }
        else
        {
            Debug.Log("[Invent�rio] J� tinha essa nota: " + note.title);
        }
    }

    public List<NoteData> GetAllNotes()
    {
        return notes;
    }

    // ==== CARREGAR ESTADO (USADO PELO CHECKPOINT) =====================

    /// <summary>
    /// Carrega um estado completo de invent�rio.
    /// Usado pelo CheckpointManager ao dar Respawn.
    /// </summary>
    public void LoadInventoryState(int newBatteryCount, List<string> newKeys, List<NoteData> newNotes)
    {
        // Pilhas
        batteryCount = Mathf.Max(0, newBatteryCount);

        // Chaves
        keys.Clear();
        if (newKeys != null)
            keys.AddRange(newKeys);

        // Notas
        notes.Clear();
        if (newNotes != null)
            notes.AddRange(newNotes);

        Debug.Log("[Invent�rio] Estado restaurado pelo Checkpoint.");
    }
}
