﻿using Facepunch.UI;
using Sandbox.Events;

namespace Facepunch;

public record TeamScoreIncrementedEvent( Team Team, int Score ) : IGameEvent;

/// <summary>
/// Keeps track of a score for each team.
/// </summary>
public sealed class TeamScoring : Component,
	IGameEventHandler<ResetScoresEvent>,
	IGameEventHandler<TeamsSwappedEvent>
{
	[Sync( SyncFlags.FromHost )]
	public NetDictionary<Team, int> Scores { get; private set; } = new();

	/// <summary>
	/// What should we set the initial scores to?
	/// </summary>
	[Property] public int InitialScores { get; set; } = 0;

	/// <summary>
	/// You can define a scoring format for the scores, say you want to format in currency.
	/// </summary>
	[Sync( SyncFlags.FromHost )] public string ScoreFormat { get; set; } = "";
	[Sync( SyncFlags.FromHost )] public string ScorePrefix { get; set; } = "";

	public int MyTeamScore => Scores.GetValueOrDefault( Client.Local.Team );
	public int OpposingTeamScore => Scores.GetValueOrDefault( Client.Local.Team.GetOpponents() );

	public string MyTeamScoreFormatted => $"{ScorePrefix}{MyTeamScore.ToString( ScoreFormat )}";
	public string OpposingTeamScoreFormatted => $"{ScorePrefix}{OpposingTeamScore.ToString( ScoreFormat )}";

	void IGameEventHandler<ResetScoresEvent>.OnGameEvent( ResetScoresEvent eventArgs )
	{
		Scores.Clear();

		SetInitial();
	}

	public Team GetHighest()
	{
		var tScore = Scores.GetValueOrDefault( Team.Terrorist );
		var ctScore = Scores.GetValueOrDefault( Team.CounterTerrorist );

		if ( tScore > ctScore ) return Team.Terrorist;
		if ( ctScore > tScore ) return Team.CounterTerrorist;

		return Team.Unassigned;
	}

	public void SetInitial()
	{
		if ( InitialScores != 0 )
		{
			Scores[Team.Terrorist] = InitialScores;
			Scores[Team.CounterTerrorist] = InitialScores;
		}
	}

	public string GetFormattedScore( Team team )
	{
		return Scores.GetValueOrDefault( team, 0 ).ToString( ScoreFormat );
	}

	public void IncrementScore( Team team, int amount = 1 )
	{
		var score = Scores.GetValueOrDefault( team ) + amount;

		Scores[team] = score;

		Scene.Dispatch( new TeamScoreIncrementedEvent( team, score ) );
	}

	public void Flip()
	{
		var ctScores = Scores.GetValueOrDefault( Team.CounterTerrorist );
		var tScores = Scores.GetValueOrDefault( Team.Terrorist );

		Scores[Team.Terrorist] = ctScores;
		Scores[Team.CounterTerrorist] = tScores;
	}

	void IGameEventHandler<TeamsSwappedEvent>.OnGameEvent( TeamsSwappedEvent eventArgs )
	{
		Flip();
	}

	protected override void OnStart()
	{
		SetInitial();
	}
}

/// <summary>
/// Transitions to specified states if one team's score is higher than the other's.
/// </summary>
public sealed class CompareTeamScores : Component,
	IGameEventHandler<EnterStateEvent>,
	IGameEventHandler<UpdateStateEvent>
{
	/// <summary>
	/// Minimum difference between scores to trigger a transition.
	/// </summary>
	[Property]
	public int MinMargin { get; set; } = 1;

	[Property]
	public StateComponent TerroristVictoryState { get; set; }

	[Property]
	public StateComponent CounterTerroristVictoryState { get; set; }

	private void CheckScores()
	{
		var teamScoring = GameMode.Instance.Get<TeamScoring>( true );

		var tScore = teamScoring.Scores.GetValueOrDefault( Team.Terrorist );
		var ctScore = teamScoring.Scores.GetValueOrDefault( Team.CounterTerrorist );

		if ( tScore >= ctScore + MinMargin )
		{
			GameMode.Instance.StateMachine.Transition( TerroristVictoryState );
		}
		else if ( ctScore >= tScore + MinMargin )
		{
			GameMode.Instance.StateMachine.Transition( CounterTerroristVictoryState );
		}
	}

	void IGameEventHandler<EnterStateEvent>.OnGameEvent( EnterStateEvent eventArgs )
	{
		CheckScores();
	}

	void IGameEventHandler<UpdateStateEvent>.OnGameEvent( UpdateStateEvent eventArgs )
	{
		CheckScores();
	}
}
