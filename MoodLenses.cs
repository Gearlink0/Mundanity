using ConsoleLib.Console;
using System;
using System.Text;
using System.Collections.Generic;
using XRL;
using XRL.Core;
using XRL.Language;
using XRL.UI;

namespace XRL.World.Parts
{
  public class MUNDANITY_MoodLenses : IPart
  {
    int Radius = 40;

    string Color = "b";
    string Detail = "B";
    string Background = "k";

    public void Initialize()
    {
      XRLCore.RegisterAfterRenderCallback(new Action<XRLCore, ScreenBuffer>(this.AfterRender));
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
        if ( Equipper != null && this.ParentObject.Equipped.IsPlayer() )
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
      if( E.Actor.IsPlayer() )
        XRLCore.RegisterAfterRenderCallback(new Action<XRLCore, ScreenBuffer>(this.AfterRender));

        StringBuilder StringBuilder = new StringBuilder();
        StringBuilder.Append(this.Color);
        StringBuilder.Append(this.Detail);
        StringBuilder.Append(this.Background);
        if ( E.Actor.IsPlayer() )
        {
          E.Actor.SetStringProperty( "MUNDANITY_MoodLensesSettings", StringBuilder.ToString() );
        }
      return base.HandleEvent(E);
    }

    public override bool HandleEvent(UnequippedEvent E)
    {
      if( E.Actor.IsPlayer() )
        E.Actor.SetStringProperty( "MUNDANITY_MoodLensesSettings", "" );
      return base.HandleEvent(E);
    }

    private void AfterRender(XRLCore core, ScreenBuffer sb)
    {
      GameObject Player = XRLCore.Core?.Game?.Player?.Body;
      if (Player == null )
        return;

      string MoodLensesSettings = Player.GetStringProperty( "MUNDANITY_MoodLensesSettings" );

      if ( MoodLensesSettings.Length > 0 )
      {
        for(int row = 0; row < sb.Width; row++)
        {
          for(int col = 0; col< sb.Height; col++)
          {
            sb[row, col].SetForeground(MoodLensesSettings[0]);
            sb[row, col].SetDetail(MoodLensesSettings[1]);
            sb[row, col].SetBackground(MoodLensesSettings[2]);
          }
        }
      }
    }
	}
}
