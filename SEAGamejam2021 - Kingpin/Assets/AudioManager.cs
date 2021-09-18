using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
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

    public void PlaySFX(string targetEvent, GameObject targetGO)
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(targetEvent, targetGO);
    }
}
