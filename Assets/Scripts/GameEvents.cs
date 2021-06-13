using System;
using UnityEngine;

namespace Managers
{
    public class GameEvents : MonoBehaviour
    {
        public static GameEvents instance;
        public event Action onPlayersBaseHealthTextUpdate;
        public event Action<int> onPlayersBaseHealthChange;
        public event Action onFreezeEnemy;
        public event Action onLevelEnd;

        private void Awake()
        {
            //Check if instance already exists
            if (instance == null)
            {
                //if not, set instance to this
                instance = this;
            }
            else if (instance != this) //If instance already exists and it's not this:
            {
                //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
                Destroy(gameObject);
            }

            //Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(gameObject);
        }

        //Called when UI text on the screen representing players base health needs to be updated
        public void PlayersBaseHealthTextUpdate()
        {
            onPlayersBaseHealthTextUpdate?.Invoke();
        }

        //Called when players base health points need to be increased or reduced
        public void PlayersBaseHealthChange(int amount)
        {
            onPlayersBaseHealthChange?.Invoke(amount);
        }

        //Called when enemies need to be frozen and skip some turns
        public void FreezeEnemy()
        {
            onFreezeEnemy?.Invoke();
        }

        //Called when level is finished
        public void LevelEnd()
        {
            onLevelEnd?.Invoke();
        }
    }
}
