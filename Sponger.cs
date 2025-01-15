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
      int dramsToEvaporate = INITIAL_EVAPORATION;
      LiquidCovered liquidCovering;
      LiquidStained liquidStain;
      if( this.ParentObject.TryGetEffect<LiquidCovered>(out liquidCovering) )
      {
        LiquidVolume liquidVolume = liquidCovering.Liquid;
        if( liquidVolume != null )
          for( int index = 0; index < (liquidVolume.Volume - dramsToEvaporate); index++ )
            if( EVAPORATIVITY_FACTOR.in100())
              ++dramsToEvaporate;
      }
      if( this.ParentObject.TryGetEffect<LiquidStained>(out liquidStain) )
      {
        LiquidVolume liquidVolume = liquidCovering.Liquid;
        if( liquidVolume != null )
          for( int index = 0; index < (liquidVolume.Volume - dramsToEvaporate); index++ )
            if( EVAPORATIVITY_FACTOR.in100())
              ++dramsToEvaporate;
      }
      if( liquidCovering != null && dramsToEvaporate > 0 )
      {
        LiquidVolume liquidVolume = liquidCovering.Liquid;
        if( liquidVolume != null )
        {
          int coveringDramsToEvaporate = Math.Min(dramsToEvaporate, liquidVolume.Volume);
          liquidVolume.UseDramsByEvaporativity( coveringDramsToEvaporate );
          dramsToEvaporate -= coveringDramsToEvaporate;
        }
      }
      if( liquidStain != null && liquidStain.Liquid != null && dramsToEvaporate > 0 )
        liquidStain.Liquid.UseDramsByEvaporativity( Math.Min(dramsToEvaporate, liquidStain.Liquid.Volume) );
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
            if( liquidCovering.Liquid != null )
              existingVolume += liquidCovering.Liquid.Volume;
          LiquidStained liquidStain;
          if( this.ParentObject.TryGetEffect<LiquidStained>(out liquidStain) )
            if( liquidStain.Liquid != null )
              existingVolume += liquidStain.Liquid.Volume;
          if( existingVolume < maxVolume )
          {
            bool requestInterfaceExit = false;
            liquidVolume.ProcessContact(
              this.ParentObject,
              Initial: true,
              Poured: true,
              PouredBy: this.ParentObject,
              ContactVolume: Math.Min((maxVolume - existingVolume), liquidVolume.Volume)
            );
            return true;
          }
        }
      }
      return false;
    }
  }
}
