using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Player SFX Events")]
    [FMODUnity.EventRef]
    public string PlayerHurtEvent = "";

    [FMODUnity.EventRef]
    public string PlayerPunchEvent = "";

    [Header("Pin SFX Events")]
    [FMODUnity.EventRef]
    public string PinAttackEvent = "";

    [FMODUnity.EventRef]
    public string GiantPinAttackEvent = "";

    [FMODUnity.EventRef]
    public string PinHurtEvent = "";

    [FMODUnity.EventRef]
    public string PinBowlingStrikeEvent = "";

    [Header("Dialogue/Voice SFX Events")]
    [FMODUnity.EventRef]
    public string AnikiEvent = "";

    [FMODUnity.EventRef]
    public string SussyBakaEvent = "";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void PlaySFX(string targetEvent, GameObject targetGO)
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(targetEvent, targetGO);
    }
}
