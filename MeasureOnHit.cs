using System.Text;
using XRL.Core;
using XRL.Language;
using XRL.UI;

namespace XRL.World.Parts
{
	public class MUNDANITY_MeasureOnHit : IPart
	{

		public string Metric = "distance";

		public override void Register(GameObject Object)
    {
			Object.RegisterPartEvent((IPart) this, "WeaponMissileWeaponHit");
      base.Register(Object);
    }

		public override bool FireEvent(Event E)
    {
			if (E.ID == "WeaponMissileWeaponHit")
			{
				GameObject Attacker = E.GetGameObjectParameter("Attacker");
      	GameObject Defender = E.GetGameObjectParameter("Defender");
				this.Measure(Attacker, Defender);
			}
      return base.FireEvent(E);
    }

		public bool Measure(GameObject Attacker, GameObject Defender)
    {
			if (Attacker.IsPlayer())
			{
				StringBuilder Message = Event.NewStringBuilder();
				Message.Append(this.ParentObject.the).Append(this.ParentObject.ShortDisplayName);
				Message.Append(" beeps and reports ");
				if (this.Metric == "distance")
				{
					Message.Append("the size and distance of ");
					Message.Append(Defender.the).Append(Defender.ShortDisplayName);
					Message.Append(" but you cannot make sense of the measurements.");
				}
				else if (this.Metric == "weight")
				{
					Message.Append(Defender.the).Append(Defender.ShortDisplayName);
					Message.Append(" weighs ");
					Message.Append(Defender.Weight.ToString());
					Message.Append(" units.");
				}
				else
				{
					Message.Append("some measurements you cannoy make sense of.");
				}
				XRL.Messages.MessageQueue.AddPlayerMessage(Message.ToString());
				return true;
			}
			return false;
    }
	}
}
