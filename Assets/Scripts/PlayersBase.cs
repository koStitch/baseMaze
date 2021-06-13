using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
    public class PlayersBase : DestructibleObject
    {
        private Text baseHpText;						//UI Text to display current player's base HP total.
        private string baseHpLabel = "Base HP: ";   //Label describing the text for base hp

        private void Start()
        {
            Managers.GameEvents.instance.onPlayersBaseHealthTextUpdate += PlayersBaseHealthTextUpdate;
            Managers.GameEvents.instance.onPlayersBaseHealthChange += HealthChange;
            //Find the baseHpText on scene by it's tag and assign it
            baseHpText = GameObject.FindGameObjectWithTag("BaseHpText").GetComponent<Text>();
            //We are taking the health points defined inside the GameManager
            hp = Managers.GameManager.instance.playersBaseHp;
            //Set the baseHpText to reflect the current players base hp total.
            PlayersBaseHealthTextUpdate();
        }

        private void OnDestroy()
        {
            Managers.GameEvents.instance.onPlayersBaseHealthTextUpdate -= PlayersBaseHealthTextUpdate;
            Managers.GameEvents.instance.onPlayersBaseHealthChange -= HealthChange;
        }

        //This function is called when the behaviour becomes disabled or inactive.
        private void OnDisable()
        {
            //When Player object is disabled, store the current local food total in the GameManager so it can be re-loaded in next level.
            Managers.GameManager.instance.playersBaseHp = hp;
        }

        public override void DamageObject(int loss)
        {
            base.DamageObject(loss);
            //Set the baseHpText to reflect the current players base hp total.
            PlayersBaseHealthTextUpdate();
            //If hit points are less than or equal to zero:
            if (hp <= 0)
            {
                //End the game
                Managers.GameManager.instance.GameOver();
            }
        }

        public void PlayersBaseHealthTextUpdate()
        {
            baseHpText.text = baseHpLabel + hp;
        }

        private void HealthChange(int amount)
        {
            hp += amount;
        }
    }
}