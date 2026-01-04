using UnityEngine;


[System.Serializable]
public class Vital{
    public string Name;

    public float Current { get; private set; }
    public float Max { get; private set; }

    public float RegenRate { get; private set; }
    public float RegenDelay { get; private set; }

    private float regenTimer;
    private bool regenPaused;

    public event System.Action<float, float> OnChanged; // current, max

    public Vital(string name, float max, float regenRate, float regenDelay) {
        Name = name;
        Max = max;
        Current = max;
        RegenRate = regenRate;
        RegenDelay = regenDelay;
    }


    public void Tick(float deltaTime){
        if (regenPaused) return;

        regenTimer += deltaTime;
        if (regenTimer < RegenDelay) return;

        Modify(RegenRate * deltaTime);
    }

    public void Spend(float amount)
    {
        Modify(-amount);
        PauseRegen();
    }
    public void Modify(float amount){
        float prev = Current;
        Current = Mathf.Clamp(Current + amount, 0, Max);

        if (!Mathf.Approximately(prev, Current))
            OnChanged?.Invoke(Current, Max);
    }

    public void PauseRegen()
    {
        regenTimer = 0f;
    }

    public bool IsEmpty => Current <= 0;
    public bool IsFull => Current >= Max;
}
