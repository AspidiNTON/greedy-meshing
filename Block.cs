using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
	public Vector3Int coord;
	public MeshRenderer meshRenderer;
	public MeshCollider meshCollider;
	public MeshFilter meshFilter;
	public GameObject blockObject;

	int vertexIndex = 0;
	List<Vector3> vertices = new List<Vector3>();
	List<int> triangles = new List<int>();
	List<Vector2> uvs = new List<Vector2>();

	ushort[,,] voxelMap = new ushort[VoxelData.BlockWidth, VoxelData.BlockWidth, VoxelData.BlockWidth];


	public World world;

	public Block(Vector3Int _coord, World _world)
	{

		coord = _coord;
		blockObject = new GameObject();
		blockObject.transform.position = new Vector3(coord.x * VoxelData.BlockWidth, coord.y * VoxelData.BlockWidth, coord.z * VoxelData.BlockWidth);

		meshCollider = blockObject.AddComponent<MeshCollider>();
		meshRenderer = blockObject.AddComponent<MeshRenderer>();
		meshFilter = blockObject.AddComponent<MeshFilter>();
		world = _world;

		blockObject.transform.SetParent(world.transform);
		meshRenderer.material = world.material;

		blockObject.name = coord.x + ", " + coord.y + ", " + coord.z;



		PopulateVoxelMap();
		CreateMeshData();
		CreateMesh();

	}

	Vector3Int GetVector3IntFromVector3(Vector3 pos)
	{

		int x = Mathf.FloorToInt(pos.x);
		int y = Mathf.FloorToInt(pos.y);
		int z = Mathf.FloorToInt(pos.z);
		return new Vector3Int(x, y, z);

	}

	public bool IsActive
	{

		get { return blockObject.activeSelf; }
		set { blockObject.SetActive(value); }

	}


	Vector3Int position
	{

		get { return GetVector3IntFromVector3(blockObject.transform.position); }

	}

	bool IsVoxelInBlock(int x, int y, int z)
	{

		if (x < 0 || x > VoxelData.BlockWidth - 1 || y < 0 || y > VoxelData.BlockWidth - 1 || z < 0 || z > VoxelData.BlockWidth - 1)
			return false;
		else return true;

	}

	void PopulateVoxelMap()
	{
		for (int y = 0; y < VoxelData.BlockWidth; ++y)
		{
			for (int x = 0; x < VoxelData.BlockWidth; ++x)
			{
				for (int z = 0; z < VoxelData.BlockWidth; ++z)
				{
					voxelMap[x, y, z] = world.GetVoxel(new Vector3Int(x, y, z) + position);
				}
			}
		}
	}

	void CreateMeshData()
	{
		ushort[,,] voxels = new ushort[VoxelData.BlockWidth, VoxelData.BlockWidth, VoxelData.BlockWidth];
		byte[,,] greedyX = new byte[VoxelData.BlockWidth, VoxelData.BlockWidth, VoxelData.BlockWidth];
		byte[,,] greedyY = new byte[VoxelData.BlockWidth, VoxelData.BlockWidth, VoxelData.BlockWidth];

		for (int x = 0; x < VoxelData.BlockWidth; ++x)
		{
			for (int y = 0; y < VoxelData.BlockWidth; ++y)
			{
				for (int z = 0; z < VoxelData.BlockWidth; ++z)
				{
					if (!CheckVoxel(new Vector3Int(z, y, x) + VoxelData.faceChecks[0]))
					{
						voxels[z, y, x] = voxelMap[z, y, x];
					}
					else
					{
						voxels[z, y, x] = 0;
					}
					greedyX[z, y, x] = 1;
					greedyY[z, y, x] = 1;
				}
			}
			for (int y = 0; y < VoxelData.BlockWidth; ++y)
			{
				for (int z = 1; z < VoxelData.BlockWidth; ++z)
				{

					if (voxels[z, y, x] != 0 && voxels[z, y, x] == voxels[z - 1, y, x])
					{
						greedyX[z, y, x] += greedyX[z - 1, y, x];
						greedyX[z - 1, y, x] = 0;
						greedyY[z - 1, y, x] = 0;
					}
				}
			}
			for (int y = 0; y < VoxelData.BlockWidth; ++y)
			{
				for (int z = 1; z < VoxelData.BlockWidth; ++z)
				{

					if (voxels[y, z, x] != 0 && greedyX[y, z, x] != 0 && voxels[y, z, x] == voxels[y, z - 1, x] && greedyX[y, z, x] == greedyX[y, z - 1, x])
					{
						greedyY[y, z, x] += greedyY[y, z - 1, x];
						greedyX[y, z - 1, x] = 0;
						greedyY[y, z - 1, x] = 0;
					}
				}
			}
		}
		for (int x = 0; x < VoxelData.BlockWidth; ++x)
		{
			for (int y = 0; y < VoxelData.BlockWidth; ++y)
			{
				for (int z = 0; z < VoxelData.BlockWidth; ++z)
				{
					if (greedyX[x, y, z] != 0 && voxels[x, y, z] != 0)
					{
						CreatePolygon0(new Vector3Int(x, y, z), greedyX[x, y, z], greedyY[x, y, z], voxels[x, y, z]);
					}
				}
			}
		}

		for (int x = 0; x < VoxelData.BlockWidth; ++x)
		{
			for (int y = 0; y < VoxelData.BlockWidth; ++y)
			{
				for (int z = 0; z < VoxelData.BlockWidth; ++z)
				{
					if (!CheckVoxel(new Vector3Int(z, y, x) + VoxelData.faceChecks[1]))
					{
						voxels[z, y, x] = voxelMap[z, y, x];
					}
					else
					{
						voxels[z, y, x] = 0;
					}
					greedyX[z, y, x] = 1;
					greedyY[z, y, x] = 1;
				}
			}
			for (int y = 0; y < VoxelData.BlockWidth; ++y)
			{
				for (int z = 1; z < VoxelData.BlockWidth; ++z)
				{

					if (voxels[z, y, x] != 0 && voxels[z, y, x] == voxels[z - 1, y, x])
					{
						greedyX[z, y, x] += greedyX[z - 1, y, x];
						greedyX[z - 1, y, x] = 0;
						greedyY[z - 1, y, x] = 0;
					}
				}
			}
			for (int y = 0; y < VoxelData.BlockWidth; ++y)
			{
				for (int z = 1; z < VoxelData.BlockWidth; ++z)
				{

					if (voxels[y, z, x] != 0 && greedyX[y, z, x] != 0 && voxels[y, z, x] == voxels[y, z - 1, x] && greedyX[y, z, x] == greedyX[y, z - 1, x])
					{
						greedyY[y, z, x] += greedyY[y, z - 1, x];
						greedyX[y, z - 1, x] = 0;
						greedyY[y, z - 1, x] = 0;
					}
				}
			}
		}
		for (int x = 0; x < VoxelData.BlockWidth; ++x)
		{
			for (int y = 0; y < VoxelData.BlockWidth; ++y)
			{
				for (int z = 0; z < VoxelData.BlockWidth; ++z)
				{
					if (greedyX[x, y, z] != 0 && voxels[x, y, z] != 0)
					{
						CreatePolygon1(new Vector3Int(x, y, z), greedyX[x, y, z], greedyY[x, y, z], voxels[x, y, z]);
					}
				}
			}
		}

		for (int x = 0; x < VoxelData.BlockWidth; ++x)
		{
			for (int y = 0; y < VoxelData.BlockWidth; ++y)
			{
				for (int z = 0; z < VoxelData.BlockWidth; ++z)
				{
					if (!CheckVoxel(new Vector3Int(z, x, y) + VoxelData.faceChecks[2]))
					{
						voxels[z, x, y] = voxelMap[z, x, y];
					}
					else
					{
						voxels[z, x, y] = 0;
					}
					greedyX[z, x, y] = 1;
					greedyY[z, x, y] = 1;
				}
			}
			for (int y = 0; y < VoxelData.BlockWidth; ++y)
			{
				for (int z = 1; z < VoxelData.BlockWidth; ++z)
				{

					if (voxels[z, x, y] != 0 && voxels[z, x, y] == voxels[z - 1, x, y])
					{
						greedyX[z, x, y] += greedyX[z - 1, x, y];
						greedyX[z - 1, x, y] = 0;
						greedyY[z - 1, x, y] = 0;
					}
				}
			}
			for (int y = 0; y < VoxelData.BlockWidth; ++y)
			{
				for (int z = 1; z < VoxelData.BlockWidth; ++z)
				{

					if (voxels[y, x, z] != 0 && greedyX[y, x, z] != 0 && voxels[y, x, z] == voxels[y, x, z - 1] && greedyX[y, x, z] == greedyX[y, x, z - 1])
					{
						greedyY[y, x, z] += greedyY[y, x, z - 1];
						greedyX[y, x, z - 1] = 0;
						greedyY[y, x, z - 1] = 0;
					}
				}
			}
		}
		for (int x = 0; x < VoxelData.BlockWidth; ++x)
		{
			for (int y = 0; y < VoxelData.BlockWidth; ++y)
			{
				for (int z = 0; z < VoxelData.BlockWidth; ++z)
				{
					if (greedyX[x, y, z] != 0 && voxels[x, y, z] != 0)
					{
						CreatePolygon2(new Vector3Int(x, y, z), greedyX[x, y, z], greedyY[x, y, z], voxels[x, y, z]);
					}
				}
			}
		}

		for (int x = 0; x < VoxelData.BlockWidth; ++x)
		{
			for (int y = 0; y < VoxelData.BlockWidth; ++y)
			{
				for (int z = 0; z < VoxelData.BlockWidth; ++z)
				{
					if (!CheckVoxel(new Vector3Int(z, x, y) + VoxelData.faceChecks[3]))
					{
						voxels[z, x, y] = voxelMap[z, x, y];
					}
					else
					{
						voxels[z, x, y] = 0;
					}
					greedyX[z, x, y] = 1;
					greedyY[z, x, y] = 1;
				}
			}
			for (int y = 0; y < VoxelData.BlockWidth; ++y)
			{
				for (int z = 1; z < VoxelData.BlockWidth; ++z)
				{

					if (voxels[z, x, y] != 0 && voxels[z, x, y] == voxels[z - 1, x, y])
					{
						greedyX[z, x, y] += greedyX[z - 1, x, y];
						greedyX[z - 1, x, y] = 0;
						greedyY[z - 1, x, y] = 0;
					}
				}
			}
			for (int y = 0; y < VoxelData.BlockWidth; ++y)
			{
				for (int z = 1; z < VoxelData.BlockWidth; ++z)
				{

					if (voxels[y, x, z] != 0 && greedyX[y, x, z] != 0 && voxels[y, x, z] == voxels[y, x, z - 1] && greedyX[y, x, z] == greedyX[y, x, z - 1])
					{
						greedyY[y, x, z] += greedyY[y, x, z - 1];
						greedyX[y, x, z - 1] = 0;
						greedyY[y, x, z - 1] = 0;
					}
				}
			}
		}
		for (int x = 0; x < VoxelData.BlockWidth; ++x)
		{
			for (int y = 0; y < VoxelData.BlockWidth; ++y)
			{
				for (int z = 0; z < VoxelData.BlockWidth; ++z)
				{
					if (greedyX[x, y, z] != 0 && voxels[x, y, z] != 0)
					{
						CreatePolygon3(new Vector3Int(x, y, z), greedyX[x, y, z], greedyY[x, y, z], voxels[x, y, z]);
					}
				}
			}
		}

		for (int x = 0; x < VoxelData.BlockWidth; ++x)
		{
			for (int y = 0; y < VoxelData.BlockWidth; ++y)
			{
				for (int z = 0; z < VoxelData.BlockWidth; ++z)
				{
					if (!CheckVoxel(new Vector3Int(x, y, z) + VoxelData.faceChecks[4]))
					{
						voxels[x, y, z] = voxelMap[x, y, z];
					}
					else
					{
						voxels[x, y, z] = 0;
					}
					greedyX[x, y, z] = 1;
					greedyY[x, y, z] = 1;
				}
			}
			for (int y = 0; y < VoxelData.BlockWidth; ++y)
			{
				for (int z = 1; z < VoxelData.BlockWidth; ++z)
				{

					if (voxels[x, y, z] != 0 && voxels[x, y, z] == voxels[x, y, z - 1])
					{
						greedyX[x, y, z] += greedyX[x, y, z - 1];
						greedyX[x, y, z - 1] = 0;
						greedyY[x, y, z - 1] = 0;
					}
				}
			}
			for (int y = 0; y < VoxelData.BlockWidth; ++y)
			{
				for (int z = 1; z < VoxelData.BlockWidth; ++z)
				{

					if (voxels[x, z, y] != 0 && greedyX[x, z, y] != 0 && voxels[x, z, y] == voxels[x, z - 1, y] && greedyX[x, z, y] == greedyX[x, z - 1, y])
					{
						greedyY[x, z, y] += greedyY[x, z - 1, y];
						greedyX[x, z - 1, y] = 0;
						greedyY[x, z - 1, y] = 0;
					}
				}
			}
		}
		for (int x = 0; x < VoxelData.BlockWidth; ++x)
		{
			for (int y = 0; y < VoxelData.BlockWidth; ++y)
			{
				for (int z = 0; z < VoxelData.BlockWidth; ++z)
				{
					if (greedyX[x, y, z] != 0 && voxels[x, y, z] != 0)
					{
						CreatePolygon4(new Vector3Int(x, y, z), greedyX[x, y, z], greedyY[x, y, z], voxels[x, y, z]);
					}
				}
			}
		}

		for (int x = 0; x < VoxelData.BlockWidth; ++x)
		{
			for (int y = 0; y < VoxelData.BlockWidth; ++y)
			{
				for (int z = 0; z < VoxelData.BlockWidth; ++z)
				{
					if (!CheckVoxel(new Vector3Int(x, y, z) + VoxelData.faceChecks[5]))
					{
						voxels[x, y, z] = voxelMap[x, y, z];
					}
					else
					{
						voxels[x, y, z] = 0;
					}
					greedyX[x, y, z] = 1;
					greedyY[x, y, z] = 1;
				}
			}
			for (int y = 0; y < VoxelData.BlockWidth; ++y)
			{
				for (int z = 1; z < VoxelData.BlockWidth; ++z)
				{

					if (voxels[x, y, z] != 0 && voxels[x, y, z] == voxels[x, y, z - 1])
					{
						greedyX[x, y, z] += greedyX[x, y, z - 1];
						greedyX[x, y, z - 1] = 0;
						greedyY[x, y, z - 1] = 0;
					}
				}
			}
			for (int y = 0; y < VoxelData.BlockWidth; ++y)
			{
				for (int z = 1; z < VoxelData.BlockWidth; ++z)
				{

					if (voxels[x, z, y] != 0 && greedyX[x, z, y] != 0 && voxels[x, z, y] == voxels[x, z - 1, y] && greedyX[x, z, y] == greedyX[x, z - 1, y])
					{
						greedyY[x, z, y] += greedyY[x, z - 1, y];
						greedyX[x, z - 1, y] = 0;
						greedyY[x, z - 1, y] = 0;
					}
				}
			}
		}
		for (int x = 0; x < VoxelData.BlockWidth; ++x)
		{
			for (int y = 0; y < VoxelData.BlockWidth; ++y)
			{
				for (int z = 0; z < VoxelData.BlockWidth; ++z)
				{
					if (greedyX[x, y, z] != 0 && voxels[x, y, z] != 0)
					{
						CreatePolygon5(new Vector3Int(x, y, z), greedyX[x, y, z], greedyY[x, y, z], voxels[x, y, z]);
					}
				}
			}
		}

	}

	void CreatePolygon0(Vector3Int pos, byte x, byte y, ushort material)
	{
		vertices.Add(pos + new Vector3Int(1 - x, 1 - y, 0)); // left bottom
		vertices.Add(pos + new Vector3Int(1 - x, 1, 0)); // left top
		vertices.Add(pos + new Vector3Int(1, 1 - y, 0)); // right bottom
		vertices.Add(pos + new Vector3Int(1, 1, 0)); // right top

		AddTexture(world.voxeltypes[material].voxelMaterial);

		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 3);
		vertexIndex += 4;
	}

	void CreatePolygon1(Vector3Int pos, byte x, byte y, ushort material)
	{
		vertices.Add(pos + new Vector3Int(1, 1 - y, 1)); // left bottom
		vertices.Add(pos + new Vector3Int(1, 1, 1)); // left top
		vertices.Add(pos + new Vector3Int(1 - x, 1 - y, 1)); // right bottom
		vertices.Add(pos + new Vector3Int(1 - x, 1, 1)); // right top

		AddTexture(world.voxeltypes[material].voxelMaterial);

		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 3);
		vertexIndex += 4;
	}

	void CreatePolygon2(Vector3Int pos, byte x, byte y, ushort material)
	{
		vertices.Add(pos + new Vector3Int(1, 0, 1 - y)); // left bottom
		vertices.Add(pos + new Vector3Int(1, 0, 1)); // left top
		vertices.Add(pos + new Vector3Int(1 - x, 0, 1 - y)); // right bottom
		vertices.Add(pos + new Vector3Int(1 - x, 0, 1)); // right top

		AddTexture(world.voxeltypes[material].voxelMaterial);

		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 3);
		vertexIndex += 4;
	}

	void CreatePolygon3(Vector3Int pos, byte x, byte y, ushort material)
	{
		vertices.Add(pos + new Vector3Int(1 - x, 1, 1 - y)); // left bottom
		vertices.Add(pos + new Vector3Int(1 - x, 1, 1)); // left top
		vertices.Add(pos + new Vector3Int(1, 1, 1 - y)); // right bottom
		vertices.Add(pos + new Vector3Int(1, 1, 1)); // right top

		AddTexture(world.voxeltypes[material].voxelMaterial);

		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 3);
		vertexIndex += 4;
	}

	void CreatePolygon4(Vector3Int pos, byte x, byte y, ushort material)
	{
		vertices.Add(pos + new Vector3Int(0, 1 - y, 1)); // left bottom
		vertices.Add(pos + new Vector3Int(0, 1, 1)); // left top
		vertices.Add(pos + new Vector3Int(0, 1 - y, 1 - x)); // right bottom
		vertices.Add(pos + new Vector3Int(0, 1, 1 - x)); // right top

		AddTexture(world.voxeltypes[material].voxelMaterial);

		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 3);
		vertexIndex += 4;
	}

	void CreatePolygon5(Vector3Int pos, byte x, byte y, ushort material)
	{
		vertices.Add(pos + new Vector3Int(1, 1 - y, 1 - x)); // left bottom
		vertices.Add(pos + new Vector3Int(1, 1, 1 - x)); // left top
		vertices.Add(pos + new Vector3Int(1, 1 - y, 1)); // right bottom
		vertices.Add(pos + new Vector3Int(1, 1, 1)); // right top

		AddTexture(world.voxeltypes[material].voxelMaterial);

		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 3);
		vertexIndex += 4;
	}

	public ushort GetVoxelFromMap(Vector3 pos)
	{
		pos -= position;

		return voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];
	}

	bool CheckVoxel(Vector3Int pos)
	{
		if (!IsVoxelInBlock(pos.x, pos.y, pos.z))
		{
			return world.voxeltypes[world.GetVoxel(pos + position)].isSolid;
		}
		return world.voxeltypes[voxelMap[pos.x, pos.y, pos.z]].isSolid;
	}

	void CreateMesh()
	{
		if (vertices.Count != 0)
		{
			Mesh mesh = new Mesh();
			mesh.vertices = vertices.ToArray();
			mesh.triangles = triangles.ToArray();
			mesh.uv = uvs.ToArray();

			mesh.RecalculateNormals();

			meshFilter.mesh = mesh;
			meshCollider.sharedMesh = mesh;
		}
	}

	void AddTexture(int textureID)
	{
		float y = (float)(textureID / VoxelData.TextureAtlasSizeInBlocks) / VoxelData.TextureAtlasSizeInBlocks;
		float x = (float)(textureID - (y * VoxelData.TextureAtlasSizeInBlocks)) / VoxelData.TextureAtlasSizeInBlocks;

		uvs.Add(new Vector2(x, y));
		uvs.Add(new Vector2(x, y + VoxelData.NormalizedBlockTextureSize));
		uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y));
		uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y + VoxelData.NormalizedBlockTextureSize));
	}
}