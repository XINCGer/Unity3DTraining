
// same as Triangles but creates quads instead which generally
// saves fillrate at the expense for more triangles to issue

#pragma strict

static var meshes : Mesh[];
static var currentQuads : int = 0;

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
	if (HasMeshes () && (currentQuads == (totalWidth * totalHeight))) {
		return meshes;
	}
		
	var maxQuads : int = 65000 / 6;
	var totalQuads : int = totalWidth * totalHeight;
	currentQuads = totalQuads;
	
	var meshCount : int = Mathf.CeilToInt ((1.0f * totalQuads) / (1.0f * maxQuads));
		
	meshes = new Mesh [meshCount];
	
	var i : int = 0;
	var index : int = 0;
	for (i = 0; i < totalQuads; i += maxQuads) {
		var quads : int = Mathf.FloorToInt (Mathf.Clamp ((totalQuads-i), 0, maxQuads));
				
		meshes[index] = GetMesh (quads, i, totalWidth, totalHeight);
		index++;
	}
	
	return meshes;
}

static function GetMesh (triCount : int, triOffset : int, totalWidth : int, totalHeight : int) : Mesh
{
	var mesh = new Mesh ();
	mesh.hideFlags = HideFlags.DontSave;
	
	var verts : Vector3[] = new Vector3[triCount*4];
	var uvs : Vector2[]  = new Vector2[triCount*4];
	var uvs2 : Vector2[] = new Vector2[triCount*4];
	var tris : int[] = new int[triCount*6];
	
	var size : float = 0.0075f;
	 
	for (var i : int = 0; i < triCount; i++)
	{
		var i4 : int = i * 4;
		var i6 : int = i * 6;

		var vertexWithOffset : int = triOffset + i;
		
		var x : float = Mathf.Floor(vertexWithOffset % totalWidth) / totalWidth;
		var y : float = Mathf.Floor(vertexWithOffset / totalWidth) / totalHeight;

		var position : Vector3 = Vector3 (x*2-1,y*2-1, 1.0);
			
		verts[i4 + 0] = position;
		verts[i4 + 1] = position;
		verts[i4 + 2] = position;
		verts[i4 + 3] = position;
		
		uvs[i4 + 0] = Vector2 (0.0f, 0.0f);
		uvs[i4 + 1] = Vector2 (1.0f, 0.0f);
		uvs[i4 + 2] = Vector2 (0.0f, 1.0f);
		uvs[i4 + 3] = Vector2 (1.0f, 1.0f);
		
		uvs2[i4 + 0] = Vector2 (x, y);
		uvs2[i4 + 1] = Vector2 (x, y);
		uvs2[i4 + 2] = Vector2 (x, y);
		uvs2[i4 + 3] = Vector2 (x, y);

		tris[i6 + 0] = i4 + 0;
		tris[i6 + 1] = i4 + 1;
		tris[i6 + 2] = i4 + 2;

		tris[i6 + 3] = i4 + 1;
		tris[i6 + 4] = i4 + 2;
		tris[i6 + 5] = i4 + 3;

	}

	mesh.vertices = verts;
	mesh.triangles = tris;
	mesh.uv = uvs;
	mesh.uv2 = uvs2;

	return mesh;
}

