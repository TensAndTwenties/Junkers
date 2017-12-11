using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionPath
{
	List<Action> Actions = new List<Action>();
	bool recomputeAtRestart = false;

	public void setDefaultActionPath (behaviorProfile profile)
	{
		//create action path based on the drone AI profile
		switch (profile) {
		case behaviorProfile.bombard:
			
			break;
		case behaviorProfile.closeRange:
			break;
		case behaviorProfile.kamikaze:
			break;
		}

	}
}

public class Action {
	//a move or shoot command for the AI

}

class ActionPathUtility{
	ActionPath defaultBombardPath(){
		return new ActionPath ();
	}
}