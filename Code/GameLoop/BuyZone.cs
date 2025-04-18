﻿
namespace Facepunch;

/// <summary>
/// A buy zone where we can buy weapons in certain gamemodes.
/// </summary>
internal class BuyZone : Component, IMinimapIcon, IMinimapVolume
{
	[Property]
	public Team Team { get; set; }

	public Color Color => Team.GetColor().WithAlpha( 0.1f );
	public Color LineColor => Team.GetColor().WithAlpha( 0.5f );

	public Vector3 Size => GetComponent<BoxCollider>().Scale;

	string IMinimapIcon.IconPath => Team.GetIconPath();
	Angles IMinimapVolume.Angles => GameObject.WorldRotation.Angles();

	int IMinimapIcon.IconOrder => 15;

	Vector3 IMinimapElement.WorldPosition => WorldPosition;

	bool IMinimapElement.IsVisible( Pawn viewer ) => true;
}
