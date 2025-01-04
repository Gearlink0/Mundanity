using HarmonyLib;
using System;
using System.Collections.Generic;
using XRL.UI;
using XRL.World;
using XRL.World.Parts.Skill;

namespace XRL.World.Parts
{
  [Serializable]
  public class MUNDANITY_TreatItemsAsScrapSystem : IPart
  {
    // public override void Register(XRLGame Game, IEventRegistrar Registrar) => Registrar.Register(AfterAddSkillEvent.ID);

		public override bool WantEvent(int ID, int cascade) => base.WantEvent(ID, cascade) || ID == AfterAddSkillEvent.ID;

    public override bool HandleEvent(AfterAddSkillEvent E)
    {
			if(
				Options.GetOptionBool("MUNDANITY_TreatMundanityItemsAsScrap")
				&& E.Actor.IsPlayer()
				&& E.Skill.GetType() == typeof(Tinkering_Disassemble)
			)
			{
				Tinkering_Disassemble tinkeringDisassemble = The.Player.GetPart<Tinkering_Disassemble>();
				List<string> mundaneTrinketBlueprints = new List<string>{
					"MUNDANITY_CulinaryCyst",
					"MUNDANITY_MoodLenses",
					"MUNDANITY_ScouringGel",
					"MUNDANITY_UniversalInterfaceCable",
					"MUNDANITY_MotorizedSponge",
					"MUNDANITY_EtchingStylus",
					"MUNDANITY_Orbsinger",
					"MUNDANITY_StasisTorque",
					"MUNDANITY_ModularInstrument",
					"MUNDANITY_EndlessPuzzle",
					"MUNDANITY_LaserScaper",
					"MUNDANITY_AroMatic",
					"MUNDANITY_SelfReplicatingPigment",
					"MUNDANITY_RelaxationSand",
					"MUNDANITY_LaserScale",
					"MUNDANITY_ReprogrammableSculpture",
					"MUNDANITY_OralImplement",
					"MUNDANITY_CompleteCodex",
					"MUNDANITY_StellarMemory",
					"MUNDANITY_FrozenJoy"
				};
				foreach( string blueprint in mundaneTrinketBlueprints )
				{
					GameObject objectToBeScrap = GameObject.Create(blueprint);
					// Create a traverse instance on the player's instance of the disassemble
					// skill
					Traverse trav = Traverse.Create( tinkeringDisassemble );
					// Move the traverse instance to a call of the private method SetScrapToggle
					// with arguments objectToBeScrap and true
					trav = trav.Method("SetScrapToggle", objectToBeScrap, true);
					// Get the results of that method call, which will mutate the disassemble
					// skill's list of objects to treat as a scrap
					trav.GetValue();
				}
			}
      return base.HandleEvent(E);
    }
	}
}
