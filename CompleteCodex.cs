using System;
using System.Text;
using XRL.Language;
using XRL.Rules;
using XRL.UI;

namespace XRL.World.Parts
{
	public class MUNDANITY_CompleteCodex : IPart
	{
		public string Corpus = "LibraryCorpus.json";
		public string Title = "complete codex";
		[NonSerialized]
    public string Text;

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
        this.GeneratePage();
        BookUI.ShowBook(this.Text, this.Title);
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
