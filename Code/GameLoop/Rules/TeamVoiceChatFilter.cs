namespace Facepunch;

/// <summary>
/// Filters voice chat to teams, so players on a team can only hear each other.
/// </summary>
public partial class TeamVoiceChatFilter : Component, IVoiceFilter
{
	private bool DontSendFilter( Client player )
	{
		// Send voices to spectators
		if ( player.Team == Team.Unassigned ) return false;

		// Don't send to enemies
		return !player.IsFriendly( Client.Local );
	}

	IEnumerable<Connection> IVoiceFilter.GetExcludeFilter()
	{
		return Scene.GetAllComponents<Client>()
			.Where( DontSendFilter )
			.Select( x => x.Connection );
	}
}
