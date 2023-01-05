using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{

    public Transform player;
    public Vector3 spawn;

    public Material material;
    public VoxelType[] voxeltypes;
    public static int Seed = 3;

    FastNoiseLite noise1 = new FastNoiseLite(Seed);
    FastNoiseLite noise2 = new FastNoiseLite(Seed + 1);
    FastNoiseLite noise3 = new FastNoiseLite(Seed + 2);

    List<Vector3Int> activeBlocks = new List<Vector3Int>();
    List<Block> blocks = new List<Block>();
    List<Vector3Int> l = new List<Vector3Int>();
    int loaded = 0;
    Vector3Int playerBlockCoord;
    Vector3Int playerLastBlockCoord;

    private void Start()
    {


        for (int x = -VoxelData.ViewDistanceInBlocks; x < VoxelData.ViewDistanceInBlocks + 1; ++x)
        {
            for (int y = -VoxelData.ViewDistanceInBlocks; y < VoxelData.ViewDistanceInBlocks + 1; ++y)
            {
                for (int z = -VoxelData.ViewDistanceInBlocks; z < VoxelData.ViewDistanceInBlocks + 1; ++z)
                {
                    l.Add(new Vector3Int(x, y, z));
                }
            }
        }

        for (int i = 1; i < l.Count; i++)
        {
            Vector3Int cur = l[i];
            int j = i;
            while (j > 0 && Vector3Int.Distance(cur, new Vector3Int(0, 0, 0)) < Vector3Int.Distance(l[j - 1], new Vector3Int(0, 0, 0)))
            {
                l[j] = l[j - 1];
                j--;
            }
            l[j] = cur;
        }


        player.position = spawn;
        playerLastBlockCoord = GetVector3IntFromVector3(spawn);
        playerBlockCoord = GetVector3IntFromVector3(spawn);

    }

    private void Update()
    {

        //playerBlockCoord = GetVector3IntFromVector3(player.position);


        if (playerBlockCoord != playerLastBlockCoord)
        {
            loaded = 0;
        }

        if (l.Count > loaded)
        {
            while ((l.Count > loaded) && activeBlocks.Contains(playerBlockCoord + l[loaded]))
            {
                loaded += 1;
            }
            if (l.Count > loaded)
            {
                CreateBlock(playerBlockCoord + l[loaded]);
            }
        }

        playerLastBlockCoord = playerBlockCoord;


    }



    Vector3Int GetVector3IntFromVector3(Vector3 pos)
    {

        int x = Mathf.FloorToInt(pos.x / VoxelData.BlockWidth);
        int y = Mathf.FloorToInt(pos.y / VoxelData.BlockWidth);
        int z = Mathf.FloorToInt(pos.z / VoxelData.BlockWidth);
        return new Vector3Int(x, y, z);

    }

    Vector3 GetVector3FromVector3Int(Vector3Int pos)
    {

        int x = pos.x * VoxelData.BlockWidth;
        int y = pos.y * VoxelData.BlockWidth;
        int z = pos.z * VoxelData.BlockWidth;
        return new Vector3(x, y, z);

    }


    private void CreateBlock(Vector3Int coord)
    {
        blocks.Add(new Block(coord, this));
        activeBlocks.Add(coord);
    }


    public ushort GetVoxel(Vector3Int pos)
    {
        if (noise1.GetNoise(pos.x, pos.y, pos.z) * 128 > pos.y)
        {
            if (noise2.GetNoise(pos.x * 5, pos.y * 5, pos.z * 5) > 0.4) return 3;
            if (noise3.GetNoise(pos.x * 5, pos.y * 5, pos.z * 5) > 0.4) return 2;
            return 1;
        }
        else return 0;
    }

}


[System.Serializable]
public class VoxelType
{
    public string voxelName;
    public bool isSolid;
    public int voxelMaterial;

}