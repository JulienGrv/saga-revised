using Saga.IO;
using Saga.Packets;
using System.IO;

namespace Saga.Structures.Collections
{
    /// <summary>
    /// Collection of Rag2Items used to open a bookstore window where
    /// actord can buy items.
    /// </summary>
    public class ShopCollection : BaseShopCollection
    {
        /// <summary>
        /// Creates a collection from a file
        /// </summary>
        /// <param name="filename">File containing bookstore data</param>
        /// <returns>BookstoreCollection</returns>
        public static ShopCollection FromFile(string filename)
        {
            ShopCollection _shoplist = new ShopCollection();
            if (File.Exists(filename))
            {
                using (ShopReader reader = ShopReader.Open(filename))
                {
                    while (reader.Read())
                    {
                        if (reader.HasInformation == true)
                            if (reader.Count > 0)
                            {
                                _shoplist.AddGoods(reader.Value, reader.Count);
                            }
                            else
                            {
                                _shoplist.AddGoods(reader.Value);
                            }
                    }
                }
            }

            return _shoplist;
        }

        /// <summary>
        /// Opens the shop
        /// </summary>
        /// <param name="character">Character to show the shop</param>
        /// <param name="basenpc">Npc providing the v</param>
        public override void Open(Saga.PrimaryTypes.Character character, Saga.Templates.BaseNPC basenpc)
        {
            character.Tag = this;
            SMSG_SHOPLIST spkt = new SMSG_SHOPLIST();
            spkt.SessionId = character.id;
            spkt.SourceActor = basenpc.id;
            spkt.Zeny = basenpc.Zeny;
            for (int i = 0; i < list.Count; i++)
            {
                ShopPair c = list[i];
                if (c.NoStock == true || c.item.count > 0)
                    spkt.AddItem(c.item, c.NoStock, i);
            }
            character.client.Send((byte[])spkt);
            CommonFunctions.SendRebuylist(character);
        }
    }
}