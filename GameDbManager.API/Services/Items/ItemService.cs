using GameDbManager.API.Data;
using GameDbManager.API.Models.Items;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace GameDbManager.API.Services.Items
{
    public class ItemService
    {
        private readonly GameDbContext _context;

        public ItemService(GameDbContext context)
        {
            _context = context;
        }

        public void ImportItemsFromXml(string xmlContent)
        {
            var xml = XDocument.Parse(xmlContent);
            var items = new List<Item>();

            foreach (var element in xml.Root.Elements())
            {
                Item item = null;
                switch (element.Name.LocalName)
                {
                    case "accessorie":
                        item = new Accessory
                        {
                            Id = (int)element.Attribute("id"),
                            Name = (string)element.Attribute("name"),
                            Icon = (string)element.Element("set").Attribute("icon"),
                            BodyPart = (string)element.Element("set").Attribute("bodyPart"),
                            Grade = (string)element.Element("set").Attribute("grade"),
                            Crystallizable = (bool)element.Element("set").Attribute("crystallizable"),
                            Weight = (int)element.Element("set").Attribute("weight"),
                            Price = (int)element.Element("set").Attribute("price"),
                            Sellable = (bool)element.Element("set").Attribute("sellable"),
                            Tradeable = (bool)element.Element("set").Attribute("tradeable"),
                            Dropable = (bool)element.Element("set").Attribute("dropable"),
                            Destroyable = (bool)element.Element("set").Attribute("destroyable")
                        };
                        break;
                    case "armor":
                        var armor = new Armor
                        {
                            Id = (int)element.Attribute("id"),
                            Name = (string)element.Attribute("name"),
                            ArmorType = (string)element.Attribute("armorType"),
                            Icon = (string)element.Element("set").Attribute("icon"),
                            BodyPart = (string)element.Element("set").Attribute("bodyPart"),
                            Grade = (string)element.Element("set").Attribute("grade"),
                            Crystallizable = (bool)element.Element("set").Attribute("crystallizable"),
                            Weight = (int)element.Element("set").Attribute("weight"),
                            Price = (int)element.Element("set").Attribute("price"),
                            Sellable = (bool)element.Element("set").Attribute("sellable"),
                            Tradeable = (bool)element.Element("set").Attribute("tradeable"),
                            Dropable = (bool)element.Element("set").Attribute("dropable"),
                            Destroyable = (bool)element.Element("set").Attribute("destroyable"),
                            Stats = new List<Stat>()
                        };

                        foreach (var statElement in element.Element("stats")?.Elements() ?? new List<XElement>())
                        {
                            var stat = new Stat
                            {
                                Name = (string)statElement.Attribute("stat"),
                                Order = (string)statElement.Attribute("order"),
                                Value = (int)statElement.Attribute("val")
                            };
                            armor.Stats.Add(stat);
                        }
                        item = armor;
                        break;
                    case "etc":
                        item = new Etc
                        {
                            Id = (int)element.Attribute("id"),
                            Name = (string)element.Attribute("name"),
                            ItemType = (string)element.Attribute("itemType"),
                            Icon = (string)element.Element("set").Attribute("icon"),
                            Weight = (int)element.Element("set").Attribute("weight"),
                            Price = (int)element.Element("set").Attribute("price"),
                            Stackable = (bool)element.Element("set").Attribute("stackable"),
                            Sellable = (bool)element.Element("set").Attribute("sellable"),
                            Tradeable = (bool)element.Element("set").Attribute("tradeable"),
                            Dropable = (bool)element.Element("set").Attribute("dropable"),
                            Destroyable = (bool)element.Element("set").Attribute("destroyable")
                        };
                        break;
                    case "jewelry":
                        var jewelry = new Jewelry
                        {
                            Id = (int)element.Attribute("id"),
                            Name = (string)element.Attribute("name"),
                            Icon = (string)element.Element("set").Attribute("icon"),
                            BodyPart = (string)element.Element("set").Attribute("bodyPart"),
                            Grade = (string)element.Element("set").Attribute("grade"),
                            Crystallizable = (bool)element.Element("set").Attribute("crystallizable"),
                            Weight = (int)element.Element("set").Attribute("weight"),
                            Price = (int)element.Element("set").Attribute("price"),
                            Sellable = (bool)element.Element("set").Attribute("sellable"),
                            Tradeable = (bool)element.Element("set").Attribute("tradeable"),
                            Dropable = (bool)element.Element("set").Attribute("dropable"),
                            Destroyable = (bool)element.Element("set").Attribute("destroyable"),
                            Stats = new List<Stat>()
                        };

                        foreach (var statElement in element.Element("stats")?.Elements() ?? new List<XElement>())
                        {
                            var stat = new Stat
                            {
                                Name = (string)statElement.Attribute("stat"),
                                Order = (string)statElement.Attribute("order"),
                                Value = (int)statElement.Attribute("val")
                            };
                            jewelry.Stats.Add(stat);
                        }
                        item = jewelry;
                        break;
                    case "weapon":
                        var weapon = new Weapon
                        {
                            Id = (int)element.Attribute("id"),
                            Name = (string)element.Attribute("name"),
                            WeaponType = (string)element.Attribute("weaponType"),
                            Icon = (string)element.Element("set").Attribute("icon"),
                            BodyPart = (string)element.Element("set").Attribute("bodyPart"),
                            Grade = (string)element.Element("set").Attribute("grade"),
                            Crystallizable = (bool)element.Element("set").Attribute("crystallizable"),
                            Weight = (int)element.Element("set").Attribute("weight"),
                            Price = (int)element.Element("set").Attribute("price"),
                            Sellable = (bool)element.Element("set").Attribute("sellable"),
                            Tradeable = (bool)element.Element("set").Attribute("tradeable"),
                            Dropable = (bool)element.Element("set").Attribute("dropable"),
                            Destroyable = (bool)element.Element("set").Attribute("destroyable"),
                            Stats = new List<Stat>()
                        };

                        foreach (var statElement in element.Element("stats")?.Elements() ?? new List<XElement>())
                        {
                            var stat = new Stat
                            {
                                Name = (string)statElement.Attribute("stat"),
                                Order = (string)statElement.Attribute("order"),
                                Value = (int)statElement.Attribute("val")
                            };
                            weapon.Stats.Add(stat);
                        }
                        item = weapon;
                        break;
                }

                if (item != null)
                {
                    items.Add(item);
                }
            }

            _context.Items.AddRange(items);
            _context.SaveChanges();
        }

        public List<Item> GetAllItems()
        {
            return _context.Items.ToList();
        }

        public Item GetItemById(int id)
        {
            return _context.Items.Find(id);
        }

        public void CreateItem(Item item)
        {
            _context.Items.Add(item);
            _context.SaveChanges();
        }

        public void UpdateItem(Item item)
        {
            _context.Items.Update(item);
            _context.SaveChanges();
        }

        public void DeleteItem(int id)
        {
            var item = _context.Items.Find(id);
            if (item != null)
            {
                _context.Items.Remove(item);
                _context.SaveChanges();
            }
        }
    }
}