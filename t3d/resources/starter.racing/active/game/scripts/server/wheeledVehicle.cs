function WheeledVehicleData::create(%block)
{
   %obj = new WheeledVehicle() {
      dataBlock = %block;
   };
   return(%obj);
}


function WheeledVehicleData::onCollision(%this,%obj,%col,%vec,%speed)
{
   // Collision with other objects, including items
}   
