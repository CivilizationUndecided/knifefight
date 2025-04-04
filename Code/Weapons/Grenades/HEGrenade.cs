namespace Facepunch;

[Title( "HE Grenade" )]
public partial class HEGrenade : BaseGrenade, IMarkerObject, IDescription
{
	[Property] public float DamageRadius { get; set; } = 512f;
	[Property] public float MaxDamage { get; set; } = 100f;
	[Property] public float ScreenShakeIntensity { get; set; } = 3f;
	[Property] public float ScreenShakeLifeTime { get; set; } = 1.5f;
	[Property] public Curve DamageFalloff { get; set; } = new Curve( new Curve.Frame( 1.0f, 1.0f ), new Curve.Frame( 0.0f, 0.0f ) );

	/// <summary>
	/// Where is the marker?
	/// </summary>
	Vector3 IMarkerObject.MarkerPosition => WorldPosition;

	/// <summary>
	/// How far can we see this marker?
	/// </summary>
	float IMarkerObject.MarkerMaxDistance => 512f;

	/// <summary>
	/// What icon?
	/// </summary>
	string IMarkerObject.MarkerIcon => "/ui/weapons/grenades/he_grenade_01.png";

	/// <summary>
	/// What text?
	/// </summary>
	string IMarkerObject.DisplayText => "HE Grenade";
	string IDescription.DisplayName => "HE Grenade";

	protected override void Explode()
	{
		if ( Networking.IsHost )
			Explosion.AtPoint( WorldPosition, DamageRadius, MaxDamage, Player, this, DamageFalloff );

		var screenShaker = ScreenShaker.Main;
		var viewer = Client.Viewer;
		
		if ( screenShaker.IsValid() && viewer.IsValid() )
		{
			var distance = viewer.GameObject.WorldPosition.Distance( WorldPosition );
			var falloff = DamageFalloff;
			
			if ( falloff.Frames.IsEmpty )
			{
				falloff = new( new Curve.Frame( 1f, 1f ), new Curve.Frame( 0f, 0f ) );
			}

			var radiusWithPadding = DamageRadius * 1.2f;
			
			if ( distance <= radiusWithPadding )
			{
				var scalar = falloff.Evaluate( distance / radiusWithPadding );
				var shake = new ScreenShake.Random( ScreenShakeIntensity * scalar, ScreenShakeLifeTime );
				screenShaker.Add( shake );
			}
		}

		base.Explode();
	}
}
