using Sandbox.Events;

namespace Facepunch;

public partial class PlayerLoadout : Component
{
	[Property] public Client Client { get; set; }

	[Property] public List<EquipmentResource> Equipment { get; set; }
	[Property, Sync( SyncFlags.FromHost )] public bool HasDefuseKit { get; set; }

	/// <summary>
	/// Clears the player's loadout equipment.
	/// </summary>
	public void SetFrom( PlayerPawn playerPawn )
	{
		Equipment.Clear();
		Equipment.AddRange( playerPawn.Inventory.Equipment.Select( x => x.Resource ) );
	}
}
