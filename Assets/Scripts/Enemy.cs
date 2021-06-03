using UnityEngine;
using System.Collections.Generic;

namespace Completed
{
	//Enemy inherits from MovingObject, our base class for objects that can move, Player also inherits from this.
	public class Enemy : MovingObject
	{
		public int baseDamage; 							//The amount of food points to subtract from the player when attacking.
		public AudioClip attackSound1;						//First of two audio clips to play when attacking the player.
		public AudioClip attackSound2;						//Second of two audio clips to play when attacking the player.
		
		
		private Animator animator;							//Variable of type Animator to store a reference to the enemy's Animator component.
		private Transform target;							//Transform to attempt to move toward each turn.
		private bool skipMove;								//Boolean to determine whether or not enemy should skip a turn or move this turn.
        List<PathNode> path;                                //Path from enemy position to the target


        //Start overrides the virtual Start function of the base class.
        protected override void Start ()
		{
			//Register this enemy with our instance of GameManager by adding it to a list of Enemy objects. 
			//This allows the GameManager to issue movement commands.
			GameManager.instance.AddEnemyToList (this);
			
			//Get and store a reference to the attached Animator component.
			animator = GetComponent<Animator> ();
			
			//Find the Base GameObject using it's tag and store a reference to its transform component.
			target = GameObject.FindGameObjectWithTag ("Base").transform;
			
			//Call the start function of our base class MovingObject.
			base.Start ();

            //Stash pathfinding into a local variable
            var pathfinding = GameManager.instance.GetBoardScript().Pathfinding;

            //Get X and Y coords for the target
            pathfinding.GetGrid().GetXY(target.position, out int targetX, out int targetY);

            //Get X and Y coords for the enemy
            pathfinding.GetGrid().GetXY(transform.position, out int selfX, out int selfY);

            //Find a path from enemy to the target
            path = pathfinding.FindPath(selfX, selfY, targetX, targetY);
        }
		
		
		//Override the AttemptMove function of MovingObject to include functionality needed for Enemy to skip turns.
		//See comments in MovingObject for more on how base AttemptMove function works.
		protected override void AttemptMove <T> (int xDir, int yDir)
		{
			//Call the AttemptMove function from MovingObject.
			base.AttemptMove <T> (xDir, yDir);
		}


        //MoveEnemy is called by the GameManger each turn to tell each Enemy to try to move towards the player.
        public void MoveEnemy()
        {
            //Declare variables for X and Y axis move directions, these range from -1 to 1.
            //These values allow us to choose between the cardinal directions: up, down, left and right.
            int xDir = 0;
            int yDir = 0;

            //Calculate in witch direction should enemy move
            var xMovement = (int)transform.position.x - path[0].x;
            var yMovement = (int)transform.position.y - path[0].y;
            if (xMovement == 1 || xMovement == -1)
            {
                xDir = -xMovement;
            }
            else if (yMovement == 1 || yMovement == -1)
            {
                yDir = -yMovement;
            }
            //Remove the path node we already moved to
            //If number of path nodes are 1 that means enemy is in front of base
            //so no more movement, enemy just stands and hits the base
            //TODO: figure out a safer option for this
            if (path.Count != 1)
            {
                path.RemoveAt(0);
            }

            //Call the AttemptMove function and pass in the generic parameter Player, because Enemy is moving and expecting to potentially encounter a Player
            AttemptMove<PlayersBase>(xDir, yDir);
        }
		
		
		//OnCantMove is called if Enemy attempts to move into a space occupied by a Player, it overrides the OnCantMove function of MovingObject 
		//and takes a generic parameter T which we use to pass in the component we expect to encounter, in this case Player
		protected override void OnCantMove <T> (T component)
		{
            //Declare hitBase and set it to equal the encountered component.
            PlayersBase hitBase = component as PlayersBase;

            //Call the DamageObject function of base class passing it baseDamage, the amount of hitpoints to be subtracted.
            hitBase.DamageObject (baseDamage);
			
			//Set the attack trigger of animator to trigger Enemy attack animation.
			animator.SetTrigger ("enemyAttack");
			
			//Call the RandomizeSfx function of SoundManager passing in the two audio clips to choose randomly between.
			SoundManager.instance.RandomizeSfx (attackSound1, attackSound2);
		}
	}
}
