/**************************************									
	Copyright 2014 Unluck Software	
 	www.chemicalbliss.com						
***************************************/
#pragma strict
@HideInInspector
var landingChild: FlockChild;
@HideInInspector
var landing: boolean;
private var lerpCounter: int;
@HideInInspector
var _controller: LandingSpotController;
@HideInInspector
var _offsetPlatformHeight: float = 1;
private var _idle:boolean;

function Start() {		
    if (!_controller)
        _controller = transform.parent.GetComponent(LandingSpotController);
    if (_controller._autoCatchDelay.x > 0)
        GetFlockChild(_controller._autoCatchDelay.x, _controller._autoCatchDelay.y);   
	RandomRotate();
}

function OnDrawGizmos() {
	if (!_controller)
        _controller = transform.parent.GetComponent(LandingSpotController);
    
    Gizmos.color = Color.yellow;
    // Draw a yellow cube at the transforms position
    if (landingChild && landing)
        Gizmos.DrawLine(transform.position, landingChild.transform.position);
    if (transform.rotation.eulerAngles.x != 0 || transform.rotation.eulerAngles.z != 0)
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    Gizmos.DrawWireCube(Vector3(transform.position.x, transform.position.y + _offsetPlatformHeight, transform.position.z), Vector3(.2, .2, .2));
    Gizmos.DrawWireCube(transform.position + (transform.forward * .2) + Vector3(0, _offsetPlatformHeight, 0), Vector3(.1, .1, .1));
    Gizmos.color = Color(1, 1, 0, .05);
    Gizmos.DrawWireSphere(transform.position, _controller._maxBirdDistance);
}

function LateUpdate() {
    if (landing && landingChild) {
    	//Check distance to flock child
        var distance: float = Vector3.Distance(landingChild.transform.position, transform.position);
        //Start landing if distance is close enough
        if (distance < 5 && distance > .5) {
            if(_controller._soarLand){
            	landingChild._model.animation.CrossFade(landingChild._spawner._soarAnimation, .5);
            	if (distance < 2)
           	 		landingChild._model.animation.CrossFade(landingChild._spawner._flapAnimation, .5);
            }
            landingChild._targetSpeed = landingChild._spawner._maxSpeed*.5;
          	landingChild._wayPoint = transform.position;      	
            landingChild._damping = 2.5;
        } else if (distance <= .5) {
        	 
            landingChild._wayPoint = transform.position;
           
            if (distance < .1 && !_idle) {
                _idle = true;
                landingChild._model.animation.CrossFade(landingChild._spawner._idleAnimation, .55); 
            }
            
            if (distance > .01){       	
            	landingChild._targetSpeed = landingChild._spawner._minSpeed*this._controller._landingSpeedModifier;
          	    landingChild.transform.position += (transform.position - landingChild.transform.position) * Time.deltaTime *landingChild._speed*_controller._landingSpeedModifier;     	
          	}
            
            landingChild._move = false;
            lerpCounter++;
     
                landingChild.transform.rotation.eulerAngles.y = Mathf.LerpAngle(landingChild.transform.rotation.eulerAngles.y, transform.transform.rotation.eulerAngles.y, lerpCounter * Time.deltaTime * .005);//  Quaternion.Lerp(landingChild.transform.rotation, transform.rotation, lerpCounter * Time.deltaTime * .01);

            landingChild._damping = 3;
        } else {
        	//Move towards landing spot
            landingChild._wayPoint = transform.position;
            landingChild._damping = 1;
        }

    } 
}

function GetFlockChild(minDelay: float, maxDelay: float): IEnumerator {
    yield(WaitForSeconds(Random.Range(minDelay, maxDelay)));
    if (!landingChild) {
		RandomRotate();
    
        var __child: FlockChild;

        for (var i: int; i < _controller._flock._roamers.length; i++) {
            var child: FlockChild = _controller._flock._roamers[i] as FlockChild;
            if (!child._landing && !child._dived) {         
            	if(!_controller._onlyBirdsAbove){     	
	                if (!__child && _controller._maxBirdDistance > Vector3.Distance(child.transform.position, transform.position) && _controller._minBirdDistance < Vector3.Distance(child.transform.position, transform.position)) {
	                    __child = child;
	                    if (!_controller._takeClosest) break;
	                } else if (__child && Vector3.Distance(__child.transform.position, transform.position) > Vector3.Distance(child.transform.position, transform.position)) {
	                    __child = child;
	                }
                }else{
                	if (!__child && child.transform.position.y > transform.position.y && _controller._maxBirdDistance > Vector3.Distance(child.transform.position, transform.position) && _controller._minBirdDistance < Vector3.Distance(child.transform.position, transform.position)) {
	                    __child = child;
	                    if (!_controller._takeClosest) break;
	                } else if (__child && child.transform.position.y > transform.position.y && Vector3.Distance(__child.transform.position, transform.position) > Vector3.Distance(child.transform.position, transform.position)) {
						__child = child;
	                }
                }
            }
        }
        if (__child) {
            landingChild = __child;
            landing = true;
           	landingChild._landing = true;
           	ReleaseFlockChild(_controller._autoDismountDelay.x, _controller._autoDismountDelay.y);
        } else if (_controller._autoCatchDelay.x > 0) {
            GetFlockChild(_controller._autoCatchDelay.x, _controller._autoCatchDelay.y);
        }
    }
}

function RandomRotate(){
		
	if (_controller._randomRotate)
		transform.rotation.eulerAngles.y = Random.Range(0, 360);
	
}

function InstantLand() {
    if (!landingChild) {
        var __child: FlockChild;
      
        for (var i: int; i < _controller._flock._roamers.length; i++) {
            var child: FlockChild = _controller._flock._roamers[i] as FlockChild;
            if (!child._landing && !child._dived) {
                     __child = child;           
            }
        }
        if (__child) {
            landingChild = __child;
            landing = true;

            landingChild._landing = true;
            landingChild.transform.position = transform.position;
            landingChild._model.animation.Play(landingChild._spawner._idleAnimation);
            ReleaseFlockChild(_controller._autoDismountDelay.x, _controller._autoDismountDelay.y);
        } else if (_controller._autoCatchDelay.x > 0) {
            GetFlockChild(_controller._autoCatchDelay.x, _controller._autoCatchDelay.y);
        }
    }
}

function ReleaseFlockChild(minDelay: float, maxDelay: float) {
    yield(WaitForSeconds(Random.Range(minDelay, maxDelay)));
    if (landingChild) {
        lerpCounter = 0;
        if (_controller._feathers){
        	_controller._feathers.transform.position = landingChild.transform.position;
            _controller._feathers.transform.particleSystem.Emit(Random.Range(0,3));
            }           
		landing = false;
        _idle = false;
        //Reset flock child to flight mode
        landingChild._damping = landingChild._spawner._maxDamping;
        landingChild._model.animation.CrossFade(landingChild._spawner._flapAnimation, .2);
		landingChild._dived = true;     
        landingChild._speed = 0;       
        landingChild._move = true;
        landingChild._landing = false;
        landingChild.Flap();
      //  landingChild._wayPoint = transform.position;
        landingChild._wayPoint.y = transform.position.y+10;
        
        
        
         yield(WaitForSeconds(.1));
         if (_controller._autoCatchDelay.x > 0) {
            GetFlockChild(_controller._autoCatchDelay.x, _controller._autoCatchDelay.y);
        }
        landingChild = null;
    }
}