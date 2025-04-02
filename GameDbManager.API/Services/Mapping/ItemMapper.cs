using GameDbManager.API.Models.Items;
using GameDbManager.API.Models.Items.Dtos;
using System.Collections.Generic;
using System.Linq;

namespace GameDbManager.API.Services.Mapping
{
    public static class ItemMapper
    {
        public static ItemDto ToDto(Item item)
        {
            if (item == null)
                return null;

            var dto = new ItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Icon = item.Icon,
                Weight = item.Weight,
                Price = item.Price,
                Sellable = item.Sellable,
                Tradeable = item.Tradeable,
                Dropable = item.Dropable,
                Destroyable = item.Destroyable,
                ItemType = item.GetType().Name,
                BodyPart = item.BodyPart,
                Grade = item.Grade,
                Crystallizable = item.Crystallizable ?? true,

                Stats = item.Stats?.Select(s => new ItemStatDto
                {
                    Id = s.Id,
                    StatType = s.StatType,
                    Stat = s.Stat,
                    Order = s.Order,
                    Value = s.Value
                }).ToList() ?? new List<ItemStatDto>(),

                Skills = item.Skills?.Select(s => new ItemSkillDto
                {
                    Id = s.Id,
                    SkillId = s.SkillId,
                    SkillLevel = s.SkillLevel
                }).ToList() ?? new List<ItemSkillDto>()
            };

            if (item is Armor armor)
            {
                dto.ArmorType = armor.ArmorType;
            }
            else if (item is Weapon weapon)
            {
                dto.WeaponType = weapon.WeaponType;
            }
            else if (item is Etc etc)
            {
                dto.Stackable = etc.Stackable;
                dto.EtcItemType = etc.ItemType;
            }

            return dto;
        }

        public static Item FromDto(ItemDto dto)
        {
            if (dto == null)
                return null;

            Item item;

            // Crear el tipo correcto de item basado en ItemType
            switch (dto.ItemType?.ToLower())
            {
                case "armor":
                    item = new Armor { ArmorType = dto.ArmorType };
                    break;
                case "weapon":
                    item = new Weapon { WeaponType = dto.WeaponType };
                    break;
                case "etc":
                    item = new Etc { Stackable = dto.Stackable, ItemType = dto.EtcItemType };
                    break;
                case "accessory":
                    item = new Accessory();
                    break;
                case "jewelry":
                    item = new Jewelry();
                    break;
                default:
                    item = new Etc(); // Default
                    break;
            }

            // Propiedades comunes
            item.Id = dto.Id;
            item.Name = dto.Name;
            item.Icon = dto.Icon;
            item.Weight = dto.Weight;
            item.Price = dto.Price;
            item.Sellable = dto.Sellable;
            item.Tradeable = dto.Tradeable;
            item.Dropable = dto.Dropable;
            item.Destroyable = dto.Destroyable;
            item.BodyPart = dto.BodyPart;
            item.Grade = dto.Grade;
            item.Crystallizable = dto.Crystallizable;

            // Colecciones
            item.Stats = dto.Stats?.Select(s => new ItemStat
            {
                Id = s.Id,
                StatType = s.StatType,
                Stat = s.Stat,
                Order = s.Order,
                Value = s.Value,
                Item = item
            }).ToList() ?? new List<ItemStat>();

            item.Skills = dto.Skills?.Select(s => new ItemSkill
            {
                Id = s.Id,
                SkillId = s.SkillId,
                SkillLevel = s.SkillLevel,
                Item = item
            }).ToList() ?? new List<ItemSkill>();

            return item;
        }
    }
}