using UnityEngine;

namespace Managers
{	
	public class Loader : MonoBehaviour 
	{
		public GameObject gameManager;			//GameManager prefab to instantiate.
		public GameObject gameEventsSystem;     //GameEventsSystem prefab to instantiate.

        void Awake ()
		{
			//Check if a GameManager has already been assigned to static variable GameManager.instance or if it's still null
			if (GameManager.instance == null)
				
				//Instantiate gameManager prefab
				Instantiate(gameManager);

            //Check if a GameManager has already been assigned to static variable GameManager.instance or if it's still null
            if (GameEvents.instance == null)

                //Instantiate gameManager prefab
                Instantiate(gameEventsSystem);
        }
	}
}