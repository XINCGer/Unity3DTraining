//This script will make your cube wrap the texture around it

function Start () {
     var mf = GetComponent(MeshFilter);
     var mesh : Mesh;
     if (mf != null)
         mesh = mf.mesh;
         
     if (mesh == null || mesh.uv.Length != 24) {
         Debug.Log("Attach To Cube");
         return;
     }
 
     var uvs = mesh.uv;
     
     uvs[0]  = Vector2(0.0, 0.0);
     uvs[1]  = Vector2(0.333, 0.0);
     uvs[2]  = Vector2(0.0, 0.333);
     uvs[3]  = Vector2(0.333, 0.333);
     
     uvs[8]  = Vector2(0.334, 0.0);
     uvs[9]  = Vector2(0.666, 0.0);
     uvs[4]  = Vector2(0.334, 0.333);
     uvs[5]  = Vector2(0.666, 0.333);
     
     uvs[10] = Vector2(0.667, 0.0);
     uvs[11] = Vector2(1.0, 0.0);
     uvs[6]  = Vector2(0.667, 0.333);
     uvs[7]  = Vector2(1.0, 0.333);
     
     uvs[12] = Vector2(0.0, 0.334);
     uvs[14] = Vector2(0.333, 0.334);
     uvs[15] = Vector2(0.0, 0.666);
     uvs[13] = Vector2(0.333, 0.666);                
     
     uvs[16] = Vector2(0.334, 0.334);
     uvs[18] = Vector2(0.666, 0.334);
     uvs[19] = Vector2(0.334, 0.666);
     uvs[17] = Vector2(0.666, 0.666);    
            
     uvs[20] = Vector2(0.667, 0.334);
     uvs[22] = Vector2(1.00, 0.334);
     uvs[23] = Vector2(0.667, 0.666);
     uvs[21] = Vector2(1.0, 0.666);    
     
     mesh.uv = uvs;
 }
 