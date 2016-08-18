@script ExecuteInEditMode()

var occluder : GameObject;

function Update () {
   if (null != occluder) {
      GetComponent(Renderer).sharedMaterial.SetVector("_SpherePosition", 
         occluder.transform.position);
      GetComponent(Renderer).sharedMaterial.SetFloat("_SphereRadius", 
         occluder.transform.localScale.x / 2.0);
   }
}