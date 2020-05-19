using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GTA;
using GTA.UI;
using GTA.Math;

namespace JetWorks
{
    public class Countermeasures
	{
		#region properties
		private static Dictionary<Countermeasures.Type, CountermeasurePtfx> ptfxDict;
		#endregion




		#region constructor
		/// <summary>
		/// Static constructor; invoke once during setup
		/// </summary>
		static Countermeasures()
		{
			// initialize particleFX dictionary
			ptfxDict = new Dictionary<Type, CountermeasurePtfx>
			{
				{
					Countermeasures.Type.Chaff, 
					new CountermeasurePtfx() {
						asset = new ParticleEffectAsset("scr_sm_counter"),
						effectName = "scr_sm_counter_chaff",
					}
				}
			};
		}
		#endregion





		#region typedefs
		public enum Type : int
		{
			Chaff,
			Smoke,
		}


		private struct CountermeasurePtfx {
			public ParticleEffectAsset asset;
			public string effectName;
		}
		#endregion




		#region publicMethods
		/// <summary>
		/// Activate ParticleEffects associated with a countermeasure. This will not affect
		/// any incoming projectiles.
		/// </summary>
		/// <param name="veh">Vehicle to deploy countermeasures from</param>
		/// <param name="type">Type of countermeasure. See <c>Countermeasures.Type</c></param>
		/// <param name="?"></param>
		/// <returns></returns>
		public static void deployCountermeasures(Vehicle veh, Countermeasures.Type type)
		{
			// prepare the ParticleEffectAsset
			CountermeasurePtfx ptfx = ptfxDict[type];
			ptfx.asset.Request();

			// create the PTFX
			switch (type)
			{
				case Type.Chaff:
					deployChaff(veh, ptfx); break;
			}
		}


		#endregion




		#region helperMethods

		private static void deployChaff (Vehicle veh, CountermeasurePtfx ptfx)
		{
			Vector3 position = veh.Position;
			Vector3 fwd = veh.ForwardVector;
			Vector3[] rotationArray = new Vector3[4] {
				new Vector3(fwd.X + 45f, fwd.Y, fwd.Z),
				new Vector3(fwd.X + 135f, fwd.Y, fwd.Z),
				new Vector3(fwd.X - 45f, fwd.Y, fwd.Z),
				new Vector3(fwd.X - 135f, fwd.Y, fwd.Z),
			};
			foreach (Vector3 rotation in rotationArray)
			{
				World.CreateParticleEffectNonLooped(ptfx.asset, ptfx.effectName, position, rotation, 3f);
			}
		}
		#endregion
	}
}
