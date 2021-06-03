namespace Completed
{
    public class Wall : DestructibleObject
    {
        public override void DamageObject(int loss)
        {
            base.DamageObject(loss);
            //If hit points are less than or equal to zero:
            if (hp <= 0)
            {
                //Disable the gameObject.
                gameObject.SetActive(false);
            }
        }
    }
}
