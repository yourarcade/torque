// ============================================================
// Project            :  starter.racing
// File               :  ..\..\..\..\T3D_clean\My Projects\starter.racing\game\scripts\server\tgeBuggy.cs
// Copyright          :  
// Author             :  kent
// Created on         :  Tuesday, September 13, 2016 12:47 PM
//
// Editor             :  TorqueDev v. 1.2.5129.4848
//
// Description        :  
//                    :  
//                    :  
// ============================================================


function TgeBuggyCar::onAdd(%this,%obj)
{
   // Setup the car with some defaults tires & springs
   for (%i = %obj.getWheelCount() - 1; %i >= 0; %i--) {
      %obj.setWheelTire(%i,BuggyCarTire);
      %obj.setWheelSpring(%i,BuggyCarSpring);
      %obj.setWheelPowered(%i,false);
   }
   
   // Steer front tires
   %obj.setWheelSteering(0,1);
   %obj.setWheelSteering(1,1);

   // Only power the two rear wheels...
   %obj.setWheelPowered(2,true);
   %obj.setWheelPowered(3,true);
   
   // Set engine and overrides
   %obj.setEngine( BuggyEngine );
   %obj.fuelFlow = .2;

}