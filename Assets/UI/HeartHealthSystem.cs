
using System;
using System.Collections.Generic;
using UnityEngine;

public class HeartHealthSystem{

    public event EventHandler OnDamage;

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

    }

}
