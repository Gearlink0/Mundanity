using System;
using System.Text;
using XRL.Language;
using XRL.Rules;
using XRL.UI;

namespace XRL.World.Parts
{
	public class MUNDANITY_CompleteCodex : IPoweredPart
	{
		public string Corpus = "LibraryCorpus.json";
		public string Title = "complete codex";
		[NonSerialized]
    public string Text;

		public MUNDANITY_CompleteCodex()
    {
      this.ChargeUse = 10;
		  this.WorksOnSelf = true;
    }

		public override bool WantEvent(int ID, int cascade)
    {
      return base.WantEvent(ID, cascade) || ID == GetInventoryActionsEvent.ID || ID == InventoryActionEvent.ID;
    }

		public override bool HandleEvent(GetInventoryActionsEvent E)
    {
      E.AddAction("Read", "read", "Read", Key: 'r', Default: 100);
      return base.HandleEvent(E);
    }

		public override bool HandleEvent(InventoryActionEvent E)
    {
      if (E.Command == "Read" && E.Actor.IsPlayer())
      {
				ActivePartStatus Status = this.GetActivePartStatus();
				switch (Status)
        {
					case ActivePartStatus.Broken:
		        Popup.ShowFail(this.ParentObject.Itis + " broken...");
		        break;
	        case ActivePartStatus.Booting:
	          Popup.ShowFail(this.ParentObject.The + this.ParentObject.ShortDisplayName + this.ParentObject.Is + " still starting up.");
						break;
					case ActivePartStatus.Unpowered:
						int Charge = this.ParentObject.QueryCharge();
						if( Charge > 0 && Charge < this.ChargeUse )
						{
							Popup.ShowFail(this.ParentObject.The + this.ParentObject.ShortDisplayName + this.ParentObject.GetVerb("hum") + " for a moment, then powers down. " + this.ParentObject.It + this.ParentObject.GetVerb("don't") + " have enough charge to function.");
	            break;
	          }
	          Popup.ShowFail(this.ParentObject.The + this.ParentObject.ShortDisplayName + this.ParentObject.GetVerb("don't") + " have enough charge to function.");
						break;
					case ActivePartStatus.Operational:
						this.GeneratePage();
						this.ConsumeCharge();
		        BookUI.ShowBook(this.Text, this.Title);
	  				break;
					default:
	          Popup.ShowFail("Nothing happens.");
	          break;
				}
      }
      return base.HandleEvent(E);
    }

		public void GeneratePage()
    {
			string Data = MarkovChain.GenerateParagraph(MarkovBook.CorpusData[Corpus]).Replace("\n", " ").Replace("  ", " ").Trim();
			StringBuilder stringBuilder = new StringBuilder();

			for (int index = 0; index < Data.Length; ++index)
      {
        if (Stat.RandomCosmetic(1, 100) <= 5)
          stringBuilder.Append((char) Stat.RandomCosmetic(45, 230));
        else
          stringBuilder.Append(Data[index]);
      }

			this.Text = stringBuilder.ToString();
    }
	}
}
