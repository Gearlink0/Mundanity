using System;
using System.Collections.Generic;
using XRL.Core;
using XRL.Rules;
using XRL.UI;

namespace XRL.World.Parts
{
	public class MUNDANITY_ReprogrammableSculpture : IPoweredPart
	{
		public string DefaultTile;

		public string SculptureTile;
		[FieldSaveVersion(263)]
    public string ColorStrings = "&C,&b,&c,&B";
    [FieldSaveVersion(263)]
    public string DetailColors = "c,C,b,b";

		[NonSerialized]
    public int FrameOffset;
    [NonSerialized]
    public int FlickerFrame;
		[NonSerialized]
    private List<string> ColorStringParts;
    [NonSerialized]
    private List<string> DetailColorParts;

		public MUNDANITY_ReprogrammableSculpture()
    {
      this.ChargeUse = 10;
			this.IsRustSensitive = true;
		  this.WorksOnSelf = true;
    }

		public override bool SameAs(IPart p)
    {
      MUNDANITY_ReprogrammableSculpture That = p as MUNDANITY_ReprogrammableSculpture;
      return !(That.DefaultTile != this.DefaultTile) && !(That.SculptureTile != this.SculptureTile) && base.SameAs(p);
    }

		public override bool WantEvent(int ID, int cascade)
    {
      return base.WantEvent(ID, cascade) || ID == GetInventoryActionsEvent.ID || ID == InventoryActionEvent.ID;
    }

		public override bool HandleEvent(GetInventoryActionsEvent E)
		{
      E.AddAction("Activate", "scan", "Mundanity_ActivateReprogrammableSculpture", Key: 's', Default: 100);
			E.AddAction("Deactivate", "forget", "Mundanity_DeactivateReprogrammableSculpture", Key: 'f');
			return base.HandleEvent(E);
		}

		public override bool HandleEvent(InventoryActionEvent E)
		{
			if ( E.Command == "Mundanity_ActivateReprogrammableSculpture" )
      {
				ActivePartStatus Status = this.GetActivePartStatus();
				switch (Status)
        {
					case ActivePartStatus.Broken:
		        Popup.ShowFail(this.ParentObject.Itis + " broken...");
		        break;
					case ActivePartStatus.Rusted:
            Popup.ShowFail(this.ParentObject.Itis + " aperture is rusted together.");
            break;
					case ActivePartStatus.Unpowered:
						int Charge = this.ParentObject.QueryCharge();
						if( Charge > 0 && Charge < this.ChargeUse )
						{
							Popup.ShowFail(this.ParentObject.The + this.ParentObject.ShortDisplayName + this.ParentObject.GetVerb("hum") + " for a moment, then powers down. " + this.ParentObject.It + this.ParentObject.GetVerb("don't") + " have enough charge to function.");
	            break;
	          }
	          Popup.ShowFail(this.ParentObject.The + this.ParentObject.ShortDisplayName + this.ParentObject.GetVerb("don't") + " have enough charge to function.");
						break;
					case ActivePartStatus.Operational:
						ScanObject( E.Actor );
						this.ConsumeCharge();
		        E.Actor.UseEnergy(1000, "Item Reprogrammable Sculpture");
	          break;
					default:
	          Popup.ShowFail("Nothing happens.");
	          break;
				}
        E.RequestInterfaceExit();
      }
			if ( E.Command == "Mundanity_DeactivateReprogrammableSculpture" )
      {
				this.SculptureTile = null;
        E.Actor.UseEnergy(1000, "Item Reprogrammable Sculpture");
        E.RequestInterfaceExit();
      }
			return base.HandleEvent(E);
		}

		public bool ScanObject( GameObject Actor = null )
		{
			GameObject ScanTarget = null;
			int chosenOption = Popup.ShowOptionList(
				Options: new string[2] {
					"Scan something in your inventory.",
					"Scan something nearby."
				},
				Hotkeys: new char[2]{ 'a', 'b' },
				Intro: ("What do you want to scane?"),
				AllowEscape: true
			);
			if ( chosenOption < 0 )
				return false;
			if ( chosenOption == 0 )
			{
				Inventory inventory = Actor.Inventory;
				this.ParentObject.SplitFromStack();
				List<GameObject> inventoryItems = Event.NewGameObjectList();
				inventory.GetObjects(inventoryItems);
				inventoryItems.Remove(this.ParentObject);

				ScanTarget = PickItem.ShowPicker(inventoryItems, Actor: Actor);
			}
			if ( chosenOption == 1 )
			{
				string Direction = XRL.UI.PickDirection.ShowPicker(nameof (ScanObject));
        if (Direction == null)
          return false;
        Cell TargetCell = Actor.GetCurrentCell().GetCellFromDirection(Direction, false);
				ScanTarget = TargetCell.GetHighestRenderLayerObject();
			}

			if (ScanTarget == null)
				return false;
			this.SculptureTile = ScanTarget.pRender.Tile;
			return true;
		}

		public override bool Render(RenderEvent E)
		{
			if ( !string.IsNullOrEmpty(this.SculptureTile) && this.GetActivePartStatus() == ActivePartStatus.Operational )
			{
				this.ParentObject.pRender.Tile = this.SculptureTile;

				int num = (XRLCore.CurrentFrame + this.FrameOffset) % 200;
	      if (!this.ColorStrings.IsNullOrEmpty())
	      {
	        if (this.ColorStringParts == null)
	          this.ColorStringParts = this.ColorStrings.CachedCommaExpansion();
	        int count = this.ColorStringParts.Count;
	        int index = num / count;
	        E.ColorString = index < count ? this.ColorStringParts[index] : this.ColorStringParts[count - 1];
	      }
	      if (!this.DetailColors.IsNullOrEmpty())
	      {
	        if (this.DetailColorParts == null)
	          this.DetailColorParts = this.DetailColors.CachedCommaExpansion();
	        int count = this.DetailColorParts.Count;
	        int index = num / count;
	        E.DetailColor = index < count ? this.DetailColorParts[index] : this.DetailColorParts[count - 1];
	      }
	      if (this.FlickerFrame > 0 || Stat.RandomCosmetic(1, 200) == 1)
	      {
	        E.Tile = (string) null;
	        if (this.FlickerFrame == 0)
	          E.RenderString = "_";
	        if (this.FlickerFrame == 1)
	          E.RenderString = "-";
	        if (this.FlickerFrame == 2)
	          E.RenderString = "|";
	        if (this.FlickerFrame == 0)
	          this.FlickerFrame = 3;
	        --this.FlickerFrame;
	        E.ColorString = "&Y";
	      }
	      else if (Stat.RandomCosmetic(1, 400) == 1)
	        E.ColorString = "&Y";
	      if (!Options.DisableTextAnimationEffects)
	        this.FrameOffset += Stat.Random(0, 20);
	      return true;
			}
			else
				this.ParentObject.pRender.Tile = this.DefaultTile;
			return true;
		}
	}
}
