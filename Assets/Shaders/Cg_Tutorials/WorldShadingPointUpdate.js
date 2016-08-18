#pragma strict

@script ExecuteInEditMode() // make sure to run in edit mode

var other : GameObject; // another user-specified object

function Update () // this function is called for every frame
{
    if (null != other) // has the user specified an object?
    {
        GetComponent(Renderer).sharedMaterial.SetVector("_Point", 
           other.transform.position); // set the shader property 
        // _Point to the position of the other object
    }
}