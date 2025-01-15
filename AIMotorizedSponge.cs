using System;
using System.Collections.Generic;
using XRL.Rules;
using XRL.World.AI;
using XRL.World.AI.GoalHandlers;

namespace XRL.World.Parts
{
  [Serializable]
  public class MUNDANITY_AIMotorizedSponge : AIBehaviorPart
  {
		public static int LIQUID_PUDDLE_SIZE = 200;

    public override bool WantEvent(int ID, int cascade) => base.WantEvent(ID, cascade) || ID == SingletonEvent<GetDebugInternalsEvent>.ID;

    public override bool HandleEvent(GetDebugInternalsEvent E)
    {
			if (this.ParentObject.HasGoal("MUNDANITY_GoToPuddle"))
				E.AddEntry((IPart) this, "Liquid seeking status", "Going to puddle");
      return base.HandleEvent(E);
    }

    public override bool WantTurnTick() => true;

    public override void TurnTick(long TimeTick, int Amount) => this.CheckGoToPuddle();

    public bool CheckGoToPuddle()
    {
      if (this.ParentObject.IsBusy() || !this.ParentObject.FireEvent("CanAIDoIndependentBehavior") || this.ParentObject.IsPlayerControlled())
				return false;

      List<GlobalLocation> urns = AIUrnDuster.GetUrns(this.ParentObject.CurrentZone);
      List<Cell> puddles = this.GetPuddles(this.ParentObject.CurrentZone);
      if (puddles.IsNullOrEmpty<Cell>())
        return false;
      int puddleIndex = Stat.Random(0, puddles.Count - 1);
      this.ParentObject.Brain.PushGoal((GoalHandler) new MUNDANITY_GoToPuddle(puddles[puddleIndex]));
      return true;
    }

    public List<Cell> GetPuddles( Zone zone )
    {
      Predicate<Cell> pred = cell => (cell.IsPassable() && cell.GetOpenLiquidVolume() != null && cell.GetOpenLiquidVolume().LiquidVolume.Volume <= LIQUID_PUDDLE_SIZE);
      return zone.GetCells(pred);
    }
	}
}
