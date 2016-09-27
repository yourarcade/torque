// ============================================================
// Project            :  starter.racing
// File               :  starter.racing\game\art\datablocks\vehicles\tgeBuggyCar.cs
// Copyright          :  
// Author             :  kent
// Created on         :  Tuesday, September 13, 2016 12:29 PM
//
// Editor             :  TorqueDev v. 1.2.5129.4848
//
// Description        :  
//                    :  
//                    :  
// ============================================================
//-----------------------------------------------------------------------------
// Torque Game Engine 
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------

// Information extacted from the shape.
//
// Wheel Sequences
//    spring#        Wheel spring motion: time 0 = wheel fully extended,
//                   the hub must be displaced, but not directly animated
//                   as it will be rotated in code.
// Other Sequences
//    steering       Wheel steering: time 0 = full right, 0.5 = center
//    breakLight     Break light, time 0 = off, 1 = breaking
//
// Wheel Nodes
//    hub#           Wheel hub, the hub must be in it's upper position
//                   from which the springs are mounted.
//
// The steering and animation sequences are optional.
// The center of the shape acts as the center of mass for the car.

//-----------------------------------------------------------------------------
datablock SFXProfile(BuggyEngineSound){
   preload = "1";
   description = "AudioCloseLoop3D";
   fileName = "art/sound/cheetah/cheetah_engine.ogg";
};

datablock ParticleData(BuggyTireParticle)
{
   textureName          = "art/shapes/tgeBuggy/dustParticle.png";
   dragCoefficient      = 2.0;
   gravityCoefficient   = -0.1;
   inheritedVelFactor   = 0.1;
   constantAcceleration = 0.0;
   lifetimeMS           = 1000;
   lifetimeVarianceMS   = 400;
   colors[0]     = "0.46 0.36 0.26 1.0";
   colors[1]     = "0.46 0.46 0.36 0.0";
   sizes[0]      = 1.0;
   sizes[1]      = 4.0;
};

datablock ParticleEmitterData(BuggyTireEmitter)
{
   ejectionPeriodMS = 20;
   periodVarianceMS = 10;
   ejectionVelocity = 1;
   velocityVariance = 1.0;
   ejectionOffset   = 0.0;
   thetaMin         = 0;
   thetaMax         = 60;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "buggyTireParticle";
};


//----------------------------------------------------------------------------

datablock WheeledVehicleTire(BuggyCarTire)
{
   // Tires act as springs and generate lateral and longitudinal
   // forces to move the vehicle. These distortion/spring forces
   // are what convert wheel angular velocity into forces that
   // act on the rigid body.
   //shapeFile = "~/data/shapes/car/wheel.dts";
   shapeFile = "art/shapes/tgeBuggy/wheel.dts";
   staticFriction = 4.2;
   kineticFriction = 3.15;
   rollingResist = .125; // Advanced Engine:
   // Spring that generates lateral tire forces
   lateralForce = 32000;
   lateralDamping = 28000;   // Original:6000
   lateralRelaxation = 0.6;

   // Spring that generates longitudinal tire forces
   longitudinalForce = 18000;
   longitudinalDamping = 3000;      // (4000)
   longitudinalRelaxation = 1;
};

datablock WheeledVehicleSpring(BuggyCarSpring)
{
   // Wheel suspension properties
   length = 0.60;             // Suspension travel
   force = 2500;              // Spring force
   damping = 5800;             // Spring damping
   antiSwayForce = 8;         // Lateral anti-sway force (3)
};

datablock WheeledVehicleEngine(BuggyEngine) {
	// Default Engine info...	
   MinRPM = 10.0;
	MaxRPM = 5000.0;
	idleRPM = 500.0;
   
	throttleIdle = 0.1;
	
	numGears = 5;
	gearRatios[0] = 3.78;
	gearRatios[1] = 2.06;
	gearRatios[2] = 1.35;
	gearRatios[3] = 0.97;
	gearRatios[4] = 0.74;
	
/*	gearRatios[0] = 10;
	gearRatios[1] = 9;
	gearRatios[2] = 8;
	gearRatios[3] = 7;
	gearRatios[4] = 6;
*/
   
  diffRatio = 2.9;
	reverseRatio = 3.60;
	
  shiftUpRPM = 3500.0;
	shiftDownRPM = 1800.0;

	numTorqueLevels = 8;
	
	rpmValues[0] = 500.0;
	torqueLevel[0] = 170.0;	
	rpmValues[1] = 1000.0;
	torqueLevel[1] = 175.0;	
	rpmValues[2] = 1500.0;
	torqueLevel[2] = 80.0;	
	rpmValues[3] = 2000.0;
	torqueLevel[3] = 85.0;
	rpmValues[4] = 2500.0;
	torqueLevel[4] = 90.0;	
	rpmValues[5] = 3000.0;
	torqueLevel[5] = 100.0;	
	rpmValues[6] = 3500.0;
	torqueLevel[6] = 95.0;
	rpmValues[7] = 4000.0;
	torqueLevel[7] = 110.0;	
	rpmValues[8] = 4500.0;
	torqueLevel[8] = 200.0;
};

datablock WheeledVehicleData(TgeBuggyCar)
{
   category = "Vehicles";
   shapeFile = "art/shapes/tgeBuggy/buggy.dts";
   emap = true;

   maxDamage = 1.0;
   destroyedLevel = 0.5;

   maxSteeringAngle = 0.385;  // Maximum steering angle, should match animation
   steeringBoostVelocity = 12;
   steeringBoost = .3;
   tireEmitter = TireEmitter; // All the tires use the same dust emitter

   // 3rd person camera setting
   useEyePoint = true;		  // Use the vehile eye (uses player eye if false)
   cameraRoll = false;         // Roll the camera with the vehicle
   cameraMaxDist = 10.8;         // Far distance from vehicle
   cameraOffset = 6.5;        // Vertical offset from camera mount point
   cameraLag = 0.26;           // Velocity lag of camera
   cameraDecay = 1.25;        // Decay per sec. rate of velocity lag

   // Rigid Body
   mass = 380;
   massCenter = "0 -0.2 0";    // Center of mass for rigid body
   massBox = "0 0 0";         // Size of box used for moment of inertia,
                              // if zero it defaults to object bounding box
   drag = 0.6;                // Drag coefficient
   bodyFriction = 0.6;
   bodyRestitution = 0.4;
	
   minImpactSpeed = 5;        // Impacts over this invoke the script callback
   softImpactSpeed = 5;       // Play SoftImpact Sound
   hardImpactSpeed = 15;      // Play HardImpact Sound
   integration = 8;           // Physics integration: TickSec/Rate
   collisionTol = 0.1;        // Collision distance tolerance
   contactTol = 0.1;          // Contact velocity tolerance


   // Engine
   engineTorque = 4300;       // Engine power
   engineBrake = "5000";         // Braking when throttle is 0
   maxWheelSpeed = 50;        // Engine scale by current speed / max speed   
   
   // Brakes
   brakeTorque = "10000";        // When brakes are applied




   // Energy
   maxEnergy = 100;
   jetForce = 3000;
   minJetEnergy = 30;
   jetEnergyDrain = 2;

   // Sounds
//   jetSound = ScoutThrustSound;
   engineSound = BuggyEngineSound;
//   squealSound = ScoutSquealSound;
//   softImpactSound = SoftImpactSound;
//   hardImpactSound = HardImpactSound;
//   wheelImpactSound = WheelImpactSound;

//   explosion = VehicleExplosion;

   maxMountSpeed = 0.1;
   mountDelay = 2;
   dismountDelay = 1;

   stationaryThreshold = 0.5;
   maxDismountSpeed = 0.1;
   numMountPoints = 2;
   mountable = true;

   mountPose[0] = "Sitting";
   mountPointTransform[0] = "0 0 0 0 0 1 0";
   isProtectedMountPoint[0] = false;   
   mountPose[1] = "Sitting";
   mountPointTransform[1] = "0 0 0 0 0 1 0";
   isProtectedMountPoint[1] = false;

};
