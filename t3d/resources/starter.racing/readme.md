<h1>Starter.Racing</h1>
<p>This is a non-enhanced port of the original TGE starter.racing demo. Based on the resource as packaged in TGE 1.52.</p>
<p>Primarily a script resource, this includes one engine source file to fix potentially game-crashing bugs in guiSpeedometer.cpp found in T3D versions 3.9 and lower. To use the resource in stock T3D without rebuilding, the GuiSpeedometerHud control should be removed from /art/gui/playGui.gui before trying to play the game.</p>
<h2>Gameplay</h2>
<p>This is a simple multiplayer racer using typical Torque keyboard/mouse controls featuring a *very* basic demo level. The game begins and waits for network players to join. The race will start after a set wait period  elapses or when a player triggers race start (depending on settings). Players drive through series of checkpoints (defined by trigger objects) to complete a determined number of laps. When a player enters the final trigger the race is complete. A simple scoreboard of race wins is displayed for a set time and then new round will begin. If multiple levels have been defined, the game will cycle through them when progressing through rounds.</p>
<p>If the game is not set to autoStart, racing rounds must be started with the ctrl-s keyboard command.</p>
<h2>Usage</h2>
<p>The game is controlled by a handful of global variables set in /sripts/server/game.cs as follows:</p>
<table>
<tr><td>$Game::laps</td><td>Number of laps per race</td></tr>
<tr><td>$Game::autoStart</td><td>If true, the race starts automatically after WaitTime seconds.<br>If false, each race is started with a keyboard command (ctrl+s)</td></tr>
<tr><td>$Game::WaitTime</td><td>How long to wait for players to enter the game before starting a race (if autoStart=true).</td></tr>
<tr><td>$Game::EndGamePause</td><td>How long to display the scoreboard after a race is complete.</td></tr>
</table>
<p>Checkpoints are laid out using standard trigger objects defined with the following fields:</p>
<table>
<tr><td>datablock</td><td>must be "<b>CheckpointTrigger</b>".</td></tr>
<tr><td>checkpoint</td><td>Integer defining the order checkpoints should be entered.</td></tr>
<tr><td>isLast</td><td>Boolean defining which checkpoint ends the race.</td></tr>
</table>
