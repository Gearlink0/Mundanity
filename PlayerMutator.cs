using ConsoleLib.Console;
using System;
using XRL;
using XRL.Core;
using XRL.UI;
using XRL.World;
using XRL.World.Parts;

[PlayerMutator]
[HasCallAfterGameLoadedAttribute]
public class MUNDANITY_AddMoodLensesCallbackToPlayer : IPlayerMutator
{
	public void mutate(GameObject player)
	{
		XRLCore.RegisterAfterRenderCallback(new Action<XRLCore, ScreenBuffer>(AfterRender));
		player.AddPart<MUNDANITY_TreatItemsAsScrapSystem>();
	}

	[CallAfterGameLoadedAttribute]
  public static void AfterLoadGameCallback()
  {
		XRLCore.RegisterAfterRenderCallback(new Action<XRLCore, ScreenBuffer>(AfterRender));
  }

	private static void AfterRender(XRLCore core, ScreenBuffer sb)
	{
		GameObject Player = The.Player;
		if ( Player == null )
			return;

		string MoodLensesSettings = Player.GetStringProperty( "MUNDANITY_MoodLensesSettings" );

		if (
			!MoodLensesSettings.IsNullOrEmpty()
			&& !Keyboard.bAlt
			&& !Player.HasEffect("Skulk_Tonic")
			&& !Options.DisableFullscreenColorEffects
		)
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
