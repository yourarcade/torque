//-----------------------------------------------------------------------------
// Copyright (c) 2012 GarageGames, LLC
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//-----------------------------------------------------------------------------

//----------------------------------------------------------------------------
// Misc server commands
//----------------------------------------------------------------------------

function clientCmdSyncClock(%time)
{
   // Time update from the server, this is only sent at the start of a mission
   // or when a client joins a game in progress.
}

//----------------------------------------------------------------------------
// Game start / end events sent from the server
//----------------------------------------------------------------------------

function clientCmdSetCounter(%count)
{
	if(%count == 3)
		counter.setVisible(true);

	counter.setBitmap("art/gui/" @ %count @ ".png");
	sfxPlayOnce(Audio2D, "art/sound/beep1.wav");


	
}


function clientCmdCheckpointPassed(%count)
{
   sfxPlayOnce(Audio2D, "art/sound/beep1.wav");
}


function clientCmdGameStart(%seq)
{
	// Display the GO bitmap and play a sound
	counter.setBitmap("art/gui/go.png");
	sfxPlayOnce(Audio2D, "art/sound/beep2.wav");
	// Remove the GO after a second.
	counter.schedule(1000, setVisible, false);
}

function clientCmdGameEnd(%seq)
{
   // Stop local activity... the game will be destroyed on the server
   sfxStopAll();

   // Copy the current scores from the player list into the
   // end game gui (bit of a hack for now).
   EndGameGuiList.clear();
   for (%i = 0; %i < PlayerListGuiList.rowCount(); %i++) {
      %text = PlayerListGuiList.getRowText(%i);
      %id = PlayerListGuiList.getRowId(%i);
      EndGameGuiList.addRow(%id,%text);
   }
   EndGameGuiList.sortNumerical(1,false);

   // Display the end-game screen
   Canvas.setContent(EndGameGui);
}
