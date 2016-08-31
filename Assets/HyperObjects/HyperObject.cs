using UnityEngine;
using UnityEditor;
using System.Collections;

public class HyperObject : MonoBehaviour {

	public int w; //point on the w axis
	//public bool movable = true; //is the object movable?
	public float dullCoef = 1.0f; //how much to dull the color of the object by
    public int w_depth;
    public FourthDManager IVDManager; //the 4D manager
    public bool isParent = false;
    public bool vanishWhenTransparent = false;
    //public bool childrenHaveColliders = false;

    const int TRANSPARENT_QUEUE_ORDER = 3000;

    void Start()
    {
        //locate the 4Dmanager
        IVDManager = Object.FindObjectOfType<FourthDManager>();

        /*if (!IVDManager)
            Debug.LogError("Failed to find the 4DManager, did you put one in the sceen?");
        if (w > IVDManager.MAX_W || w < IVDManager.MIN_W)
            Debug.LogError("Initial w value is out of bounds, check to make sure the initial w value is withing the range set in the 4DManager");*/
        if (isParent)
        {
            setW(w);
            WMove(GameObject.FindGameObjectWithTag("Player").GetComponent<HyperCreature>().w);
        }
    }

	//the player has moved to a new w point, remove once 4D shader is implemented
	public void WMove(int newW){
        if(isVisibleSolid(newW)){//this object is on the player's w point or is wide enough to be seen and touched
			StartCoroutine(ColorTrans(newW, 1.0f));
		}
		else{
            float targA = .5f;
            if (vanishWhenTransparent)
                targA = 0.0f;

			//fade out if not on player's w point
            if(w_depth > 0)
            {
                if(w > newW)
                    StartCoroutine(ColorTrans(w, targA));
                else
                    StartCoroutine(ColorTrans(w + w_depth, targA));
            }
            else
            {
                if (w < newW)
                    StartCoroutine(ColorTrans(w, targA));
                else
                    StartCoroutine(ColorTrans(w + w_depth, targA));
            }
		}
        if (GetComponent<HyperColliderManager>())
            GetComponent<HyperColliderManager>().WMove(newW);
        else
        {
            recurseChildrenWMove(transform, newW);
        }

        //GetComponent<MeshRenderer>().material.SetFloat("_WPos", (float)w);
        //GetComponent<MeshRenderer>().material.renderQueue = TRANSPARENT_QUEUE_ORDER + getNewOrder(newW);
    }

    void recurseChildrenWMove(Transform t, int newW)
    {
        foreach (Transform child in t)
        {
            if (child.GetComponent<HyperObject>())
                child.GetComponent<HyperObject>().WMove(newW);
            else if (child.GetComponent<HyperColliderManager>())
                child.GetComponent<HyperColliderManager>().WMove(newW);
            else if (child.childCount > 0)
                recurseChildrenWMove(child, newW);
        }
    }

    public void setW(int newW)
    {
        w = newW;
        if (GetComponent<HyperColliderManager>())
            GetComponent<HyperColliderManager>().setW(newW);
        else
        {
            recurseChildrenSetW(transform, newW);
        }
    }

    void recurseChildrenSetW(Transform t, int newW)
    {
        if (GetComponent<HyperColliderManager>())
            GetComponent<HyperColliderManager>().setW(newW);
        else
        {
            foreach (Transform child in transform)
            {
                if (child.GetComponent<HyperObject>())
                    child.GetComponent<HyperObject>().setW(newW);
                else if (child.GetComponent<HyperColliderManager>())
                    child.GetComponent<HyperColliderManager>().setW(newW);
                else if (child.childCount > 0)
                    recurseChildrenSetW(child, newW);
            }
        }
    }

    int getNewOrder(int creatureW)
    {
        int controllerPos = creatureW;

        if (controllerPos == 0)
        {
            return 7 - w;
        }
        else if (controllerPos == 1)
        {
            if (w == 1)
                return 7;
            else if (w == 0)
                return 6;
            else
                return 7 - w;
        }
        else if (controllerPos == 2)
        {
            if (w == 2)
                return 7;
            else if (w == 1)
                return 6;
            else if (w == 3)
                return 5;
            else if (w == 0)
                return 4;
            else
                return 7 - w;
        }
        else if (controllerPos == 3)
        {
            if (w == 3)
                return 7;
            else if (w == 2)
                return 6;
            else if (w == 4)
                return 5;
            else if (w == 1)
                return 4;
            else if (w == 5)
                return 3;
            else if (w == 0)
                return 2;
            else
                return 1;
        }
        else if (controllerPos == 4)
        {
            if (w == 4)
                return 7;
            else if (w == 3)
                return 6;
            else if (w == 5)
                return 5;
            else if (w == 2)
                return 4;
            else if (w == 6)
                return 3;
            else
                return 1 + w;
        }
        else if (controllerPos == 5)
        {
            if (w == 5)
                return 7;
            else if (w == 4)
                return 6;
            else if (w == 6)
                return 5;
            else
                return 1 + w;
        }
        else if (controllerPos == 6)
        {
            return 1 + w;
        }

        return 0;
    }

    //smoothly change the color of this object, rmove once 4D shader is implemented
    IEnumerator ColorTrans(int newW, float targetA){
		//Color targetColor = new Color(Mathf.Clamp(0.0f, 1.0f, ((newW + 3f) % 6f) - 2f),
        //                          Mathf.Clamp(0.0f, 1.0f, ((newW + 1f) % 6f) - 2f) + Mathf.Max(0.0f, (1f - ((newW + 5f) % 6f))) * 0.5f,
        //                          Mathf.Clamp(0.0f, 1.0f, newW - 3f), 
        //			   		        1.0f);
        Color targetColor;
        Color curColor;
		curColor = gameObject.GetComponent<Renderer>().material.color;
		
		//deturmine the target color based on w point
        
		if(newW == 0)
			targetColor = Color.red;
		else if(newW == 1)
			targetColor = new Color(1,.45f,0);
		else if(newW == 2)
			targetColor = Color.yellow;
		else if(newW == 3)
			targetColor = Color.green;
		else if(newW == 4)
			targetColor = Color.cyan;
		else if(newW == 5)
			targetColor = Color.blue;
		else
			targetColor = Color.magenta;
            

		targetColor.a = targetA;
		targetColor.r /= dullCoef;
		targetColor.g /= dullCoef;
		targetColor.b /= dullCoef;
		
		for(float i = 0.0f; i < 1.0f; i += .05f){

			gameObject.GetComponent<Renderer>().material.color = Color.Lerp(curColor, targetColor, i);
			if(transform.childCount > 0){
				foreach(Transform child in transform){
					child.gameObject.GetComponent<Renderer>().material.color = Color.Lerp(curColor, targetColor, i);
				}
			}
			/*if(i >= .95f && targetA == 1.0f)
				GetComponent<Renderer>().material.shader = Shader.Find ("Diffuse");
			else if(GetComponent<Renderer>().material.shader == Shader.Find ("Diffuse"))
				GetComponent<Renderer>().material.shader = Shader.Find ("Transparent/Diffuse");*/
			yield return null;
		}
	}

    //move this object along the w axis by deltaW
	public bool SlideW(int deltaW){
        if ((deltaW > 0 && w != 6 && w + w_depth != 6) || (deltaW < 0 && w != 0 && w + w_depth != 0))
        {
            bool childrenClear = true;


            recurseChildrenSlideW(transform, deltaW, childrenClear);

            if (childrenClear)
            {
                w += deltaW;
                return true;
            }
        }
        return false;
        //SetCollisions();
    }

    void recurseChildrenSlideW(Transform t, int deltaW, bool childrenClear)
    {
        foreach (Transform child in t)
        {
            if (child.GetComponent<HyperColliderManager>())
                childrenClear = childrenClear && child.GetComponent<HyperColliderManager>().SlideW(deltaW);
            else if (child.GetComponent<HyperObject>())
                childrenClear = childrenClear && child.GetComponent<HyperObject>().SlideW(deltaW);
            else if (child.childCount > 0)
                recurseChildrenSlideW(child, deltaW, childrenClear);
        }
    }

    /*bool CanCollide(GameObject other)
    {
        if(w < other.GetComponent<HyperObject>().w)
        {
            if(other.GetComponent<HyperObject>().w_depth >= 0)
                return (w + w_depth >= other.GetComponent<HyperObject>().w);
            else
                return (w + w_depth >= other.GetComponent<HyperObject>().w + other.GetComponent<HyperObject>().w_depth);
        }
        else if (w > other.GetComponent<HyperObject>().w)
        {
            if (other.GetComponent<HyperObject>().w_depth > 0)
                return (w + w_depth <= other.GetComponent<HyperObject>().w + other.GetComponent<HyperObject>().w_depth);
            else
                return (w + w_depth <= other.GetComponent<HyperObject>().w);
        }
        else
            return (w == other.GetComponent<HyperObject>().w);
    }*/

    //deturmine if this can be seen from the other w as a solid object, remove once 4D shader is implemented
    public bool isVisibleSolid(int otherW)
    {
        if (w_depth > 0)
        {
            if (otherW <= w + w_depth && otherW >= w)
                return true;
            else
                return false;
        }
        else if (w_depth < 0)
        {
            if (otherW >= w + w_depth && otherW <= w)
                return true;
            else
                return false;
        }
        else
            return (w == otherW);
    }

    //setup collisions for this object
    /*void SetCollisions()
    {
        HyperObject[] hyperObjects; //all hyper objects in the world

        //hyperObjects = GameObject.FindGameObjectsWithTag("HyperObject");
        hyperObjects = Object.FindObjectsOfType<HyperObject>();

        if (GetComponent<Collider>())
        {
            foreach (var hypObj in hyperObjects)
            {
                if (hypObj)
                {
                    if (!GetComponent<HyperObject>().CanCollide(hypObj.gameObject))
                    {
                        Physics.IgnoreCollision(GetComponent<Collider>(), hypObj.gameObject.GetComponent<Collider>(), true);
                        if (childrenHaveColliders)
                        {
                            foreach (Transform child in transform)
                            {
                                Physics.IgnoreCollision(child.GetComponent<Collider>(), hypObj.gameObject.GetComponent<Collider>(), true);
                            }
                        }
                        if (hypObj.childrenHaveColliders)
                        {
                            foreach (Transform child in hypObj.transform)
                            {
                                Physics.IgnoreCollision(GetComponent<Collider>(), child.GetComponent<Collider>(), true);
                            }
                        }
                    }
                    else
                    {
                        Physics.IgnoreCollision(GetComponent<Collider>(), hypObj.gameObject.GetComponent<Collider>(), false);
                        if (childrenHaveColliders)
                        {
                            foreach (Transform child in transform)
                            {
                                Physics.IgnoreCollision(child.GetComponent<Collider>(), hypObj.gameObject.GetComponent<Collider>(), false);
                            }
                        }
                        if (hypObj.childrenHaveColliders)
                        {
                            foreach (Transform child in hypObj.transform)
                            {
                                Physics.IgnoreCollision(GetComponent<Collider>(), child.GetComponent<Collider>(), false);
                            }
                        }
                    }
                }
            }
        }
    }*/
}
