using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.XR;

public class DialogueTyper : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text nameText;   

    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject dialogueIndicatorImage;

    [Header("Speaker")]
    [SerializeField] private string speakerName;
    [Header("Name UI")]
    [SerializeField] private GameObject nameTextField;


    public void HideName()
    {
        if (nameTextField != null)
            nameTextField.SetActive(false);
    }

    public void ShowName()
    {
        if (nameTextField != null)
            nameTextField.SetActive(true);
    }




    [Header("Typing")]
    [SerializeField] private float typingSpeed = 0.04f;

    private string[] dialogues;
    private AudioClip[] audioClips;

    private int index;
    private bool canContinue;
    private bool lastBState;

    public bool IsDialogueFinished { get; private set; }

    [SerializeField] private GameObject dialogueRoot;
    // assign the parent UI object in Inspector

    public void HideDialogueUI()
    {
        if (dialogueRoot != null)
            dialogueRoot.SetActive(false);
    }

    public void ShowDialogueUI()
    {
        if (dialogueRoot != null)
            dialogueRoot.SetActive(true);
    }



    public void StartDialogue(string[] dialogueArray)
    {
        StartDialogue(dialogueArray, null);
    }

    public void StartDialogue(string[] dialogueArray, AudioClip[] clips)
    {
        dialogues = dialogueArray;
        audioClips = clips;
        index = 0;
        IsDialogueFinished = false;

        dialogueText.text = "";

        if (nameText != null)
            nameText.text = speakerName;

        if (dialogueIndicatorImage != null)
            dialogueIndicatorImage.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(TypeDialogue());
    }

    /* ---------------- UPDATE ---------------- */

    void Update()
    {
        if (!canContinue || IsDialogueFinished) return;

        if (IsBPressed())
            NextDialogue();
    }

    /* ---------------- CORE ---------------- */

    IEnumerator TypeDialogue()
    {
        canContinue = false;
        dialogueText.text = "";

        if (audioSource != null &&
            audioClips != null &&
            index < audioClips.Length &&
            audioClips[index] != null)
        {
            audioSource.clip = audioClips[index];
            audioSource.Play();
        }

        foreach (char c in dialogues[index])
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        if (audioSource != null && audioSource.isPlaying)
            yield return new WaitWhile(() => audioSource.isPlaying);

        canContinue = true;
    }

    void NextDialogue()
    {
        index++;

        if (index >= dialogues.Length)
        {
            dialogueText.text = "";

            if (dialogueIndicatorImage != null)
                dialogueIndicatorImage.SetActive(false);

            IsDialogueFinished = true;
            return;
        }

        StopAllCoroutines();
        StartCoroutine(TypeDialogue());
    }

    /* ---------------- VR INPUT ---------------- */

    bool IsBPressed()
    {
        InputDevice rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        if (!rightHand.isValid) return false;

        bool pressed;
        rightHand.TryGetFeatureValue(CommonUsages.secondaryButton, out pressed);

        bool result = pressed && !lastBState;
        lastBState = pressed;

        return result;
    }
}
