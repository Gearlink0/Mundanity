using System;
using System.Text;
using System.Collections.Generic;
using XRL;
using XRL.Language;
using XRL.UI;

namespace XRL.World.Parts
{
  [Serializable]
  public class MUNDANITY_MoodLenses : IPart
  {
    public string Color = "b";
    public string Detail = "B";
    public string Background = "k";

    public override bool SameAs(IPart p)
    {
      MUNDANITY_MoodLenses That = p as MUNDANITY_MoodLenses;
      return !(That.Color != this.Color) && !(That.Detail != this.Detail) && !(That.Background != this.Background) && base.SameAs(p);
    }

		public override bool WantEvent(int ID, int cascade)
    {
      return base.WantEvent(ID, cascade) || ID == GetInventoryActionsEvent.ID || ID == InventoryActionEvent.ID || ID == EquippedEvent.ID  || ID == UnequippedEvent.ID;
    }

    public override bool HandleEvent(GetInventoryActionsEvent E)
		{
      E.AddAction("Activate", "configure", "Mundanity_ActivateMoodLenses", Key: 'c', Default: 100);
			return base.HandleEvent(E);
		}

    public override bool HandleEvent(InventoryActionEvent E)
		{
			if ( E.Command == "Mundanity_ActivateMoodLenses" )
      {
        this.Color = Popup.ShowColorPicker("Choose a primary color.", includeNone: false);
        this.Detail = Popup.ShowColorPicker("Choose a secondary color.", includeNone: false);
        this.Background = Popup.ShowColorPicker("Choose a background color.", includeNone: false);
        GameObject Equipper = this.ParentObject.Equipped;
        if ( Equipper != null )
        {
          StringBuilder StringBuilder = new StringBuilder();
          StringBuilder.Append(this.Color);
          StringBuilder.Append(this.Detail);
          StringBuilder.Append(this.Background);
          Equipper.SetStringProperty( "MUNDANITY_MoodLensesSettings", StringBuilder.ToString() );
        }
        E.Actor.UseEnergy(1000, "Item Mood Lenses");
        E.RequestInterfaceExit();
      }
			return base.HandleEvent(E);
		}

    public override bool HandleEvent(EquippedEvent E)
    {
      StringBuilder StringBuilder = new StringBuilder();
      StringBuilder.Append(this.Color);
      StringBuilder.Append(this.Detail);
      StringBuilder.Append(this.Background);
      E.Actor.SetStringProperty( "MUNDANITY_MoodLensesSettings", StringBuilder.ToString() );
      return base.HandleEvent(E);
    }

    public override bool HandleEvent(UnequippedEvent E)
    {
      E.Actor.SetStringProperty( "MUNDANITY_MoodLensesSettings", "" );
      return base.HandleEvent(E);
    }
	}
}
