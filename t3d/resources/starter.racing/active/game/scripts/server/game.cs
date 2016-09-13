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

$Game::laps = 3;			// Number of laps per race
$Game::WaitTime = 2;		// How long to wait for players to enter the game before starting a race (if autoStart=true).
$Game::EndGamePause = 7;	// How long the scoreboard displays for after a race is done
$Game::autoStart = false;	// If true, the race starts automatically after WaitTime seconds.
							// If false, each race is started with a keyboard command (ctrl+s)
//-----------------------------------------------------------------------------
// What kind of "player" is spawned is either controlled directly by the
// SpawnSphere or it defaults back to the values set here. This also controls
// which SimGroups to attempt to select the spawn sphere's from by walking down
// the list of SpawnGroups till it finds a valid spawn object.
// These override the values set in core/scripts/server/spawn.cs
//-----------------------------------------------------------------------------

// Leave $Game::defaultPlayerClass and $Game::defaultPlayerDataBlock as empty strings ("")
// to spawn a the $Game::defaultCameraClass as the control object.
$Game::DefaultPlayerClass = "";
$Game::DefaultPlayerDataBlock = "";
$Game::DefaultPlayerSpawnGroups = "CameraSpawnPoints PlayerSpawnPoints PlayerDropPoints";

//-----------------------------------------------------------------------------
// What kind of "camera" is spawned is either controlled directly by the
// SpawnSphere or it defaults back to the values set here. This also controls
// which SimGroups to attempt to select the spawn sphere's from by walking down
// the list of SpawnGroups till it finds a valid spawn object.
// These override the values set in core/scripts/server/spawn.cs
//-----------------------------------------------------------------------------
$Game::DefaultCameraClass = "Camera";
$Game::DefaultCameraDataBlock = "Observer";
$Game::DefaultCameraSpawnGroups = "CameraSpawnPoints PlayerSpawnPoints PlayerDropPoints";

// Global movement speed that affects all Cameras
$Camera::MovementSpeed = 30;

//-----------------------------------------------------------------------------
// GameConnection manages the communication between the server's world and the
// client's simulation. These functions are responsible for maintaining the
// client's camera and player objects.
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// This is the main entry point for spawning a control object for the client.
// The control object is the actual game object that the client is responsible
// for controlling in the client and server simulations. We also spawn a
// convenient camera object for use as an alternate control object. We do not
// have to spawn this camera object in order to function in the simulation.
//
// Called for each client after it's finished downloading the mission and is
// ready to start playing.
//-----------------------------------------------------------------------------
function GameConnection::onClientEnterGame(%this)
{
   commandToClient(%this, 'SyncClock', $Sim::Time - $Game::StartTime);
   commandToClient(%this, 'SetMaxLaps', $Game::Laps);
   
   // This function currently relies on some helper functions defined in
   // core/scripts/spawn.cs. For custom spawn behaviors one can either
   // override the properties on the SpawnSphere's or directly override the
   // functions themselves.

   // Find a spawn point for the camera
   %cameraSpawnPoint = pickCameraSpawnPoint($Game::DefaultCameraSpawnGroups);
   // Spawn a camera for this client using the found %spawnPoint
   %this.spawnCamera(%cameraSpawnPoint.getTransform());
	if(!$Game::Running)
	{
		// Create a car object.
		%this.spawnCar(%this);

         // Orbit the camera around the car
		%this.camera.setOrbitMode(%this.car, %this.car.getTransform(), 0.5, 4.5, 4.5);
	}

}

//-----------------------------------------------------------------------------
// Clean up the client's control objects
//-----------------------------------------------------------------------------
function GameConnection::onClientLeaveGame(%this)
{
   // Cleanup the camera
   if (isObject(%this.camera))
      %this.camera.delete();
   // Cleanup the player
   if (isObject(%this.car))
      %this.car.delete();
}

//-----------------------------------------------------------------------------
// Handle a player's death
//-----------------------------------------------------------------------------
function GameConnection::onDeath(%this, %sourceObject, %sourceClient, %damageType, %damLoc)
{
   // Clear out the name on the corpse
   if (isObject(%this.player))
   {
      if (%this.player.isMethod("setShapeName"))
         %this.player.setShapeName("");
   }

    // Switch the client over to the death cam
    if (isObject(%this.camera) && isObject(%this.player))
    {
        %this.camera.setMode("Corpse", %this.player);
        %this.setControlObject(%this.camera);
    }

    // Unhook the player object
    %this.player = 0;
}

//-----------------------------------------------------------------------------
//  Server, mission, and game management
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// The server has started up so do some game start up
//-----------------------------------------------------------------------------
function onServerCreated()
{
   // Server::GameType is sent to the master server.
   // This variable should uniquely identify your game and/or mod.
   $Server::GameType = "Torque 3D";

   // Server::MissionType sent to the master server.  Clients can
   // filter servers based on mission type.
   $Server::MissionType = "pureLIGHT";

   // GameStartTime is the sim time the game started. Used to calculated
   // game elapsed time.
   $Game::StartTime = 0;

   // Create the server physics world.
   physicsInitWorld( "server" );
   
   // Load up any objects or datablocks saved to the editor managed scripts
   %datablockFiles = new ArrayObject();
   %datablockFiles.add( "art/ribbons/ribbonExec.cs" );   
   %datablockFiles.add( "art/particles/managedParticleData.cs" );
   %datablockFiles.add( "art/particles/managedParticleEmitterData.cs" );
   %datablockFiles.add( "art/decals/managedDecalData.cs" );
   %datablockFiles.add( "art/datablocks/managedDatablocks.cs" );
   %datablockFiles.add( "art/forest/managedItemData.cs" );
   %datablockFiles.add( "art/datablocks/datablockExec.cs" );   
   loadDatablockFiles( %datablockFiles, true );

   // Run the other gameplay scripts in this folder
   exec("./scriptExec.cs");

   // Keep track of when the game started
   $Game::StartTime = $Sim::Time;
}

//-----------------------------------------------------------------------------
// This function is called as part of a server shutdown
//-----------------------------------------------------------------------------
function onServerDestroyed()
{
   // Destroy the server physcis world
   physicsDestroyWorld( "server" );   
}

//-----------------------------------------------------------------------------
// Called by loadMission() once the mission is finished loading
//-----------------------------------------------------------------------------
function onMissionLoaded()
{
   // Start the server side physics simulation
   physicsStartSimulation( "server" );

   // Nothing special for now, just start up the game play
   //startGame();
}

function serverCmdDoStartGame(%client){
	startGame();
	
}

//-----------------------------------------------------------------------------
// Called by endMission(), right before the mission is destroyed
//-----------------------------------------------------------------------------
function onMissionEnded()
{
   // Stop the server physics simulation
   physicsStopSimulation( "server" );
   
   // Normally the game should be ended first before the next
   // mission is loaded, this is here in case loadMission has been
   // called directly.  The mission will be ended if the server
   // is destroyed, so we only need to cleanup here.
   $Game::Running = false;
   $Game::Cycling = false;
}

//-----------------------------------------------------------------------------
// Called once the game has started
//-----------------------------------------------------------------------------
function startGame()
{
    if ($Game::Running)
    {
        error("startGame(): End the game first!");
        return;
    }

    //$Game::Running = true;
	%t = $Game::autoStart ? $Game::WaitTime : 0;
    schedule(%t * 1000, 0, "countThree");
}

//-----------------------------------------------------------------------------
// Called once the game has ended
//-----------------------------------------------------------------------------
function endGame()
{
   if (!$Game::Running)
   {
      error("endGame(): No game running!");
      return;
   }

   // Inform the client the game is over
   for( %clientIndex = 0; %clientIndex < ClientGroup.getCount(); %clientIndex++ )
   {
      %cl = ClientGroup.getObject( %clientIndex );
      commandToClient(%cl, 'GameEnd');
   }

   // Delete all the temporary mission objects
   resetMission();
   $Game::Running = false;
}

// *****************************************************************************
// *****************************************************************************
// starter.racing game control functions
// *****************************************************************************
// *****************************************************************************
function onGameDurationEnd()
{
   // This "redirect" is here so that we can abort the game cycle if
   // the $Game::Duration variable has been cleared, without having
   // to have a function to cancel the schedule.
   if ($Game::Duration && !isObject(EditorGui))
      cycleGame();
}


//-----------------------------------------------------------------------------

function cycleGame()
{
   // This is setup as a schedule so that this function can be called
   // directly from object callbacks.  Object callbacks have to be
   // carefull about invoking server functions that could cause
   // their object to be deleted.
   if (!$Game::Cycling) {
      $Game::Cycling = true;
      $Game::Schedule = schedule(0, 0, "onCycleExec");
   }
}

function onCycleExec()
{
   // End the current game and start another one, we'll pause for a little
   // so the end game victory screen can be examined by the clients.
   endGame();
   $Game::Schedule = schedule($Game::EndGamePause * 1000, 0, "onCyclePauseEnd");
}

function onCyclePauseEnd()
{
   $Game::Cycling = false;

   // Just cycle through the missions for now.
   %search = $Server::MissionFileSpec;
   for (%file = findFirstFile(%search); %file !$= ""; %file = findNextFile(%search)) {
      if (%file $= $Server::MissionFile) {
         // Get the next one, back to the first if there
         // is no next.
         %file = findNextFile(%search);
         if (%file $= "")
           %file = findFirstFile(%search);
         break;
      }
   }
   loadMission(%file);
}

function GameConnection::spawnCar(%this)
{
   // Combination create player and drop him somewhere
   %spawnPoint = pickPlayerSpawnPoint($Game::DefaultPlayerSpawnGroups);
   %this.createCar(%spawnPoint);
}   

//-----------------------------------------------------------------------------

function GameConnection::createCar(%this, %spawnPoint)
{
   if ( isObject(%this.car))  {
      // The client should not have a player currently
      // assigned.  Assigning a new one could result in 
      // a player ghost.	
      error( "Attempting to create an angus ghost!" );
   }

   // Create the player object
   %car = new WheeledVehicle() {
      dataBlock = CheetahCar;
      client = %this;
   };
   MissionCleanup.add(%car);
   // Player setup...
   %car.setTransform(%spawnPoint.getTransform());
   %car.setShapeName("Racer");

   // Update the camera to start with the player
   %this.camera.setTransform(%car.getEyeTransform());

   // Give the client control of the player
   %this.car = %car;
}



function countThree()
{
   // Tell the client to count.
   for( %clientIndex = 0; %clientIndex < ClientGroup.getCount(); %clientIndex++ ) {
      %cl = ClientGroup.getObject( %clientIndex );
      commandToClient(%cl, 'SetCounter', 3);
   }

	schedule(1200, 0, "countTwo");
}

function countTwo()
{
   // Tell the client to count.
   for( %clientIndex = 0; %clientIndex < ClientGroup.getCount(); %clientIndex++ ) {
      %cl = ClientGroup.getObject( %clientIndex );
      commandToClient(%cl, 'SetCounter', 2);
   }

	schedule(1200, 0, "countOne");
}

function countOne()
{
   // Tell the client to count.
   for( %clientIndex = 0; %clientIndex < ClientGroup.getCount(); %clientIndex++ ) {
      %cl = ClientGroup.getObject( %clientIndex );
      commandToClient(%cl, 'SetCounter', 1);
   }

	schedule(1200, 0, "startRace");
}

function startRace()
{
   // Inform the client we're starting up
   for( %clientIndex = 0; %clientIndex < ClientGroup.getCount(); %clientIndex++ ) {
      %cl = ClientGroup.getObject( %clientIndex );
      commandToClient(%cl, 'GameStart');

      // Other client specific setup..
      %cl.lap = 0;
      %cl.nextCheck = 1;
      %cl.setControlObject(%cl.car);
      %cl.camera.setFlyMode();
	%cl.setFirstPerson(false);
   }
   $Game::Running = true;
   // Start the game timer
//   if ($Game::Duration)
//      $Game::Schedule = schedule($Game::Duration * 1000, 0, "onGameDurationEnd" );
   
}
