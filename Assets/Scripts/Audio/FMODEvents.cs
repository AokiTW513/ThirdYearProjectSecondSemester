using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    public static FMODEvents instance { get; private set; }

    [field: Header("SFX")]
    [field: SerializeField] public EventReference hitSFX { get; private set; }
    [field: SerializeField] public EventReference parrySFX { get; private set; }
    [field: SerializeField] public EventReference chargeSFX { get; private set; }
    [field: SerializeField] public EventReference dashSFX { get; private set; }
    [field: SerializeField] public EventReference getItemSFX { get; private set; }
    [field: SerializeField] public EventReference countDownSFX { get; private set; }

    [field: Header("BGM")]
    [field: SerializeField] public EventReference BGM { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Found another FMODEvents in this scene.");
        }
        instance = this;
    }
}