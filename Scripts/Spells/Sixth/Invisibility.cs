using System;
using System.Collections;
using Server;
using Server.Targeting;
using Server.Items;

namespace Server.Spells.Sixth
{
	public class InvisibilitySpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Invisibility", "An Lor Xen",
				206,
				9002,
				Reagent.Bloodmoss,
				Reagent.Nightshade
			);

        // EFFECT=
        public override double CastDelayFastScalar { get { return 0; } }
        public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds(1.7); } }
        public override bool ClearHandsOnCast { get { return false; } }
        public override int ScaleMana(int mana) { return 20; }

		public override SpellCircle Circle { get { return SpellCircle.Sixth; } }

        public override void SelectTarget()
        {
            Caster.Target = new InternalSphereTarget(this);
        }

        public override void OnSphereCast()
        {
            if (SpellTarget != null)
            {
                if (SpellTarget is Mobile)
                {
                    Target((Mobile)SpellTarget);
                }
                else
                {
                    Caster.SendAsciiMessage("This spell needs a target object");
                }
            }
            FinishSequence();
        }

	    public InvisibilitySpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public void Target( Mobile m )
		{
			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
            else if (!CheckLineOfSight(m))
            {
                this.DoFizzle();
                Caster.SendAsciiMessage("Target is not in line of sight");
            }
			else if ( m is Mobiles.BaseVendor || m is Mobiles.PlayerVendor || m is Mobiles.PlayerBarkeeper || m.AccessLevel > Caster.AccessLevel )
			{
				Caster.SendLocalizedMessage( 501857 ); // This spell won't work on that!
			}
			else if ( CheckBSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				Effects.SendLocationParticles( EffectItem.Create( new Point3D( m.X, m.Y, m.Z + 16 ), Caster.Map, EffectItem.DefaultDuration ), 0x376A, 10, 15, 5045 );
				m.PlaySound( 0x3C4 );

				m.Hidden = true;
				m.Combatant = null;
				m.Warmode = false;

				RemoveTimer( m );

				TimeSpan duration = TimeSpan.FromSeconds( (( 1.2 * Caster.Skills.Magery.Fixed) / 10 ));

				Timer t = new InternalTimer( m, duration );

				BuffInfo.RemoveBuff( m, BuffIcon.HidingAndOrStealth );
				BuffInfo.AddBuff( m, new BuffInfo( BuffIcon.Invisibility, 1075825, duration, m ) );	//Invisibility/Invisible

				m_Table[m] = t;

				t.Start();
			}

			FinishSequence();
		}

		private static Hashtable m_Table = new Hashtable();

		public static bool HasTimer( Mobile m )
		{
			return m_Table[m] != null;
		}

		public static void RemoveTimer( Mobile m )
		{
			Timer t = (Timer)m_Table[m];

			if ( t != null )
			{
				t.Stop();
				m_Table.Remove( m );
			}
		}

		private class InternalTimer : Timer
		{
			private Mobile m_Mobile;

			public InternalTimer( Mobile m, TimeSpan duration ) : base( duration )
			{
				Priority = TimerPriority.OneSecond;
				m_Mobile = m;
			}

			protected override void OnTick()
			{
				if ( !m_Mobile.Hidden )
				m_Mobile.RevealingAction();
				RemoveTimer( m_Mobile );
			}
		}

        private class InternalSphereTarget : Target
        {
            private InvisibilitySpell m_Owner;

            public InternalSphereTarget(InvisibilitySpell owner)
                : base(Core.ML ? 10 : 12, false, TargetFlags.Beneficial)
            {
                m_Owner = owner;
                m_Owner.Caster.SendAsciiMessage("Selecione o alvo...");
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Mobile)
                {
                    m_Owner.SpellTarget = o;
                    m_Owner.CastSpell();
                }
                else
                {
                    m_Owner.Caster.SendAsciiMessage("This spell needs a target object");
                }
            }

            protected override void OnTargetFinish(Mobile from)
            {
                if (m_Owner.SpellTarget == null)
                {
                    m_Owner.Caster.SendAsciiMessage("Target cancelado.");
                }
            }
        }

		public class InternalTarget : Target
		{
			private InvisibilitySpell m_Owner;

			public InternalTarget( InvisibilitySpell owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Beneficial )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Mobile )
				{
					m_Owner.Target( (Mobile)o );
				}
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}