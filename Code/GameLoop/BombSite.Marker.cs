
using Facepunch;

partial class BombSite : IMinimapLabel, IMinimapVolume
{
	Vector3 IMinimapElement.WorldPosition => WorldPosition;
	bool IMinimapElement.IsVisible( Pawn viewer ) => true;
	Color IMinimapVolume.Color => "rgba( #992d32, 0.25 )";
	Vector3 IMinimapVolume.Size => GetComponent<BoxCollider>().Scale;
	Color IMinimapVolume.LineColor => new Color32( 183, 85, 70 );
	string IMinimapLabel.Label => $"{(char)('A' + Index)}";
	Color IMinimapLabel.LabelColor => new Color32( 255, 199, 0 );
	Angles IMinimapVolume.Angles => Angles.Zero;
}
