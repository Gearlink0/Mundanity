using System;
using XRL.World.Effects;

namespace XRL.World.Parts
{
  [Serializable]
  public class MUNDANITY_Sponger : IPart
  {
		public int SPONGE_AMOUNT = 20;
    public int INITIAL_EVAPORATION = 5;
    public int EVAPORATIVITY_FACTOR = 5;

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
      this.Sponge();
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
          liquidVolume.UseDramsByEvaporativity( dramsToEvaporate );
        }
      }
      return base.HandleEvent(E);
    }

    public bool Sponge()
    {
      Cell currentCell = this.ParentObject.GetCurrentCell();
      if( currentCell != null )
      {
        LiquidVolume liquidVolume = currentCell.GetOpenLiquidVolume()?.LiquidVolume;
        if( liquidVolume != null )
        {
          int existingVolume = 0;
          int maxVolume = this.ParentObject.GetMaximumLiquidExposure();
          LiquidCovered liquidCovering;
          if( this.ParentObject.TryGetEffect<LiquidCovered>(out liquidCovering) )
          {
            if( liquidCovering.Liquid != null )
              existingVolume = liquidCovering.Liquid.Volume;
          }
          if( existingVolume < maxVolume )
          {
            bool requestInterfaceExit = false;
            liquidVolume.ProcessContact(
              this.ParentObject,
              Initial: true,
              Poured: true,
              PouredBy: this.ParentObject,
              ContactVolume: (maxVolume - existingVolume)
            );
            return true;
          }
        }
      }
      return false;
    }
  }
}
