#pragma strict

@script ExecuteInEditMode() // make sure to run in edit mode

var other : GameObject; // another user-specified object

function Update () // this function is called for every frame
{
    if (null != other) // has the user specified an object?
    {
        GetComponent(Renderer).sharedMaterial.SetMatrix("_Cutaway", 
        	other.GetComponent(Renderer).worldToLocalMatrix);
    }
}