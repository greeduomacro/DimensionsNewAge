using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0xF62, 0xF63 )]
	public class SpearMytheril : BaseSpear
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ArmorIgnore; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ParalyzingBlow; } }

		public override int AosStrengthReq{ get{ return 50; } }
		public override int AosSpeed{ get{ return 42; } }
		public override float MlSpeed{ get{ return 2.75f; } }

		public override int OldStrengthReq{ get{ return 30; } }
		public override int OldSpeed{ get{ return 46; } }

        public override int AosMinDamage { get { return ItemQualityHelper.GetWeaponDamageByItemQuality(DamageTypeEnum.DamageWeaponType.Spear, DamageTypeEnum.DamageType.AosMinDamage, CraftResource.Mytheril); } }
        public override int AosMaxDamage { get { return ItemQualityHelper.GetWeaponDamageByItemQuality(DamageTypeEnum.DamageWeaponType.Spear, DamageTypeEnum.DamageType.AosMaxDamage, CraftResource.Mytheril); } }
        public override int InitMinHits { get { return ItemQualityHelper.GetWeaponDamageByItemQuality(DamageTypeEnum.DamageWeaponType.Spear, DamageTypeEnum.DamageType.InitMinHits, CraftResource.Mytheril); } }
        public override int InitMaxHits { get { return ItemQualityHelper.GetWeaponDamageByItemQuality(DamageTypeEnum.DamageWeaponType.Spear, DamageTypeEnum.DamageType.InitMaxHits, CraftResource.Mytheril); } }

        public override SkillName DefSkill { get { return SkillName.Fencing; } }
        public override WeaponType DefType { get { return WeaponType.Piercing; } }
        public override WeaponAnimation DefAnimation { get { return WeaponAnimation.Pierce1H; } }

		[Constructable]
		public SpearMytheril() : base( 0xF62 )
		{
			Weight = 7.0;
            Hue = DimensionsNewAge.Scripts.HueOreConst.HueMytheril;
            Name = "Mytheril Short Spear";
		}

        public SpearMytheril(Serial serial)
            : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}