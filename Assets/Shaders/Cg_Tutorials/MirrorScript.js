@script ExecuteInEditMode()
 
var objectInFrontOfMirror : GameObject;
var mirrorPlane : GameObject;
 
function Update () 
{
   if (null != mirrorPlane) 
   {
      GetComponent(Renderer).sharedMaterial.SetMatrix("_WorldToMirror", 
         mirrorPlane.GetComponent(Renderer).worldToLocalMatrix);
      if (null != objectInFrontOfMirror) 
      {
         transform.position = objectInFrontOfMirror.transform.position;
         transform.rotation = objectInFrontOfMirror.transform.rotation;
         transform.localScale = 
            -objectInFrontOfMirror.transform.localScale; 
         transform.RotateAround(objectInFrontOfMirror.transform.position, 
            mirrorPlane.transform.TransformDirection(
            Vector3(0.0, 1.0, 0.0)), 180.0);
 
         var positionInMirrorSpace : Vector3 = 
            mirrorPlane.transform.InverseTransformPoint(
            objectInFrontOfMirror.transform.position);
         positionInMirrorSpace.y = -positionInMirrorSpace.y;
         transform.position = mirrorPlane.transform.TransformPoint(
            positionInMirrorSpace);
      }
   }
}