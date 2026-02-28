using UnityEngine;
using TMPro;
using Neocortex.Data;
using System.Collections;

public class NPC_Response : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueTyper dialogueTyper;
    [SerializeField] private TMP_Text nameText;

    [Header("Names")]
    [SerializeField] private string npcName = "Lena";
    [SerializeField] private string playerName = "Player";

    [Header("Colors")]
    [SerializeField] private Color npcNameColor = new Color(0.85f, 0.4f, 0.9f);
    [SerializeField] private Color playerNameColor = Color.cyan;
    [SerializeField] private Color npcTextColor = Color.white;
    [SerializeField] private Color playerTextColor = Color.white;

    private void Awake()
    {
        if (nameText == null)
            nameText = GetComponentInChildren<TMP_Text>(true);

        if (nameText != null)
            nameText.gameObject.SetActive(false);
    }

    //Player voice input

    public void SendPlayerText(string playerText)
    {
        if (string.IsNullOrEmpty(playerText)) return;

        HideName(); // hide name for player
        dialogueTyper.SetTextColor(playerTextColor);
        dialogueTyper.ShowDialogueUI();
        dialogueTyper.StartDialogue(new string[] { playerText });

        StartCoroutine(AutoHideRoutine());
    }

    //NPC Response

    public void SendNPCResponse(ChatResponse response)
    {
        if (response == null || string.IsNullOrEmpty(response.message))
            return;

        ShowName(npcName, npcNameColor); // show name for NPC
        dialogueTyper.SetTextColor(npcTextColor);
        dialogueTyper.ShowDialogueUI();
        dialogueTyper.StartDialogue(new string[] { response.message });

        StartCoroutine(AutoHideRoutine());
    }

    

    private void ShowName(string value, Color color)
    {
        if (nameText == null) return;

        nameText.text = value;
        nameText.color = color;
        nameText.gameObject.SetActive(true);
    }

    public void HideName()
    {
        if (nameText != null)
            nameText.gameObject.SetActive(false);
    }

 

    private IEnumerator AutoHideRoutine()
    {
        if (dialogueTyper == null) yield break;

        // wait until typing finished
        yield return new WaitUntil(() => dialogueTyper.IsDialogueFinished);

        // small delay optional
        yield return new WaitForSeconds(0.2f);

        // hide UI
        dialogueTyper.HideDialogueUI();
        HideName();
    }
}
