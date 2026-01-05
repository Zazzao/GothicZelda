using UnityEngine;
using UnityEngine.UI;

public class StaminaBarUI : MonoBehaviour
{
    [SerializeField]private Image staminiaBarIamge;

    private Vital stamina;

    private void Awake()
    {
        staminiaBarIamge.fillAmount = 0.3f;
    }

    private void OnEnable()
    {
        // Subscribe to the event when this object is enabled
        //PlayerHealth.OnPlayerDeath += DisplayGameOver;
    }

    private void OnDisable()
    {
        // Crucial: Unsubscribe to prevent memory leaks or errors when the object is disabled or destroyed
        //PlayerHealth.OnPlayerDeath -= DisplayGameOver;
    }

    public void Bind(Vital staminaVital)
    {

        // Unbind first (safety)
        if (stamina != null) stamina.OnChanged -= OnStaminaChanged;

        stamina = staminaVital;
        stamina.OnChanged += OnStaminaChanged;

        // Initial sync
        OnStaminaChanged(stamina.Current, stamina.Max);
    }

    private void OnDestroy()
    {
        if (stamina != null)
            stamina.OnChanged -= OnStaminaChanged;
    }

    private void OnStaminaChanged(float current, float max)
    {
        staminiaBarIamge.fillAmount = current/ max;
    }

}



