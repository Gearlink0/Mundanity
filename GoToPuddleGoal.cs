using System;
using XRL.World.Effects;
using XRL.World.Parts;

namespace XRL.World.AI.GoalHandlers
{
  [Serializable]
  public class MUNDANITY_GoToPuddle : GoalHandler
  {
    public Cell Target;
		public bool Arrived;

    public MUNDANITY_GoToPuddle(Cell Target) => this.Target = Target;

    public override bool Finished() => this.Arrived;

    public void MoveToPuddle()
    {
      if (this.Arrived && !this.ParentObject.HasEffect<LiquidCovered>() && !this.ParentObject.GetCurrentCell().HasOpenLiquidVolume() )
        this.FailToParent();
			if (!this.Arrived)
      {
        if (this.ParentObject.DistanceTo(Target) <= 1)
        	this.Arrived = true;
				else
          this.ParentBrain.PushGoal((GoalHandler) new MoveTo(Target));
      }
    }

    public override void TakeAction() => this.MoveToPuddle();
  }
}
