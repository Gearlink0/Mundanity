using System.Collections.Generic;
using XRL.Core;
using XRL.Language;
using XRL.UI;

namespace XRL.World.Parts
{
  public class MUNDANITY_Cleaner : IPart
  {
		public override bool WantEvent(int ID, int cascade)
    {
      return base.WantEvent(ID, cascade) || ID == GetInventoryActionsEvent.ID ||
        ID == InventoryActionEvent.ID || ID == CheckAnythingToCleanWithEvent.ID ||
        ID == CheckAnythingToCleanWithNearbyEvent.ID || ID == GetCleaningItemsEvent.ID ||
        ID == GetCleaningItemsNearbyEvent.ID;
    }

		public override bool HandleEvent(GetInventoryActionsEvent E)
		{
      if( CheckAnythingToCleanEvent.Check(IComponent<GameObject>.ThePlayer, this.ParentObject) )
      {
        E.AddAction("CleanAll", "clean all your items", "Mundanity_CleanWithCleaner", Key: 'n', Default: 100);
      }
			return base.HandleEvent(E);
		}

		public override bool HandleEvent(InventoryActionEvent E)
		{
			if ( E.Command == "Mundanity_CleanWithCleaner" )
      {
        if (this.ParentObject.IsInStasis())
        {
          if (E.Actor.IsPlayer())
            Popup.ShowFail("You cannot seem to interact with " + this.ParentObject.t() + " in any way.");
          return false;
        }
        if (!E.Actor.CanMoveExtremities(ShowMessage: true, AllowTelekinetic: true))
          return false;
        List<GameObject> Objects = (List<GameObject>) null;
        List<string> Types = (List<string>) null;
        CleanItemsEvent.PerformFor(E.Actor, E.Actor, this.ParentObject, out Objects, out Types);
        LiquidVolume.CleaningMessage(E.Actor, Objects, Types, this.ParentObject, null, true);
        E.Actor.UseEnergy(1000, "Cleaning");
        E.RequestInterfaceExit();
      }
			return base.HandleEvent(E);
		}
	}
}
