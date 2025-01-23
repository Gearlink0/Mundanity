using System;

namespace XRL.World.Parts
{
	[Serializable]
  public class MUNDANITY_BecomeItemOnDeactivate : IPart
  {
		public override void Register(GameObject Object, IEventRegistrar Registrar)
    {
      Registrar.Register("PowerSwitchDeactivated");
      base.Register(Object, Registrar);
    }

		public override bool FireEvent(Event E)
    {
      if (E.ID == "PowerSwitchDeactivated")
      {
				Cell currentCell = this.ParentObject.CurrentCell;
        currentCell.AddObject("MUNDANITY_MotorizedSponge");
        this.ParentObject.Destroy();
      }
      return base.FireEvent(E);
    }
	}
}
