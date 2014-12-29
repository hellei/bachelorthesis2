/**************************************									
	FlockChild v2.2
	Copyright 2013 Unluck Software	
 	www.chemicalbliss.com								
***************************************/
#pragma strict
#pragma downcast
@HideInInspector public var _spawner:FlockController;
@HideInInspector public var _wayPoint : Vector3;
public var _speed:float;
@HideInInspector public var _dived:boolean =true;
@HideInInspector public var _stuckCounter:float;			//prevents looping around a waypoint by increasing minimum distance to waypoint
@HideInInspector public var _damping:float;
@HideInInspector public var _soar:boolean = true;
@HideInInspector public var _landing:boolean;
private var _lerpCounter:int;
public var _targetSpeed:float;
@HideInInspector public var _move:boolean = true;
var _model:GameObject;

var _avoidValue:float;		//Random value used to check for obstacles. Randomized to lessen uniformed behaviour when avoiding
var _avoidDistance:float;

private var _soarTimer:float;	

function Start(){

   Wander(0);
   var sc = Random.Range(_spawner._minScale, _spawner._maxScale);
   transform.localScale=Vector3(sc,sc,sc);
   transform.position = (Random.insideUnitSphere *_spawner._spawnSphere) + _spawner.transform.position;
   transform.position.y = Random.Range(-_spawner._spawnSphereHeight, _spawner._spawnSphereHeight*1.0) +_spawner.transform.position.y;
   if(!_model)_model= transform.FindChild("Model").gameObject;
   for (var state : AnimationState in _model.animation) {
   	 	state.time = Random.value * state.length;
   }
	if(collider)
	Physics.IgnoreCollision(collider, _spawner.collider);
	_avoidValue = Random.Range(.3, .1);
	if(_spawner._birdAvoidDistanceMax != _spawner._birdAvoidDistanceMin)
	this._avoidDistance = Random.Range(_spawner._birdAvoidDistanceMax ,_spawner._birdAvoidDistanceMin);
	else
	this._avoidDistance = _spawner._birdAvoidDistanceMin;
	_speed = _spawner._minSpeed;
}
 
function Update() {
	//Soar Timeout - Limits how long a bird can soar
	if(this._soar && _spawner._soarMaxTime > 0){ 		
   		if(_soarTimer > _spawner._soarMaxTime){
   			this.Flap();
   			_soarTimer = 0;
   		}else {
   			_soarTimer+=Time.deltaTime;
   		}
   	}
    
    if(!_landing && (transform.position - _wayPoint).magnitude < _spawner._waypointDistance+_stuckCounter){
        Wander(0);	//create a new waypoint
        _stuckCounter=0;
    }else if(!_landing){
    	_stuckCounter+=Time.deltaTime;
    }else{
    	_stuckCounter=0;
    }
    var lookit:Vector3 = _wayPoint - transform.position;
    if(_targetSpeed > -1 && lookit != Vector3.zero){
    var rotation = Quaternion.LookRotation(lookit);
	transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * _damping);
	}
	if(_spawner._childTriggerPos){
		if((transform.position - _spawner.transform.position).magnitude < 1){
			_spawner.randomPosition();
		}
	}
	_speed = Mathf.Lerp(_speed, _targetSpeed, _lerpCounter * Time.deltaTime *.05);
	_lerpCounter++;
	//Position forward based on object rotation
	if(_move){
		transform.position += transform.TransformDirection(Vector3.forward)*_speed*Time.deltaTime;
		//Avoidance
		if(_spawner._birdAvoid && _move){	
			var hit : RaycastHit;
			if (Physics.Raycast(transform.position, _model.transform.forward+(_model.transform.right*_avoidValue), hit, _avoidDistance)){
				if(!hit.transform.gameObject.GetComponent(FlockController)){
					transform.rotation.eulerAngles.y -= _spawner._birdAvoidHorizontalForce*Time.deltaTime*_damping;
					
				}
			}else if (Physics.Raycast(transform.position, _model.transform.forward+(_model.transform.right*-_avoidValue), hit, _avoidDistance)){
				if(!hit.transform.gameObject.GetComponent(FlockController)){
					transform.rotation.eulerAngles.y += _spawner._birdAvoidHorizontalForce*Time.deltaTime*_damping;
					
				}
			}
			
			if (_spawner._birdAvoidDown && !this._landing && Physics.Raycast(transform.position, -Vector3.up, hit, _avoidDistance)){
				if(!hit.transform.gameObject.GetComponent(FlockController)){
					transform.rotation.eulerAngles.x -= _spawner._birdAvoidVerticalForce*Time.deltaTime*_damping;
					transform.position.y += _spawner._birdAvoidVerticalForce*Time.deltaTime*.01;	//Add some push movement to aid downwards avoidance (Just using rotation looks unatural)
				}
			}else if (_spawner._birdAvoidUp && !this._landing && Physics.Raycast(transform.position, Vector3.up, hit, _avoidDistance)){
				if(!hit.transform.gameObject.GetComponent(FlockController)){
					transform.rotation.eulerAngles.x += _spawner._birdAvoidVerticalForce*Time.deltaTime*_damping;
					transform.position.y -= _spawner._birdAvoidVerticalForce*Time.deltaTime*.01;
				}
			}
		}
	}
	//Counteract Pitch Rotation When Flying Upwards
	if((_soar && _spawner._flatSoar|| _spawner._flatFly && !_soar)&& _wayPoint.y > transform.position.y||_landing)
		_model.transform.localEulerAngles.x = Mathf.LerpAngle(_model.transform.localEulerAngles.x, -transform.localEulerAngles.x, _lerpCounter * Time.deltaTime * .75);
	else
		_model.transform.localEulerAngles.x = Mathf.LerpAngle(_model.transform.localEulerAngles.x, 0, _lerpCounter * Time.deltaTime * .75);
	
//	if((_soar && _spawner._flatSoar|| _spawner._flatFly && !_soar)&& _wayPoint.y > transform.position.y||_landing){
//		_model.transform.localEulerAngles.x = Mathf.LerpAngle(_model.transform.localEulerAngles.x, -transform.localEulerAngles.x, Time.deltaTime * 5);
//		}
//	else{	
//		_model.transform.localEulerAngles.x = Mathf.LerpAngle(_model.transform.localEulerAngles.x, 0, Time.deltaTime * 1);	
//		}
	//if(_landing &&( _model.transform.localEulerAngles.x < 60 || _model.transform.localEulerAngles.x > 300))
	//	_model.transform.localEulerAngles.x =0;
	
}

function Wander(delay:float){
	yield(WaitForSeconds(delay));
	if(!_landing){
		_damping = Random.Range(_spawner._minDamping, _spawner._maxDamping);       
	    _targetSpeed = Random.Range(_spawner._minSpeed, _spawner._maxSpeed);
	    _lerpCounter = 0;
	    if(!_dived && Random.value < _spawner._soarFrequency){
	   	 	Soar();
		}else if(!_dived && Random.value < _spawner._diveFrequency){	
			Dive();
		}else{	
			Flap();
		}
	}
}

function Flap(){
	if(_move){
		//Debug.Log("Flap");
	 	if(this._model) _model.animation.CrossFade(_spawner._flapAnimation, .5);
		_soar=false;
		animationSpeed();
		_wayPoint = (Random.insideUnitSphere *_spawner._spawnSphere) + _spawner.transform.position;
		_wayPoint.y = Random.Range(-_spawner._spawnSphereHeight, _spawner._spawnSphereHeight*1.0) +_spawner.transform.position.y;
		_dived = false;
	}
}

function Soar(){
	//Debug.Log("Soar");
	 if(this._model)
		_model.animation.CrossFade(_spawner._soarAnimation, 1.5);
   	_wayPoint= (Random.insideUnitSphere *_spawner._spawnSphere) + _spawner.transform.position;
	_wayPoint.y = Random.Range(-_spawner._spawnSphereHeight, _spawner._spawnSphereHeight*1.0) +_spawner.transform.position.y;
    _soar = true;
}

function Dive(){
	if (this._model){
		if(_spawner._soarAnimation!=null){
				_model.animation.CrossFade(_spawner._soarAnimation, 1.5);
			}else{
				for (var state : AnimationState in _model.animation) {
		   	 		if(transform.position.y < _wayPoint.y +25){
		   	 			state.speed = 0.1;
		   	 		}
		   	 	}
	   	 	}
	   	 	_wayPoint.x = transform.position.x + Random.Range(-1, 1);
	    	_wayPoint.z = transform.position.z + Random.Range(-1, 1);
	    	_wayPoint.y = Random.Range(-_spawner._spawnSphereHeight, _spawner._spawnSphereHeight) +_spawner.transform.position.y;
	    	_wayPoint.y -= _spawner._diveValue;
	    	_dived = true;
    }
}

function animationSpeed(){
	if (this._model){
	for (var state : AnimationState in _model.animation) {
		if(!_dived && !_landing){
			state.speed = Random.Range(_spawner._minAnimationSpeed, _spawner._maxAnimationSpeed);
		}else{
			state.speed = _spawner._maxAnimationSpeed;
		}   
	}
	}
}