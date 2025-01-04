using XRL.Core;
using XRL.Language;
using XRL.UI;

namespace XRL.World.Parts
{
  public class MUNDANITY_EndlessPuzzle : IActivePart
  {
		public int difficulty = 15;

    public MUNDANITY_EndlessPuzzle()
    {
      this.IsBreakageSensitive = true;
      this.IsRustSensitive = true;
      this.WorksOnSelf = true;
    }

		public override bool WantEvent(int ID, int cascade)
    {
      return base.WantEvent(ID, cascade) || ID == GetInventoryActionsEvent.ID || ID == InventoryActionEvent.ID;
    }

		public override bool HandleEvent(GetInventoryActionsEvent E)
		{
      E.AddAction("Activate", "solve", "Mundanity_ActivateEndlessPuzzle", Key: 's', Default: 100);
			return base.HandleEvent(E);
		}

		public override bool HandleEvent(InventoryActionEvent E)
		{
			if ( E.Command == "Mundanity_ActivateEndlessPuzzle" )
      {
        ActivePartStatus Status = this.GetActivePartStatus();
        switch (Status)
        {
          case ActivePartStatus.Broken:
            Popup.ShowFail( this.ParentObject.Itis + " broken..." );
            break;
          case ActivePartStatus.Rusted:
            Popup.ShowFail(this.ParentObject.Itis + " rusted together.");
            break;
          case ActivePartStatus.Operational:
            if ( E.Actor.MakeSave("Intelligence", difficulty, Vs: "Puzzle", Source: this.ParentObject) )
    				{
    					Popup.Show("You solve the puzzle and it retangles itself.");
    				}
    				else
    				{
    					Popup.Show("You are stumped.");
    				}
            E.Actor.UseEnergy(1000, "Item Endless Puzzle");
            break;
          default:
            Popup.ShowFail("Nothing happens.");
            break;
        }
      }
			return base.HandleEvent(E);
		}
	}
}
