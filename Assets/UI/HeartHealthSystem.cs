
using System;
using System.Collections.Generic;
using UnityEngine;

public class HeartHealthSystem{

    public const int MAX_FRAGMENT_AMOUNT = 4;
    public event EventHandler OnDamage;
    public event EventHandler OnHeal;
    public event EventHandler OnDead;

    private List<Heart> heartList;

    public HeartHealthSystem(int hearthAmount) { 
        heartList = new List<Heart>();
        for (int i = 0; i < hearthAmount; i++) { 
            Heart heart = new Heart(4);
            heartList.Add(heart);
        }
    }

    public List<Heart> GetHeartList() { 
        return heartList;
    }

    public void Damage(int damageAmount) {
        
        //cycle through all hearts starting from the end
        for (int i = heartList.Count - 1; i >= 0; i--) { 
            Heart heart = heartList[i];
            // Test if this heart can absord damageAmount
            if (damageAmount > heart.GetFragmentAmount())
            {
                // heart cannot absorb full amount, damage heart and keep going to the next heart
                damageAmount -= heart.GetFragmentAmount();
                heart.Damage(heart.GetFragmentAmount());
            }
            else {
                // Heart can take full damage, absord and break out of the cycle
                heart.Damage(damageAmount);
                break;
            }
        }

        if (OnDamage != null) OnDamage(this,EventArgs.Empty); // send event
        if (IsDead()) {
            if (OnDead != null) OnDead(this, EventArgs.Empty); // send event
        }

    }

    public void Heal(int healAmount) {

        for (int i = 0; i < heartList.Count; i++) {
            Heart heart = heartList[i];
            int missingFragments = MAX_FRAGMENT_AMOUNT - heart.GetFragmentAmount();
            if (healAmount > missingFragments) {
                healAmount -= missingFragments;
                heart.Heal(missingFragments);
            }
            else
            {
                heart.Heal(healAmount);
                break;
            }
        }

        if (OnHeal != null) OnHeal(this, EventArgs.Empty); // send event

    }


    public bool IsDead() {
        return heartList[0].GetFragmentAmount() == 0;
    }


    //represents a single heart
    public class Heart {

        private int fragments;
        public Heart(int fragments) {
            this.fragments = fragments;
        }

        public int GetFragmentAmount() { 
            return fragments;
        }

        public void SetFragmentAmount(int fragments) { this.fragments = fragments; }

        public void Damage(int damageAmount) {
            if (damageAmount >= fragments){
                fragments = 0;
            }
            else {
                fragments -= damageAmount;
            }
        }


        public void Heal(int healAmount) {
            if (fragments + healAmount > MAX_FRAGMENT_AMOUNT)
            {
                fragments = MAX_FRAGMENT_AMOUNT;
            }
            else { 
                fragments += healAmount;
            }
        }

    }

}
