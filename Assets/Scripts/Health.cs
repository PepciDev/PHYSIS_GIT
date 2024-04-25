using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float health = 2f;
    public Slider hpSlider;
    public bool dead = false;
    public bool shield = false;
    bool invincible = false;

    public Image flashScreen;
    bool flashing = false;
    float flashTime = 0.08f;
    bool flipped = false;

    public bool playerHealth = false;

    void Update()
    {
        //die
        if (health <= 0f && !dead)
        {
            dead = true;
            SendMessage("Die");
        }

        if (flashing)
        {
            flashScreen.color = Color.Lerp(flashScreen.color, new Color(1, 1, 1, 0), flashTime);
            if (flashScreen.color.a < 0.06f)
            {
                flashing = false;
                flashScreen.color = new Color(1, 1, 1, 0);
            }
        }
    }

    public void HPChange(float damage)
    {
        if (damage > 0)
        {
            //healing
            health += damage;
        }
        else if (damage < 0 && !invincible)
        {
            if (shield)
            {
                //destroy shield
                shield = false;
            }
            else
            {
                //take damage
                health += damage;
                SendMessage("TakeDamage");

                if (playerHealth)
                {
                    //flash screen fx
                    flashing = true;
                    flashScreen.color = new Color(1, 1, 1, 0.6f);
                    if (!flipped)
                    {
                        flashScreen.transform.rotation = Quaternion.Euler(0, 0, 180f + Random.Range(-15f, 15f));
                        flipped = true;
                    }
                    else
                    {
                        flashScreen.transform.rotation = Quaternion.Euler(0, 0, 0 + Random.Range(-15f, 15f));
                        flipped = false;
                    }
                }
            }
        }

        //player UI change
        if (playerHealth)
        {
            hpSlider.value = health;
        }
    }

    public void Invincible()
    {
        //Cant take damage
        invincible = true;
    }

    public void Mortal()
    {
        //can take damage
        invincible = false;
    }
}
