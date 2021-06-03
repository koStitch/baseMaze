namespace Completed
{
    public class PlayersBase : DestructibleObject
    {
        private void Start()
        {
            //We are taking the health points defined inside the GameManager
            hp = GameManager.instance.playersBaseHp;
        }

        public override void DamageObject(int loss)
        {
            base.DamageObject(loss);
            //If hit points are less than or equal to zero:
            if (hp <= 0)
            {
                //End the game
                GameManager.instance.GameOver();
            }
        }
    }
}