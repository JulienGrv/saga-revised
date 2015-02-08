using Saga.IO;
using Saga.Packets;
using System.IO;

namespace Saga.Structures.Collections
{
    /// <summary>
    /// Collection of Rag2Items used to open a bookstore window where
    /// actord can buy items.
    /// </summary>
    public class BookstoreCollection : BaseShopCollection
    {
        /// <summary>
        /// Creates a bookstore collection from a file
        /// </summary>
        /// <param name="filename">File containing bookstore data</param>
        /// <returns>BookstoreCollection</returns>
        public static BookstoreCollection FromFile(string filename)
        {
            BookstoreCollection _shoplist = new BookstoreCollection();
            if (File.Exists(filename))
            {
                using (ShopReader reader = ShopReader.Open(filename))
                {
                    while (reader.Read())
                    {
                        if (reader.HasInformation == true)
                            _shoplist.AddGoods(reader.Value);
                    }
                }
            }

            return _shoplist;
        }

        /// <summary>
        /// Opens the bookstore.
        /// </summary>
        /// <param name="character">Character to show the bookstore</param>
        /// <param name="basenpc">Npc providing the bookstore</param>
        public override void Open(Saga.PrimaryTypes.Character character, Saga.Templates.BaseNPC basenpc)
        {
            character.Tag = this;
            SMSG_SENDBOOKLIST spkt = new SMSG_SENDBOOKLIST();
            spkt.SessionId = character.id;
            spkt.SourceActor = basenpc.id;
            spkt.Zeny = basenpc.Zeny;
            foreach (BaseShopCollection.ShopPair c in this.list)
                spkt.Add(c.item.info.item);
            character.client.Send((byte[])spkt);
        }
    }
}