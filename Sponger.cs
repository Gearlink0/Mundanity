using System;
using XRL.World.Effects;

namespace XRL.World.Parts
{
  [Serializable]
  public class MUNDANITY_Sponger : IPart
  {
		public int SPONGE_AMOUNT = 20;
    public int INITIAL_EVAPORATION = 5;
    public int EVAPORATIVITY_FACTOR = 20;

    public override bool WantEvent(int ID, int cascade)
    {
      return base.WantEvent(ID, cascade)
      || ID == GetMaximumLiquidExposureEvent.ID
      || ID == EndTurnEvent.ID;
    }

    public override bool HandleEvent(GetMaximumLiquidExposureEvent E)
    {
      E.LinearIncrease = 64;
      return base.HandleEvent(E);
    }

    public override bool HandleEvent(EndTurnEvent E) {
      LiquidCovered liquidCovering;
      if( this.ParentObject.TryGetEffect<LiquidCovered>(out liquidCovering) )
      {
        LiquidVolume liquidVolume = liquidCovering.Liquid;
        if( liquidVolume != null )
        {
          int dramsToEvaporate = INITIAL_EVAPORATION;
          for( int index = 0; index < (liquidVolume.Volume - dramsToEvaporate); index++ )
          {
            if( EVAPORATIVITY_FACTOR.in100())
              ++dramsToEvaporate;
          }
          liquidVolume.UseDrams( dramsToEvaporate );
        }
      }
      return base.HandleEvent(E);
    }
  }
}
