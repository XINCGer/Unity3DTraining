
#pragma strict

static var meshes : Mesh[];
static var currentTris : int = 0;

static function HasMeshes () : boolean {
	if (!meshes)
		return false;
	for (var m : Mesh in meshes)
		if (null == m)
			return false;

	return true;	
}

static function Cleanup () {
	if (!meshes)
		return;
		
	for (var m : Mesh in meshes) {
		if (null != m) {
			DestroyImmediate (m);	
			m = null;	
		}
	}
	meshes = null;
}

static function GetMeshes (totalWidth : int, totalHeight : int) : Mesh[]
{
	if (HasMeshes () && (currentTris == (totalWidth * totalHeight))) {
		return meshes;
	}
		
	var maxTris : int = 65000 / 3;
	var totalTris : int = totalWidth * totalHeight;
	currentTris = totalTris;
	
	var meshCount : int = Mathf.CeilToInt ((1.0f * totalTris) / (1.0f * maxTris));
		
	meshes = new Mesh[meshCount];
	
	var i : int = 0;
	var index : int = 0;
	for (i = 0; i < totalTris; i += maxTris) {
		var tris : int = Mathf.FloorToInt (Mathf.Clamp ((totalTris-i), 0, maxTris));
				
		meshes[index] = GetMesh (tris, i, totalWidth, totalHeight);
		index++;
	}
	
	return meshes;
}

static function GetMesh (triCount : int, triOffset : int, totalWidth : int, totalHeight : int) : Mesh
{
	var mesh = new Mesh ();
	mesh.hideFlags = HideFlags.DontSave;
	
	var verts : Vector3[] = new Vector3[triCount*3];
	var uvs : Vector2[]  = new Vector2[triCount*3];
	var uvs2 : Vector2[] = new Vector2[triCount*3];
	var tris : int[] = new int[triCount*3];
	
	var size : float = 0.0075f;
	 
	for (var i : int = 0; i < triCount; i++)
	{
		var i3 : int = i * 3;
		var vertexWithOffset : int = triOffset + i;
		
		var x : float = Mathf.Floor(vertexWithOffset % totalWidth) / totalWidth;
		var y : float = Mathf.Floor(vertexWithOffset / totalWidth) / totalHeight;

		var position : Vector3 = Vector3 (x*2-1,y*2-1, 1.0);
			
		verts[i3 + 0] = position;
		verts[i3 + 1] = position;
		verts[i3 + 2] = position;

		uvs[i3 + 0] = Vector2 (0.0f, 0.0f);
		uvs[i3 + 1] = Vector2 (1.0f, 0.0f);
		uvs[i3 + 2] = Vector2 (0.0f, 1.0f);
		
		uvs2[i3 + 0] = Vector2 (x, y);
		uvs2[i3 + 1] = Vector2 (x, y);
		uvs2[i3 + 2] = Vector2 (x, y);

		tris[i3 + 0] = i3 + 0;
		tris[i3 + 1] = i3 + 1;
		tris[i3 + 2] = i3 + 2;
	}

	mesh.vertices = verts;
	mesh.triangles = tris;
	mesh.uv = uvs;
	mesh.uv2 = uvs2;

	return mesh;
}

