﻿using Editor;
using Sandbox;

public class HideObject
{
	[Menu("Editor", "Nicked/Toggle Selected" )]
	[Shortcut( "hc1.editor.toggleselect", "H", ShortcutType.Window )]
	static void ToggleSelection()
	{
		using var scope = SceneEditorSession.Scope();

		foreach ( var item in EditorScene.Selection )
		{
			var thing = item as GameObject;

			thing.Enabled = !thing.Enabled;
		}

		SceneEditorSession.Active.FullUndoSnapshot( "Toggle Selection" );
	}
}
