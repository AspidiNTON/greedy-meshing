using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VoxelData
{
	public static readonly int TextureAtlasSizeInBlocks = 32;
	public static readonly int BlockWidth = 32;
	public static readonly int ChunkSizeInBlocks = 8;
	public static readonly int ViewDistanceInBlocks = 2;


	public static float NormalizedBlockTextureSize
	{

		get { return 1f / TextureAtlasSizeInBlocks; }

	}

	public static readonly Vector3Int[] voxelVerts = new Vector3Int[8] {

		new Vector3Int(1, 0, 0),
		new Vector3Int(1, 1, 0),
		new Vector3Int(0, 0, 0),
		new Vector3Int(0, 1, 0),
		new Vector3Int(0, 0, 1),
		new Vector3Int(0, 1, 1),
		new Vector3Int(1, 0, 1),
		new Vector3Int(1, 1, 1),

	};

	public static readonly Vector3Int[] faceChecks = new Vector3Int[6] {

		new Vector3Int(0, 0, -1),
		new Vector3Int(0, 0, 1),
		new Vector3Int(0, -1, 0),
		new Vector3Int(0, 1, 0),
		new Vector3Int(-1, 0, 0),
		new Vector3Int(1, 0, 0)

	};

	public static readonly int[,] voxelTris = new int[6, 4] {

		// 0 1 2 2 1 3
		{0, 1, 2, 3}, // Back Face
		{4, 5, 6, 7}, // Front Face
		{6, 0, 4, 2}, // Bottom Face
		{5, 3, 7, 1}, // Top Face
		{2, 3, 4, 5}, // Left Face
		{6, 7, 0, 1} // Right Face

	};

	public static readonly Vector2[] voxelUvs = new Vector2[4] {

		new Vector2 (0.0f, 0.0f),
		new Vector2 (0.0f, 1.0f),
		new Vector2 (1.0f, 0.0f),
		new Vector2 (1.0f, 1.0f)

	};


}
