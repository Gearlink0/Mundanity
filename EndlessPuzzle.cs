using XRL.Core;
using XRL.Language;
using XRL.UI;

namespace XRL.World.Parts
{
  public class MUNDANITY_EndlessPuzzle : IPart
  {
		public int difficulty = 15;

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
				if ( E.Actor.MakeSave("Intelligence", difficulty, Vs: "Puzzle", Source: this.ParentObject) )
				{
					Popup.Show("You untangle the puzzle and it retangles itself.");
				}
				else
				{
					Popup.Show("You are stumped.");
				}
        E.Actor.UseEnergy(1000, "Item Endless Puzzle");
        E.RequestInterfaceExit();
      }
			return base.HandleEvent(E);
		}
	}
}
