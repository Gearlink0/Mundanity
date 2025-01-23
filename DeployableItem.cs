using System;
using XRL.World.Capabilities;

namespace XRL.World.Parts
{
	[Serializable]
  public class MUNDANITY_DeployableItem : IActivePart
  {
		public string ObjectBlueprint;

		public MUNDANITY_DeployableItem() => this.IsBreakageSensitive = true;

		public override bool WantEvent(int ID, int cascade)
    {
      return base.WantEvent(ID, cascade) || ID == GetInventoryActionsEvent.ID || ID == InventoryActionEvent.ID;
    }

		public override bool HandleEvent(GetInventoryActionsEvent E)
		{
      E.AddAction("Activate", "deploy", "Mundanity_DeployItem", Key: 'y');
			return base.HandleEvent(E);
		}

		public override bool HandleEvent(InventoryActionEvent E)
		{
			if ( E.Command == "Mundanity_DeployItem" && !this.ObjectBlueprint.IsNullOrEmpty())
      {
				XRL.Messages.MessageQueue.AddPlayerMessage("Mundanity_DeployItem ran");
				if (E.Actor.OnWorldMap())
        {
          E.Actor.Fail("You cannot do that on the world map.");
          return false;
        }
				string objectName = this.ParentObject.GetDisplayName(Single: true, Stripped: true, WithoutTitles: true, Short: true);
				if (!E.Actor.CanMoveExtremities("Deploy " + objectName, true, AllowTelekinetic: true))
          return false;
				Cell cell = E.Actor.PickDirection("Deploy " + objectName);
				if (cell == null)
          return false;
				if (!cell.IsPassable())
        {
          E.Actor.Fail("You can't deploy there!");
          return false;
        }
				GameObject itemToDeploy = GameObject.Create( this.ObjectBlueprint );
        cell.AddObject( itemToDeploy );
        itemToDeploy.MakeActive();
				Messaging.XDidYToZ(E.Actor, "deploy", Object: itemToDeploy, ColorAsGoodFor: E.Actor, IndefiniteObject: true);
        E.Actor.UseEnergy(1000, "Item Deploy Item");
				this.ParentObject.Destroy();
				E.RequestInterfaceExit();
			}
			return base.HandleEvent(E);
		}
	}
}
