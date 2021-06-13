using UnityEngine;
using Managers;

namespace Gameplay
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class DestructibleObject : MonoBehaviour
    {
        [SerializeField]
        private AudioClip[] chopSounds;             //2 of 2 audio clips that play when the wall is attacked by the player.
        [SerializeField]
        private Sprite dmgSprite;                   //Alternate sprite to display after Wall has been attacked by player.
        public int hp = 3;                          //Object's health points

        private SpriteRenderer spriteRenderer;      //Store a component reference to the attached SpriteRenderer.

        void Awake()
        {
            //Get a component reference to the SpriteRenderer.
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        //DamageWall is called when someone attacks this object.
        public virtual void DamageObject(int loss)
        {
            //Call the RandomizeSfx function of SoundManager to play one of two chop sounds.
            SoundManager.instance.RandomizeSfx(chopSounds);

            //Set spriteRenderer to the damaged object sprite.
            spriteRenderer.sprite = dmgSprite;

            //Subtract loss from hit point total.
            hp -= loss;
        }
    }
}
