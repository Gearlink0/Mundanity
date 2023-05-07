namespace XRL.World.Parts
{
  public class MUNDANITY_RealityDistortionDescription : IActivePart
  {
		public string StabilizedDescription;

		public MUNDANITY_RealityDistortionDescription()
    {
      this.IsRealityDistortionBased = true;
      this.WorksOnSelf = true;
    }

		public override bool WantEvent(int ID, int cascade) => base.WantEvent(ID, cascade) || ID == GetShortDescriptionEvent.ID;

		public override bool HandleEvent(GetShortDescriptionEvent E)
    {
			if ( this.GetActivePartStatus() == ActivePartStatus.RealityStabilized )
				E.Base.Clear().Append(this.StabilizedDescription);
      return base.HandleEvent(E);
    }
	}
}
