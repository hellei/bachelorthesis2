/****************************************
	FlockController Editor v2.11		
	Copyright 2012 Unluck Software	
 	www.chemicalbliss.com																															
*****************************************/
@CustomEditor (FlockController)
@CanEditMultipleObjects

class FlockControllerEditor extends Editor {
    function OnInspectorGUI () {
    	target._childPrefab = EditorGUILayout.ObjectField("Bird Prefab", target._childPrefab, typeof(FlockChild),false) as FlockChild;
    	EditorGUILayout.LabelField("Drag & Drop bird prefab from project folder", EditorStyles.miniLabel); 
    	EditorGUILayout.Space();
    	EditorGUILayout.LabelField("Size of area the flock roams within", EditorStyles.boldLabel); 	
    	target._positionSphere = EditorGUILayout.IntField("Roaming Area Width" , target._positionSphere);	
    	target._positionSphereHeight = EditorGUILayout.IntField("Roaming Area Height" , target._positionSphereHeight);
    	EditorGUILayout.Space();
    	EditorGUILayout.LabelField("Size of the flock", EditorStyles.boldLabel);
    	
    	target._childAmount = EditorGUILayout.Slider("Bird Amount", target._childAmount, 0,999);
    	target._spawnSphere = EditorGUILayout.FloatField("Flock Width" , target._spawnSphere);
    	target._spawnSphereHeight = EditorGUILayout.FloatField("Flock Height" , target._spawnSphereHeight);
    	target._slowSpawn = EditorGUILayout.Toggle("Slowly Spawn Birds" , target._slowSpawn);
    	EditorGUILayout.Space();
    	EditorGUILayout.LabelField("Behaviors and Appearance", EditorStyles.boldLabel); 
    	EditorGUILayout.LabelField("Change how the birds move and behave", EditorStyles.miniLabel);
    	target._minSpeed = EditorGUILayout.FloatField("Birds Min Speed" , target._minSpeed);
    	target._maxSpeed = EditorGUILayout.FloatField("Birds Max Speed" , target._maxSpeed);
    	target._diveValue = EditorGUILayout.FloatField("Birds Dive Depth" , target._diveValue);  	
    	target._diveFrequency = EditorGUILayout.Slider("Birds Dive Chance" , target._diveFrequency, 0.0, .7);
    	target._soarFrequency = EditorGUILayout.Slider("Birds Soar Chance" , target._soarFrequency, 0.0, 1.0);
    	target._soarMaxTime = EditorGUILayout.FloatField("Soar Time (0 = Always)" , target._soarMaxTime);
    	
    	EditorGUILayout.Space();
    	target._minDamping = EditorGUILayout.FloatField("Min Damping Turns" , target._minDamping); 	
    	target._maxDamping = EditorGUILayout.FloatField("Max Damping Turns" , target._maxDamping);
    	EditorGUILayout.LabelField("Bigger number = faster turns", EditorStyles.miniLabel);  
    	EditorGUILayout.Space();
    	
    	target._minScale = EditorGUILayout.FloatField("Birds Min Scale" , target._minScale);
    	target._maxScale = EditorGUILayout.FloatField("Birds Max Scale" , target._maxScale);
    	EditorGUILayout.LabelField("Randomize size of birds when added", EditorStyles.miniLabel);
    	
    	EditorGUILayout.Space();
    	EditorGUILayout.LabelField("Disable Pitch Rotation", EditorStyles.boldLabel);
    	EditorGUILayout.LabelField("Flattens out rotation when flying or soaring upwards", EditorStyles.miniLabel);   	
    	target._flatSoar = EditorGUILayout.Toggle("Flat Soar" , target._flatSoar);
		target._flatFly = EditorGUILayout.Toggle("Flat Fly" , target._flatFly);
 		
 		EditorGUILayout.Space();
    	EditorGUILayout.LabelField("Animations", EditorStyles.boldLabel);
    	target._soarAnimation = EditorGUILayout.TextField("Soar Animation", target._soarAnimation);
    	target._flapAnimation = EditorGUILayout.TextField("Flap Animation", target._flapAnimation);
    	target._idleAnimation = EditorGUILayout.TextField("Idle Animation", target._idleAnimation);
    	target._minAnimationSpeed = EditorGUILayout.FloatField("Min Anim Speed" , target._minAnimationSpeed);
    	target._maxAnimationSpeed = EditorGUILayout.FloatField("Max Anim Speed" , target._maxAnimationSpeed);  	
		
    	EditorGUILayout.Space();
    	EditorGUILayout.LabelField("Bird Trigger Flock Waypoint", EditorStyles.boldLabel);
    	EditorGUILayout.LabelField("Birds own waypoit triggers a new flock waypoint", EditorStyles.miniLabel);
    	target._childTriggerPos = EditorGUILayout.Toggle("Bird Trigger Waypoint" , target._childTriggerPos);
    	target._waypointDistance = EditorGUILayout.FloatField("Distance To Waypoint" , target._waypointDistance);
    	
    	EditorGUILayout.Space();
		EditorGUILayout.LabelField("Automatic Flock Waypoint", EditorStyles.boldLabel);
		EditorGUILayout.LabelField("Automaticly change the flock waypoint (0 = never)", EditorStyles.miniLabel);
		target._randomPositionTimer = EditorGUILayout.FloatField("Auto Waypoint Delay" , target._randomPositionTimer);
		if(target._randomPositionTimer < 0){
			target._randomPositionTimer = 0;
		}
		
		
    	EditorGUILayout.Space();
    	EditorGUILayout.LabelField("Force Bird Waypoints", EditorStyles.boldLabel);
    	EditorGUILayout.LabelField("Force all birds to change waypoints when flock changes waypoint", EditorStyles.miniLabel);
		target._forceChildWaypoints = EditorGUILayout.Toggle("Force Bird Waypoints" , target._forceChildWaypoints);
		target._forcedRandomDelay = EditorGUILayout.IntField("Bird Waypoint Delay" , target._forcedRandomDelay);
		
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Avoidance", EditorStyles.boldLabel);
		EditorGUILayout.LabelField("Flock collisions (Physics - Needs Ignore Raycast)", EditorStyles.miniLabel);
		target._flockAvoid = EditorGUILayout.Toggle("Flock Avoid" , target._flockAvoid);
		target._flockColliderSize = EditorGUILayout.FloatField("Flock Collider Size" , target._flockColliderSize);
		
		
		EditorGUILayout.LabelField("Birds will steer away from colliders (Ray)", EditorStyles.miniLabel);
		target._birdAvoid = EditorGUILayout.Toggle("Bird Avoid" , target._birdAvoid);
		if(target._birdAvoid){
			target._birdAvoidHorizontalForce = EditorGUILayout.FloatField("Avoid Horizontal Force" , target._birdAvoidHorizontalForce);
			
			//target._birdAvoidDistanceMin = EditorGUILayout.FloatField("_birdAvoidDistanceMin" , target._birdAvoidDistanceMin);
			//target._birdAvoidDistanceMax = EditorGUILayout.FloatField("_birdAvoidDistanceMax" , target._birdAvoidDistanceMax);
			
			var minVal : float = target._birdAvoidDistanceMin;
			var minLimit : float = .5;
			var maxVal : float = target._birdAvoidDistanceMax;
			var maxLimit : float = 8;
			
			EditorGUILayout.LabelField("Min Avoid Distance:", minVal.ToString());
			EditorGUILayout.LabelField("Max Avoid Distance:", maxVal.ToString());
			
			EditorGUILayout.MinMaxSlider(minVal, maxVal, minLimit, maxLimit);
			target._birdAvoidDistanceMin = minVal;
			target._birdAvoidDistanceMax = maxVal;
			
			target._birdAvoidDown = EditorGUILayout.Toggle("Bird Avoid Up" , target._birdAvoidDown);
			target._birdAvoidUp = EditorGUILayout.Toggle("Bird Avoid Down" , target._birdAvoidUp);
			if(target._birdAvoidDown || target._birdAvoidUp)
				target._birdAvoidVerticalForce = EditorGUILayout.FloatField("Avoid Vertical Force" , target._birdAvoidVerticalForce);
		}
		
		if(target._forcedRandomDelay < 0){
			target._forcedRandomDelay = 0;
		}	
        if (GUI.changed)
            EditorUtility.SetDirty (target);
    }
}